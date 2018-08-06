Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
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
        AddHandler sc.OnFinishedFrame, Sub() Threading.Interlocked.Increment(frames)
        sc.FramesPerSecond = 15
        sc.Start()
    End Sub
    Public Sub FPSUpdater()
        Console.Title = frames.ToString + " fps"
        frames = 0
    End Sub
End Module
Public Class ScreenCapture
    Inherits Systems.Machinery
    Private _Quality As Single
    Public Event OnFinishedFrame(Frame As Long)
    Public Property Directory As String = "Images\"
    Public Property FramesPerSecond As Double
        Get
            Return 1 / Generator.Delay.TotalSeconds
        End Get
        Set(value As Double)
            Generator.Delay = TimeSpan.FromSeconds(1 / value)
        End Set
    End Property
    Public Property Quality As Single
        Get
            Return _Quality
        End Get
        Set(value As Single)
            If value < 0 Then value = 0
            If value > 1 Then value = 1
            SetQuality(value * 100)
        End Set
    End Property

    Private Generator As New Timers.AsyncSource(Of Tuple(Of Bitmap, Integer))(Me, AddressOf Generate) With {.Delay = TimeSpan.FromSeconds(0.1)}
    Private Processor As New AsyncConverter(Of Tuple(Of Bitmap, Integer), Tuple(Of Byte(), Integer))(Me, AddressOf Process) With {.MustConvert = True}
    Private Flusher As New SyncSink(Of Tuple(Of Byte(), Integer))(Me, AddressOf Flush)

    Private JpegEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
    Private Parameters As New EncoderParameters(1)

    Private frameId As Long = 0
    Private Function Generate(ByRef output As Tuple(Of Bitmap, Integer)) As Boolean
        Dim bounds = Screen.PrimaryScreen.Bounds
        Dim shot As New Bitmap(bounds.Size.Width, bounds.Size.Height)
        Dim id As Long
        Using g = Graphics.FromImage(shot)
            id = Threading.Interlocked.Increment(frameId)
            Dim position = Cursor.Position
            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size)
            Dim pci As CURSORINFO
            pci.cbSize = Marshal.SizeOf(GetType(CURSORINFO))
            If GetCursorInfo(pci) AndAlso pci.flags = CURSOR_SHOWING Then
                DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor)
                g.ReleaseHdc()
            End If
        End Using
        output = New Tuple(Of Bitmap, Integer)(shot, id)
        Return True
    End Function
    Private Function Process(input As Tuple(Of Bitmap, Integer), ByRef output As Tuple(Of Byte(), Integer)) As Boolean
        Dim bytes As Byte() = Nothing
        Using stream As New IO.MemoryStream()
            input.Item1.Save(stream, JpegEncoder, Parameters)
            bytes = stream.ToArray
        End Using
        output = New Tuple(Of Byte(), Integer)(bytes, input.Item2)
        input.Item1.Dispose()
        Return True
    End Function
    Private Function Flush(input As Tuple(Of Byte(), Integer)) As Boolean
        IO.File.WriteAllBytes(Directory + input.Item2.ToString("D6") + ".jpg", input.Item1)
        RaiseEvent OnFinishedFrame(input.Item2)
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
        SetDpiAwareness()
        Quality = 1
        Add(Generator)
        Add(Processor)
        Add(Flusher)
        Generator.Sink = Processor
        Processor.Sink = Flusher
        Processor.Start()
        Flusher.Start()
    End Sub

    <StructLayout(LayoutKind.Sequential)>
    Private Structure CURSORINFO
        Public cbSize As Int32
        Public flags As Int32
        Public hCursor As IntPtr
        Public ptScreenPos As POINTAPI
    End Structure
    <StructLayout(LayoutKind.Sequential)>
    Private Structure POINTAPI
        Public x As Integer
        Public y As Integer
    End Structure
    <DllImport("user32.dll")>
    Private Shared Function GetCursorInfo(<Out> ByRef pci As CURSORINFO) As Boolean
    End Function
    <DllImport("user32.dll")>
    Private Shared Function DrawIcon(ByVal hDC As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal hIcon As IntPtr) As Boolean
    End Function
    Const CURSOR_SHOWING As Int32 = &H1

    Private Sub SetQuality(Quality As Integer)
        Dim myEncoder As Encoder = Encoder.Quality
        Dim myEncoderParameter As New EncoderParameter(myEncoder, Quality)
        Parameters.Param(0) = myEncoderParameter
    End Sub
    Private Function GetEncoder(ByVal format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        For Each codec As ImageCodecInfo In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Private Enum ProcessDPIAwareness
        ProcessDPIUnaware = 0
        ProcessSystemDPIAware = 1
        ProcessPerMonitorDPIAware = 2
    End Enum
    <Runtime.InteropServices.DllImport("shcore.dll")>
    Private Shared Function SetProcessDpiAwareness(ByVal value As ProcessDPIAwareness) As Integer
    End Function
    Private Shared Sub SetDpiAwareness()
        Try
            If Environment.OSVersion.Version.Major >= 6 Then
                SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware)
            End If
        Catch
        End Try
    End Sub
End Class