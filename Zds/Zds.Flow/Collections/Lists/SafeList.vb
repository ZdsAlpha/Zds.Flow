Namespace Collections
    <DebuggerStepThrough>
    Public Class SafeList(Of T)
        Private List As New List(Of T)
        Public ReadOnly Property Elements As T() = New T(-1) {}
        Public ReadOnly Property Count As Integer
            Get
                Return Elements.Length
            End Get
        End Property
        Public Sub Add(Item As T)
            SyncLock List
                List.Add(Item)
                UpdateCache()
            End SyncLock
        End Sub
        Public Sub Remove(Item As T)
            SyncLock List
                If List.Remove(Item) Then UpdateCache()
            End SyncLock
        End Sub
        Public Sub Add(Items As T())
            SyncLock List
                List.AddRange(Items)
                UpdateCache()
            End SyncLock
        End Sub
        Public Sub Remove(Items As T())
            SyncLock List
                For Each Element In Items
                    List.Remove(Element)
                Next
                UpdateCache()
            End SyncLock
        End Sub
        Public Sub Clear()
            SyncLock List
                List.Clear()
                UpdateCache()
            End SyncLock
        End Sub
        Public Function Contains(Obj As T) As Boolean
            Return Elements.Contains(Obj)
        End Function
        Public Sub UpdateCache()
            SyncLock List
                _Elements = List.ToArray
            End SyncLock
        End Sub
    End Class
End Namespace