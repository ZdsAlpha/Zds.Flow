Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Base
        Implements IDestroyable, IActivatable
        Public ReadOnly Property IsDestroyed As Boolean Implements IDestroyable.IsDestroyed
        Public MustOverride Sub Activate() Implements IActivatable.Activate
        Public Overridable Sub Discard(Obj As Object)
            Dim Disposable As IDisposable = TryCast(Obj, IDisposable)
            If Disposable IsNot Nothing Then Disposable.Dispose()
        End Sub
        Public Overridable Sub Destroy() Implements IDestroyable.Destroy
            _IsDestroyed = True
        End Sub
    End Class
End Namespace