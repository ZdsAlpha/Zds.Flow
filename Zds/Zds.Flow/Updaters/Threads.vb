Namespace Updaters
    <DebuggerStepThrough>
    Public Class Threads
        Inherits Updater
        Private _Threads() As Threading.Thread
        Private _Count As Integer
        Protected ReadOnly Property ActionLock As New Object
        Public Overrides Sub Start()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If Not IsRunning Then
                    MyBase.Start()
                    For i = 0 To _Threads.Length - 1
                        If Not _Threads(i).IsAlive Then
                            _Threads(i) = New Threading.Thread(AddressOf Work)
                            _Threads(i).Start()
                        End If
                    Next
                End If
            End SyncLock
        End Sub
        Public Overrides Sub [Stop]()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If IsRunning Then
                    MyBase.[Stop]()
                End If
            End SyncLock
        End Sub
        Public Overrides Sub Destroy()
            SyncLock ActionLock
                MyBase.Destroy()
            End SyncLock
        End Sub
        Protected Overridable Sub Work()
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            Do
                If IsRunning = False Then Exit Do
                If IsPaused = False Then
                    Update()
                End If
                If IsRunning = False Then Exit Do
                If DelayHandler IsNot Nothing Then DelayHandler.Delay()
            Loop
        End Sub
        Sub New(Count As Integer)
            _Threads = New Threading.Thread(Count - 1) {}
            For i = 0 To Count - 1
                _Threads(i) = New Threading.Thread(AddressOf Work)
            Next
            _Count = Count
        End Sub
    End Class
End Namespace