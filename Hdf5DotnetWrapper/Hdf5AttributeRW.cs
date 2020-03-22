using System;
using System.Collections.Generic;
using System.Text;
using Hdf5DotnetWrapper.Interfaces;

namespace Hdf5DotnetWrapper
{
    public class Hdf5AttributeRW : IHdf5ReaderWriter
    {
        public (bool success, Array result) ReadToArray<T>(long groupId, string name, string alternativeName)
        {
            return Hdf5.ReadPrimitiveAttributes<T>(groupId, name, alternativeName);
        }

        public (int success, long CreatedgroupId) WriteFromArray<T>(long groupId, string name, Array dset, string datasetName)
        {
            return Hdf5.WritePrimitiveAttribute<T>(groupId, name, dset, datasetName);
        }

        public (int success, long CreatedgroupId) WriteStrings(long groupId, string name, IEnumerable<string> collection, string datasetName = null)
        {
            return Hdf5.WriteStringAttributes(groupId, name, (string[])collection, datasetName);
        }


        public (bool success, IEnumerable<string>) ReadStrings(long groupId, string name, string alternativeName)
        {
            return Hdf5.ReadStringAttributes(groupId, name, alternativeName);
        }

    }
}
