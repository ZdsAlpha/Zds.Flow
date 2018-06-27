Namespace Machinery
    Public Interface ISource(Of Output)
        Inherits Interfaces.IActivatable
        Property Sink As ISink(Of Output)
        Property Dropping As Boolean
    End Interface
End Namespace