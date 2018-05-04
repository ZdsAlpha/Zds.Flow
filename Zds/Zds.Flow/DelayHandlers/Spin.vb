#Const SpinWaitSupported = True
#If SpinWaitSupported Then
Namespace DelayHandlers
    <DebuggerStepThrough>
    Public Class Spin
        Implements IDelayHandler, IDelayModifiable
        Private SpinWait As New Threading.SpinWait
        Public Property Spins As UInteger = 1
        Public ReadOnly Property CanDecrease As Boolean Implements IDelayModifiable.CanDecrease
            Get
                If Spins <= 1 Then Return False
                Return True
            End Get
        End Property
        Public ReadOnly Property CanIncrease As Boolean Implements IDelayModifiable.CanIncrease
            Get
                If Spins >= UInteger.MaxValue Then Return False
                Return True
            End Get
        End Property
        Public Sub Decrease() Implements IDelayModifiable.Decrease
            If CanDecrease Then _Spins -= 1
        End Sub
        Public Sub Increase() Implements IDelayModifiable.Increase
            If CanIncrease Then _Spins += 1
        End Sub
        Public Sub Delay() Implements IDelayHandler.Delay
            For i As UInteger = 1 To Spins
                SpinWait.SpinOnce()
            Next
        End Sub
        Sub New()
        End Sub
        Sub New(Spins As UInteger)
            Me.Spins = Spins
        End Sub
    End Class
End Namespace
#End If