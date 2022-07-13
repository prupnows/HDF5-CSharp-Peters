Public Class TestClassWithArrayWithNulls
    Public Property TestBooleanNonNull As Boolean
    Public Property TestBooleanNull As Boolean
    Public Property TestBooleanNullableNonNull As Boolean?
    Public Property TestBooleanNullableNull As Boolean?
    Public TestFieldBooleanNonNull As Boolean
    Public TestFieldBooleanNull As Boolean
    Public TestFieldBooleanNullableNonNull As Boolean?
    Public TestFieldBooleanNullableNull As Boolean?

    Protected Overloads Function Equals(other As TestClassWithArrayWithNulls) As Boolean
        Return TestFieldBooleanNonNull.Equals(other.TestFieldBooleanNonNull) AndAlso TestFieldBooleanNull.Equals(other.TestFieldBooleanNull) AndAlso TestFieldBooleanNullableNonNull.Equals(other.TestFieldBooleanNullableNonNull) AndAlso TestFieldBooleanNullableNull.Equals(other.TestFieldBooleanNullableNull) AndAlso TestBooleanNonNull.Equals(other.TestBooleanNonNull) AndAlso TestBooleanNull.Equals(other.TestBooleanNull) AndAlso TestBooleanNullableNonNull.Equals(other.TestBooleanNullableNonNull) AndAlso TestBooleanNullableNull.Equals(other.TestBooleanNullableNull)
    End Function

    Public Overloads Overrides Function Equals(obj As Object) As Boolean
        If ReferenceEquals(Nothing, obj) Then Return False
        If ReferenceEquals(Me, obj) Then Return True
        If obj.GetType IsNot Me.GetType Then Return False
        Return Equals(DirectCast(obj, TestClassWithArrayWithNulls))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return HashCode.Combine(TestFieldBooleanNonNull, TestFieldBooleanNull, TestFieldBooleanNullableNonNull, TestFieldBooleanNullableNull, TestBooleanNonNull, TestBooleanNull, TestBooleanNullableNonNull, TestBooleanNullableNull)
    End Function
End Class
