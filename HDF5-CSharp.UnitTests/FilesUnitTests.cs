using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDF5CSharp.UnitTests.Core
{
    [TestClass]
    public class FilesUnitTests : Hdf5BaseUnitTests
    {
        [TestMethod]
        public void TestReadStructure()
        {
            string fileName = @"FileStructure.h5";
            if (File.Exists(fileName))
            {
                var tree = Hdf5.ReadTreeFileStructure(fileName);
                var flat = Hdf5.ReadFlatFileStructure(fileName);
                File.Delete(fileName);
                Assert.IsFalse(File.Exists(fileName));
            }
        }
    }
}

