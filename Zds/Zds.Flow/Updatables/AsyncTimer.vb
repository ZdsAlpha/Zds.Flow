Namespace Updatables
    <DebuggerStepThrough>
    Public Class AsyncTimer
        Inherits AsyncObject
        Implements IAsyncTimer
        Public Event TickEvent(Sender As AsyncTimer, ByRef Time As TimeSpan)
        Private _LastError As Long
        Private _IsLocked As Boolean
        Protected ReadOnly Property Lock As New Object
        Public Property LastTick As TimeSpan Implements IAsyncTimer.LastTick
        Public Property Delay As TimeSpan = TimeSpan.FromSeconds(1) Implements IAsyncTimer.Delay
        Public Property IsTolerant As Boolean Implements IAsyncTimer.IsTolerant
        Public Property ErrorCorrection As Boolean Implements IAsyncTimer.ErrorCorrection
        Public ReadOnly Property Stopwatch As Stopwatch.IStopwatch = New Stopwatch.Stopwatch Implements IAsyncTimer.Stopwatch
        Public Property NextTick As TimeSpan
            Get
                Return _LastTick + _Delay
            End Get
            Set(value As TimeSpan)
                _LastTick = value - _Delay
            End Set
        End Property
        Public Property LastError As TimeSpan Implements IAsyncTimer.LastError
            Get
                Return TimeSpan.FromTicks(_LastError)
            End Get
            Set(value As TimeSpan)
                _LastError = value.Ticks
            End Set
        End Property
        Public Sub TickNow()
            LastTick = Stopwatch.Elapsed - Delay
        End Sub
        Public Overrides Sub Start()
            MyBase.Start()
            If IsRunning Then _Stopwatch.Start()
        End Sub
        Public Overrides Sub [Stop]()
            MyBase.[Stop]()
            If Not IsRunning Then _Stopwatch.Stop()
        End Sub
        Protected Overrides Sub AsyncUpdate()
            MyBase.AsyncUpdate()
            Dim DoUpdate As Boolean = False
            Dim Time As TimeSpan
            If Not _IsLocked AndAlso Threading.Monitor.TryEnter(_Lock) Then
                _IsLocked = True
                Time = _LastTick + _Delay
                Dim Elapsed As TimeSpan = Stopwatch.Elapsed
                If (Not ErrorCorrection AndAlso Time <= Elapsed) OrElse (ErrorCorrection AndAlso Time - TimeSpan.FromTicks(_LastError) <= Elapsed) Then
                    If ErrorCorrection Then _LastError = _LastError + (Elapsed - Time).Ticks
                    If IsTolerant Then _LastTick = Elapsed Else _LastTick = Time
                    DoUpdate = True
                End If
                _IsLocked = False
                Threading.Monitor.Exit(_Lock)
            End If
            If DoUpdate Then
                Try
                    Tick(Time)
                Catch ex As Exception
                    Handle(ex)
                End Try
            End If
        End Sub
        Protected Overridable Sub Tick(ByRef Time As TimeSpan) Implements IAsyncTimer.Tick
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