Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncSink(Of Input)
        Inherits Sink(Of Input)
        Private ReadOnly Values As New DynamicRound(Of Input)
        Private Threads As Integer = 0
        Public Property InternalQueueSize As Integer
            Get
                Return Values.AverageSize
            End Get
            Set(value As Integer)
                Values.AverageSize = value
            End Set
        End Property
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Threading.Interlocked.Increment(Threads)
            Dim _Queue = Queue
            Dim Value As Input
            Dim HasValue As Boolean = False
            Do
                If IsDestroyed Then Exit Do
                If _Queue IsNot Nothing AndAlso Values.Length + Threads < InternalQueueSize Then HasValue = _Queue.Dequeue(Value)
                If Not HasValue Then HasValue = Values.Dequeue(Value)
                If Not HasValue Then Exit Do
                If Sink(Value) Then
                    HasValue = False
                    Value = Nothing
                Else
                    Values.Enqueue(Value)
                    Exit Do
                End If
            Loop While Recursive
            Threading.Interlocked.Decrement(Threads)
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            Dim Array = Values.ToArray
            Values.Clear()
            For Each Obj In Array
                Discard(Obj)
            Next
        End Sub
        Sub New()
            MyBase.New
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            MyBase.New(Buffer)
        End Sub
        Sub New(Sink As SinkDelegate)
            MyBase.New(Sink)
        End Sub
        Sub New(Buffer As IQueue(Of Input), Sink As SinkDelegate)
            MyBase.New(Buffer, Sink)
        End Sub
    End Class
End Namespace