Namespace Updatables
    Public Interface IRigidAnimation
        Inherits IRigidStateMachine(Of Decimal), Interfaces.IResetable
        Property Delta As Decimal
        Property TotalFrames As ULong
        Property CurrentFrame As ULong
        Property IsLoop As Boolean
        Property IsReversed As Boolean
        Property FramesSkipping As Boolean
        Property FramesSkipped As ULong
        Sub Animate(ByRef State As Decimal)
    End Interface
End Namespace