Namespace Updatables
    Public Interface ICorrectableTimer
        Inherits ITimer
        Property ErrorCorrection As Boolean
        Property LastError As TimeSpan
    End Interface
End Namespace