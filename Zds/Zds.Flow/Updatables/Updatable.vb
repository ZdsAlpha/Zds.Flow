Imports Zds.Flow.Collections
Namespace Updatables
    <DebuggerStepThrough>
    Public Class Updatable
        Implements IUpdatable
        Private _OnException As SafeList(Of ExceptionHandling.IThrowsException.OnExceptionDelegate)
        Private _PreUpdateEvent As SafeList(Of UpdatableEventDelegate)
        Private _PostUpdateEvent As SafeList(Of UpdatableEventDelegate)
        Private _OnDestroyedEvent As SafeList(Of UpdatableEventDelegate)
        Private _OnStartedEvent As SafeList(Of UpdatableEventDelegate)
        Private _OnStoppedEvent As SafeList(Of UpdatableEventDelegate)
        Private _IsDestroyed As Boolean = False
        Private _IsRunning As Boolean = False
        Private _Updater As Updaters.IUpdater
        Public Custom Event OnException As ExceptionHandling.IThrowsException.OnExceptionDelegate Implements ExceptionHandling.IThrowsException.OnException
            AddHandler(value As ExceptionHandling.IThrowsException.OnExceptionDelegate)
                If _OnException Is Nothing Then _OnException = New SafeList(Of ExceptionHandling.IThrowsException.OnExceptionDelegate)
                _OnException.Add(value)
            End AddHandler
            RemoveHandler(value As ExceptionHandling.IThrowsException.OnExceptionDelegate)
                If _OnException IsNot Nothing Then _OnException.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As Object, Exception As Exception)
                If _OnException IsNot Nothing Then
                    For Each [Delegate] In _OnException.Elements
                        [Delegate].Invoke(Sender, Exception)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event PreUpdateEvent As UpdatableEventDelegate
            AddHandler(value As UpdatableEventDelegate)
                If _PreUpdateEvent Is Nothing Then _PreUpdateEvent = New SafeList(Of UpdatableEventDelegate)
                _PreUpdateEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdatableEventDelegate)
                If _PreUpdateEvent IsNot Nothing Then _PreUpdateEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IUpdatable)
                If _PreUpdateEvent IsNot Nothing Then
                    For Each [Delegate] In _PreUpdateEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event PostUpdateEvent As UpdatableEventDelegate
            AddHandler(value As UpdatableEventDelegate)
                If _PostUpdateEvent Is Nothing Then _PostUpdateEvent = New SafeList(Of UpdatableEventDelegate)
                _PostUpdateEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdatableEventDelegate)
                If _PostUpdateEvent IsNot Nothing Then _PostUpdateEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IUpdatable)
                If _PostUpdateEvent IsNot Nothing Then
                    For Each [Delegate] In _PostUpdateEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event OnDestroyedEvent As UpdatableEventDelegate
            AddHandler(value As UpdatableEventDelegate)
                If _OnDestroyedEvent Is Nothing Then _OnDestroyedEvent = New SafeList(Of UpdatableEventDelegate)
                _OnDestroyedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdatableEventDelegate)
                If _OnDestroyedEvent IsNot Nothing Then _OnDestroyedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IUpdatable)
                If _OnDestroyedEvent IsNot Nothing Then
                    For Each [Delegate] In _OnDestroyedEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event OnStartedEvent As UpdatableEventDelegate
            AddHandler(value As UpdatableEventDelegate)
                If _OnStartedEvent Is Nothing Then _OnStartedEvent = New SafeList(Of UpdatableEventDelegate)
                _OnStartedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdatableEventDelegate)
                If _OnStartedEvent IsNot Nothing Then _OnStartedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IUpdatable)
                If _OnStartedEvent IsNot Nothing Then
                    For Each [Delegate] In _OnStartedEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event OnStoppedEvent As UpdatableEventDelegate
            AddHandler(value As UpdatableEventDelegate)
                If _OnStoppedEvent Is Nothing Then _OnStoppedEvent = New SafeList(Of UpdatableEventDelegate)
                _OnStoppedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdatableEventDelegate)
                If _OnStoppedEvent IsNot Nothing Then _OnStoppedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IUpdatable)
                If _OnStoppedEvent IsNot Nothing Then
                    For Each [Delegate] In _OnStoppedEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Property Updater As Updaters.IUpdater Implements IUpdatable.Updater
            Get
                Return _Updater
            End Get
            Set(Updater As Updaters.IUpdater)
                If IsDestroyed Then Exit Property
                If Updater IsNot Nothing AndAlso Updater.IsDestroyed Then Exit Property
                If Updater Is _Updater Then Exit Property
                Dim OldUpdater As Updaters.IUpdater = _Updater
                If Updater Is Nothing And _Updater Is Nothing Then
                ElseIf Updater Is Nothing And _Updater IsNot Nothing Then
                    _Updater = Nothing
                    Dim HasMe As Boolean = False
                    For Each Updatable In OldUpdater.Targets
                        If Updatable Is Me Then
                            HasMe = True
                            Exit For
                        End If
                    Next
                    If HasMe Then
                        OldUpdater.Remove(Me)
                    End If
                ElseIf Updater IsNot Nothing And _Updater Is Nothing Then
                    _Updater = Updater
                    Dim HasMe As Boolean = False
                    For Each Updatable In Updater.Targets
                        If Updatable Is Me Then
                            HasMe = True
                            Exit For
                        End If
                    Next
                    If Not HasMe Then
                        Updater.Add(Me)
                    End If
                ElseIf Updater IsNot Nothing And _Updater IsNot Nothing Then
#Disable Warning BC42026 ' Expression recursively calls the containing property
                    Me.Updater = Nothing
                    Me.Updater = Updater
#Enable Warning BC42026 ' Expression recursively calls the containing property
                End If
            End Set
        End Property
        Public ReadOnly Property IsDestroyed As Boolean Implements Interfaces.IDestroyable.IsDestroyed
            Get
                Return _IsDestroyed
            End Get
        End Property
        Public ReadOnly Property IsRunning As Boolean Implements Interfaces.IStartStopable.IsRunning
            Get
                Return _IsRunning
            End Get
        End Property
        Public Overridable Sub Start() Implements Interfaces.IStartStopable.Start
            If IsDestroyed Then Exit Sub
            If Not _IsRunning Then
                _IsRunning = True
                RaiseEvent OnStartedEvent(Me)
            End If
        End Sub
        Public Overridable Sub [Stop]() Implements Interfaces.IStartStopable.Stop
            If IsDestroyed Then Exit Sub
            If _IsRunning Then
                _IsRunning = False
                RaiseEvent OnStoppedEvent(Me)
            End If
        End Sub
        Public Sub Update() Implements IUpdatable.Update, Interfaces.IActivatable.Activate
            If IsDestroyed Then Exit Sub
            If IsRunning Then
                Try
                    RaiseEvent PreUpdateEvent(Me)
                    OnUpdated()
                Catch ex As Exception
                    Handle(ex)
                Finally
                    RaiseEvent PostUpdateEvent(Me)
                End Try
            End If
        End Sub
        Protected Overridable Sub OnUpdated()
        End Sub
        Public Overridable Sub Destroy() Implements Interfaces.IDestroyable.Destroy
            If IsDestroyed Then Exit Sub
            [Stop]()
            Updater = Nothing
            _IsDestroyed = True
            RaiseEvent OnDestroyedEvent(Me)
        End Sub
        Protected Overridable Sub Handle(ex As Exception)
            [Throw](Me, ex)
            Dim Updatable As IUpdatable = Me
            Dim Updater As Updaters.IUpdater = Updatable.Updater
            While Updater IsNot Nothing
                [Throw](Updatable, ex)
                Updatable = TryCast(Updater, IUpdatable)
                If Updatable IsNot Nothing Then
                    Updater = Updatable.Updater
                Else
                    Updater = Nothing
                End If
            End While
            If ex.GetType Is GetType(Threading.ThreadAbortException) Then Threading.Thread.ResetAbort()
        End Sub
        Private Sub [Throw](Sender As Object, Exception As Exception) Implements ExceptionHandling.IThrowsException.Throw
            RaiseEvent OnException(Sender, Exception)
        End Sub
        Sub New()
            If DefaultUpdater IsNot Nothing Then
                DefaultUpdater.Add(Me)
                Updater = DefaultUpdater
            End If
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            If Updater IsNot Nothing Then
                Updater.Add(Me)
                Me.Updater = Updater
            End If
        End Sub
        Sub New(Update As UpdatableEventDelegate)
            AddHandler PreUpdateEvent, Update
            If DefaultUpdater IsNot Nothing Then
                DefaultUpdater.Add(Me)
                Updater = DefaultUpdater
            End If
        End Sub
        Sub New(Updater As Updaters.IUpdater, Update As UpdatableEventDelegate)
            If Updater IsNot Nothing Then
                Updater.Add(Me)
                Me.Updater = Updater
            End If
            AddHandler PreUpdateEvent, Update
        End Sub

        Public Delegate Sub UpdatableEventDelegate(Sender As IUpdatable)
        Public Shared Property DefaultUpdater As Updaters.IUpdater
        Shared Sub New()
            DefaultUpdater = New Updaters.UpdaterX()
            DefaultUpdater.Start()
        End Sub
    End Class
End Namespace