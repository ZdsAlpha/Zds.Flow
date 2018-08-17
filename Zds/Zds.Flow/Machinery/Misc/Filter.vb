Namespace Machinery.Misc
    Public Class Filter(Of T)
        Implements IConverter(Of T, T)
        Private ReadOnly HashList As New Collections.Int32List()
        Public Property Sink As ISink(Of T) Implements ISource(Of T).Sink
        Public Property HashFunction As HashDelegate
        Public Function Receive(obj As T) As Boolean Implements ISink(Of T).Receive
            SyncLock HashList
                Dim _Sink = Sink
                Dim Hash = Me.Hash(obj)
                If Contains(Hash) Then Return True
                Dim Sent As Boolean = False
                If _Sink IsNot Nothing Then Sent = _Sink.Receive(obj)
                If Sent Then Add(Hash)
                Return Sent
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