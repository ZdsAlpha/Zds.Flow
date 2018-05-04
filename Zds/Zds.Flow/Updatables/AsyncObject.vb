Namespace Updatables
    <DebuggerStepThrough>
    Public Class AsyncObject
        Inherits Updatable
        Implements IAsyncObject
        Public Event AsyncUpdateEvent(Sender As AsyncObject)
        Protected Overrides Sub OnUpdated()
            MyBase.OnUpdated()
            AsyncUpdate()
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