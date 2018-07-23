Namespace Collections
    Public Class DynamicRound(Of T)
        Inherits Round(Of T)
        Public Property ScalingFactor As Single
        Public Property MinSize As Integer
        Public Property AverageSize As Integer
        Public Property MaxSize As Integer

        Public Overrides Function SetLength(Length As Integer, Optional AutoResize As Boolean = True) As Boolean
            SyncLock _Lock
                EnsureLength(Length, AutoResize)
                Return MyBase.SetLength(Length, False)
            End SyncLock
        End Function
        Public Overrides Function AddFirst(Count As Integer) As Integer
            SyncLock _Lock
                EnsureLength(Length + Count)
                Return MyBase.AddFirst(Count)
            End SyncLock
        End Function
        Public Overrides Function AddLast(Count As Integer) As Integer
            SyncLock _Lock
                EnsureLength(Length + Count)
                Return MyBase.AddLast(Count)
            End SyncLock
        End Function
        Public Overrides Function AddFirst(Element As T) As Boolean
            SyncLock _Lock
                EnsureLength(Length + 1)
                Return MyBase.AddFirst(Element)
            End SyncLock
        End Function
        Public Overrides Function AddLast(Element As T) As Boolean
            SyncLock _Lock
                EnsureLength(Length + 1)
                Return MyBase.AddLast(Element)
            End SyncLock
        End Function
        Public Overrides Function AddFirst(Elements() As T, Start As Integer, Length As Integer, Optional Overwrite As Boolean = False) As Integer
            SyncLock _Lock
                EnsureLength(Me.Length + Length)
                Return MyBase.AddFirst(Elements, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function AddLast(Elements() As T, Start As Integer, Length As Integer, Optional Overwrite As Boolean = False) As Integer
            SyncLock _Lock
                EnsureLength(Me.Length + Length)
                Return MyBase.AddLast(Elements, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overrides Function RemoveFirst(Count As Integer) As Integer
            SyncLock _Lock
                Dim num = MyBase.RemoveFirst(Count)
                EnsureLength(Length - num)
                Return num
            End SyncLock
        End Function
        Public Overrides Function RemoveLast(Count As Integer) As Integer
            SyncLock _Lock
                Dim num = MyBase.RemoveLast(Count)
                EnsureLength(Length - num)
                Return num
            End SyncLock
        End Function
        Public Overrides Function RemoveFirst(ByRef Element As T) As Boolean
            SyncLock _Lock
                Dim output = MyBase.RemoveFirst(Element)
                EnsureLength(Length - 1)
                Return output
            End SyncLock
        End Function
        Public Overrides Function RemoveLast(ByRef Element As T) As Boolean
            SyncLock _Lock
                Dim output = MyBase.RemoveLast(Element)
                EnsureLength(Length - 1)
                Return output
            End SyncLock
        End Function

        Protected Sub EnsureLength(Length As Integer, Optional Forced As Boolean = False)
            If ScalingFactor <= 1 Then Return

        End Sub

        Public Shared Property DefaultScalingFactor As Single = Math.E
        Public Shared Property DefaultMinSize As Integer = 0
        Public Shared Property DefaultAverageSize As Integer = DefaultSize
        Public Shared Property DefaultMaxSize As Integer = Integer.MaxValue
        Sub New()
            ScalingFactor = DefaultScalingFactor
            MinSize = DefaultMinSize
            AverageSize = DefaultAverageSize
            MaxSize = DefaultMaxSize
        End Sub
    End Class
End Namespace