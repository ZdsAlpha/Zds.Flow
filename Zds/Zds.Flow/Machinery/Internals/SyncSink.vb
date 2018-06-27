Imports Zds.Flow.Collections

Namespace Machinery.Internals
    Public Class SyncSink(Of Input)
        Implements ISink(Of Input)
        Public Property Sink As SinkDelegate
        Public Property Buffer As IQueue(Of Input)
        Public Property Recursive As Boolean Implements ISink(Of Input).Recursive
        Private HasValue As Boolean
        Private Value As Input
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            If Buffer Is Nothing Then Return Nothing
            Return Buffer.Enqueue(obj)
        End Function
        Public Sub Activate()
            Dim _Buffer = Buffer
            If _Buffer IsNot Nothing AndAlso Not HasValue Then HasValue = _Buffer.Dequeue(Value)
            Do
                If HasValue AndAlso Sink(Value) Then
                    HasValue = False
                    Value = Nothing
                Else
                    Exit Do
                End If
                If _Buffer IsNot Nothing Then HasValue = _Buffer.Dequeue(Value)
            Loop While Recursive
        End Sub
        Sub New()
            Buffer = New Collection(Of Input)(4096)
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

        Public Delegate Function SinkDelegate(obj As Input) As Boolean
    End Class
End Namespace