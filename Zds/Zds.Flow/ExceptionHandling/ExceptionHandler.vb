Imports Zds.Flow.Collections
Namespace ExceptionHandling
    <DebuggerStepThrough>
    Public Class ExceptionHandler
        Implements IExceptionHandler, IThrowsException
        Private _OnException As SafeList(Of IThrowsException.OnExceptionDelegate)
        Public Custom Event OnException As IThrowsException.OnExceptionDelegate Implements IThrowsException.OnException
            AddHandler(value As IThrowsException.OnExceptionDelegate)
                If _OnException Is Nothing Then _OnException = New SafeList(Of IThrowsException.OnExceptionDelegate)
                _OnException.Add(value)
            End AddHandler
            RemoveHandler(value As IThrowsException.OnExceptionDelegate)
                If _OnException IsNot Nothing Then _OnException.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As Object, Exception As Exception)
                If _OnException IsNot Nothing Then
                    For Each [Delegate] In _OnException.Elements
                        [Delegate].Invoke(Sender, Exception)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Overridable Sub Add(Obj As IThrowsException) Implements IExceptionHandler.Add
            AddHandler Obj.OnException, AddressOf Handle
        End Sub
        Public Overridable Sub Remove(Obj As IThrowsException) Implements IExceptionHandler.Remove
            RemoveHandler Obj.OnException, AddressOf Handle
        End Sub
        Public Overridable Sub Handle(Sender As Object, Exception As Exception) Implements IExceptionHandler.Handle, IThrowsException.Throw
            RaiseEvent OnException(Sender, Exception)
        End Sub
        Public Sub New()
        End Sub
        Public Sub New(OnException As IThrowsException.OnExceptionDelegate)
            AddHandler OnException, OnException
        End Sub
    End Class
End Namespace