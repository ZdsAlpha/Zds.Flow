Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncConverter(Of Input, Output)
        Inherits Converter(Of Input, Output)
        Public ReadOnly InputsCache As New SafeDynamicRound(Of Input)
        Public ReadOnly OutputsCache As New SafeDynamicRound(Of Output)
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Dim Worked As Boolean = False
            Do
                If IsDestroyed Then Exit Do
                Dim _Sink = Sink
                Dim _Queue = Queue
                Dim Value As Input
                Dim HasValue As Boolean = False
                Dim Converted As Output
                Dim HasConverted As Boolean = False
                Worked = False
                If _Queue IsNot Nothing AndAlso InputsCache.Length < InputsCache.AverageSize Then HasValue = _Queue.Dequeue(Value)
                If Not HasValue AndAlso OutputsCache.Length < OutputsCache.AverageSize Then HasValue = InputsCache.Dequeue(Value)
                If Not HasValue Then HasConverted = OutputsCache.Dequeue(Converted)
                If Not HasValue And Not HasConverted Then Exit Do
                If HasValue Then
                    If Convert(Value, Converted) Then
                        HasConverted = True
                        Worked = True
                    ElseIf MustConvert Then
                        InputsCache.Enqueue(Value)
                    End If
                End If
                If HasConverted Then
                    If (_Sink IsNot Nothing AndAlso _Sink.Receive(Converted)) OrElse Dropping Then
                        Worked = True
                    Else
                        OutputsCache.Enqueue(Converted)
                    End If
                End If
            Loop While Recursive And Worked
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