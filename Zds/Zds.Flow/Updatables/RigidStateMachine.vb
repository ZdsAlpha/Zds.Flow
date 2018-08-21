Imports Zds.Flow.Collections
Namespace Updatables
    <DebuggerStepThrough>
    Public Class RigidStateMachine(Of StateType)
        Inherits SyncTimer
        Implements IRigidStateMachine(Of StateType)
        Private _MachineEvent As SafeList(Of RigidStateMachineEventDelegate)
        Public Custom Event MachineEvent As RigidStateMachineEventDelegate
            AddHandler(value As RigidStateMachineEventDelegate)
                If _MachineEvent Is Nothing Then _MachineEvent = New SafeList(Of RigidStateMachineEventDelegate)
                _MachineEvent.Add(value)
            End AddHandler
            RemoveHandler(value As RigidStateMachineEventDelegate)
                If _MachineEvent IsNot Nothing Then _MachineEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IRigidStateMachine(Of StateType), ByRef State As StateType, ByRef Time As TimeSpan)
                If _MachineEvent IsNot Nothing Then
                    For Each [Delegate] In _MachineEvent.Elements
                        [Delegate].Invoke(Sender, State, Time)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Overridable Property State As StateType Implements IRigidStateMachine(Of StateType).State
        Protected Overrides Sub Tick(ByRef Time As TimeSpan)
            MyBase.Tick(Time)
            Dim State As StateType = Me.State
            Try
                Machine(State, Time)
                Me.State = State
            Catch ex As Exception
                Me.State = State
                Handle(ex)
            End Try
        End Sub
        Protected Overridable Sub Machine(ByRef State As StateType, ByRef Time As TimeSpan) Implements IRigidStateMachine(Of StateType).Machine
            RaiseEvent MachineEvent(Me, State, Time)
        End Sub
        Sub New()
            Delay = TimeSpan.Zero
            IsTolerant = True
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            Delay = TimeSpan.Zero
            IsTolerant = True
        End Sub
        Sub New(MachineEvent As RigidStateMachineEventDelegate)
            Me.New()
            AddHandler MachineEvent, MachineEvent
        End Sub
        Sub New(Updater As Updaters.IUpdater, MachineEvent As RigidStateMachineEventDelegate)
            Me.New(Updater)
            AddHandler MachineEvent, MachineEvent
        End Sub

        Public Delegate Sub RigidStateMachineEventDelegate(Sender As IRigidStateMachine(Of StateType), ByRef State As StateType, ByRef Time As TimeSpan)
    End Class
End Namespace