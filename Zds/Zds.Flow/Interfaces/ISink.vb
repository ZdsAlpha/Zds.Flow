Namespace Machinery
    Public Interface ISink(Of Input)
        Property Recursive As Boolean
        Function Receive(obj As Input) As Boolean
    End Interface
End Namespace