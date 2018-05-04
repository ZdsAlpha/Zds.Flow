Namespace Updaters
    Public Interface IUpdater
        Inherits Interfaces.IDestroyable, Interfaces.IStartStopable, Interfaces.IPauseResumable, Interfaces.IExceptionControlable
        ReadOnly Property Targets As Updatables.IUpdatable()
        Property DelayHandler As DelayHandlers.IDelayHandler
        Sub Add(Updatable As Updatables.IUpdatable)
        Sub Remove(Updatable As Updatables.IUpdatable)
        Sub Clear()
    End Interface
End Namespace