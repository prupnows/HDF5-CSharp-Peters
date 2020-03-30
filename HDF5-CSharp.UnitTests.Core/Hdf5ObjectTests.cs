using System;
using System.IO;
using HDF5CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hdf5DotnetWrapper.UnitTests.Core
{
    public partial class Hdf5UnitTests
    {
        [TestMethod]
        public void WriteAndReadObjectWithPropertiesTest()
        {
            string filename = Path.Combine(folder, "testObjects.H5");
            try
            {
                testClass.TestInteger = 2;
                testClass.TestDouble = 1.1;
                testClass.TestBoolean = true;
                testClass.TestString = "test string";
                // 31-Oct-2003, 18:00 is  731885.75 in matlab
                testClass.TestTime = new DateTime(2003, 10, 31, 18, 0, 0);

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId > 0);

                Hdf5.WriteObject(fileId, testClass, "objectWithProperties");
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

                TestClass readObject = new TestClass();
                readObject = Hdf5.ReadObject(fileId, readObject, "objectWithProperties");
                Assert.IsTrue(testClass.Equals(readObject));

                readObject = Hdf5.ReadObject<TestClass>(fileId, "objectWithProperties");
                Assert.IsTrue(testClass.Equals(readObject));

                Assert.IsTrue(Hdf5.CloseFile(fileId) >= 0);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }

        [TestMethod]
        public void WriteAndReadObjectWithPropertiesAndArrayPropertyTest()
        {
            try
            {
                testClassWithArrays.TestInteger = 2;
                testClassWithArrays.TestDouble = 1.1;
                testClassWithArrays.TestBoolean = true;
                testClassWithArrays.TestString = "test string";
                testClassWithArrays.TestDoubles = new[] { 1.1, 1.2, -1.1, -1.2 };
                testClassWithArrays.TestStrings = new[] { "one", "two", "three", "four" };
                testClassWithArrays.testDoublesField = new[] { 1.1, 1.2, -1.1, -1.2 };
                testClassWithArrays.testStringsField = new[] { "one", "two", "three", "four" };
                string filename = Path.Combine(folder, "testArrayObjects.H5");

                var fileId = Hdf5.CreateFile(filename);
                Assert.IsTrue(fileId >= 0);

                Hdf5.WriteObject(fileId, testClassWithArrays, "objectWithTwoArrays");

                TestClassWithArray readObject = new TestClassWithArray
                {
                    TestStrings = new string[0],
                    TestDoubles = null,
                    TestDouble = double.NaN
                };

                readObject = Hdf5.ReadObject(fileId, readObject, "objectWithTwoArrays");
                Assert.IsTrue(testClassWithArrays.Equals(readObject));

                readObject = Hdf5.ReadObject<TestClassWithArray>(fileId, "objectWithTwoArrays");
                Assert.IsTrue(testClassWithArrays.Equals(readObject));

                Assert.IsTrue(Hdf5.CloseFile(fileId) >= 0);
            }
            catch (Exception ex)
            {
                CreateExceptionAssert(ex);
            }
        }
    }
}
