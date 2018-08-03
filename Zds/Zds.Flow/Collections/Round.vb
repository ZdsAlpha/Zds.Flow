Namespace Collections
    <DebuggerStepThrough>
    Public Class Round(Of T)
        Implements IQueue(Of T), IStack(Of T), IStream(Of T), Interfaces.IInput(Of T), Interfaces.IOutput(Of T)
        Protected _Lock As New Object
        Protected _Buffer As T()
        Protected _Position As Integer
        Protected _Length As Integer
        Public ReadOnly Property Length As Integer
            Get
                Return _Length
            End Get
        End Property
        Default Public Property Element(Index As Integer) As T
            Get
                If Index >= Length Then Throw New IndexOutOfRangeException()
                Return _Buffer((_Position + Index) Mod Size)
            End Get
            Set(value As T)
                If Index >= Length Then Throw New IndexOutOfRangeException()
                _Buffer((_Position + Index) Mod Size) = value
            End Set
        End Property
        Public ReadOnly Property Size As Integer
            Get
                Return _Buffer.Length
            End Get
        End Property
        Public ReadOnly Property FreeSpace As Integer
            Get
                Return Size - Length
            End Get
        End Property
        'Round / Array / Pipe
        Public Overridable Function SetSize(Size As Integer, Optional Forced As Boolean = True) As Boolean
            SyncLock _Lock
                Return _SetSize(Size, Forced)
            End SyncLock
        End Function
        Public Overridable Function SetLength(Length As Integer, Optional AutoResize As Boolean = True) As Boolean
            SyncLock _Lock
                Return _SetLength(Length, AutoResize)
            End SyncLock
        End Function
        Public Overridable Function AddFirst(Count As Integer) As Integer
            SyncLock _Lock
                Return _AddFirst(Count)
            End SyncLock
        End Function
        Public Overridable Function AddLast(Count As Integer) As Integer
            SyncLock _Lock
                Return _AddLast(Count)
            End SyncLock
        End Function
        Public Overridable Function AddFirst(Element As T) As Boolean
            SyncLock _Lock
                Return _AddFirst(Element)
            End SyncLock
        End Function
        Public Overridable Function AddLast(Element As T) As Boolean
            SyncLock _Lock
                Return _AddLast(Element)
            End SyncLock
        End Function
        Public Overridable Function AddFirst(Elements As T(), Start As Integer, Length As Integer, Optional Overwrite As Boolean = False) As Integer
            SyncLock _Lock
                Return _AddFirst(Elements, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overridable Function AddLast(Elements As T(), Start As Integer, Length As Integer, Optional Overwrite As Boolean = False) As Integer
            SyncLock _Lock
                Return _AddLast(Elements, Start, Length, Overwrite)
            End SyncLock
        End Function
        Public Overridable Function RemoveFirst(Count As Integer) As Integer
            SyncLock _Lock
                Return _RemoveFirst(Count)
            End SyncLock
        End Function
        Public Overridable Function RemoveLast(Count As Integer) As Integer
            SyncLock _Lock
                Return _RemoveLast(Count)
            End SyncLock
        End Function
        Public Overridable Function RemoveFirst(ByRef Element As T) As Boolean
            SyncLock _Lock
                Return _RemoveFirst(Element)
            End SyncLock
        End Function
        Public Overridable Function RemoveLast(ByRef Element As T) As Boolean
            SyncLock _Lock
                Return _RemoveLast(Element)
            End SyncLock
        End Function
        Public Overridable Function CopyTo(Destination As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return _CopyTo(Destination, SourceIndex, DestinationIndex, Length)
            End SyncLock
        End Function
        Public Overridable Function CopyFrom(Source As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            SyncLock _Lock
                Return _CopyFrom(Source, SourceIndex, DestinationIndex, Length)
            End SyncLock
        End Function
        Public Function AddFirst(Elements As T(), Optional Overwrite As Boolean = False) As Integer
            Return AddFirst(Elements, 0, Elements.Length, Overwrite)
        End Function
        Public Function AddLast(Elements As T(), Optional Overwrite As Boolean = False) As Integer
            Return AddLast(Elements, Overwrite)
        End Function

        'Stream
        Public Function Write(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Write
            SyncLock _Lock
                Dim count = AddLast(Length)
                Return CopyFrom(Elements, Start, Me.Length - count, count)
            End SyncLock
        End Function
        Public Function Read(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Read
            SyncLock _Lock
                Dim count = CopyTo(Elements, 0, Start, Length)
                Return RemoveFirst(count)
            End SyncLock
        End Function
        Public Function Write(Elements As T()) As Integer
            Return Write(Elements, 0, Elements.Length)
        End Function
        Public Function Read(Elements As T()) As Integer
            Return Read(Elements, 0, Elements.Length)
        End Function

        'Queue
        Public Function Enqueue(ByRef Element As T) As Boolean Implements IQueue(Of T).Enqueue, Interfaces.IInput(Of T).Input
            Return AddLast(Element)
        End Function
        Public Function Dequeue(ByRef Element As T) As Boolean Implements IQueue(Of T).Dequeue, Interfaces.IOutput(Of T).Output
            Return RemoveFirst(Element)
        End Function

        'Stack
        Public Function Push(ByRef Element As T) As Boolean Implements IStack(Of T).Push
            Return Enqueue(Element)
        End Function
        Public Function Pop(ByRef Element As T) As Boolean Implements IStack(Of T).Pop
            Return RemoveLast(Element)
        End Function

        'Others
        Public Sub Clear()
            SetLength(0)
        End Sub
        Public Function ToArray() As T()
            SyncLock _Lock
                Dim array(Length - 1) As T
                CopyTo(array, 0, 0, array.Length)
                Return array
            End SyncLock
        End Function

        Public Shared Property DefaultSize As Integer = 1024
        Sub New()
            Me.New(DefaultSize)
        End Sub
        Sub New(Size As Integer)
            _Buffer = New T(Size - 1) {}
        End Sub

        'Internal Methods
        Protected Function _SetSize(Size As Integer, Forced As Boolean) As Boolean
            If Size < 0 Then Return False
            If Not Forced AndAlso Size < _Length Then Return False
            Dim Buffer = New T(Size - 1) {}
            _CopyTo(Buffer, 0, 0, Size)
            _Buffer = Buffer
            _Position = 0
            If Size < _Length Then _Length = Size
            Return True
        End Function
        Protected Function _SetLength(Length As Integer, AutoResize As Boolean) As Boolean
            If Length < 0 Then Return False
            If Length > _Buffer.Length Then
                If AutoResize Then
                    _SetSize(Length, False)
                Else
                    Return False
                End If
            End If
            _AddLast(Length)
            Return True
        End Function
        Protected Function _AddFirst(Count As Integer) As Integer
            If Count <= 0 Then Return 0
            If Count > FreeSpace Then Count = FreeSpace
            _Position = Modulo(_Position - Count, _Buffer.Length)
            _Length += Count
            Return Count
        End Function
        Protected Function _AddLast(Count As Integer) As Integer
            If Count <= 0 Then Return 0
            If Count > FreeSpace Then Count = FreeSpace
            _Length += Count
            Return Count
        End Function
        Protected Function _RemoveFirst(Count As Integer) As Integer
            If Count <= 0 Then Return 0
            If Count > _Length Then Count = _Length
            If _Position + Count > _Buffer.Length Then
                Array.Clear(_Buffer, _Position, _Buffer.Length - _Position)
                Array.Clear(_Buffer, 0, Count + _Position - _Buffer.Length)
            Else
                Array.Clear(_Buffer, _Position, Count)
            End If
            _Position = Modulo(_Position + Count, _Buffer.Length)
            _Length -= Count
            Return Count
        End Function
        Protected Function _RemoveLast(Count As Integer) As Integer
            If Count > _Length Then Count = _Length
            If _Position + _Length > _Buffer.Length And _Position + _Length - Count < _Buffer.Length Then
                Array.Clear(_Buffer, _Position + _Length - Count, _Buffer.Length - _Position - _Length + Count)
                Array.Clear(_Buffer, 0, _Position + _Length - _Buffer.Length)
            Else
                Array.Clear(_Buffer, Modulo(_Position + _Length - Count, _Buffer.Length), Count)
            End If
            _Length -= Count
            Return Count
        End Function
        Protected Function _AddFirst(Element As T) As Boolean
            If _Length = _Buffer.Length Then Return False
            _Position = Modulo(_Position - 1, _Buffer.Length)
            _Buffer(_Position) = Element
            _Length += 1
            Return True
        End Function
        Protected Function _AddLast(Element As T) As Boolean
            If _Length = _Buffer.Length Then Return False
            _Buffer((_Position + Length) Mod _Buffer.Length) = Element
            _Length += 1
            Return True
        End Function
        Protected Function _RemoveFirst(ByRef Element As T) As Boolean
            If _Length = 0 Then Return False
            Element = _Buffer(_Position)
            _Buffer(_Position) = Nothing
            _Position = (_Position + 1) Mod _Buffer.Length
            _Length -= 1
            Return True
        End Function
        Protected Function _RemoveLast(ByRef Element As T) As Boolean
            If _Length = 0 Then Return False
            Element = _Buffer((_Position + Length) Mod _Buffer.Length)
            _Buffer((_Position + Length) Mod _Buffer.Length) = Nothing
            _Length -= 1
            Return True
        End Function
        Protected Function _CopyTo(Destination As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            If SourceIndex < 0 Or DestinationIndex < 0 Or Length <= 0 Then Return 0
            If SourceIndex + Length > _Length Then Length = _Length - SourceIndex
            If DestinationIndex + Length > Destination.Length Then Length = Destination.Length - DestinationIndex
            If _Position + SourceIndex < _Buffer.Length And _Position + SourceIndex + Length > _Buffer.Length Then
                Array.Copy(_Buffer, _Position + SourceIndex, Destination, DestinationIndex, _Buffer.Length - _Position - SourceIndex)
                Array.Copy(_Buffer, 0, Destination, DestinationIndex + _Buffer.Length - _Position - SourceIndex, Length + _Position + SourceIndex - _Buffer.Length)
            Else
                Array.Copy(_Buffer, Modulo(_Position + SourceIndex, _Buffer.Length), Destination, DestinationIndex, Length)
            End If
            Return Length
        End Function
        Protected Function _CopyFrom(Source As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer
            If SourceIndex < 0 Or DestinationIndex < 0 Or Length <= 0 Then Return 0
            If SourceIndex + Length > Source.Length Then Length = Source.Length - SourceIndex
            If DestinationIndex + Length > _Length Then Length = _Length - DestinationIndex
            If _Position + DestinationIndex < _Buffer.Length And _Position + DestinationIndex + Length > _Buffer.Length Then
                Array.Copy(Source, SourceIndex, _Buffer, _Position + DestinationIndex, _Buffer.Length - _Position - DestinationIndex)
                Array.Copy(Source, SourceIndex + _Buffer.Length - _Position - DestinationIndex, _Buffer, 0, Length + _Position + DestinationIndex - _Buffer.Length)
            Else
                Array.Copy(Source, SourceIndex, _Buffer, Modulo(_Position + DestinationIndex, _Buffer.Length), Length)
            End If
            Return Length
        End Function
        Protected Function _AddFirst(Elements As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            If Start < 0 Or Length <= 0 Then Return 0
            If Start + Length > Elements.Length Then Length = Elements.Length - Start
            If Overwrite Then
                _RemoveLast(Length - FreeSpace)
                If Length > _Buffer.Length Then Length = _Buffer.Length
            End If
            Length = _AddFirst(Length)
            Return _CopyFrom(Elements, Start, 0, Length)
        End Function
        Protected Function _AddLast(Elements As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            If Start < 0 Or Length <= 0 Then Return 0
            If Start + Length > Elements.Length Then Length = Elements.Length - Start
            If Overwrite Then
                _RemoveFirst(Length - FreeSpace)
                If Length > _Buffer.Length Then
                    Start = (Start + Length) - _Buffer.Length
                    Length = _Buffer.Length
                End If
            End If
            Length = _AddLast(Length)
            Return _CopyFrom(Elements, Start, _Length - Length, Length)
        End Function

        'Shared Methods
        Private Shared Function Modulo(Dividend As Integer, Divisor As Integer) As Integer
            Return (Dividend Mod Divisor + Divisor) Mod Divisor
        End Function
    End Class
End Namespace