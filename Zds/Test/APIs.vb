Imports Zds.Flow.Updatables
Imports Zds.Flow.Updaters

Public Module APIs
    Public Function GetAllUpdaters() As IUpdater()
        Return Updater.Updaters
    End Function
    Public Function GetUpdatables(Updater As IUpdater)
        Dim Updatables As New List(Of IUpdatable)
        Dim Targets As IUpdatable() = Updater.Targets
        Updatables.AddRange(Targets)
        For Each Obj As IUpdatable In Targets
            Dim _Updater As IUpdater = TryCast(Obj, IUpdater)
            If _Updater IsNot Nothing Then Updatables.AddRange(GetUpdatables(_Updater))
        Next
        Return Updatables.ToArray()
    End Function
    Public Function GetAllUpdatables() As IUpdatable()
        Dim Updatables As New List(Of IUpdatable)
        For Each Updater In GetAllUpdaters()
            Updatables.AddRange(GetUpdatables(Updater))
        Next
        Return Updatables.ToArray()
    End Function
End Module
