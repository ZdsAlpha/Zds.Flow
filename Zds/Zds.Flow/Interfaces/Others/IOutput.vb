Namespace Interfaces
    Public Interface IOutput(Of T)
        Function Output(ByRef obj As T) As Boolean
    End Interface
End Namespace