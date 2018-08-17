Namespace Collections
    <DebuggerStepThrough>
    Public Class SafeDynamicRound(Of T)
        Inherits DynamicRound(Of T)
        Protected _Lock As New Object
        Public ReadOnly Property Lock As Object
            Get
                Return _Lock
            End Get
        End Property
        Public Overrides Function SetSize(Size As Integer, Forced As Boolean) As Boolean
            SyncLock _Lock
                Return MyBase.SetSize(Size, Forced)
            End SyncLock
        End Function
        Public Overrides Function SetLength(Length As Integer, AutoResize As Boolean) As Boolean
            SyncLock _Lock
                Return MyBase.SetLength(Length, AutoResize)
            End SyncLock
        End Function
        Public Overrides Function CopyTo(Destination() As T, SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return MyBase.CopyTo(Destination, SourceIndex, DestinationIndex, Length)
            End SyncLock
        End Function
        Public Overrides Function CopyFrom(Source() As T, SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return MyBase.CopyFrom(Source, SourceIndex, DestinationIndex, Length)
            End SyncLock
        End Function
        Public Overrides Function ExtendFirst(Count As Integer) As Integer
            SyncLock _Lock
                Return MyBase.ExtendFirst(Count)
            End SyncLock
        End Function
        Public Overrides Function ExtendLast(Count As Integer) As Integer
            SyncLock _Lock
                Return MyBase.ExtendLast(Count)
            End SyncLock
        End Function
        Public Overrides Function ShrinkFirst(Count As Integer) As Integer
            SyncLock _Lock
                Return MyBase.ShrinkFirst(Count)
            End SyncLock
        End Function
        Public Overrides Function ShrinkLast(Count As Integer) As Integer
            SyncLock _Lock
                Return MyBase.ShrinkLast(Count)
            End SyncLock
        End Function
        Public Overrides Function AddFirst(Element As T, Overwrite As Boolean) As Boolean
            SyncLock _Lock
                Return MyBase.AddFirst(Element, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function AddLast(Element As T, Overwrite As Boolean) As Boolean
            SyncLock _Lock
                Return MyBase.AddLast(Element, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function AddFirst(Source() As T, Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            SyncLock _Lock
                Return MyBase.AddFirst(Source, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function AddLast(Source() As T, Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            SyncLock _Lock
                Return MyBase.AddLast(Source, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function RemoveFirst(ByRef Element As T) As Boolean
            SyncLock _Lock
                Return MyBase.RemoveFirst(Element)
            End SyncLock
        End Function
        Public Overrides Function RemoveLast(ByRef Element As T) As Boolean
            SyncLock _Lock
                Return MyBase.RemoveLast(Element)
            End SyncLock
        End Function
        Public Overrides Function RemoveFirst(Destination() As T, Start As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return MyBase.RemoveFirst(Destination, Start, Length)
            End SyncLock
        End Function
        Public Overrides Function RemoveLast(Destination() As T, Start As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return MyBase.RemoveLast(Destination, Start, Length)
            End SyncLock
        End Function
        Public Overrides Function ElementAt(Index As Integer, ByRef Element As T) As Boolean
            SyncLock _Lock
                Return MyBase.ElementAt(Index, Element)
            End SyncLock
        End Function

        Public Overrides Sub Clear()
            SyncLock _Lock
                MyBase.Clear()
            End SyncLock
        End Sub
        Public Overrides Function ToArray() As T()
            SyncLock _Lock
                Return MyBase.ToArray()
            End SyncLock
        End Function
    End Class
End Namespace