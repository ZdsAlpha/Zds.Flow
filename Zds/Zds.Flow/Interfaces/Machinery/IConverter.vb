Namespace Machinery
    Public Interface IConverter
        Inherits Interfaces.IActivatable
        Inherits ISink, ISource
    End Interface
    Public Interface IConverter(Of Input, Output)
        Inherits IConverter
        Inherits ISink(Of Input), ISource(Of Output)
    End Interface
End Namespace