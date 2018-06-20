Imports Zds.Flow.Collections

Namespace Machinery
    Public Class Processor(Of Input, Output)
        Implements IProcessor(Of Input, Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Process As ProcessDelegate
        Public Property Buffer As IQueue(Of Input)
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Return Buffer.Enqueue(obj)
        End Function
        Public Sub Activate()
            If Buffer IsNot Nothing Then
                Dim obj As Input
                If Buffer.Dequeue(obj) Then
                    Sink.Receive(Process(obj))
                End If
            End If
        End Sub
        Sub New()
            Buffer = New Collection(Of Input)(4096)
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Buffer = Buffer
        End Sub
        Sub New(Process As ProcessDelegate)
            Me.New()
            Me.Process = Process
        End Sub
        Sub New(Buffer As IQueue(Of Input), Process As ProcessDelegate)
            Me.New(Buffer)
            Me.Process = Process
        End Sub
        Public Delegate Function ProcessDelegate(Input As Input) As Output
    End Class
End Namespace