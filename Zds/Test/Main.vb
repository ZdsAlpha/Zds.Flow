Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports Test
Imports Zds.Flow
Imports Zds.Flow.Collections
Imports Zds.Flow.DelayHandlers
Imports Zds.Flow.ExceptionHandlers
Imports Zds.Flow.Interfaces
Imports Zds.Flow.Stopwatch
Imports Zds.Flow.Updatables
Imports Zds.Flow.Updaters
Public Module Main
    Public ReadOnly Program As New UpdaterX
    Public Sub Main()
        Program.Start()
        Dim Timer As New AsyncTimer(Program)
        Dim Stopwatch As New Stopwatch
        AddHandler Timer.TickEvent, Sub(sender, ByRef time)
                                        Console.WriteLine("Tick occured at time: " + Stopwatch.Elapsed.ToString)
                                        Thread.Sleep(10000)
                                    End Sub
        Timer.Delay = TimeSpan.FromSeconds(1)
        Stopwatch.Start()
        Timer.Start()
    End Sub
End Module