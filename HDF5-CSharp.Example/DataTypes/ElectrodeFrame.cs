using System;

namespace HDF5CSharp.Example.DataTypes
{
    [Serializable]
    public class ElectrodeFrame
    {
        public (float Re, float Im)[] ComplexVoltageMatrix { get; set; }

        // CHANNELS * CHANNELS entries
        public (float Re, float Im)[] ComplexCurrentMatrix { get; set; }

        public long timestamp;

        public UInt64 SaturationMask;

        public void GenerateDummyData(int electrodeNum)
        {
            ComplexVoltageMatrix = new ValueTuple<float, float>[electrodeNum * electrodeNum];
            ComplexCurrentMatrix = new ValueTuple<float, float>[electrodeNum * electrodeNum];

            Random r = new Random();

            for (int i = 0; i < electrodeNum * electrodeNum; i++)
            {
                ComplexVoltageMatrix[i].Re = r.Next(0, 1000) / 1000.0f;
                ComplexVoltageMatrix[i].Im = r.Next(0, 1000) / 1000.0f;
                ComplexCurrentMatrix[i].Im = r.Next(0, 1000) / 1000.0f;
                ComplexCurrentMatrix[i].Re = r.Next(0, 1000) / 1000.0f;
            }

        }
    }
}
