Namespace Machinery.Internals
    Public Class SyncSource(Of Output)
        Implements ISource(Of Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Generate As GenerateDelegate
        Public Property Dropping As Boolean = True Implements ISource(Of Output).Dropping
        Private HasValue As Boolean
        Private Value As Output
        Public Sub Activate() Implements ISource(Of Output).Activate
            Dim _Sink = Sink
            If Not HasValue Then HasValue = Generate(Value)
            If HasValue Then
                If Dropping Then
                    If _Sink IsNot Nothing Then _Sink.Receive(Value)
                    HasValue = False
                    Value = Nothing
                Else
                    If _Sink IsNot Nothing AndAlso _Sink.Receive(Value) Then
                        HasValue = False
                        Value = Nothing
                    End If
                End If
            End If
        End Sub
        Sub New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            Me.Generate = Generate
        End Sub

        Public Delegate Function GenerateDelegate(ByRef obj As Output) As Boolean
    End Class
End Namespace