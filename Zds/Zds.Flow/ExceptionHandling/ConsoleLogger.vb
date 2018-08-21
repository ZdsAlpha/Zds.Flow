Namespace ExceptionHandling
    <DebuggerStepThrough>
    Public Class ConsoleLogger
        Inherits ExceptionHandler
        Public Overrides Sub Handle(Sender As Object, Exception As Exception)
            MyBase.Handle(Sender, Exception)
            Console.WriteLine("Exception occured in " + Sender.GetType.ToString + ": " + Sender.GetHashCode.ToString)
            Console.WriteLine(Exception.ToString)
        End Sub
    End Class
End Namespace