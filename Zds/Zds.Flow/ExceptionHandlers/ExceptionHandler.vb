Namespace ExceptionHandlers
    Public Class ExceptionHandler
        Implements IExceptionHandler
        Public Event OnException(Sender As Object, Exception As Exception)
        Public Overridable Sub [Catch](Sender As Object, Exception As Exception) Implements IExceptionHandler.Catch
            RaiseEvent OnException(Sender, Exception)
        End Sub
        Public Sub New()
        End Sub
        Public Sub New(OnException As OnExceptionEventHandler)
            AddHandler OnException, OnException
        End Sub
    End Class
End Namespace