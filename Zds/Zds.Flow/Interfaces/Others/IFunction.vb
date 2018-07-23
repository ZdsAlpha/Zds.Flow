Namespace Updaters
    Public Interface IFunction(Of [in], [out])
        Inherits Updatables.IUpdatable
        ReadOnly Property Input As Interfaces.IInput(Of [in])
        ReadOnly Property Output As Interfaces.IOutput(Of out)
    End Interface
End Namespace