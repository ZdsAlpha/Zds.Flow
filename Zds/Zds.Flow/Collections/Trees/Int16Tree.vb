Namespace Collections
    Public Class Int16Tree
        Public Left As Int16Tree
        Public Right As Int16Tree
        Public Value As Int16
        Public Sub Insert(Number As Int16)
            Dim Target As Int16Tree = Me
            While Target IsNot Nothing
                If Number < Target.Value Then
                    If Target.Left Is Nothing Then
                        Target.Left = New Int16Tree(Number)
                        Target = Nothing
                    Else
                        Target = Target.Left
                    End If
                ElseIf Number > Target.Value Then
                    If Target.Right Is Nothing Then
                        Target.Right = New Int16Tree(Number)
                        Target = Nothing
                    Else
                        Target = Target.Right
                    End If
                Else
                    Target = Nothing
                End If
            End While
        End Sub
        Public Function Contains(Number As Int16) As Boolean
            If Number = Value Then
                Return True
            Else
                Return (Left IsNot Nothing AndAlso Left.Contains(Number)) OrElse (Right IsNot Nothing AndAlso Right.Contains(Number))
            End If
        End Function
        Public Function Depth() As Integer
            Dim Left As Integer = 0
            Dim Right As Integer = 0
            If Me.Left IsNot Nothing Then Left = Me.Left.Depth()
            If Me.Right IsNot Nothing Then Right = Me.Right.Depth()
            Return Math.Max(Left, Right) + 1
        End Function
        Public Sub New(Number As Int16)
            Me.Value = Number
        End Sub
    End Class
End Namespace