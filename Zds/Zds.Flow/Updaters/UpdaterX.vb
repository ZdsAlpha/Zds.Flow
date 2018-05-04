Imports Zds.Flow.Collections
Imports Zds.Flow.Updatables

Namespace Updaters
    <DebuggerStepThrough>
    Public Class UpdaterX
        Inherits Updater
        Private _Threads As New SafeList(Of Threading.Thread)
        Private _TotalThreads As ULong = 0
        Private _IdleThreads As ULong = 0
        Private _AvailableThreads As ULong = 0
        Public Event ThreadCreated(Sender As UpdaterX)
        Public Event ThreadDestroyed(Sender As UpdaterX)
        Public Property MaxThreads As ULong = 50
        Public Property MaxIdleThreads As ULong = 10
        Protected Property Lock As New Object
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
            SyncLock Lock
                Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
                _Threads.Add(Thread)
                _TotalThreads += 1
                _IdleThreads += 1
            End SyncLock
            Do
                If Not IsRunning Or Targets.Length = 0 Then Exit Do
                If IsPaused = False Then
                    SyncLock Lock
                        _IdleThreads -= 1
                        If _IdleThreads = 0 And _TotalThreads <= MaxThreads Then
                            CreateThread()
                        End If
                    End SyncLock
                    Update()
                    SyncLock Lock
                        _IdleThreads += 1
                        If _IdleThreads > MaxIdleThreads Then
                            Exit Do
                        End If
                    End SyncLock
                End If
                If Not IsRunning Or Targets.Length = 0 Then Exit Do
                If DelayHandler IsNot Nothing Then DelayHandler.Delay()
            Loop
            SyncLock Lock
                Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
                _Threads.Remove(Thread)
                _TotalThreads -= 1
                _IdleThreads -= 1
            End SyncLock
            RaiseEvent ThreadDestroyed(Me)
        End Sub
    End Class
End Namespace