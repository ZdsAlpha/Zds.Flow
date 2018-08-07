Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncSource(Of Output)
        Inherits Source(Of Output)
        Private ReadOnly Values As New DynamicRound(Of Output)
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
            Dim _Sink = Sink
            Dim Value As Output
            Dim HasValue As Boolean = False
            If Values.Length + Threads < InternalQueueSize Then HasValue = Generate(Value)
            If Not HasValue Then HasValue = Values.Dequeue(Value)
            If HasValue Then
                If _Sink IsNot Nothing AndAlso Not IsDestroyed AndAlso _Sink.Receive(Value) Then
                ElseIf Dropping OrElse IsDestroyed Then
                    Discard(Value)
                Else
                    Values.Enqueue(Value)
                End If
            End If
            Threading.Interlocked.Decrement(Threads)
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            Destroy(Values)
        End Sub
        Sub New()
            MyBase.New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            MyBase.New(Generate)
        End Sub
    End Class
End Namespace