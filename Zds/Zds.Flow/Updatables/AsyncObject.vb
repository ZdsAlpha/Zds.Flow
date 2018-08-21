Imports System.Threading
Imports Zds.Flow.Collections

Namespace Updatables
    <DebuggerStepThrough>
    Public Class AsyncObject
        Inherits Updatable
        Implements IAsyncObject
        Private _AsyncUpdateEvent As SafeList(Of AsyncObjectEventDelegate)
        Public Custom Event AsyncUpdateEvent As AsyncObjectEventDelegate
            AddHandler(value As AsyncObjectEventDelegate)
                If _AsyncUpdateEvent Is Nothing Then _AsyncUpdateEvent = New SafeList(Of AsyncObjectEventDelegate)
                _AsyncUpdateEvent.Add(value)
            End AddHandler
            RemoveHandler(value As AsyncObjectEventDelegate)
                If _AsyncUpdateEvent IsNot Nothing Then _AsyncUpdateEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As IAsyncObject)
                If _AsyncUpdateEvent IsNot Nothing Then
                    For Each [Delegate] In _AsyncUpdateEvent.Elements
                        [Delegate].Invoke(Sender)
                    Next
                End If
            End RaiseEvent
        End Event
        Public ReadOnly Property ActiveThreads As Integer
        Public Property MaxThreads As Integer = -1
        Protected Overrides Sub OnUpdated()
            MyBase.OnUpdated()
            If _MaxThreads = -1 OrElse _ActiveThreads <= _MaxThreads Then
                Interlocked.Increment(_ActiveThreads)
                Try
                    If _MaxThreads = -1 OrElse _ActiveThreads <= _MaxThreads Then AsyncUpdate()
                Catch ex As Exception
                    Handle(ex)
                End Try
                Interlocked.Decrement(_ActiveThreads)
            End If
        End Sub
        Protected Overridable Sub AsyncUpdate() Implements IAsyncObject.AsyncUpdate
            RaiseEvent AsyncUpdateEvent(Me)
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
        End Sub
        Sub New(AsyncUpdate As AsyncObjectEventDelegate)
            AddHandler AsyncUpdateEvent, AsyncUpdate
        End Sub
        Sub New(Updater As Updaters.IUpdater, AsyncUpdate As AsyncObjectEventDelegate)
            MyBase.New(Updater)
            AddHandler AsyncUpdateEvent, AsyncUpdate
        End Sub
        Public Delegate Sub AsyncObjectEventDelegate(Sender As IAsyncObject)
    End Class
End Namespace