﻿Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class SyncSink(Of Input)
        Inherits Sink(Of Input)
        Private HasValue As Boolean
        Private Value As Input
        Public Overrides Sub Activate()
            If IsDestroyed Then Exit Sub
            Dim _Queue = Queue
            If _Queue IsNot Nothing AndAlso Not HasValue Then HasValue = _Queue.Dequeue(Value)
            Do
                If IsDestroyed Then Exit Do
                If HasValue AndAlso Sink(Value) Then
                    HasValue = False
                    Value = Nothing
                Else
                    Exit Do
                End If
                If _Queue IsNot Nothing AndAlso Not IsDestroyed Then HasValue = _Queue.Dequeue(Value)
            Loop While Recursive
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            HasValue = False
            Discard(Value)
            Value = Nothing
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