Namespace Machinery
    Public Interface ISink
    End Interface
    Public Interface ISink(Of Input)
        Inherits ISink
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace