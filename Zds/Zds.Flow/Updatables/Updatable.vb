Namespace Updatables
    <DebuggerStepThrough>
    Public Class Updatable
        Implements IUpdatable
        Public Event OnUpdatedEvent(Sender As IUpdatable)
        Public Event OnDestroyedEvent(Sender As IUpdatable)
        Public Event OnStartedEvent(Sender As IUpdatable)
        Public Event OnStoppedEvent(Sender As IUpdatable)
        Private _IsDestroyed As Boolean = False
        Private _IsRunning As Boolean = False
        Private _Updater As Updaters.IUpdater
        Public Property ExceptionHandler As ExceptionHandlers.IExceptionHandler Implements IUpdatable.ExceptionHandler
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
                        updater.Add(Me)
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
        Public Sub Update() Implements IUpdatable.Update
            If IsDestroyed Then Exit Sub
            If IsRunning Then
                Try
                    OnUpdated()
                Catch ex As Exception
                    Handle(ex)
                End Try
            End If
        End Sub
        Protected Overridable Sub OnUpdated()
            RaiseEvent OnUpdatedEvent(Me)
        End Sub
        Public Overridable Sub Destroy() Implements Interfaces.IDestroyable.Destroy
            If IsDestroyed Then Exit Sub
            [Stop]()
            Updater = Nothing
            _IsDestroyed = True
            RaiseEvent OnDestroyedEvent(Me)
        End Sub
        Protected Sub Handle(ex As Exception)
            Dim Updater As Updaters.IUpdater = Me.Updater
            If ExceptionHandler IsNot Nothing Then ExceptionHandler.Catch(Me, ex)
            If Updater IsNot Nothing AndAlso Updater.ExceptionHandler IsNot Nothing Then Updater.ExceptionHandler.Catch(Me, ex)
            If ex.GetType Is GetType(Threading.ThreadAbortException) Then Threading.Thread.ResetAbort()
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
        Sub New(Update As OnUpdatedEventEventHandler)
            AddHandler OnUpdatedEvent, Update
            If DefaultUpdater IsNot Nothing Then
                DefaultUpdater.Add(Me)
                Updater = DefaultUpdater
            End If
        End Sub
        Sub New(Updater As Updaters.IUpdater, Update As OnUpdatedEventEventHandler)
            If Updater IsNot Nothing Then
                Updater.Add(Me)
                Me.Updater = Updater
            End If
            AddHandler Me.OnUpdatedEvent, Update
        End Sub

        Public Shared Property DefaultUpdater As Updaters.IUpdater
        Shared Sub New()
            DefaultUpdater = New Updaters.UpdaterX()
            DefaultUpdater.Start()
        End Sub
    End Class
End Namespace