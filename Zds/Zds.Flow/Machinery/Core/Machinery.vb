Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Machinery
        Implements IDestroyable, IActivatable
        Public ReadOnly Property IsDestroyed As Boolean Implements IDestroyable.IsDestroyed
        Public MustOverride Sub Activate() Implements IActivatable.Activate
        Public Overridable Sub Destroy() Implements IDestroyable.Destroy
            _IsDestroyed = True
        End Sub
    End Class
End Namespace