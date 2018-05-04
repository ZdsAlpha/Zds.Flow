Namespace Interfaces
    Public Interface IQueue(Of T)
        Function Enqueue(ByRef Obj As T) As Boolean
        Function Dequeue(ByRef Obj As T) As Boolean
    End Interface
End Namespace