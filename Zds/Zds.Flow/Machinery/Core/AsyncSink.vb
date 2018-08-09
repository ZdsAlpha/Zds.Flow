Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncSink(Of Input)
        Inherits Sink(Of Input)
        Public ReadOnly InputsCache As New DynamicRound(Of Input)
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Do
                If IsDestroyed Then Exit Do
                Dim _Queue = Queue
                Dim Value As Input
                Dim HasValue As Boolean = False
                If _Queue IsNot Nothing AndAlso InputsCache.Length < InputsCache.AverageSize Then HasValue = _Queue.Dequeue(Value)
                If Not HasValue Then HasValue = InputsCache.Dequeue(Value)
                If Not HasValue Then Exit Do
                If Sink(Value) Then
                Else
                    InputsCache.Enqueue(Value)
                    Exit Do
                End If
            Loop While Recursive
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