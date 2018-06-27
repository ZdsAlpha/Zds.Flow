Namespace Machinery
    Public Interface ISource(Of Output)
        Property Sink As ISink(Of Output)
        Property Dropping As Boolean
    End Interface
End Namespace