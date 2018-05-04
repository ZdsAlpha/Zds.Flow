Namespace Stopwatch
    <DebuggerStepThrough>
    Public Class SystemStopwatch
        Implements IStopwatch
        Private _Stopwatch As New Diagnostics.Stopwatch
        Public ReadOnly Property IsRunning As Boolean Implements Interfaces.IStartStopable.IsRunning
            Get
                Return _Stopwatch.IsRunning
            End Get
        End Property
        Public Property Elapsed As TimeSpan Implements IStopwatch.Elapsed
            Get
                Return _Stopwatch.Elapsed
            End Get
            Set(value As TimeSpan)
                Throw New InvalidOperationException("You can assign this property to SystemStopwatch.")
            End Set
        End Property
        Public Sub Start() Implements Interfaces.IStartStopable.Start
            _Stopwatch.Start()
        End Sub
        Public Sub [Stop]() Implements Interfaces.IStartStopable.Stop
            _Stopwatch.Stop()
        End Sub
        Public Sub Reset() Implements Interfaces.IResetable.Reset
            _Stopwatch.Reset()
        End Sub
    End Class
End Namespace