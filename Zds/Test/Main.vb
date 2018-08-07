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
        Threading.Thread.Sleep(10000)
        sc.Destroy()
        Threading.Thread.Sleep(5000)
        Dim x = _Bitmap.List.Elements
        Dim y = _Bitmap.GetLeakedBitmaps
        Dim a = _Buffer.List.Elements
        Dim b = _Buffer.GetLeakedBuffers
        Dim xyz = Collections.Round.AllRounds
        Stop
    End Sub
    Public Function Extract(Machine As IContainer) As Object()
        Dim List As New List(Of Object)
        For Each obj In Machine.Targets

        Next
        Return List.ToArray
    End Function
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

    Private Generator As New Timers.AsyncSource(Of Tuple(Of _Bitmap, Integer))(Me, AddressOf Generate) With {.Delay = TimeSpan.FromSeconds(0.1)}
    Private Processor As New AsyncConverter(Of Tuple(Of _Bitmap, Integer), Tuple(Of _Buffer, Integer))(Me, AddressOf Process) With {.MustConvert = True}
    Private Flusher As New SyncSink(Of Tuple(Of _Buffer, Integer))(Me, AddressOf Flush)

    Private JpegEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
    Private Parameters As New EncoderParameters(1)

    Private frameId As Long = 0
    Private Function Generate(ByRef output As Tuple(Of _Bitmap, Integer)) As Boolean
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
        output = New Tuple(Of _Bitmap, Integer)(New _Bitmap(shot), id)
        Return True
    End Function
    Private Function Process(input As Tuple(Of _Bitmap, Integer), ByRef output As Tuple(Of _Buffer, Integer)) As Boolean
        Dim bytes As Byte() = Nothing
        Using stream As New IO.MemoryStream()
            input.Item1.Bitmap.Save(stream, JpegEncoder, Parameters)
            bytes = stream.ToArray
        End Using
        output = New Tuple(Of _Buffer, Integer)(New _Buffer(bytes), input.Item2)
        input.Item1.Dispose()
        Return True
    End Function
    Private Function Flush(input As Tuple(Of _Buffer, Integer)) As Boolean
        IO.File.WriteAllBytes(Directory + input.Item2.ToString("D6") + ".jpg", input.Item1.Data)
        input.Item1.Dispose()
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
Public Class _Bitmap
    Implements IDisposable
    Private Shared UId As Long
    Public Shared ReadOnly Property List As New Collections.SafeList(Of _Bitmap)
    Public ReadOnly Property IsDispsoed As Boolean
    Public ReadOnly Property Bitmap As Bitmap
    Public ReadOnly Property Id As Long
    Public Sub Dispose() Implements IDisposable.Dispose
        _IsDispsoed = True
        Bitmap.Dispose()
    End Sub
    Sub New(Bitmap As Bitmap)
        List.Add(Me)
        Id = Threading.Interlocked.Increment(UId)
        Me.Bitmap = Bitmap
    End Sub
    Public Shared Function GetLeakedBitmaps() As _Bitmap()
        Return List.Elements.Where(Function(x) Not x.IsDispsoed).ToArray()
    End Function
End Class
Public Class _Buffer
    Implements IDisposable
    Private Shared UId As Long
    Public Shared ReadOnly Property List As New Collections.SafeList(Of _Buffer)
    Public ReadOnly Property IsDispsoed As Boolean
    Public ReadOnly Property Data As Byte()
    Public ReadOnly Property Id As Long
    Public Sub Dispose() Implements IDisposable.Dispose
        _IsDispsoed = True
        _Data = Nothing
    End Sub
    Sub New(Data As Byte())
        List.Add(Me)
        Id = Threading.Interlocked.Increment(UId)
        Me.Data = Data
    End Sub
    Public Shared Function GetLeakedBuffers() As _Buffer()
        Return List.Elements.Where(Function(x) Not x.IsDispsoed).ToArray()
    End Function
End Class