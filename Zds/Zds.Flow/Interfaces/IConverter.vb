Namespace Machinery
    Public Interface IConverter(Of Input, Output)
        Inherits ISink(Of Input), ISource(Of Output)
    End Interface
End Namespace