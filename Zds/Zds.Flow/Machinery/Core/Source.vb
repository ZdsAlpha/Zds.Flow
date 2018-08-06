Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Source(Of Output)
        Implements ISource(Of Output)
        Public ReadOnly Property IsDestroyed As Boolean Implements IDestroyable.IsDestroyed
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Generate As GenerateDelegate
        Public Property Dropping As Boolean = False
        Public MustOverride Sub Activate() Implements ISource(Of Output).Activate
        Private Sub Destroy() Implements IDestroyable.Destroy

        End Sub
        Sub New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            Me.Generate = Generate
        End Sub

        Public Delegate Function GenerateDelegate(ByRef obj As Output) As Boolean
    End Class
End Namespace