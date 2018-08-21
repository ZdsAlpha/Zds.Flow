Namespace ExceptionHandling
    Public Interface IThrowsException
        Event OnException As OnExceptionDelegate
        Sub [Throw](Sender As Object, Exception As Exception)
        Delegate Sub OnExceptionDelegate(Sender As Object, Exception As Exception)
    End Interface
End Namespace