Namespace ExceptionHandlers
    <DebuggerStepThrough>
    Public Class ConsoleLogger
        Inherits ExceptionHandler
        Public Overrides Sub [Catch](Sender As Object, Exception As Exception)
            MyBase.Catch(Sender, Exception)
            Console.WriteLine("Exception occured in Object: " + Sender.GetHashCode.ToString)
            Console.WriteLine(Exception.ToString)
        End Sub
    End Class
End Namespace