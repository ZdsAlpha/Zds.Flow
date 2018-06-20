Imports System.Threading

Namespace Updatables
    <DebuggerStepThrough>
    Public Class AsyncObject
        Inherits Updatable
        Implements IAsyncObject
        Public Event AsyncUpdateEvent(Sender As AsyncObject)
        Public ReadOnly Property ActiveThreads As Integer
        Public Property MaxThreads As Integer = -1
        Protected Overrides Sub OnUpdated()
            MyBase.OnUpdated()
            If _MaxThreads = -1 OrElse _ActiveThreads < _MaxThreads Then
                Interlocked.Increment(_ActiveThreads)
                Try
                    If _MaxThreads = -1 OrElse _ActiveThreads <= _MaxThreads Then AsyncUpdate()
                Catch ex As Exception
                    Handle(ex)
                End Try
                Interlocked.Decrement(_ActiveThreads)
            End If
        End Sub
        Protected Overridable Sub AsyncUpdate() Implements IAsyncObject.AsyncUpdate
            RaiseEvent AsyncUpdateEvent(Me)
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            Updater.Add(Me)
        End Sub
        Sub New(AsyncUpdate As AsyncUpdateEventEventHandler)
            AddHandler AsyncUpdateEvent, AsyncUpdate
        End Sub
        Sub New(Updater As Updaters.IUpdater, AsyncUpdate As AsyncUpdateEventEventHandler)
            Updater.Add(Me)
            AddHandler AsyncUpdateEvent, AsyncUpdate
        End Sub
    End Class
End Namespace