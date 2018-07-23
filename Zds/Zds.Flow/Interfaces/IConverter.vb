Namespace Machinery
    Public Interface IConverter(Of Input, Output)
        Inherits Interfaces.IActivatable
        Inherits ISink(Of Input), ISource(Of Output)
    End Interface
End Namespace