Imports Zds.Flow.Collections
Imports Zds.Flow.Interfaces

Namespace Machinery.Core
    Public MustInherit Class Source(Of Output)
        Inherits Machine
        Implements ISource(Of Output)
        Private _OnDroppedEvent As SafeList(Of OnDroppedEventDelegate)
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
        Public Property Generate As GenerateDelegate
        Public Property Dropping As Boolean = False
        Protected Sub OnDropped(obj As Output)
            RaiseEvent OnDroppedEvent(obj)
        End Sub
        Sub New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            Me.Generate = Generate
        End Sub

        Public Delegate Function GenerateDelegate(ByRef obj As Output) As Boolean
        Public Delegate Sub OnDroppedEventDelegate(obj As Output)
    End Class
End Namespace