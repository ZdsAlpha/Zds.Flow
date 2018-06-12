Namespace Collections
    Public Interface IStream(Of T)
        Function Write(Objects As T(), Start As Integer, Length As Integer) As Integer
        Function Read(Objects As T(), Start As Integer, Length As Integer) As Integer
    End Interface
End Namespace