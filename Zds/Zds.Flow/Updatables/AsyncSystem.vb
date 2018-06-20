Imports Zds.Flow.Collections
Imports Zds.Flow.Updaters

Namespace Updatables
    Public MustInherit Class AsyncSystem
        Inherits AsyncObject
        Implements IAsyncSystem
        Implements IUpdater
        Private _IsPaused As Boolean = False
        Private _Targets As New SafeList(Of Updatable)
        Protected Property DelayHandler As DelayHandlers.IDelayHandler Implements IUpdater.DelayHandler
        Public ReadOnly Property IsPaused As Boolean Implements IUpdater.IsPaused
            Get
                Return _IsPaused
            End Get
        End Property
        Protected ReadOnly Property Targets As IUpdatable() Implements IUpdater.Targets
            Get
                Return _Targets.Elements
            End Get
        End Property
        Protected Overrides Sub AsyncUpdate()
            MyBase.AsyncUpdate()
            If Not IsPaused Then
                For Each Target In Targets
                    Try
                        Target.Update()
                    Catch ex As Exception
                        Handle(ex)
                    End Try
                    If Not IsRunning Or IsPaused Then Exit For
                Next
            End If
            If DelayHandler IsNot Nothing Then DelayHandler.Delay()
        End Sub
        Protected Sub Add(Updatable As IUpdatable) Implements IUpdater.Add
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
        Protected Sub Remove(Updatable As IUpdatable) Implements IUpdater.Remove
            'Security checks
            If Updatable Is Nothing Then Exit Sub
            If IsDestroyed Then Exit Sub
            'Removing from list
            _Targets.Remove(Updatable)
            'Removing itself from Updatable
            Updatable.Updater = Nothing
        End Sub
        Protected Sub Clear() Implements IUpdater.Clear
            If IsDestroyed Then Exit Sub
            Dim Cache As Updatables.IUpdatable() = Targets
            _Targets.Clear()
            For Each Obj In Cache
                Obj.Updater = Nothing
            Next
        End Sub
        Protected Sub UpdateCache()
            _Targets.UpdateCache()
        End Sub
        Public Overrides Sub Start()
            MyBase.Start()
            _IsPaused = False
        End Sub
        Public Overrides Sub [Stop]()
            MyBase.Stop()
            _IsPaused = False
        End Sub
        Public Overridable Sub Pause() Implements IUpdater.Pause
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            _IsPaused = True
        End Sub
        Public Overridable Sub [Resume]() Implements IUpdater.Resume
            If IsDestroyed Then Exit Sub
            If Not IsRunning Then Exit Sub
            _IsPaused = False
        End Sub
        Public Overrides Sub Destroy()
            If IsDestroyed Then Exit Sub
            [Stop]()
            Dim Cache As Updatables.IUpdatable() = Targets
            _Targets.Clear()
            For Each Obj In Cache
                Obj.Destroy()
            Next
            MyBase.Destroy()
        End Sub
        Sub New()
        End Sub
        Sub New(Updater As IUpdater)
            MyBase.New(Updater)
        End Sub
    End Class
End Namespace