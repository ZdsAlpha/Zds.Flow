Namespace Updaters
    <DebuggerStepThrough>
    Public Class UniUpdater
        Inherits Updater
        Private _Thread As New Threading.Thread(AddressOf Work)
        Private _IsActive As Boolean = False
        Protected ReadOnly Property ActionLock As New Object
        Public ReadOnly Property Thread As Threading.Thread
            Get
                Return _Thread
            End Get
        End Property
        Public Property IsBackground As Boolean
            Get
                Return _Thread.IsBackground
            End Get
            Set(value As Boolean)
                _Thread.IsBackground = value
            End Set
        End Property
        Public Overrides Sub Start()
            SyncLock ActionLock
                If IsDestroyed Then Exit Sub
                If Not IsRunning Then
                    MyBase.Start()
                    If Not _Thread.IsAlive Then
                        _Thread = New Threading.Thread(AddressOf Work)
                        _Thread.Start()
                    End If
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
            _IsActive = True
            Do
                If IsRunning = False Then Exit Do
                If IsPaused = False Then
                    Update()
                End If
                If IsRunning = False Then Exit Do
                If DelayHandler IsNot Nothing Then DelayHandler.Delay()
            Loop
            _IsActive = False
        End Sub
    End Class
End Namespace