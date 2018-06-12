Namespace Machinery
    Public Interface ISink(Of Input)
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace