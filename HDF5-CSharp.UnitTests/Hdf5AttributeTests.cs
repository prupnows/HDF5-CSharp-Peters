using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HDF5CSharp.DataTypes;

namespace HDF5CSharp.UnitTests.Core
{
    public partial class Hdf5UnitTests
    {
        [TestMethod]
        public void WriteAndReadAttributeByPath()
        {
            string filename = Path.Combine(folder, "testAttributeByPath.H5");
            string path = "/A/B/C/D/E/F/I";
            string attributeValue = "test";
            Hdf5.Settings.LowerCaseNaming = false;
            var fileId = Hdf5.CreateFile(filename);
            Assert.IsTrue(fileId > 0);
            var groupId = Hdf5.CreateGroupRecursively(fileId, Hdf5Utils.NormalizedName(path));
            var result = Hdf5Utils.WriteAttributeByPath(filename, path, "VALID", attributeValue);
            Assert.IsTrue(result);
            var write = Hdf5Utils.ReadAttributeByPath(filename, path, "VALID");
            Assert.IsTrue(write.success);
            Assert.IsTrue(write.value == attributeValue);
            Assert.IsTrue(H5G.close(groupId) == 0);
            Assert.IsTrue(Hdf5.CloseFile(fileId) == 0);
        }

        [TestMethod]
        public void WriteAndReadAttribute()
        {
            string filename = Path.Combine(folder, "testAttribute.H5");
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var groupId = Hdf5.CreateOrOpenGroup(fileId, "test");
                DateTime nowTime = DateTime.Now;
                Hdf5.WriteAttribute(groupId, "time", nowTime);
                Hdf5.WriteAttributes<DateTime>(groupId, "times", new List<DateTime> { nowTime, nowTime.AddDays(1) }.ToArray());

                DateTime readTime = Hdf5.ReadAttribute<DateTime>(groupId, "time");
                var allTimes = Hdf5.ReadAttributes<DateTime>(groupId, "times");

                Assert.IsTrue(readTime == nowTime);
                Assert.IsTrue(Hdf5.CloseFile(fileId) == 0);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }

        [TestMethod]
        public void WriteReadAndEditAttribute()
        {
            string filename = Path.Combine(folder, "WriteReadAndEditAttribute.H5");
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var groupId = Hdf5.CreateOrOpenGroup(fileId, "test");
                DateTime nowTime = DateTime.Now;
                Hdf5.WriteAttribute(groupId, "time", nowTime);
                DateTime readTime = Hdf5.ReadAttribute<DateTime>(groupId, "time");
                Assert.IsTrue(readTime == nowTime);
                Hdf5.CloseFile(fileId);
                fileId = Hdf5.OpenFile(filename, false);
                readTime = Hdf5.ReadAttribute<DateTime>(groupId, "time");
                Assert.IsTrue(readTime == nowTime);

                nowTime = DateTime.Now;
                Hdf5.WriteAttribute(groupId, "time", nowTime);
                readTime = Hdf5.ReadAttribute<DateTime>(groupId, "time");
                Assert.IsTrue(readTime == nowTime);
                Hdf5.CloseFile(fileId);

                fileId = Hdf5.OpenFile(filename, false);
                readTime = Hdf5.ReadAttribute<DateTime>(groupId, "time");
                Assert.IsTrue(readTime == nowTime);
                Hdf5.CloseFile(fileId);

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }

        [TestMethod]
        public void WriteAndReadStringAttribute()
        {
            string filename = Path.Combine(folder, "testAttributeString.H5");
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var groupId = Hdf5.CreateOrOpenGroup(fileId, "test");

                string attrStr = "this is an attribute";
                Hdf5.WriteAttribute(groupId, "time", attrStr);
                string readStr = Hdf5.ReadAttribute<string>(groupId, "time_Non_Exist");
                Assert.IsTrue(string.IsNullOrEmpty(readStr));
                readStr = Hdf5.ReadAttribute<string>(groupId, "time");
                Assert.IsTrue(readStr == attrStr);
                Assert.IsTrue(H5G.close(groupId) == 0);
                Assert.IsTrue(Hdf5.CloseFile(fileId) == 0);
                ErrorCountExpected = 2;
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }

        //[TestMethod]
        //public void t()
        //{
        //    Directory.CreateDirectory("c:/测试");
        //    var fileId = Hdf5.CreateFile("c:/测试/test测试.h5");
        //    Hdf5.CloseFile(fileId);
        //}

        [TestMethod]
        public void WriteAndReadAttributesWithBothSaveAndReadAttributes()
        {
            Hdf5.Settings.EnableThrowOnErrors(false);
            Hdf5.Settings.EnableH5InternalErrorReporting(false);
            Hdf5.Settings.EnableLogging(false);
            string filename = Path.Combine(folder, $"{nameof(WriteAndReadAttributesWithBothSaveAndReadAttributes)}.H5");
            int value = 10;
            SaveReadAttributeClass testClass = new SaveReadAttributeClass(value);
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                Hdf5.WriteObject(fileId, testClass);
                Hdf5.CloseFile(fileId);
                fileId = Hdf5.OpenFile(filename);
                Assert.IsTrue(fileId > 0);
                SaveReadAttributeClass readObject = new SaveReadAttributeClass();
                readObject = Hdf5.ReadObject(fileId, readObject, "/test_object");
                Assert.IsTrue(readObject.TestIntDoNotRead==0);
                Assert.IsTrue(readObject.TestIntReadWrite == value);
                Assert.IsTrue(readObject.TestIntReadOnly == 0);
                Assert.IsTrue(readObject.TestIntNoAttribute == value);
                Assert.IsTrue(readObject.TestIntReadOnlyOfOtherProperty == value);

            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
            finally
            {
                Hdf5.Settings.EnableThrowOnErrors(true);
                Hdf5.Settings.EnableH5InternalErrorReporting(true);
                Hdf5.Settings.EnableLogging(true);

            }
        }

        [TestMethod]
        public void WriteAndReadAttributes()
        {
            string filename = Path.Combine(folder, "testAttributes.H5");
            int[] intValues = new[] { 1, 2 };
            double dblValue = 1.1;
            string strValue = "test";
            string[] strValues = new string[2] { "test", "another test" };
            bool boolValue = true;
            DateTime dateValue = new DateTime(1969, 1, 12);
            var groupStr = "/test";

            //string concatFunc(string x) => string.Concat(groupStr, "/", x);
            string intName = nameof(intValues);
            string dblName = nameof(dblValue);
            string strName = nameof(strValue);
            string strNames = nameof(strValues);
            string boolName = nameof(boolValue);
            string dateName = nameof(dateValue);

            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                var groupId = Hdf5.CreateOrOpenGroup(fileId, groupStr);
                Hdf5.WriteAttributes<int>(groupId, intName, intValues);
                Hdf5.WriteAttribute(groupId, dblName, dblValue);
                Hdf5.WriteAttribute(groupId, strName, strValue);
                Hdf5.WriteAttributes<string>(groupId, strNames, strValues);
                Hdf5.WriteAttribute(groupId, boolName, boolValue);
                Hdf5.WriteAttribute(groupId, dateName, dateValue);
                H5G.close(groupId);
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
                var groupId = H5G.open(fileId, groupStr);
                IEnumerable<int> readInts = (int[])Hdf5.ReadAttributes<int>(groupId, intName).result;
                Assert.IsTrue(intValues.SequenceEqual(readInts));
                double readDbl = Hdf5.ReadAttribute<double>(groupId, dblName);
                Assert.IsTrue(dblValue == readDbl);
                string readStr = Hdf5.ReadAttribute<string>(groupId, strName);
                Assert.IsTrue(strValue == readStr);
                IEnumerable<string> readStrs = (string[])Hdf5.ReadAttributes<string>(groupId, strNames).result;
                Assert.IsTrue(strValues.SequenceEqual(readStrs));
                bool readBool = Hdf5.ReadAttribute<bool>(groupId, boolName);
                Assert.IsTrue(boolValue == readBool);
                DateTime readDate = Hdf5.ReadAttribute<DateTime>(groupId, dateName);
                Assert.IsTrue(dateValue == readDate);
                H5G.close(groupId);
                Hdf5.CloseFile(fileId);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }

        [TestMethod]
        public void WriteAndReadObjectWithHdf5Attributes()
        {
            string groupName = "anObject";
            string filename = Path.Combine(folder, "testHdf5Attribute.H5");
            var attObject = new AttributeClass();
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                Hdf5.WriteObject(fileId, attObject, groupName);
                Assert.IsTrue(Hdf5.CloseFile(fileId) == 0);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            var OpenFileId = Hdf5.OpenFile(filename);
            var data = Hdf5.ReadObject<AttributeClass>(OpenFileId, groupName);
            Assert.IsTrue(Math.Abs(data.noAttribute - 10.0f) < 0.001);
        }

        [TestMethod]
        public void WriteOverrideAndReadObjectWithHdf5Attributes()
        {
            string groupName = "simpleObject";
            string filename = Path.Combine(folder, "testSimpleHdf5Attribute.H5");
            var attObject = new AttributeSimpleClass();
            attObject.SetStringProperty("new value");
            try
            {
                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);
                Hdf5.WriteObject(fileId, attObject, groupName);
                Assert.IsTrue(Hdf5.CloseFile(fileId) == 0);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }

            var openFileId = Hdf5.OpenFile(filename);
            var data = Hdf5.ReadObject<AttributeSimpleClass>(openFileId, groupName);
            Hdf5.CloseFile(openFileId);
            Assert.IsTrue(data.Equals(attObject));

            attObject.SetStringProperty("third");
            attObject.datetime = DateTime.Now;
            openFileId = Hdf5.OpenFile(filename);
            Hdf5.WriteObject(openFileId, attObject, groupName);
            Assert.IsTrue(Hdf5.CloseFile(openFileId) == 0);

            openFileId = Hdf5.OpenFile(filename);
            data = Hdf5.ReadObject<AttributeSimpleClass>(openFileId, groupName);
            Hdf5.CloseFile(openFileId);
            Assert.IsTrue(data.Equals(attObject));
            File.Delete(filename);
        }

        [TestMethod]
        public void TestReadFullTree()
        {
            string filename = Path.Combine(folder, "files", "testfile2.H5");
            var results = Hdf5.ReadTreeFileStructure(filename);

        }
    }
}
