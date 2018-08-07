Imports Zds.Flow.Collections
Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Converter(Of Input, Output)
        Inherits Base
        Implements IConverter(Of Input, Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Convert As ConvertDelegate
        Public Property Queue As IQueue(Of Input)
        Public Property Dropping As Boolean = False
        Public Property Recursive As Boolean = True
        Public Property MustConvert As Boolean = False
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Dim _Queue = Queue
            If IsDestroyed OrElse _Queue Is Nothing Then Return False
            Return _Queue.Enqueue(obj)
        End Function
        Public Overrides Sub Destroy() Implements IDestroyable.Destroy
            If IsDestroyed Then Exit Sub
            MyBase.Destroy()
            Dim _Queue = Queue
            _Queue = Nothing
            Dim Round As Round(Of Input) = TryCast(_Queue, Round(Of Input))
            If Round IsNot Nothing Then
                Dim Array = Round.ToArray()
                Round.Clear()
                For Each obj In Array
                    Discard(obj)
                Next
            Else
                Discard(_Queue)
            End If
        End Sub
        Sub New()
            Queue = New Round(Of Input)()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Queue = Buffer
        End Sub
        Sub New(Process As ConvertDelegate)
            Me.New()
            Me.Convert = Process
        End Sub
        Sub New(Buffer As IQueue(Of Input), Process As ConvertDelegate)
            Me.New(Buffer)
            Me.Convert = Process
        End Sub

        Public Delegate Function ConvertDelegate(Input As Input, ByRef Output As Output) As Boolean
    End Class
End Namespace