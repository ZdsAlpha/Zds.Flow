Namespace Machinery
    Public Interface ISource(Of T)
        Property Sink As ISink(Of T)
    End Interface
End Namespace