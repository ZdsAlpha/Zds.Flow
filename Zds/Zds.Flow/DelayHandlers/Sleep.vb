Namespace DelayHandlers
    <DebuggerStepThrough>
    Public Class Sleep
        Implements IDelayHandler, IDelayModifiable
        Public Property Time As UInteger = 1
        Public ReadOnly Property CanDecrease As Boolean Implements IDelayModifiable.CanDecrease
            Get
                If _Time <= 1 Then Return False
                Return True
            End Get
        End Property
        Public ReadOnly Property CanIncrease As Boolean Implements IDelayModifiable.CanIncrease
            Get
                If _Time = UInteger.MaxValue Then Return False
                Return True
            End Get
        End Property
        Public Sub Decrease() Implements IDelayModifiable.Decrease
            If CanDecrease Then _Time -= 1
        End Sub
        Public Sub Increase() Implements IDelayModifiable.Increase
            If CanIncrease Then _Time += 1
        End Sub
        Public Sub Delay() Implements IDelayHandler.Delay
            System.Threading.Thread.Sleep(_Time)
        End Sub
        Sub New()
        End Sub
        Sub New(Time As UInteger)
            _Time = Time
        End Sub
    End Class
End Namespace