Namespace Collections
    Public Interface IRound(Of T)
        ReadOnly Property Size As Integer
        ReadOnly Property Length As Integer
        Function SetSize(Size As Integer, Forced As Boolean) As Boolean
        Function SetLength(Length As Integer, AutoResize As Boolean) As Boolean
        Function CopyTo(Destination As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
        Function CopyFrom(Source As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
        Function ExtendFirst(Count As Integer) As Integer
        Function ExtendLast(Count As Integer) As Integer
        Function ShrinkFirst(Count As Integer) As Integer
        Function ShrinkLast(Count As Integer) As Integer
        Function AddFirst(Element As T, Overwrite As Boolean) As Boolean
        Function AddLast(Element As T, Overwrite As Boolean) As Boolean
        Function AddFirst(Source As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
        Function AddLast(Source As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
        Function RemoveFirst(ByRef Element As T) As Boolean
        Function RemoveLast(ByRef Element As T) As Boolean
        Function RemoveFirst(Destination As T(), Start As Integer, Length As Integer) As Integer
        Function RemoveLast(Destination As T(), Start As Integer, Length As Integer) As Integer
        Function ElementAt(Index As Integer, ByRef Element As T) As Boolean
        Sub Clear()
        Function ToArray() As T()
    End Interface
End Namespace