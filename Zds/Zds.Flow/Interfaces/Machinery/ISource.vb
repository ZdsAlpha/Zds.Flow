Namespace Machinery
    Public Interface ISource(Of Output)
        Inherits Interfaces.IActivatable, Interfaces.IDestroyable
        Property Sink As ISink(Of Output)
    End Interface
End Namespace