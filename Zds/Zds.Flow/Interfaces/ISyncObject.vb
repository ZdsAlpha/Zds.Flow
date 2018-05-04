Namespace Updatables
    Public Interface ISyncObject
        Inherits IUpdatable
        ReadOnly Property IsLocked As Boolean
        Sub SyncUpdate()
    End Interface
End Namespace