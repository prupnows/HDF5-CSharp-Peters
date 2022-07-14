Public Structure TestStructWithNulls
    Public BooleanTrue, BooleanFalse, BooleanNull As Boolean
    Public BooleanNullableTrue, BooleanNullableFalse, BooleanNullableNull As Boolean?

    Public Shared Function Construct() As TestStructWithNulls
        Dim testStruct As New TestStructWithNulls
        testStruct.BooleanTrue = True
        testStruct.BooleanFalse = False
        testStruct.BooleanNull = Nothing '== False
        testStruct.BooleanNullableTrue = True
        testStruct.BooleanNullableFalse = False
        testStruct.BooleanNullableNull = Nothing
        Return testStruct
    End Function
End Structure

Public Class TestContainerNullableStructs
    Public Property TestPropStructNullableNull As TestStructWithNulls?
    Public Property TestPropStructNull As TestStructWithNulls
    Public Property TestPropStructNullable As TestStructWithNulls?
    Public Property TestPropStruct As TestStructWithNulls
    Public TestFieldStructNullableNull As TestStructWithNulls?
    Public TestFieldStructNull As TestStructWithNulls
    Public TestFieldStructNullable As TestStructWithNulls?
    Public TestFieldStruct As TestStructWithNulls
End Class