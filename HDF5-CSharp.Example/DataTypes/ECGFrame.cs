using System;
using System.Collections.Generic;

namespace HDF5CSharp.Example.DataTypes
{
    [Serializable]
    public class ECGFrame
    {
        // The sample rate of the ECG frames 
        public const int AcqSampleRate = 640;
        // by default size of a reading is 30
        public int NumberOfFrameWithinReading = 3;

        public List<(double LA_RA, double LL_RA, long Timestamp)> FrameData;
        public List<(double LA_RA, double LL_RA, long Timestamp)> FilteredFrameData;


        /// <summary>
        /// Prior the stabilization (normalization need to be done), the frame will be raised as invalid
        /// </summary>
        public bool IsValid;

        public bool IsTriggerToInjection;

        public ECGFrame()
        {
            FrameData = new List<(double LA_RA, double LL_RA, long Timestamp)>();
            FilteredFrameData = new List<(double LA_RA, double LL_RA, long Timestamp)>();
            IsTriggerToInjection = false;
        }
    }
}
