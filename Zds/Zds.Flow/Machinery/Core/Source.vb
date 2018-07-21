Namespace Machinery.Core
    Public MustInherit Class Source(Of Output)
        Implements ISource(Of Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Generate As GenerateDelegate
        Public Property Dropping As Boolean = True Implements ISource(Of Output).Dropping
        Public MustOverride Sub Activate() Implements ISource(Of Output).Activate
        Sub New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            Me.Generate = Generate
        End Sub

        Public Delegate Function GenerateDelegate(ByRef obj As Output) As Boolean
    End Class
End Namespace