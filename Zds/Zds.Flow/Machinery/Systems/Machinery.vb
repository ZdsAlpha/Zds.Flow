Imports Zds.Flow.Updatables

Namespace Machinery.Systems
    Public MustInherit Class Machinery
        Inherits AsyncSystem
        Implements IMachinery
        Private UnsafeObjects As New List(Of Object)
        Protected Sub Register(Obj As Object)
            SyncLock UnsafeObjects
                If IsDestroyed Then
                    Destroy(Obj)
                Else
                    UnsafeObjects.Add(Obj)
                End If
            End SyncLock
        End Sub
        Protected Sub Unregister(Obj As Object)
            SyncLock UnsafeObjects
                UnsafeObjects.Remove(Obj)
            End SyncLock
        End Sub
        Public Overrides Sub Start()
            MyBase.Start()
            If IsRunning Then
                For Each Updatable In Targets
                    Updatable.Start()
                Next
            End If
        End Sub
        Public Overrides Sub [Stop]()
            MyBase.Stop()
            If Not IsRunning Then
                For Each Updatable In Targets
                    Updatable.Stop()
                Next
            End If
        End Sub
        Protected Overridable Overloads Sub Destroy(Obj As Object)
            Dim Disposable As IDisposable = TryCast(Obj, IDisposable)
            If Disposable IsNot Nothing Then Disposable.Dispose()
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            SyncLock UnsafeObjects
                For Each Obj In UnsafeObjects
                    Destroy(Obj)
                Next
            End SyncLock
        End Sub
    End Class
End Namespace