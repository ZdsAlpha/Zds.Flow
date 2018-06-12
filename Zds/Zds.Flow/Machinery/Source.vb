Namespace Machinery
    Public Class Source(Of Input)
        Implements ISource(Of Input)
        Public Property Sink As ISink(Of Input) Implements ISource(Of Input).Sink
        Public Property Generate As GenerateDelegate
        Public Sub Activate()
            Dim obj As Input
            If Generate(obj) Then
                Sink.Receive(obj)
            End If
        End Sub
        Sub New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            Me.Generate = Generate
        End Sub
        Public Delegate Function GenerateDelegate(ByRef obj As Input) As Boolean
    End Class
End Namespace