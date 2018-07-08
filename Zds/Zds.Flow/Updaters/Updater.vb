Imports Zds.Flow.Collections

Namespace Updaters
    <DebuggerStepThrough>
    Public Class Updater
        Implements IUpdater
        Private _IsDestroyed As Boolean = False
        Private _IsRunning As Boolean = False
        Private _IsPaused As Boolean = False
        Private _Targets As New SafeList(Of Updatables.IUpdatable)
        Public Property DelayHandler As DelayHandlers.IDelayHandler Implements IUpdater.DelayHandler
        Public Property ExceptionHandler As ExceptionHandlers.IExceptionHandler Implements IUpdater.ExceptionHandler
        Public ReadOnly Property Targets As Updatables.IUpdatable() Implements IUpdater.Targets
            Get
                Return _Targets.Elements
            End Get
        End Property
        Public ReadOnly Property IsPaused As Boolean Implements Interfaces.IPauseResumable.IsPaused
            Get
                Return _IsPaused
            End Get
        End Property
        Public ReadOnly Property IsDestroyed As Boolean Implements Interfaces.IDestroyable.IsDestroyed
            Get
                Return _IsDestroyed
            End Get
        End Property
        Public ReadOnly Property IsRunning As Boolean Implements Interfaces.IStartStopable.IsRunning
            Get
                Return _IsRunning
            End Get
        End Property
        Protected Sub Update()
            For Each Target In Targets
                Try
                    Target.Update()
                Catch ex As Exception
                    Handle(ex)
                End Try
                If Not IsRunning Or IsPaused Then Exit For
            Next
        End Sub
        Protected Sub Handle(ex As Exception)
            If ExceptionHandler IsNot Nothing Then ExceptionHandler.Catch(Me, ex)
            If ex.GetType Is GetType(Threading.ThreadAbortException) Then Threading.Thread.ResetAbort()
        End Sub
        Public Overridable Sub Add(Updatable As Updatables.IUpdatable) Implements IUpdater.Add
            'Security checks
            If Updatable Is Nothing Then Exit Sub
            If IsDestroyed Then Exit Sub
            If Updatable.IsDestroyed Then Exit Sub
            'If already existing
            Dim Exists As Boolean = False
            For Each Target In Targets
                If Target Is Updatable Then
                    Exists = True
                    Exit For
                End If
            Next
            'Adding to list
            If Not Exists Then _Targets.Add(Updatable)
            'Adding itself to Updatable
            Updatable.Updater = Me
        End Sub
        Public Overridable Sub Remove(Updatable As Updatables.IUpdatable) Implements IUpdater.Remove
            'Security checks
            If Updatable Is Nothing Then Exit Sub
            If IsDestroyed Then Exit Sub
            'Removing from list
            _Targets.Remove(Updatable)
            'Removing itself from Updatable
            Updatable.Updater = Nothing
        End Sub
        Public Overridable Sub Clear() Implements IUpdater.Clear
            If IsDestroyed Then Exit Sub
            Dim Cache As Updatables.IUpdatable() = Targets
            _Targets.Clear()
            For Each Obj In Cache
                Obj.Updater = Nothing
            Next
        End Sub
        Public Sub UpdateCache()
            _Targets.UpdateCache()
        End Sub
        Public Overridable Sub Start() Implements Interfaces.IStartStopable.Start
            If IsDestroyed Then Exit Sub
            _IsRunning = True
            _IsPaused = False
        End Sub
        Public Overridable Sub [Stop]() Implements Interfaces.IStartStopable.Stop
            If IsDestroyed Then Exit Sub
            _IsRunning = False
            _IsPaused = False
        End Sub
        Public Overridable Sub Pause() Implements Interfaces.IPauseResumable.Pause
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            _IsPaused = True
        End Sub
        Public Overridable Sub [Resume]() Implements Interfaces.IPauseResumable.Resume
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            _IsPaused = False
        End Sub
        Public Overridable Sub Destroy() Implements Interfaces.IDestroyable.Destroy
            If IsDestroyed Then Exit Sub
            [Stop]()
            Dim Cache As Updatables.IUpdatable() = Targets
            _Targets.Clear()
            For Each Obj In Cache
                Obj.Destroy()
            Next
            _IsDestroyed = True
            Deregister(Me)
        End Sub
        Sub New()
            Register(Me)
            DelayHandler = DelayHandlers.ConstantSleep.UniversalDelayHandler
        End Sub

        Private Shared _Updaters As New SafeList(Of IUpdater)
        Public Shared ReadOnly Property Updaters As IUpdater()
            Get
                Return _Updaters.Elements
            End Get
        End Property
        Public Shared Sub Register(Updater As IUpdater)
            If Not _Updaters.Contains(Updater) Then _Updaters.Add(Updater)
        End Sub
        Public Shared Sub Deregister(Updater As IUpdater)
            _Updaters.Remove(Updater)
        End Sub
    End Class
End Namespace