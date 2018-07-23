Namespace Updatables
    Public Interface IRigidStateMachine(Of StateType)
        Inherits ISyncTimer
        Property State As StateType
        Sub Machine(ByRef State As StateType, ByRef Time As TimeSpan)
    End Interface
End Namespace