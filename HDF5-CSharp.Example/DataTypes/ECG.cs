using HDF5CSharp.DataTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDF5CSharp.Example.DataTypes
{
    [Hdf5GroupName("ecg")]
    public class ECG : Hdf5BaseFile, IDisposable
    {
        [Hdf5EntryName("start_datetime")] public long? StartDateTime { get; set; }
        [Hdf5EntryName("end_datetime")] public long EndDateTime { get; set; }
        [Hdf5EntryName("sampling_rate")] public int SamplingRate { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] public Dictionary<string, string> Parameters { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] public Dictionary<string, string> Header { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private ChunkedDataset<double> UnFiltered { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private ChunkedDataset<double> Filtered { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private ChunkedDataset<long> Timestamps { get; set; }
        //[Hdf5Save(Hdf5Save.DoNotSave)] private ChunkedDataset<long> Detections { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private BlockingCollectionQueue<ECGFrame> EcgSamplesData { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private Task EcgTaskWriter { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] private int ChunkSize;
        [Hdf5Save(Hdf5Save.DoNotSave)] private bool completed;
        public ECG(long fileId, long groupRoot, int chunkSize, ILogger logger) : base(fileId, groupRoot, "ecg", logger)
        {
            ChunkSize = chunkSize;
            SamplingRate = ECGFrame.AcqSampleRate;
            var pool = ArrayPool<ECGFrame>.Shared;
            EcgSamplesData = new BlockingCollectionQueue<ECGFrame>();
            Parameters = new Dictionary<string, string>();
            Header = new Dictionary<string, string>();
            UnFiltered = new ChunkedDataset<double>("Signals_unfiltered", GroupId);
            Filtered = new ChunkedDataset<double>("Signals_filtered", GroupId);
            Filtered = new ChunkedDataset<double>("Signals_filtered", GroupId);
            Timestamps = new ChunkedDataset<long>("timestamps", GroupId);
            //Detections = new ChunkedDataset<long>("detections", GroupId);
            EcgTaskWriter = Task.Factory.StartNew(() =>
            {
                var buffer = pool.Rent(ChunkSize);
                completed = false;
                int count = 0;
                foreach (ECGFrame data in EcgSamplesData.GetConsumingEnumerable())
                {
                    buffer[count++] = data;
                    if (count == ChunkSize)
                    {
                        AppendSample(buffer, chunkSize);
                        count = 0;
                    }
                }
                if (count != 0)
                {
                    AppendSample(buffer, count);
                }
                FlushData();//end of data samples. flush data
                pool.Return(buffer);
            });
        }

        private void AppendSample(ECGFrame[] samples, int length)
        {

            double[,] unFilteredData = new double[length * samples.First().FrameData.Count, 2];
            double[,] filteredData = new double[length * samples.First().FilteredFrameData.Count, 2];
            long[,] timestampData = new long[length * samples.First().FrameData.Count, 1];
            for (var i = 0; i < length; i++)
            {
                var data = samples[i];
                var frameLength = data.FrameData.Count;
                for (var j = 0; j < frameLength; j++)
                {
                    var sample = data.FrameData[j];
                    unFilteredData[i * frameLength + j, 0] = sample.LA_RA;
                    unFilteredData[i * frameLength + j, 1] = sample.LL_RA;
                    EndDateTime = sample.Timestamp;
                }



                frameLength = data.FilteredFrameData.Count;
                for (var j = 0; j < frameLength; j++)
                {
                    var sample = data.FilteredFrameData[j];
                    filteredData[i * frameLength + j, 0] = sample.LA_RA;
                    filteredData[i * frameLength + j, 1] = sample.LL_RA;
                    timestampData[i * frameLength + j, 0] = sample.Timestamp;

                }
            }
            UnFiltered.AppendOrCreateDataset(unFilteredData);
            Filtered.AppendOrCreateDataset(filteredData);
            Timestamps.AppendOrCreateDataset(timestampData);
        }


        public void Enqueue(ECGFrame ecgFrame)
        {
            if (!completed)
            {
                if (!StartDateTime.HasValue)
                {
                    StartDateTime = ecgFrame.FrameData.First().Timestamp;
                }

                EcgSamplesData.Enqueue(ecgFrame);
            }
        }
        public void CompleteAdding()
        {
            if (completed)
            {
                return;
            }

            completed = true;
            EcgSamplesData.CompleteAdding();
        }

        public async Task WaitForDataWritten()
        {
            CompleteAdding();
            await EcgTaskWriter;
        }

        public void Dispose()
        {
            try
            {
                if (!Disposed)
                {
                    UnFiltered.Dispose();
                    Filtered.Dispose();
                    Timestamps.Dispose();
                    //Detections.Dispose();
                    EcgSamplesData.Dispose();
                    EcgTaskWriter.Dispose();
                    Hdf5.CloseGroup(GroupId);
                    Disposed = true;
                }
            }
            catch (Exception)
            {
                //nothing
            }
        }

        public void AppendEcgCycleDescriptionSample(ECGCycleDescription e)
        {
            Hdf5.WriteStrings(GroupId, "ecg_cycle_description", new List<string> { e.AsJson() });
        }
    }
}
