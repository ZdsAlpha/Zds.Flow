Imports Zds.Flow.Updaters

Namespace Updatables
    Public Interface IContainer
        Inherits ISystem, IUpdater
    End Interface
    Public Interface IAsyncContainer
        Inherits IContainer, IAsyncObject
    End Interface
    Public Interface ISyncContainer
        Inherits IContainer, ISyncObject
    End Interface
End Namespace