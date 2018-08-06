Namespace Machinery
    Public Interface ISink(Of Input)
        Inherits Interfaces.IActivatable, Interfaces.IDestroyable
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace