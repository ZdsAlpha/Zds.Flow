﻿Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public MustInherit Class Sink(Of Input)
        Implements ISink(Of Input)
        Public Property Sink As SinkDelegate
        Public Property Queue As IQueue(Of Input)
        Public Property Recursive As Boolean = True
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            If Queue Is Nothing Then Return Nothing
            Return Queue.Enqueue(obj)
        End Function
        Public MustOverride Sub Activate() Implements ISink(Of Input).Activate
        Sub New()
            Queue = New Round(Of Input)()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Queue = Buffer
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
