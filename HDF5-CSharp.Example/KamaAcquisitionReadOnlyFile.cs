using HDF5CSharp.Example.DataTypes;
using System;
using System.Collections.Generic;

namespace HDF5CSharp.Example
{
    public class KamaAcquisitionReadOnlyFile : IDisposable
    {
        public string FileName { get; }
        public ProcedureInformation ProcedureInformation { get; set; }
        public SystemInformation SystemInformation { get; set; }
        public Patient PatientInformation { get; set; }
        public ECGData ECG { get; set; }
        public List<EITEntry> EITs { get; set; }
        public Hdf5Events Events { get; set; }
        private long fileId;
        private string rootName = "/";
        private string rootNameOld = "/root";
        private string system_informationName = "/system_information";
        private string procedure_informationName = "/procedure_information";
        private string patient_informationName = "/patient";
        private string ecgName = "/ecg";
        private string eitName = "/eit";
        private string eventsName = "/events/system_events";
        private bool fileClosed;
        public KamaAcquisitionReadOnlyFile(string filename)
        {
            FileName = filename;
            ProcedureInformation = new ProcedureInformation();
            SystemInformation = new SystemInformation();
            PatientInformation = new Patient();
            ECG = new ECGData();
            EITs = new List<EITEntry>();
            Hdf5.Settings.LowerCaseNaming = true;
            Hdf5.Settings.DateTimeType = DateTimeType.UnixTimeMilliseconds;
            fileId = Hdf5.OpenFile(filename);
        }

        public void ReadSystemInformation()
        {
            string groupName = rootName + system_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                SystemInformation = Hdf5.ReadObject<SystemInformation>(fileId, groupName);
                return;
            }
            groupName = rootNameOld + system_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                SystemInformation = Hdf5.ReadObject<SystemInformation>(fileId, groupName);
            }
        }
        public void ReadProcedureInformation()
        {
            string groupName = rootName + procedure_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                ProcedureInformation = Hdf5.ReadObject<ProcedureInformation>(fileId, groupName);
                return;
            }
            groupName = rootNameOld + procedure_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                ProcedureInformation = Hdf5.ReadObject<ProcedureInformation>(fileId, groupName);
            }
        }
        public void ReadPatientInformation()
        {
            string groupName = rootName + patient_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                PatientInformation = Hdf5.ReadObject<Patient>(fileId, groupName);
                return;
            }
            groupName = rootNameOld + patient_informationName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                PatientInformation = Hdf5.ReadObject<Patient>(fileId, groupName);
            }
        }

        public void ReadECGData()
        {
            string groupName = rootName + ecgName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                ECG = Hdf5.ReadObject<ECGData>(fileId, groupName);
                return;
            }
            groupName = rootNameOld + ecgName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                ECG = Hdf5.ReadObject<ECGData>(fileId, groupName);
            }
        }
        public void ReadEITData()
        {

            int index = 1;
            string rootGroup = rootName + eitName;
            if (!Hdf5Utils.ItemExists(fileId, rootGroup, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                rootGroup = rootNameOld + eitName;
            }

            while (Hdf5Utils.ItemExists(fileId, rootGroup + "/d" + index, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                var entry = Hdf5.ReadObject<EITEntry>(fileId, rootGroup + "/d" + index);
                EITs.Add(entry);
                index++;
            }
        }

        public void ReadSystemEvents()
        {
            string groupName = rootName + eventsName;
            if (Hdf5Utils.ItemExists(fileId, groupName, HDF5CSharp.DataTypes.Hdf5ElementType.Group))
            {
                Events = Hdf5.ReadObject<Hdf5Events>(fileId, groupName);
            }


        }

        public void Dispose()
        {
            if (!fileClosed)
            {
                Hdf5.CloseFile(fileId);
                fileClosed = true;
            }
        }
    }
}
