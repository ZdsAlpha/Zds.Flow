Namespace Updatables
    Public Class SyncContainer
        Inherits SyncSystem
        Implements ISyncContainer
        Public Overloads ReadOnly Property Targets As IUpdatable()
            Get
                Return MyBase.Targets
            End Get
        End Property
        Public Overloads Property DelayHandler As DelayHandling.IDelayHandler
            Get
                Return MyBase.DelayHandler
            End Get
            Set(value As DelayHandling.IDelayHandler)
                MyBase.DelayHandler = value
            End Set
        End Property
        Public Overloads Sub Add(Updatable As IUpdatable)
            MyBase.Add(Updatable)
        End Sub
        Public Overloads Sub Remove(Updatable As IUpdatable)
            MyBase.Remove(Updatable)
        End Sub
        Public Overloads Sub Clear()
            MyBase.Clear()
        End Sub
        Public Overloads Sub UpdateCache()
            MyBase.UpdateCache()
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
        End Sub
    End Class
End Namespace