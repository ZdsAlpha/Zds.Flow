Namespace Stopwatch
    <DebuggerStepThrough>
    Public Class Stopwatch
        Implements IStopwatch
        Private _IsRunning As Boolean = False
        Private _Stopwatch As IStopwatch
        Private _Time As TimeSpan
        Public ReadOnly Property IsRunning As Boolean Implements Interfaces.IStartStopable.IsRunning
            Get
                Return _IsRunning
            End Get
        End Property
        Public Property Elapsed As TimeSpan Implements IStopwatch.Elapsed
            Get
                If _IsRunning = True Then
                    Return _Stopwatch.Elapsed - _Time
                Else
                    Return _Time
                End If
            End Get
            Set(value As TimeSpan)
                If _IsRunning = True Then
                    _Time = _Stopwatch.Elapsed - value
                Else
                    _Time = value
                End If
            End Set
        End Property
        Public Sub Start() Implements Interfaces.IStartStopable.Start
            If _IsRunning = False Then
                _Time = _Stopwatch.Elapsed - Me.Elapsed
                _IsRunning = True
            End If
        End Sub
        Public Sub [Stop]() Implements Interfaces.IStartStopable.Stop
            If _IsRunning = True Then
                _Time = Elapsed
                _IsRunning = False
            End If
        End Sub
        Public Sub Reset() Implements Interfaces.IResetable.Reset
            [Stop]()
            _Time = TimeSpan.Zero
        End Sub
        Sub New(Reference As IStopwatch)
            _Stopwatch = Reference
        End Sub
        Sub New()
            Me.New(UniversalStopwatch)
        End Sub
        Public Shared ReadOnly UniversalStopwatch As IStopwatch
        Shared Sub New()
            UniversalStopwatch = New SystemStopwatch
            UniversalStopwatch.Start()
        End Sub
    End Class
End Namespace