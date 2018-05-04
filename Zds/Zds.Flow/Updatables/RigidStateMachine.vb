Namespace Updatables
    <DebuggerStepThrough>
    Public Class RigidStateMachine(Of StateType)
        Inherits SyncTimer
        Implements IRigidStateMachine(Of StateType)
        Public Event MachineEvent(Sender As RigidStateMachine(Of StateType), ByRef State As StateType, ByRef Time As TimeSpan)
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
            Me.New(Nothing, Nothing)
        End Sub
        Sub New(MachineEvent As MachineEventEventHandler)
            Me.New(Nothing, MachineEvent)
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            Me.New(Updater, Nothing)
        End Sub
        Sub New(Updater As Updaters.IUpdater, MachineEvent As MachineEventEventHandler)
            Me.Updater = Updater
            AddHandler MachineEvent, MachineEvent
            Delay = TimeSpan.Zero
            IsTolerant = True
        End Sub
    End Class
End Namespace