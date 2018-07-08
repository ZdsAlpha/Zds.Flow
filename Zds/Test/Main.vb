Imports System.Drawing
Imports Zds.Flow
Imports Zds.Flow.Machinery
Imports Zds.Flow.Updatables
Public Module Main
    Public Sub Main()
        Dim frames As Integer = 0
        Dim Timer As New SyncTimer(Sub()
                                       Console.Title = frames.ToString + " fps"
                                       frames = 0
                                   End Sub)
        Timer.Start()
        Dim sc As New ScreenCapture
        IO.Directory.CreateDirectory(sc.Directory)
        sc.ExceptionHandler = New ExceptionHandlers.ConsoleLogger
        AddHandler sc.OnFinishedFrame, Sub() frames += 1
        sc.Start()
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