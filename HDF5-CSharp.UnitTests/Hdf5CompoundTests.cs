using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDF5CSharp.UnitTests
{
    public partial class Hdf5UnitTests
    {

        [TestMethod]
        public void WriteAndReadObjectWithStructs()
        {
            string filename = Path.Combine(folder, "testObjectWithStructArray.H5");


            try
            {

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var status = Hdf5.WriteObject(fileId, classWithStructs, "test");
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            try
            {
                TestClassWithStructs objWithStructs;
                var fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                objWithStructs = Hdf5.ReadObject<TestClassWithStructs>(fileId, "test");
                CollectionAssert.AreEqual(wDataList, objWithStructs.DataList);
                Hdf5.CloseFile(fileId);


            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

        }




        public struct SystemEvent
        {
            [Hdf5EntryName("timestamp")] public long timestamp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)] [Hdf5EntryName("type")] public string type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)] [Hdf5EntryName("data")] public string data;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)] [Hdf5EntryName("description")] public string description;


            public SystemEvent(long timestamp, string type, string description, string data)
            {
                this.timestamp = timestamp;
                this.type = type;
                this.description = description;
                this.data = data;
            }
        }
        [TestMethod]
        public void WriteAndReadSystemEvent()
        {
            string filename = Path.Combine(folder, "testCompounds.H5");
            List<SystemEvent> se = new List<SystemEvent>();
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            try
            {

                se.Add(new SystemEvent(5, "55", "3300000000000000000000000", "555555555555555555555555555555555"));
                se.Add(new SystemEvent(1, "255", "3d3000000000007777773", "ggggggggggggdf"));

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var status = Hdf5.WriteCompounds(fileId, "/test", se, attributes);
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            try
            {
                var fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                var cmpList = Hdf5.ReadCompounds<SystemEvent>(fileId, "/test").ToArray();
                Hdf5.CloseFile(fileId);
                CollectionAssert.AreEqual(se, cmpList);

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

        }

        [TestMethod]
        public void WriteAndReadStructs()
        {
            string filename = Path.Combine(folder, "testCompounds.H5");
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            try
            {

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var status = Hdf5.WriteCompounds(fileId, "/test", wData2List, attributes);
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            try
            {
                var fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                var cmpList = Hdf5.ReadCompounds<WData2>(fileId, "/test").ToArray();
                Hdf5.CloseFile(fileId);
                CollectionAssert.AreEqual(wData2List, cmpList);

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

        }

        [TestMethod]
        public void WriteAndReadStructsWithDatetime()
        {
            string filename = Path.Combine(folder, "testCompounds.H5");
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            try
            {

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var status = Hdf5.WriteCompounds(fileId, "/test", wDataList, attributes);
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            try
            {
                var fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                var cmpList = Hdf5.ReadCompounds<WData>(fileId, "/test").ToArray();
                Hdf5.CloseFile(fileId);
                CollectionAssert.AreEqual(wDataList, cmpList);

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

        }

        [TestMethod]
        public void WriteAndReadStructsWithArray()
        {
            string filename = Path.Combine(folder, "testArrayCompounds.H5");
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            try
            {

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var status = Hdf5.WriteCompounds(fileId, "/test", responseList, attributes);
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            try
            {
                var fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                Responses[] cmpList = Hdf5.ReadCompounds<Responses>(fileId, "/test").ToArray();
                Hdf5.CloseFile(fileId);
                var isSame = responseList.Zip(cmpList, (r, c) =>
                {
                    return r.MCID == c.MCID &&
                    r.PanelIdx == c.PanelIdx &&
                    r.ResponseValues.Zip(c.ResponseValues, (rr, cr) => rr == cr).All(v => v == true);
                });
                Assert.IsTrue(isSame.All(s => s == true));

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

        }

    }
}
