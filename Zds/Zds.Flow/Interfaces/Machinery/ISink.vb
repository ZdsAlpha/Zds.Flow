Namespace Machinery
    Public Interface ISink
        Inherits Interfaces.IActivatable, Interfaces.IDestroyable
    End Interface
    Public Interface ISink(Of Input)
        Inherits ISink
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace