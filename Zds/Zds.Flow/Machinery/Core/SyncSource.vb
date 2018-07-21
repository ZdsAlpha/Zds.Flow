Namespace Machinery.Core
    Public Class SyncSource(Of Output)
        Inherits Source(Of Output)
        Private HasValue As Boolean
        Private Value As Output
        Public Overrides Sub Activate()
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
            MyBase.New()
        End Sub
        Sub New(Generate As GenerateDelegate)
            MyBase.New(Generate)
        End Sub
    End Class
End Namespace