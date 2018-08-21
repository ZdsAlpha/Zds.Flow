Namespace ExceptionHandling
    Public Interface IExceptionHandler
        Sub Add(Obj As IThrowsException)
        Sub Remove(Obj As IThrowsException)
        Sub Handle(Sender As Object, Exception As Exception)
    End Interface
End Namespace