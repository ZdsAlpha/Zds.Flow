Imports Zds.Flow.Collections
Imports Zds.Flow.Updatables

Namespace Updaters
    <DebuggerStepThrough>
    Public Class UpdaterX
        Inherits Updater
        Private _ThreadCreatedEvent As SafeList(Of UpdaterXEventDelegate)
        Private _ThreadDestroyedEvent As SafeList(Of UpdaterXEventDelegate)
        Private _Threads As New SafeList(Of Threading.Thread)
        Private _TotalThreads As Integer = 0
        Private _IdleThreads As Integer = 0
        Private _AvailableThreads As Integer = 0
        Public Custom Event ThreadCreatedEvent As UpdaterXEventDelegate
            AddHandler(value As UpdaterXEventDelegate)
                If _ThreadCreatedEvent Is Nothing Then _ThreadCreatedEvent = New SafeList(Of UpdaterXEventDelegate)
                _ThreadCreatedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdaterXEventDelegate)
                If _ThreadCreatedEvent IsNot Nothing Then _ThreadCreatedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As UpdaterX, Thread As Threading.Thread)
                If _ThreadCreatedEvent IsNot Nothing Then
                    For Each [Delegate] In _ThreadCreatedEvent.Elements
                        [Delegate].Invoke(Sender, Thread)
                    Next
                End If
            End RaiseEvent
        End Event
        Public Custom Event ThreadDestroyedEvent As UpdaterXEventDelegate
            AddHandler(value As UpdaterXEventDelegate)
                If _ThreadDestroyedEvent Is Nothing Then _ThreadDestroyedEvent = New SafeList(Of UpdaterXEventDelegate)
                _ThreadDestroyedEvent.Add(value)
            End AddHandler
            RemoveHandler(value As UpdaterXEventDelegate)
                If _ThreadDestroyedEvent IsNot Nothing Then _ThreadDestroyedEvent.Remove(value)
            End RemoveHandler
            RaiseEvent(Sender As UpdaterX, Thread As Threading.Thread)
                If _ThreadDestroyedEvent IsNot Nothing Then
                    For Each [Delegate] In _ThreadDestroyedEvent.Elements
                        [Delegate].Invoke(Sender, Thread)
                    Next
                End If
            End RaiseEvent
        End Event
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
            Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
            RaiseEvent ThreadCreatedEvent(Me, Thread)
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
            RaiseEvent ThreadDestroyedEvent(Me, Thread)
        End Sub

        Public Delegate Sub UpdaterXEventDelegate(Sender As UpdaterX, Thread As Threading.Thread)
    End Class
End Namespace