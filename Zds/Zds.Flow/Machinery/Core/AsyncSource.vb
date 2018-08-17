Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class AsyncSource(Of Output)
        Inherits Source(Of Output)
        Public ReadOnly OutputsCache As New SafeDynamicRound(Of Output)
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Dim _Sink = Sink
            Dim Value As Output
            Dim HasValue As Boolean = False
            If OutputsCache.Length <= OutputsCache.AverageSize Then HasValue = Generate(Value)
            If Not HasValue Then HasValue = OutputsCache.Dequeue(Value)
            If HasValue Then
                If (_Sink IsNot Nothing AndAlso _Sink.Receive(Value)) OrElse Dropping Then
                Else
                    OutputsCache.Enqueue(Value)
                End If
            End If
        End Sub
        Sub New()
            MyBase.New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            MyBase.New(Generate)
        End Sub
    End Class
End Namespace