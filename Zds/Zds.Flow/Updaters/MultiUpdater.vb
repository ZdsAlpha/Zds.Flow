Imports Zds.Flow.Collections
Namespace Updaters
    <DebuggerStepThrough>
    Public Class MultiUpdater
        Inherits Updater
        Private _Threads As New SafeList(Of Threading.Thread)
        Private _TotalThreads As ULong = 0
        Private _IdleThreads As ULong = 0
        Private _IsBackground As Boolean = False
        Public Event ThreadCreated(Sender As MultiUpdater)
        Public Event ThreadDestroyed(Sender As MultiUpdater)
        Public Property MaxThreads As ULong = 50
        Public Property MaxIdleThreads As ULong = 10
        Public Property AutoCreateThreads As Boolean = True
        Public Property UseThreadPool As Boolean = True
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
        Public Property IsBackground As Boolean
            Get
                Return _IsBackground
            End Get
            Set(value As Boolean)
                SyncLock Lock
                    For Each Thread In _Threads.Elements
                        Thread.IsBackground = value
                    Next
                    _IsBackground = value
                End SyncLock
            End Set
        End Property
        Public Overrides Sub Start()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If Not IsRunning Then
                    MyBase.Start()
                    If AutoCreateThreads Then CreateThread()
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
            If Not UseThreadPool Then
                Dim Thread As New Threading.Thread(AddressOf Work)
                Thread.Start()
            Else
                Try
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf Serve)
                Catch ex As Exception
                End Try
            End If
        End Sub
        Protected Overridable Sub Work()
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            RaiseEvent ThreadCreated(Me)
            SyncLock Lock
                Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
                Thread.IsBackground = _IsBackground
                _Threads.Add(Thread)
                _TotalThreads += 1
                _IdleThreads += 1
            End SyncLock
            Do
                If Not IsRunning Then Exit Do
                If IsPaused = False Then
                    SyncLock Lock
                        _IdleThreads -= 1
                        If AutoCreateThreads And _IdleThreads = 0 And _TotalThreads <= MaxThreads Then
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
                If Not IsRunning Then Exit Do
                If DelayHandler IsNot Nothing Then DelayHandler.Delay()
            Loop
            SyncLock Lock
                Dim Thread As Threading.Thread = Threading.Thread.CurrentThread
                If Thread.IsThreadPoolThread Then Thread.IsBackground = True
                _Threads.Remove(Thread)
                _TotalThreads -= 1
                _IdleThreads -= 1
            End SyncLock
            RaiseEvent ThreadDestroyed(Me)
        End Sub
    End Class
End Namespace