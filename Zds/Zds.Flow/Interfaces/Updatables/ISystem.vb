Namespace Updatables
    Public Interface ISystem
        Inherits IUpdatable
    End Interface
    Public Interface IAsyncSystem
        Inherits IAsyncObject, ISystem
    End Interface
    Public Interface ISyncSystem
        Inherits ISyncObject, ISystem
    End Interface
End Namespace