Namespace Updatables
    Public Interface ITimer
        Inherits IUpdatable
        ReadOnly Property Stopwatch As Stopwatch.IStopwatch
        Property Delay As TimeSpan
        Property LastTick As TimeSpan
        Property IsTolerant As Boolean
        Sub Tick(ByRef Time As TimeSpan)
    End Interface
    Public Interface ISyncTimer
        Inherits ISyncObject, ICorrectableTimer
    End Interface
    Public Interface IAsyncTimer
        Inherits IAsyncObject, ICorrectableTimer
    End Interface
End Namespace