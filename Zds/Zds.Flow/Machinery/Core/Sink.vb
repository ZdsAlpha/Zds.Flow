Imports Zds.Flow.Collections
Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Sink(Of Input)
        Inherits Machinery
        Implements ISink(Of Input)
        Public Property Sink As SinkDelegate
        Public Property Queue As IQueue(Of Input)
        Public Property Recursive As Boolean = True
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Dim _Queue = Queue
            If IsDestroyed OrElse _Queue Is Nothing Then Return False
            Return _Queue.Enqueue(obj)
        End Function
        Public Overrides Sub Destroy()
            If IsDestroyed Then Exit Sub
            MyBase.Destroy()
            Dim _Queue = Queue
            Queue = Nothing
            Dim Disposable As IDisposable = TryCast(_Queue, IDisposable)
            If Disposable IsNot Nothing Then Disposable.Dispose()
        End Sub
        Sub New()
            Queue = New SafeRound(Of Input)()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Queue = Buffer
        End Sub
        Sub New(Sink As SinkDelegate)
            Me.New()
            Me.Sink = Sink
        End Sub
        Sub New(Buffer As IQueue(Of Input), Sink As SinkDelegate)
            Me.New(Buffer)
            Me.Sink = Sink
        End Sub

        Public Delegate Function SinkDelegate(obj As Input) As Boolean
    End Class
End Namespace
