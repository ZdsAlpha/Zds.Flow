Namespace Machinery
    Public Interface ISource
        Inherits Interfaces.IActivatable, Interfaces.IDestroyable
    End Interface
    Public Interface ISource(Of Output)
        Inherits ISource
        Property Sink As ISink(Of Output)
    End Interface
End Namespace