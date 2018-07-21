Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class SyncSink(Of Input)
        Inherits Sink(Of Input)
        Private HasValue As Boolean
        Private Value As Input
        Public Overrides Sub Activate()
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