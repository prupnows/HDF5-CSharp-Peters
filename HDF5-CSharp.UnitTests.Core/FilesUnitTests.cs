using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDF5CSharp.UnitTests.Core
{
    [TestClass]
    public class FilesUnitTests:Hdf5BaseUnitTests
    {
        [TestMethod]
        public void TestReadStructure()
        {
            string fileName = @"FileStructure.h5";
            var tree =Hdf5.ReadTreeFileStructure(fileName);
            var flat = Hdf5.ReadFlatFileStructure(fileName);
        }
    }
}
