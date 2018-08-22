Namespace Updaters
    Public Interface IUpdater
        Inherits ExceptionHandling.IThrowsException, Interfaces.IDestroyable, Interfaces.IStartStopable, Interfaces.IPauseResumable
        ReadOnly Property Targets As Updatables.IUpdatable()
        Property DelayHandler As DelayHandling.IDelayHandler
        Sub Add(Updatable As Updatables.IUpdatable)
        Sub Remove(Updatable As Updatables.IUpdatable)
        Sub Clear()
    End Interface
End Namespace