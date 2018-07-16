Imports System.Drawing
Imports Zds.Flow
Imports Zds.Flow.Machinery
Imports Zds.Flow.Machinery.Updatables
Imports Zds.Flow.Updatables
Public Module Main
    Public Sub Main()
        Dim syncTicks As Integer = 0
        Dim asyncTicks As Integer = 0
        Dim Timer As New SyncTimer(Sub()
                                       Console.Clear()
                                       Dim st = syncTicks
                                       Dim ast = asyncTicks
                                       syncTicks = 0
                                       asyncTicks = 0
                                       Console.WriteLine("Sync Ticks = " + st.ToString)
                                       Console.WriteLine("Async Ticks = " + ast.ToString)
                                       Console.WriteLine("Async - Sync = " + (ast - st).ToString)
                                       Console.WriteLine("Total Objects = " + Updatable.DefaultUpdater.Targets.Length.ToString)
                                       Console.WriteLine("Threads = " + CType(Updatable.DefaultUpdater, Updaters.UpdaterX).Threads.Length.ToString)
                                   End Sub)
        Dim sync As New SyncObject(Sub()
                                       syncTicks += 1
                                   End Sub)
        Dim async As New AsyncObject(Sub()
                                         asyncTicks += 1
                                     End Sub)
        Timer.Start()
        sync.Start()
        async.Start()
        For x = 0 To 10000
            Dim k As New RigidStateMachine(Of Integer)
            Threading.Thread.Sleep(10)
        Next
        Stop
    End Sub
    Dim frames As Integer = 0
    Public Sub ScreenCapture()
        Dim Timer As New SyncTimer(AddressOf FPSUpdater)
        Timer.Start()
        Dim sc As New ScreenCapture
        IO.Directory.CreateDirectory(sc.Directory)
        Dim consoleLogger As ExceptionHandlers.ConsoleLogger = New ExceptionHandlers.ConsoleLogger
        Updatable.DefaultUpdater.ExceptionHandler = consoleLogger
        sc.ExceptionHandler = consoleLogger
        AddHandler sc.OnFinishedFrame, Sub() frames += 1
        sc.Start()

    End Sub
    Public Sub FPSUpdater()
        Console.Title = frames.ToString + " fps"
        frames = 0
    End Sub
End Module
Public Class ScreenCapture
    Inherits Machinery
    Public Property Directory As String = "Images\"
    Private Generator As New SyncSource(Of Tuple(Of Bitmap, DateTime))(AddressOf Generate)
    Private Processor As New SyncConverter(Of Tuple(Of Bitmap, DateTime), Tuple(Of Byte(), DateTime))(AddressOf Process) With {.MustConvert = True}
    Private Flusher As New SyncSink(Of Tuple(Of Byte(), DateTime))(AddressOf Flush)
    Private Random1 As New Random
    Private Random2 As New Random
    Private Random3 As New Random
    Public Event OnFinishedFrame()
    Private Function Generate(ByRef output As Tuple(Of Bitmap, DateTime)) As Boolean
        If Random1.NextDouble >= 0.5 Then Return False
        Dim time As DateTime
        Dim screens = Windows.Forms.Screen.AllScreens
        Dim bounds = screens(0).Bounds
        Dim shot As New Bitmap(bounds.Size.Width, bounds.Size.Height)
        Using g = Graphics.FromImage(shot)
            time = DateTime.Now
            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size)
        End Using
        output = New Tuple(Of Bitmap, DateTime)(shot, time)
        Return True
    End Function
    Private Function Process(input As Tuple(Of Bitmap, DateTime), ByRef output As Tuple(Of Byte(), DateTime)) As Boolean
        If Random2.NextDouble >= 0.5 Then Return False
        Dim bytes As Byte() = Nothing
        Using stream As New IO.MemoryStream()
            input.Item1.Save(stream, Imaging.ImageFormat.Jpeg)
            bytes = stream.ToArray
        End Using
        output = New Tuple(Of Byte(), DateTime)(bytes, input.Item2)
        input.Item1.Dispose()
        Return True
    End Function
    Private Function Flush(input As Tuple(Of Byte(), DateTime)) As Boolean
        If Random3.NextDouble >= 0.5 Then Return False
        IO.File.WriteAllBytes(Directory + input.Item2.ToString("yyyy-MM-dd HH:mm:ss.fff").Replace("/", "_").Replace("\", "_").Replace(":", "_") + ".jpg", input.Item1)
        RaiseEvent OnFinishedFrame()
        Return True
    End Function
    Public Overrides Sub Start()
        MyBase.Start()
        If IsRunning Then
            Generator.Start()
            Processor.Start()
            Flusher.Start()
        End If
    End Sub
    Public Overrides Sub [Stop]()
        MyBase.Stop()
        If Not IsRunning Then
            Generator.Stop()
            Processor.Stop()
            Flusher.Stop()
        End If
    End Sub
    Sub New()
        Add(Generator)
        Add(Processor)
        Add(Flusher)
        Generator.Sink = Processor
        Processor.Sink = Flusher
    End Sub
End Class