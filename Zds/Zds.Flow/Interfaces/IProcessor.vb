Namespace Machinery
    Public Interface IProcessor(Of Input, Output)
        Inherits ISink(Of Input), ISource(Of Output)
    End Interface
End Namespace