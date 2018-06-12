Imports Zds.Flow.Collections

Namespace Machinery
    Public Class Sink(Of Input)
        Implements ISink(Of Input)
        Public Property Sink As SinkDelegate
        Public Property Buffer As IQueue(Of Input)
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Return Buffer.Enqueue(obj)
        End Function
        Public Sub Activate()
            If Buffer IsNot Nothing Then
                Dim obj As Input
                If Buffer.Dequeue(obj) Then
                    Sink.Invoke(obj)
                End If
            End If
        End Sub
        Public Delegate Sub SinkDelegate(obj As Input)
        Sub New()
            Buffer = New Round(Of Input)(4096)
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Buffer = Buffer
        End Sub
        Sub New(Sink As SinkDelegate)
            Me.New()
            Me.Sink = Sink
        End Sub
        Sub New(Buffer As IQueue(Of Input), Sink As SinkDelegate)
            Me.New(Buffer)
            Me.Sink = Sink
        End Sub
    End Class
End Namespace