Namespace Interfaces
    Public Interface IInput(Of T)
        Function Enqueue(ByRef obj As T) As Boolean
    End Interface
End Namespace