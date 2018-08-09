Imports Zds.Flow.Collections
Imports Zds.Flow.Updatables

Namespace Updaters
    <DebuggerStepThrough>
    Public Class UpdaterX
        Inherits Updater
        Private _Threads As New SafeList(Of Threading.Thread)
        Private _TotalThreads As Integer = 0
        Private _IdleThreads As Integer = 0
        Private _AvailableThreads As Integer = 0
        Public Event ThreadCreated(Sender As UpdaterX)
        Public Event ThreadDestroyed(Sender As UpdaterX)
        Public Property MaxThreads As Integer = 50
        Public Property MaxIdleThreads As Integer = 10
        Public Property MinThreadLifeSpan As TimeSpan = TimeSpan.FromMinutes(1)
        Public Property MaxThreadLifeSpan As TimeSpan = TimeSpan.MaxValue
        Protected Property ActionLock As New Object
        Public ReadOnly Property Threads As Threading.Thread()
            Get
                Return _Threads.Elements
            End Get
        End Property
        Public ReadOnly Property TotalThreads As ULong
            Get
                Return _TotalThreads
            End Get
        End Property
        Public ReadOnly Property IdleThreads As ULong
            Get
                Return _IdleThreads
            End Get
        End Property
        Public Overrides Sub Start()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If Not IsRunning Then
                    MyBase.Start()
                    If Targets.Length <> 0 And TotalThreads = 0 Then CreateThread()
                End If
            End SyncLock
        End Sub
        Public Overrides Sub [Stop]()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If IsRunning Then
                    MyBase.Stop()
                End If
            End SyncLock
        End Sub
        Public Overrides Sub Destroy()
            SyncLock ActionLock
                MyBase.Destroy()
            End SyncLock
        End Sub
        Public Sub Serve()
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            Work()
        End Sub
        Public Sub Serve(state As Object)
            Work()
        End Sub
        Public Sub CreateThread()
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            Dim Thread As New Threading.Thread(AddressOf Work)
            Thread.Start()
        End Sub
        Public Overrides Sub Add(Updatable As IUpdatable)
            MyBase.Add(Updatable)
            If Targets.Length <> 0 And TotalThreads = 0 Then CreateThread()
        End Sub
        Public Overrides Sub Remove(Updatable As IUpdatable)
            MyBase.Remove(Updatable)
        End Sub
        Protected Overridable Sub Work()
            If IsDestroyed Then Exit Sub
            If Not IsRunning Or Targets.Length = 0 Then Exit Sub
            RaiseEvent ThreadCreated(Me)
            Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
            _Threads.Add(Thread)
            Threading.Interlocked.Increment(_TotalThreads)
            Threading.Interlocked.Increment(_IdleThreads)
            Dim Stopwatch As New Stopwatch.Stopwatch
            Stopwatch.Start()
            Dim IdleThreads As Integer
            Do
                If Not IsRunning Or Targets.Length = 0 Then Exit Do
                If IsPaused = False Then
                    IdleThreads = Threading.Interlocked.Decrement(_IdleThreads)
                    If IdleThreads = 0 AndAlso _TotalThreads <= _MaxThreads Then
                        CreateThread()
                    End If
                    Update()
                    IdleThreads = Threading.Interlocked.Increment(_IdleThreads)
                    If (IdleThreads > _MaxIdleThreads AndAlso Stopwatch.Elapsed >= MinThreadLifeSpan) OrElse
                        (_TotalThreads > 1 AndAlso Stopwatch.Elapsed >= MaxThreadLifeSpan) Then
                        Exit Do
                    End If
                End If
                If Not IsRunning Or Targets.Length = 0 Then Exit Do
                If DelayHandler IsNot Nothing Then DelayHandler.Delay()
            Loop
            Stopwatch.Stop()
            _Threads.Remove(Thread)
            Threading.Interlocked.Decrement(_IdleThreads)
            Threading.Interlocked.Decrement(_TotalThreads)
            RaiseEvent ThreadDestroyed(Me)
        End Sub
    End Class
End Namespace