Namespace Updatables
    <DebuggerStepThrough>
    Public Class SyncTimer
        Inherits SyncObject
        Implements ISyncTimer
        Public Event TickEvent(Sender As SyncTimer, ByRef Time As TimeSpan)
        Private _LastError As Long
        Public Property LastTick As TimeSpan Implements ISyncTimer.LastTick
        Public Property Delay As TimeSpan = TimeSpan.FromSeconds(1) Implements ISyncTimer.Delay
        Public Property IsTolerant As Boolean Implements ISyncTimer.IsTolerant
        Public Property ErrorCorrection As Boolean Implements ISyncTimer.ErrorCorrection
        Public ReadOnly Property Stopwatch As Stopwatch.IStopwatch = New Stopwatch.Stopwatch Implements ISyncTimer.Stopwatch
        Public Property NextTick As TimeSpan
            Get
                Return _LastTick + _Delay
            End Get
            Set(value As TimeSpan)
                _LastTick = value - _Delay
            End Set
        End Property
        Public Property LastError As TimeSpan Implements ISyncTimer.LastError
            Get
                Return TimeSpan.FromTicks(_LastError)
            End Get
            Set(value As TimeSpan)
                _LastError = value.Ticks
            End Set
        End Property
        Public Overrides Sub Start()
            MyBase.Start()
            If IsRunning Then _Stopwatch.Start()
        End Sub
        Public Overrides Sub [Stop]()
            MyBase.[Stop]()
            If Not IsRunning Then _Stopwatch.Stop()
        End Sub
        Protected Overrides Sub SyncUpdate()
            MyBase.SyncUpdate()
            Dim Time As TimeSpan = _LastTick + _Delay
            Dim Elapsed As TimeSpan = Stopwatch.Elapsed
            If (Not ErrorCorrection AndAlso Time <= Elapsed) OrElse (ErrorCorrection AndAlso Time - TimeSpan.FromTicks(_LastError) <= Elapsed) Then
                If ErrorCorrection Then _LastError = _LastError + (Elapsed - Time).Ticks
                Try
                    Tick(Time)
                    If IsTolerant Then _LastTick = Elapsed Else _LastTick = Time
                Catch ex As Exception
                    If IsTolerant Then _LastTick = Elapsed Else _LastTick = Time
                    Handle(ex)
                End Try
            End If
        End Sub
        Protected Overridable Sub Tick(ByRef Time As TimeSpan) Implements ISyncTimer.Tick
            RaiseEvent TickEvent(Me, Time)
        End Sub
        Public Overrides Sub Destroy()
            If Not IsDestroyed Then
                [Stop]()
                MyBase.Destroy()
            End If
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
        End Sub
        Sub New(Tick As TickEventEventHandler)
            AddHandler TickEvent, Tick
        End Sub
        Sub New(Updater As Updaters.IUpdater, Tick As TickEventEventHandler)
            MyBase.New(Updater)
            AddHandler TickEvent, Tick
        End Sub
    End Class
End Namespace