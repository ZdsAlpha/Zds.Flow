Namespace Machinery.Systems
    Public MustInherit Class Converter(Of Input, Output)
        Inherits Machinery
        Implements ISink(Of Input), ISource(Of Output)
        Public MustOverride Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public MustOverride Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
    End Class
End Namespace