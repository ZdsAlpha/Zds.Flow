Imports Zds.Flow.Collections
Namespace Updatables
    <DebuggerStepThrough>
    Public Class SyncObject
        Inherits Updatable
        Implements ISyncObject
        Private _SyncUpdateEvent As SafeList(Of SyncObjectEventDelegate)
        Public Custom Event SyncUpdateEvent As SyncObjectEventDelegate
            AddHandler(value As SyncObjectEventDelegate)
                If _SyncUpdateEvent Is Nothing Then _SyncUpdateEvent = New SafeList(Of SyncObjectEventDelegate)
                _SyncUpdateEvent.Add(value)
            End AddHandler
            RemoveHandler(value As SyncObjectEventDelegate)
                If _SyncUpdateEvent IsNot Nothing Then _SyncUpdateEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As ISyncObject)
                If _SyncUpdateEvent IsNot Nothing Then
                    For Each [Delegate] In _SyncUpdateEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
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
        Sub New(SyncUpdate As SyncObjectEventDelegate)
            AddHandler SyncUpdateEvent, SyncUpdate
        End Sub
        Sub New(Updater As Updaters.IUpdater, SyncUpdate As SyncObjectEventDelegate)
            MyBase.New(Updater)
            AddHandler SyncUpdateEvent, SyncUpdate
        End Sub
        Public Delegate Sub SyncObjectEventDelegate(Sender As ISyncObject)
    End Class
End Namespace