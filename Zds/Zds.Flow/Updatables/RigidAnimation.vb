Namespace Updatables
    <DebuggerStepThrough>
    Public Class RigidAnimation
        Inherits RigidStateMachine(Of Decimal)
        Implements IRigidAnimation
        Public Const MinimumState As Decimal = 0
        Public Const MaximumState As Decimal = 0.9999999999999999999999999999D
        Public Event AnimateEvent(Sender As RigidAnimation, ByRef State As Decimal)
        Public Event AnimationCompletedEvent(Sender As RigidAnimation)
        Private _Delta As Decimal = 0.1D
        Public Property IsLoop As Boolean Implements IRigidAnimation.IsLoop
        Public Property FrameSkipping As Boolean Implements IRigidAnimation.FramesSkipping
        Public Property FramesSkipped As ULong Implements IRigidAnimation.FramesSkipped
        Public Overrides Property State As Decimal
            Get
                Return MyBase.State Mod 1
            End Get
            Set(value As Decimal)
                MyBase.State = value Mod 1
            End Set
        End Property
        Public Property Delta As Decimal Implements IRigidAnimation.Delta
            Get
                Return _Delta
            End Get
            Set(value As Decimal)
                If Delta > 0 And value < 0 And State = MinimumState Then
                    State = MaximumState
                ElseIf Delta < 0 And value > 0 And State = MaximumState Then
                    State = MinimumState
                End If
                _Delta = value
            End Set
        End Property
        Public Property TotalFrames As ULong Implements IRigidAnimation.TotalFrames
            Get
                Dim Inverse As Decimal = Math.Abs(1 / Delta)
                If Inverse Mod 1 = 0 Then
                    Return Inverse
                Else
                    Return Math.Truncate(Inverse) + 1
                End If
            End Get
            Set(value As ULong)
                Dim longvalue As Decimal = value
                If Delta < 0 Then
                    If value = 0 Then
                        Delta = Decimal.MinValue
                    Else
                        Delta = -1 / longvalue
                    End If
                Else
                    If value = 0 Then
                        Delta = Decimal.MaxValue
                    Else
                        Delta = 1 / longvalue
                    End If
                End If
            End Set
        End Property
        Public Property IsReversed As Boolean Implements IRigidAnimation.IsReversed
            Get
                If Delta < 0 Then Return True Else Return False
            End Get
            Set(value As Boolean)
                If value = False Then
                    If Delta < 0 Then Delta = -Delta
                Else
                    If Delta > 0 Then Delta = -Delta
                End If
            End Set
        End Property
        Public Property CurrentFrame As ULong Implements IRigidAnimation.CurrentFrame
            Get
                Return Math.Truncate(State * TotalFrames)
            End Get
            Set(value As ULong)
                Dim longtotalframes As Decimal = TotalFrames
                Dim longvalue As Decimal = value
                State = longvalue / longtotalframes
            End Set
        End Property
        Public Sub Reset() Implements IRigidAnimation.Reset
            [Stop]()
            If Not IsReversed Then
                State = MinimumState
            Else
                State = MaximumState
            End If
        End Sub
        Protected Overrides Sub Machine(ByRef State As Decimal, ByRef Time As TimeSpan)
            MyBase.Machine(State, Time)
            Dim Exception As Exception = Nothing
            If FrameSkipping = False Then
                Try
                    Animate(State)
                Catch ex As Threading.ThreadAbortException
                    Exception = ex
                    Threading.Thread.ResetAbort()
                Catch ex As Exception
                    Exception = ex
                End Try
            Else
                If Stopwatch.Elapsed < Time + Delay Then
                    Try
                        Animate(State)
                    Catch ex As Threading.ThreadAbortException
                        Exception = ex
                        Threading.Thread.ResetAbort()
                    Catch ex As Exception
                        Exception = ex
                    End Try
                Else
                    FramesSkipped += 1
                End If
            End If
            If IsLoop Then
                State = State + Delta
            Else
                Dim NextState As Decimal = State + Delta
                If NextState > MaximumState Then
                    State = MinimumState
                    [Stop]()
                    RaiseEvent AnimationCompletedEvent(Me)
                ElseIf NextState < MinimumState Then
                    State = MaximumState
                    [Stop]()
                    RaiseEvent AnimationCompletedEvent(Me)
                Else
                    State = NextState
                End If
            End If
            If Exception IsNot Nothing Then
                Handle(Exception)
            End If
        End Sub
        Protected Overridable Sub Animate(ByRef State As Decimal) Implements IRigidAnimation.Animate
            RaiseEvent AnimateEvent(Me, State)
        End Sub
        Sub New()
            Delay = TimeSpan.FromSeconds(0.1)
            IsTolerant = False
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            Delay = TimeSpan.FromSeconds(0.1)
            IsTolerant = False
        End Sub
        Sub New(Animate As AnimateEventEventHandler)
            Me.New()
            AddHandler AnimateEvent, Animate
        End Sub
        Sub New(Updater As Updaters.IUpdater, Animate As AnimateEventEventHandler)
            Me.New(Updater)
            AddHandler AnimateEvent, Animate
        End Sub
    End Class
End Namespace