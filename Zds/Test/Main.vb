Imports System.Drawing
Imports Zds.Flow
Imports Zds.Flow.Machinery
Imports Zds.Flow.Machinery.Objects
Imports Zds.Flow.Updatables
Public Module Main
    Public Sub Main()
        Dim Updater As Updaters.UpdaterX = Updatable.DefaultUpdater
        ScreenCapture()
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
        sc.FramesPerSecond = 30
        sc.Start()
    End Sub
    Public Sub FPSUpdater()
        Console.Title = frames.ToString + " fps"
        frames = 0
    End Sub
End Module
Public Class ScreenCapture
    Inherits Systems.Machinery
    Public Property Directory As String = "Images\"
    Public Property FramesPerSecond As Double
        Get
            Return 1 / Generator.Delay.TotalSeconds
        End Get
        Set(value As Double)
            Generator.Delay = TimeSpan.FromSeconds(1 / value)
        End Set
    End Property
    Private Generator As New Timers.AsyncSource(Of Tuple(Of Bitmap, Integer))(Me, AddressOf Generate) With {.Delay = TimeSpan.FromSeconds(0.1)}
    Private Processor As New AsyncConverter(Of Tuple(Of Bitmap, Integer), Tuple(Of Byte(), Integer))(Me, AddressOf Process) With {.MustConvert = True}
    Private Flusher As New SyncSink(Of Tuple(Of Byte(), Integer))(Me, AddressOf Flush)
    Public Event OnFinishedFrame()
    Private Function Generate(ByRef output As Tuple(Of Bitmap, Integer)) As Boolean
        Dim time As DateTime
        Dim screens = Windows.Forms.Screen.AllScreens
        Dim bounds = screens(0).Bounds
        Dim shot As New Bitmap(bounds.Size.Width, bounds.Size.Height)
        Using g = Graphics.FromImage(shot)
            time = DateTime.Now
            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size)
        End Using
        output = New Tuple(Of Bitmap, Integer)(shot, time)
        Return True
    End Function
    Private Function Process(input As Tuple(Of Bitmap, Integer), ByRef output As Tuple(Of Byte(), Integer)) As Boolean
        Dim bytes As Byte() = Nothing
        Using stream As New IO.MemoryStream()
            input.Item1.Save(stream, Imaging.ImageFormat.Jpeg)
            bytes = stream.ToArray
        End Using
        output = New Tuple(Of Byte(), Integer)(bytes, input.Item2)
        input.Item1.Dispose()
        Return True
    End Function
    Private Function Flush(input As Tuple(Of Byte(), Integer)) As Boolean
        IO.File.WriteAllBytes(Directory + input.Item2.ToString("yyyy-MM-dd HH:mm:ss.fff").Replace("/", "_").Replace("\", "_").Replace(":", "_") + ".jpg", input.Item1)
        RaiseEvent OnFinishedFrame()
        Return True
    End Function
    Public Overrides Sub Start()
        MyBase.Start()
        If IsRunning Then
            Generator.Start()
            'Processor.Start()
            'Flusher.Start()
        End If
    End Sub
    Public Overrides Sub [Stop]()
        MyBase.Stop()
        If Not IsRunning Then
            Generator.Stop()
            'Processor.Stop()
            'Flusher.Stop()
        End If
    End Sub
    Sub New()
        Add(Generator)
        Add(Processor)
        Add(Flusher)
        Generator.Sink = Processor
        Processor.Sink = Flusher
        Processor.Start()
        Flusher.Start()
    End Sub
End Class