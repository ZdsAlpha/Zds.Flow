Namespace DelayHandling
    Public Interface IDelayModifiable
        ReadOnly Property CanIncrease As Boolean
        ReadOnly Property CanDecrease As Boolean
        Sub Increase()
        Sub Decrease()
    End Interface
End Namespace