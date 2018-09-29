Imports Zds.Flow.Collections
Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Converter(Of Input, Output)
        Inherits Machine
        Implements IConverter(Of Input, Output)
        Private _ConversionFailedEvent As SafeList(Of ConversionFailedEventDelegate)
        Private _OnDroppedEvent As SafeList(Of OnDroppedEventDelegate)
        Public Custom Event ConversionFailedEvent As ConversionFailedEventDelegate
            AddHandler(value As ConversionFailedEventDelegate)
                If _ConversionFailedEvent Is Nothing Then _ConversionFailedEvent = New SafeList(Of ConversionFailedEventDelegate)
                _ConversionFailedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As ConversionFailedEventDelegate)
                If _ConversionFailedEvent IsNot Nothing Then _ConversionFailedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(obj As Input)
                If _ConversionFailedEvent IsNot Nothing Then
                    For Each [Delegate] In _ConversionFailedEvent.Elements
                        [Delegate].Invoke(obj)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event OnDroppedEvent As OnDroppedEventDelegate
            AddHandler(value As OnDroppedEventDelegate)
                If _OnDroppedEvent Is Nothing Then _OnDroppedEvent = New SafeList(Of OnDroppedEventDelegate)
                _OnDroppedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As OnDroppedEventDelegate)
                If _OnDroppedEvent IsNot Nothing Then _OnDroppedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(obj As Output)
                If _OnDroppedEvent IsNot Nothing Then
                    For Each [Delegate] In _OnDroppedEvent.Elements
                        [Delegate].Invoke(obj)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Convert As ConvertDelegate
        Public Property Queue As IQueue(Of Input)
        Public Property Dropping As Boolean = False
        Public Property Recursive As Boolean = False
        Public Property MustConvert As Boolean = False
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Dim _Queue = Queue
            If IsDestroyed OrElse _Queue Is Nothing Then Return False
            Return _Queue.Enqueue(obj)
        End Function
        Protected Sub ConversionFailed(obj As Input)
            RaiseEvent ConversionFailedEvent(obj)
        End Sub
        Protected Sub OnDropped(obj As Output)
            RaiseEvent OnDroppedEvent(obj)
        End Sub
        Public Overrides Sub Destroy()
            If IsDestroyed Then Exit Sub
            MyBase.Destroy()
            Dim _Queue = Queue
            Queue = Nothing
            Dim Disposable As IDisposable = TryCast(_Queue, IDisposable)
            If Disposable IsNot Nothing Then Disposable.Dispose()
        End Sub
        Sub New()
            Queue = New SafeRound(Of Input)()
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
        Public Delegate Sub ConversionFailedEventDelegate(obj As Input)
        Public Delegate Sub OnDroppedEventDelegate(obj As Output)
    End Class
End Namespace