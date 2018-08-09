Namespace Machinery.Systems
    Public MustInherit Class Sink(Of Input)
        Inherits Machinery
        Implements ISink(Of Input)
        Public MustOverride Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
    End Class
End Namespace