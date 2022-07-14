Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace HDF5CSharp.UnitTests.VB
    <TestClass>
    Public Class UnitTestNullables
        <TestMethod>
        Sub TestNullableClassSub()
            Dim fn = "TestNullableClassSub.h5"
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
            Dim fileID = Hdf5.CreateFile(fn)
            Hdf5.WriteObject(fileID, testClass, "testObject")
            Hdf5.CloseFile(fileID)
            Dim readObject As New TestClassWithArrayWithNulls()
            fileID = Hdf5.OpenFile(fn)
            readObject = Hdf5.ReadObject(fileID, readObject, "testObject")
            Hdf5.CloseFile(fileID)
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

        <TestMethod>
        Sub TestNullableSructsSub()
            Dim fn = "TestNullableSructsSub.h5"
            Dim testContainer = New TestContainerNullableStructs()

            ''Currently Passing
            'Default field values False or Null
            testContainer.TestFieldStructNull = Nothing
            testContainer.TestPropStructNull = Nothing
            'Initialized values
            testContainer.TestFieldStruct = TestStructWithNulls.Construct()
            testContainer.TestPropStruct = TestStructWithNulls.Construct()


            ''Currently Failing
            'Should be null
            testContainer.TestFieldStructNullableNull = Nothing '-> Read as default
            testContainer.TestPropStructNullableNull = Nothing '-> Read as default
            'Should have initialized values
            testContainer.TestFieldStructNullable = TestStructWithNulls.Construct() '-> Read as default
            testContainer.TestPropStructNullable = TestStructWithNulls.Construct() '-> Read as default

            Dim fileID = Hdf5.CreateFile(fn)
            Hdf5.WriteObject(fileID, testContainer, "testObject")
            Hdf5.CloseFile(fileID)
            fileID = Hdf5.OpenFile(fn)
            Dim readObject As New TestContainerNullableStructs()
            readObject = Hdf5.ReadObject(fileID, readObject, "testObject")
            Hdf5.CloseFile(fileID)
            Assert.IsTrue(testContainer.TestPropStructNullableNull Is Nothing And readObject.TestPropStructNullableNull Is Nothing)
            Assert.IsTrue(testContainer.TestPropStructNull.Equals(readObject.TestPropStructNull))
            Assert.IsTrue(testContainer.TestPropStructNullable.Equals(readObject.TestPropStructNullable))
            Assert.IsTrue(testContainer.TestPropStruct.Equals(readObject.TestPropStruct))

            Assert.IsTrue(testContainer.TestFieldStructNullableNull Is Nothing And readObject.TestFieldStructNullableNull Is Nothing)
            Assert.IsTrue(testContainer.TestFieldStructNull.Equals(readObject.TestFieldStructNull))
            Assert.IsTrue(testContainer.TestFieldStructNullable.Equals(readObject.TestFieldStructNullable))
            Assert.IsTrue(testContainer.TestFieldStruct.Equals(readObject.TestFieldStruct))
        End Sub
    End Class
End Namespace

