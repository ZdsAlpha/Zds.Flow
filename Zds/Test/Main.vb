Imports System.Drawing
Imports Zds.Flow
Imports Zds.Flow.Machinery
Imports Zds.Flow.Machinery.Objects
Imports Zds.Flow.Updatables
Public Module Main
    Public Sub Main()
        Dim Updater As Updaters.UpdaterX = Updatable.DefaultUpdater
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
    Public Event OnFinishedFrame()
    Private Function Generate(ByRef output As Tuple(Of Bitmap, DateTime)) As Boolean
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