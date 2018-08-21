Namespace Updatables
    Public Interface IUpdatable
        Inherits ExceptionHandling.IThrowsException, Interfaces.IDestroyable, Interfaces.IStartStopable, Interfaces.IActivatable
        Property Updater As Updaters.IUpdater
        Sub Update()
    End Interface
End Namespace