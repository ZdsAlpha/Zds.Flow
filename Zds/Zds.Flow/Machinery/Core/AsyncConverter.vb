Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncConverter(Of Input, Output)
        Inherits Converter(Of Input, Output)
        Private ReadOnly Values As New DynamicRound(Of Input)
        Private ReadOnly Converted As New DynamicRound(Of Output)
        Private Threads As Integer = 0
        Public Property InternalQueueSize As Integer
            Get
                Return Values.AverageSize
            End Get
            Set(value As Integer)
                Values.AverageSize = value
                Converted.AverageSize = value
            End Set
        End Property
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Threading.Interlocked.Increment(Threads)
            Dim _Sink = Sink
            Dim _Queue = Queue
            Dim Value As Input
            Dim HasValue As Boolean = False
            Dim Converted As Output
            Dim HasConverted As Boolean = False
            Dim Worked As Boolean = False
            Do
                If IsDestroyed Then Exit Do
                Worked = False
                If _Queue IsNot Nothing AndAlso Values.Length + Threads < InternalQueueSize Then HasValue = _Queue.Dequeue(Value)
                If Not HasValue AndAlso Me.Converted.Length + Threads < InternalQueueSize Then HasValue = Values.Dequeue(Value)
                If Not HasValue Then HasConverted = Me.Converted.Dequeue(Converted)
                If Not HasValue And Not HasConverted Then Exit Do
                'Converting value
                If HasValue Then
                    If Convert(Value, Converted) Then
                        HasConverted = True
                        Worked = True
                    ElseIf MustConvert Then
                        Values.Enqueue(Value)
                    Else
                        Discard(Value)
                    End If
                    HasValue = False
                    Value = Nothing
                End If
                'Sinking value
                If HasConverted Then
                    If _Sink IsNot Nothing AndAlso _Sink.Receive(Converted) Then
                        Worked = True
                    ElseIf Dropping Then
                        Discard(Value)
                    Else
                        Me.Converted.Enqueue(Converted)
                    End If
                    HasConverted = False
                    Converted = Nothing
                End If
            Loop While Recursive And Worked
            Threading.Interlocked.Decrement(Threads)
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            Dim Array1 = Values.ToArray
            Values.Clear()
            Dim Array2 = Converted.ToArray
            Converted.Clear()
            For Each Obj In Array1
                Discard(Obj)
            Next
            For Each Obj In Array2
                Discard(Obj)
            Next
        End Sub
        Sub New()
            MyBase.New()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            MyBase.New(Buffer)
        End Sub
        Sub New(Process As ConvertDelegate)
            MyBase.New(Process)
        End Sub
        Sub New(Buffer As IQueue(Of Input), Process As ConvertDelegate)
            MyBase.New(Buffer, Process)
        End Sub
    End Class
End Namespace