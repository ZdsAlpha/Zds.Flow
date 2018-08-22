Namespace DelayHandling
    <DebuggerStepThrough>
    Public Class ConstantSleep
        Implements IDelayHandler
        Public ReadOnly Property Time As UInteger = 1
        Public Sub Delay() Implements IDelayHandler.Delay
            Threading.Thread.Sleep(Time)
        End Sub
        Sub New()
        End Sub
        Sub New(Time As UInteger)
            Me.Time = Time
        End Sub
        Public Shared ReadOnly Property UniversalDelayHandler As IDelayHandler
        Shared Sub New()
            UniversalDelayHandler = New ConstantSleep()
        End Sub
    End Class
End Namespace