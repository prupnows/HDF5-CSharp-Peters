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
            string fileName = @"D:\KamaDB\2020_02_11\2020_02_11_14_08_56_John\0001_888_apt_circular_10\1_John.h5";
            var structure =Hdf5.ReadFileStructure(fileName);
        }
    }
}
