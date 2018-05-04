Namespace Interfaces
    Public Interface IPauseResumable
        ReadOnly Property IsPaused As Boolean
        Sub Pause()
        Sub [Resume]()
    End Interface
End Namespace