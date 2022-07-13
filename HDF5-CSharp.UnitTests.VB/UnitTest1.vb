Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace HDF5_CSharp.UnitTests.VB
    <TestClass>
    Public Class UnitTest1
        <TestMethod>
        Sub TestNullableSub()
            Dim fn = "TestSub.h5"
            Dim testClass = New TestClassWithArrayWithNulls() With {
                    .TestBooleanNonNull = True,          'Reads true
                    .TestBooleanNull = Nothing,          'Reads false
                    .TestBooleanNullableNonNull = True,  'Reads null <- Should read true
                    .TestBooleanNullableNull = Nothing,   'Reads null
                    .TestFieldBooleanNonNull = True,          'Reads true
                    .TestFieldBooleanNull = Nothing,          'Reads false
                    .TestFieldBooleanNullableNonNull = True,  'Reads null <- Should read true
                    .TestFieldBooleanNullableNull = Nothing   'Reads null
                    }
            Dim fileID = HDF5CSharp.Hdf5.CreateFile(fn)
            HDF5CSharp.Hdf5.WriteObject(fileID, testClass, "testObject")
            HDF5CSharp.Hdf5.CloseFile(fileID)
            Dim readObject As New TestClassWithArrayWithNulls()
            fileID = HDF5CSharp.Hdf5.OpenFile(fn)
            readObject = HDF5CSharp.Hdf5.ReadObject(fileID, readObject, "testObject")
            HDF5CSharp.Hdf5.CloseFile(fileID)
            Assert.IsTrue(readObject.TestBooleanNonNull = testClass.TestBooleanNonNull)
            Assert.IsTrue(readObject.TestBooleanNull = testClass.TestBooleanNull)
            Assert.IsTrue(readObject.TestBooleanNullableNonNull = testClass.TestBooleanNullableNonNull)
            Assert.IsTrue(readObject.TestBooleanNullableNull Is Nothing And testClass.TestBooleanNullableNull Is Nothing)

            Assert.IsTrue(readObject.TestFieldBooleanNonNull = testClass.TestFieldBooleanNonNull)
            Assert.IsTrue(readObject.TestFieldBooleanNull = testClass.TestFieldBooleanNull)
            Assert.IsTrue(readObject.TestFieldBooleanNullableNonNull = testClass.TestFieldBooleanNullableNonNull)
            Assert.IsTrue(readObject.TestFieldBooleanNullableNull Is Nothing And testClass.TestFieldBooleanNullableNull Is Nothing)


            Assert.IsTrue(readObject.Equals(testClass))

        End Sub
    End Class
End Namespace

