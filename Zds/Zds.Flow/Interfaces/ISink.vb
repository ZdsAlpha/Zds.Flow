Namespace Machinery
    Public Interface ISink(Of Input)
        Inherits Interfaces.IActivatable
        Property Recursive As Boolean
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace