Namespace Collections
    Public Class BinaryTree(Of T As IComparable)
        Public Left As BinaryTree(Of T)
        Public Right As BinaryTree(Of T)
        Public Value As T
        Public Sub Insert(Obj As T)
            Dim Target As BinaryTree(Of T) = Me
            While Target IsNot Nothing
                If Obj.CompareTo(Target.Value) > 0 Then
                    If Target.Left Is Nothing Then
                        Target.Left = New BinaryTree(Of T)(Obj)
                        Target = Nothing
                    Else
                        Target = Target.Left
                    End If
                ElseIf Obj.CompareTo(Target.Value) < 0 Then
                    If Target.Right Is Nothing Then
                        Target.Right = New BinaryTree(Of T)(Obj)
                        Target = Nothing
                    Else
                        Target = Target.Right
                    End If
                Else
                    Target = Nothing
                End If
            End While
        End Sub
        Public Function Contains(Obj As T) As Boolean
            Dim Target As BinaryTree(Of T) = Me
            If Obj.CompareTo(Target.Value) = 0 Then
                Return True
            Else
                Return (Left IsNot Nothing AndAlso Left.Contains(Obj)) OrElse (Right IsNot Nothing AndAlso Right.Contains(Obj))
            End If
        End Function
        Public Function Depth() As Integer
            Dim Left As Integer = 0
            Dim Right As Integer = 0
            If Me.Left IsNot Nothing Then Left = Me.Left.Depth
            If Me.Right IsNot Nothing Then Right = Me.Right.Depth
            Return Math.Max(Left, Right) + 1
        End Function
        Sub New(Obj As T)
            Me.Value = Obj
        End Sub
    End Class
End Namespace