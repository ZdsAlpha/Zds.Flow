Namespace Machinery.Systems
    Public MustInherit Class Source(Of Output)
        Inherits Machinery
        Implements ISource(Of Output)
        Public MustOverride Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
    End Class
End Namespace