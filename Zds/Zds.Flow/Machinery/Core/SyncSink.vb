Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class SyncSink(Of Input)
        Inherits Sink(Of Input)
        Private HasValue As Boolean
        Private Value As Input
        Public Overrides Sub Activate()
            Do
                If IsDestroyed Then Exit Do
                Dim _Queue = Queue
                If _Queue IsNot Nothing AndAlso Not HasValue Then HasValue = _Queue.Dequeue(Value)
                If HasValue AndAlso Sink(Value) Then
                    HasValue = False
                    Value = Nothing
                Else
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