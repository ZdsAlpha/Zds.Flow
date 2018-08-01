Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncConverter(Of Input, Output)
        Inherits Converter(Of Input, Output)
        Private Threads As Integer = 0
        Private Values As New DynamicRound(Of Input)
        Private Converted As New DynamicRound(Of Output)
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
            Threading.Interlocked.Increment(Threads)
            Dim _Sink = Sink
            Dim _Queue = Queue
            Dim Value As Input
            Dim HasValue As Boolean = False
            Dim Converted As Output
            Dim HasConverted As Boolean = False
            Do
                Dim Worked As Boolean = False
                If _Queue IsNot Nothing And Values.Length < InternalQueueSize Then
                    HasValue = _Queue.Dequeue(Value)
                ElseIf Me.Converted.Length < InternalQueueSize Then
                    HasValue = Values.Dequeue(Value)
                Else
                    HasConverted = Me.Converted.Dequeue(Converted)
                End If
                If Not HasValue And Not HasConverted Then Exit Do
                'Converting value
                If HasValue And Not HasConverted Then
                    If Convert(Value, Converted) Then
                        HasConverted = True
                        Worked = True
                    ElseIf MustConvert Then
                        Values.Enqueue(Value)
                    End If
                    HasValue = False
                    Value = Nothing
                End If
                'Sinking value
                If HasConverted Then
                    If _Sink IsNot Nothing AndAlso _Sink.Receive(Converted) Then
                        Worked = True
                    ElseIf Not Dropping Then
                        Me.Converted.Enqueue(Converted)
                    End If
                    HasConverted = False
                    Converted = Nothing
                End If
                If Not Worked Then Exit Do
            Loop While Recursive
            Threading.Interlocked.Decrement(Threads)
        End Sub
    End Class
End Namespace