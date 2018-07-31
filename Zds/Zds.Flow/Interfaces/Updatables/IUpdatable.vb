﻿Namespace Updatables
    Public Interface IUpdatable
        Inherits Interfaces.IDestroyable, Interfaces.IStartStopable, Interfaces.IThrowsException, Interfaces.IActivatable
        Property Updater As Updaters.IUpdater
        Sub Update()
    End Interface
End Namespace