Namespace Machinery.Misc
    Public Class BatchFilter(Of T)
        Implements IConverter(Of T(), T())
        Private ReadOnly HashList As New Collections.Int32List()
        Public Property Sink As ISink(Of T()) Implements ISource(Of T()).Sink
        Public Property HashFunction As HashDelegate
        Public Function Receive(objs As T()) As Boolean Implements ISink(Of T()).Receive
            SyncLock HashList
                Dim _Sink = Sink
                Dim Objects As New List(Of T)
                Dim Hashes As New List(Of Integer)
                For Each Obj In objs
                    Dim Hash = Me.Hash(Obj)
                    If Not Contains(Hash) And Not Hashes.Contains(Hash) Then
                        Objects.Add(Obj)
                        Hashes.Add(Hash)
                    End If
                Next
                If Objects.Count = 0 Then Return True
                If _Sink Is Nothing Then Return False
                Dim ObjectsArray = Objects.ToArray()
                If _Sink.Receive(ObjectsArray) Then
                    For Each Hash As Integer In Hashes
                        Add(Hash)
                    Next
                    Return True
                Else
                    Return False
                End If
            End SyncLock
        End Function
        Public Overridable Function Hash(Obj As T) As Integer
            If HashFunction Is Nothing Then
                Return Obj.GetHashCode
            Else
                Return HashFunction(Obj)
            End If
        End Function
        Public Sub Add(Hash As Integer)
            HashList.Add(Hash)
        End Sub
        Public Sub Remove(Hash As Integer)
            HashList.Remove(Hash)
        End Sub
        Public Function Contains(Hash As Integer) As Boolean
            Return HashList.Contains(Hash)
        End Function
        Public Sub Add(Obj As T)
            Add(Hash(Obj))
        End Sub
        Public Sub Remove(Obj As T)
            Remove(Hash(Obj))
        End Sub
        Public Function Contains(Obj As T) As Boolean
            Return Contains(Hash(Obj))
        End Function
        Public Sub Clear()
            HashList.Clear()
        End Sub
        Sub New()
        End Sub
        Sub New(HashFunction As HashDelegate)
            Me.HashFunction = HashFunction
        End Sub
        Public Delegate Function HashDelegate(Obj As T) As Integer
    End Class
End Namespace