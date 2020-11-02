using HDF.PInvoke;
using HDF5CSharp.Example.DataTypes;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HDF5CSharp.Example
{
    public class KamaAcquisitionFile
    {
        private long fileId;
        long groupRoot;
        private long groupEIT;
        //h5 data
        public ProcedureInformation ProcedureInformation { get; set; }
        public SystemInformation SystemInformation { get; set; }
        public EIT EIT { get; set; }
        public ECG ECG { get; set; }
        public InjectionGroup InjectionGroup { get; set; }
        public CalibrationGroup CalibrationGroup { get; set; }
        public Patient PatientInfo { get; set; }
        public EventGroup Events { get; set; }
        public TagsGroup Tags { get; set; }
        //end h5 data
        private ILogger Logger { get; }

        private int RecordNumber { get; set; }
        private bool RecordingInProgress { get; set; }
        private bool FileClosed { get; set; }
        public string FileName { get; private set; }
        private int EITDefaultChunkSize { get; set; }
        private int ECGDefaultChunkSize { get; set; }
        public KamaAcquisitionFile(string filename, ILogger logger, int eitDefaultChunkSize = 24000, int ecgDefaultChunkSize = 10)
        {
            FileName = filename;
            Logger = logger;
            EITDefaultChunkSize = eitDefaultChunkSize;
            ECGDefaultChunkSize = ecgDefaultChunkSize;
            Hdf5.Settings.LowerCaseNaming = true;
            Hdf5.Settings.DateTimeType = DateTimeType.UnixTimeMilliseconds;
            RecordNumber = 1;
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
            fileId = Hdf5.CreateFile(filename);
            groupRoot = fileId;
            groupEIT = Hdf5.CreateOrOpenGroup(groupRoot, "eit");
            ProcedureInformation = new ProcedureInformation(fileId, groupRoot, logger)
            {
                ProcedureDirectory = Path.GetDirectoryName(filename),
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now
            };

            SystemInformation = new SystemInformation(fileId, groupRoot, logger);
            //  InjectionGroup = new InjectionGroup(fileId, groupRoot);
            CalibrationGroup = new CalibrationGroup(fileId, groupRoot, logger);
            Events = new EventGroup(fileId, groupRoot, logger);
            Tags = new TagsGroup(fileId, groupRoot, logger);
        }

        #region public

        public void AppendElectrodeSample(ElectrodeFrame sample)
        {
            EIT.Enqueue(sample);
        }

        public void AppendEcgSample(ECGFrame e)
        {
            ECG.Enqueue(e);
        }
        public void StopRecording()
        {
            EIT?.CompleteAdding();
            ECG?.CompleteAdding();
            Events?.StopRecording();
            Tags?.StopRecording();
            RecordingInProgress = false;
        }

        public Task StartLogging(AcquisitionProtocolParameters acquisitionProtocol)
        {
            EIT = new EIT(RecordNumber++, EITDefaultChunkSize, acquisitionProtocol.AsJson(), fileId, groupEIT, Logger);
            //var acquisitionInformation = new AcquisitionInformation(acquisitionProtocol, fileId, groupEIT, Logger);
            //acquisitionInformation.FlushDataAndCloseObject();
            ECG = new ECG(fileId, groupRoot, ECGDefaultChunkSize, Logger);
            Events.StartLogging();
            Tags.StartLogging();
            return Task.CompletedTask;
        }

        private async Task CloseHandles()
        {

            ProcedureInformation.FlushDataAndCloseObject();
            SystemInformation.FlushDataAndCloseObject();
            CalibrationGroup.FlushDataAndCloseObject();
            //wait for writing all data before resetting
            if (EIT != null)
            {
                await EIT.WaitForDataWritten();
                EIT.Dispose();
            }

            if (ECG != null)
            {
                await ECG.WaitForDataWritten();
                ECG.Dispose();
            }
            if (Events != null)
            {
                await Events.WaitForDataWritten();
                Events.Dispose();
            }
            if (Tags != null)
            {
                await Tags.WaitForDataWritten();
                Tags.Dispose();
            }
            await Task.CompletedTask;
        }

        #endregion

        public async Task<(bool, string)> StopProcedure()
        {
            if (FileClosed)
            {
                return GeneralUtils.CheckFileSize(FileName);
            }

            FileClosed = true;
            Logger?.LogInformation("Stop Procedure called");
            ProcedureInformation.EndDateTime = DateTime.Now;
            if (RecordingInProgress)
            {
                StopRecording();
            }

            await CloseHandles();
            Hdf5.Flush(groupRoot, H5F.scope_t.GLOBAL);
            Hdf5.CloseGroup(groupEIT);
            Hdf5.CloseGroup(groupRoot);
            long result = Hdf5.CloseFile(fileId);
            if (result >= 0)
            {
                Logger?.LogInformation("Stop Procedure H5 File closed");
            }
            else
            {
                Logger?.LogError("Cannot close H5 File: " + result);
            }
            return GeneralUtils.CheckFileSize(FileName);
        }

        public void SavePatientInfo(ProcedureInfo procedureInfo)
        {
            PatientInfo = new Patient(fileId, groupRoot, Logger)
            {
                FirstName = procedureInfo.FirstName,
                LastName = procedureInfo.LastName,
                Age = procedureInfo.Age,
                ExamDate = procedureInfo.ExamDate,
                Gender = "unknown",
            };
            PatientInfo.FlushDataAndCloseObject();
        }

        public void UpdateSystemInformation(string systemId, string[] boardIds)
        {
            SystemInformation.SystemId = systemId;
            SystemInformation.BoardIds = boardIds;
        }

        public void SetProcedureInformation(ProcedureInfo procedureInfo)
        {
            ProcedureInformation.ProcedureType = procedureInfo.Procedure;
            ProcedureInformation.ProcedureID = procedureInfo.ProcedureID;
        }

        public void AppendEcgCycleDescriptionSample(ECGCycleDescription e) => ECG.AppendEcgCycleDescriptionSample(e);

        public void AppendSystemEvent(SystemEventModel systemEvent) => Events.Enqueue(systemEvent);


        public void AddCalibrationsData(CalibrationsSystemInformation calibrationsSystemInformation)
        {
            CalibrationGroup.AddCalibrationsData(calibrationsSystemInformation);
            CalibrationGroup.FlushDataAndCloseObject();
        }

        public void AppendTag(string tag)
        {
            Tags.Enqueue(tag);
            Logger?.LogInformation("logTag: " + tag);
        }
    }
}
