Namespace Updatables
    <DebuggerStepThrough>
    Public Class SyncObject
        Inherits Updatable
        Implements ISyncObject
        Public Event SyncUpdateEvent(Sender As SyncObject)
        Private _IsLocked As Boolean
        Protected ReadOnly Property Lock As New Object
        Public ReadOnly Property IsLocked As Boolean Implements ISyncObject.IsLocked
            Get
                Return _IsLocked
            End Get
        End Property
        Protected Overrides Sub OnUpdated()
            MyBase.OnUpdated()
            If Not _IsLocked AndAlso Threading.Monitor.TryEnter(_Lock) Then
                _IsLocked = True
                Try
                    SyncUpdate()
                    _IsLocked = False
                    Threading.Monitor.Exit(_Lock)
                Catch ex As Exception
                    _IsLocked = False
                    Threading.Monitor.Exit(_Lock)
                    Handle(ex)
                End Try
            End If
        End Sub
        Protected Overridable Sub SyncUpdate() Implements ISyncObject.SyncUpdate
            RaiseEvent SyncUpdateEvent(Me)
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
        End Sub
        Sub New(SyncUpdate As SyncUpdateEventEventHandler)
            AddHandler SyncUpdateEvent, SyncUpdate
        End Sub
        Sub New(Updater As Updaters.IUpdater, SyncUpdate As SyncUpdateEventEventHandler)
            MyBase.New(Updater)
            AddHandler SyncUpdateEvent, SyncUpdate
        End Sub
    End Class
End Namespace