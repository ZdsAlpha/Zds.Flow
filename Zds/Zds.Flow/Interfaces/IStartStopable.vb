Namespace Interfaces
    Public Interface IStartStopable
        ReadOnly Property IsRunning As Boolean
        Sub Start()
        Sub [Stop]()
    End Interface
End Namespace