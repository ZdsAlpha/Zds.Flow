Imports System.Threading
Imports Zds.Flow
Imports Zds.Flow.Machinery
Imports Zds.Flow.Stopwatch
Imports Zds.Flow.Updatables
Imports Zds.Flow.Updaters
Public Module Main
    Public Sub Main()
        Dim Random As New Random
        Const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

        Dim Source As New SyncSource(Of String)(Function(ByRef o)
                                                    o = New String(Enumerable.Repeat(chars, Random.Next(30, 50)).Select(Function(s) s(Random.Next(s.Length))).ToArray())
                                                    Return True
                                                End Function)
        Dim Converter As New SyncConverter(Of String, Byte())(Function(i, ByRef o)
                                                                  o = Text.Encoding.ASCII.GetBytes(i)
                                                                  Return True
                                                              End Function)
        Dim Sink As New SyncSink(Of Byte())(Function(b)
                                                Console.WriteLine(Text.Encoding.ASCII.GetString(b))
                                                Return True
                                            End Function)
        Source.Sink = Converter
        Converter.Sink = Sink
        Dim ticks As ULong = 0
        Dim Activator As New SyncObject(Sub()
                                            If Random.Next(2) = 1 Then Source.Activate()
                                            If Random.Next(4) = 1 Then Converter.Activate()
                                            If Random.Next(100) = 1 Then Sink.Activate()
                                            ticks += 1
                                        End Sub)
        Activator.Start()
        Dim Counter As New SyncTimer(Sub()
                                         Console.Title = ticks.ToString + " ticks/second"
                                         ticks = 0
                                     End Sub)
        Counter.Start()
        Counter.Delay = TimeSpan.FromSeconds(1)

    End Sub
End Module