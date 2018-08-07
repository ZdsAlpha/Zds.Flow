Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Base
        Implements IDestroyable, IActivatable
        Public ReadOnly Property IsDestroyed As Boolean Implements IDestroyable.IsDestroyed
        Public MustOverride Sub Activate() Implements IActivatable.Activate
        Public Overridable Sub Destroy() Implements IDestroyable.Destroy
            _IsDestroyed = True
        End Sub

        Protected Shared Sub Discard(Obj As Object)
            Dim Disposable As IDisposable = TryCast(Obj, IDisposable)
            If Disposable IsNot Nothing Then Disposable.Dispose()
        End Sub
        Protected Shared Sub Destroy(Of T)(Round As Collections.Round(Of T))
            While Round.Length <> 0
                Dim Elements As T()
                SyncLock Round.Lock
                    Elements = Round.ToArray
                    Round.Clear()
                End SyncLock
                For Each Element In Elements
                    Discard(Element)
                Next
            End While
        End Sub
    End Class
End Namespace