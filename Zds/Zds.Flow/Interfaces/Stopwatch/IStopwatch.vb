Namespace Stopwatch
    Public Interface IStopwatch
        Inherits Interfaces.IStartStopable, Interfaces.IResetable
        Property Elapsed As TimeSpan
    End Interface
End Namespace