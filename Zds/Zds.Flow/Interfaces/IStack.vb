Namespace Collections
    Public Interface IStack(Of T)
        Function Push(ByRef Obj As T) As Boolean
        Function Pop(ByRef Obj As T) As Boolean
    End Interface
End Namespace