Namespace Collections
    Public Class Round(Of T)
        Implements IQueue(Of T), IStack(Of T), IStream(Of T), Interfaces.IInput(Of T), Interfaces.IOutput(Of T)
        Private _Lock As New Object
        Private _Buffer As T()
        Private _Position As Integer
        Private _Length As Integer
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


    Public Class _Collection(Of T)
        Implements IQueue(Of T), IStack(Of T), IStream(Of T), Interfaces.IInput(Of T), Interfaces.IOutput(Of T)
        Private Lock As Object
        Private Buffer As T()
        Private Position As Integer
        Private Length As Integer
        Public ReadOnly Property TotalSpace As Integer
        Public ReadOnly Property Elements As T()
            Get
                Return ToArray()
            End Get
        End Property
        Public ReadOnly Property Size As Integer
            Get
                Return Buffer.Length
            End Get
        End Property
        Public ReadOnly Property FreeSpace As Integer
            Get
                Return TotalSpace - Length
            End Get
        End Property
        Public ReadOnly Property Count As Integer
            Get
                Return Length
            End Get
        End Property
        Public Property Element(Index As Integer) As T
            Get
                If Index >= Length Then Throw New IndexOutOfRangeException()
                Return Buffer((Position + Index) Mod TotalSpace)
            End Get
            Set(value As T)
                If Index >= Length Then Throw New IndexOutOfRangeException()
                Buffer((Position + Index) Mod TotalSpace) = value
            End Set
        End Property
        'Queue
        Public Function Enqueue(ByRef Element As T) As Boolean Implements IQueue(Of T).Enqueue, Interfaces.IInput(Of T).Input
            SyncLock Lock
                If Length = TotalSpace Then Return False
                Buffer((Position + Length) Mod TotalSpace) = Element
                Length += 1
                Return True
            End SyncLock
        End Function
        Public Function Dequeue(ByRef Element As T) As Boolean Implements IQueue(Of T).Dequeue, Interfaces.IOutput(Of T).Output
            SyncLock Lock
                If Length = 0 Then Return False
                Element = Buffer(Position)
                Buffer(Position) = Nothing
                Position = (Position + 1) Mod TotalSpace
                Length = Length - 1
                Return True
            End SyncLock
        End Function
        'Stack
        Public Function Push(ByRef Element As T) As Boolean Implements IStack(Of T).Push
            Return Enqueue(Element)
        End Function
        Public Function Pop(ByRef Element As T) As Boolean Implements IStack(Of T).Pop
            SyncLock Lock
                If Length = 0 Then Return False
                Dim Index As Integer = (Position + Length - 1) Mod TotalSpace
                Element = Buffer(Index)
                Buffer(Index) = Nothing
                Length = Length - 1
                Return True
            End SyncLock
        End Function
        'Stream
        Public Function Write(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Write
            SyncLock Lock
                If Start >= Elements.Length Or Start < 0 Then Return 0
                If Start + Length > Elements.Length Then Length = Elements.Length - Start
                If Length > FreeSpace Then Length = FreeSpace
                If Length <= 0 Then Return 0
                If (Me.Position + Me.Length) Mod TotalSpace + Length > Size Then
                    Array.Copy(Elements, Start, Buffer, (Me.Position + Me.Length) Mod TotalSpace, TotalSpace - (Me.Position + Me.Length) Mod TotalSpace)
                    Array.Copy(Elements, Start + TotalSpace - (Me.Position + Me.Length) Mod TotalSpace, Buffer, 0, Length - TotalSpace + (Me.Position + Me.Length) Mod TotalSpace)
                Else
                    Array.Copy(Elements, Start, Buffer, (Me.Position + Me.Length) Mod TotalSpace, Length)
                End If
                Me.Length += Length
                Return Length
            End SyncLock
        End Function
        Public Function Read(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Read
            SyncLock Lock
                If Start >= Elements.Length Or Start < 0 Then Return 0
                If Start + Length > Elements.Length Then Length = Elements.Length - Start
                If Length > Count Then Length = Count
                If Length <= 0 Then Return 0
                If Me.Position + Length > Size Then
                    Array.Copy(Buffer, Me.Position, Elements, Start, TotalSpace - Me.Position)
                    Array.Clear(Buffer, Me.Position, TotalSpace - Me.Position)
                    Array.Copy(Buffer, 0, Elements, Start + TotalSpace - Me.Position, Length - TotalSpace - Me.Position)
                    Array.Clear(Buffer, 0, Length - TotalSpace - Me.Position)
                Else
                    Array.Copy(Buffer, Me.Position, Elements, Start, Length)
                    Array.Clear(Buffer, Me.Position, Length)
                End If
                Me.Position = (Me.Position + Length) Mod TotalSpace
                Me.Length -= Length
                Return Length
            End SyncLock
        End Function
        'Quick Stream
        Public Function Write(Elements As T()) As Integer
            Return Write(Elements, 0, Elements.Length)
        End Function
        Public Function Read(Elements As T()) As Integer
            Return Read(Elements, 0, Elements.Length)
        End Function

        Public Sub Defrag()
            If Position = 0 Then Exit Sub
            SyncLock Lock
                Dim Temp(Buffer.Length - 1) As T
                If Position + Length >= Buffer.Length Then
                    Array.Copy(Buffer, Position, Temp, 0, Buffer.Length - Position)
                    Array.Copy(Buffer, 0, Temp, Buffer.Length - Position, Length - Buffer.Length + Position)
                Else
                    Array.Copy(Buffer, Position, Temp, 0, Length)
                End If
                Array.Copy(Temp, 0, Buffer, 0, Length)
                Position = 0
            End SyncLock
        End Sub
        Public Sub Clean()
            SyncLock Lock
                For i = 0 To FreeSpace - 1
                    Buffer((Position + Length + i) Mod TotalSpace) = Nothing
                Next
            End SyncLock
        End Sub
        Public Sub Clear()
            SyncLock Lock
                Array.Clear(Buffer, 0, Buffer.Length)
                Position = 0
                Length = 0
            End SyncLock
        End Sub
        Public Function ToArray() As T()
            Dim Round As T() = Me.Buffer
            Dim Array(Length - 1) As T
            If Position + Length >= TotalSpace Then
                System.Array.Copy(Round, Position, Array, 0, TotalSpace - Position)
                System.Array.Copy(Round, 0, Array, TotalSpace - Position, Length - (TotalSpace - Position))
            Else
                System.Array.Copy(Round, Position, Array, 0, Length)
            End If
            Return Array
        End Function
        Sub New(Space As Integer)
            Lock = New Object
            Buffer = New T(Space - 1) {}
            TotalSpace = Space
        End Sub
        Public Shared Function Copy(Source As IO.Stream, Destination As _Collection(Of Byte), Length As Integer) As Integer
            SyncLock Destination.Lock
                If Not Source.CanRead Then Return 0
                If Length > Destination.FreeSpace Then Length = Destination.FreeSpace
                If Length <= 0 Then Return 0
                Dim Read As Integer = 0
                If (Destination.Position + Destination.Length) Mod Destination.TotalSpace + Length > Destination.Size Then
                    Read = Source.Read(Destination.Buffer, (Destination.Position + Destination.Length) Mod Destination.TotalSpace, Destination.TotalSpace - (Destination.Position + Destination.Length) Mod Destination.TotalSpace)
                    If Read = Destination.TotalSpace - (Destination.Position + Destination.Length) Mod Destination.TotalSpace Then
                        Read += Source.Read(Destination.Buffer, 0, Length - Destination.TotalSpace + (Destination.Position + Destination.Length) Mod Destination.TotalSpace)
                    End If
                Else
                    Read = Source.Read(Destination.Buffer, (Destination.Position + Destination.Length) Mod Destination.TotalSpace, Length)
                End If
                Destination.Length += Read
                Return Read
            End SyncLock
        End Function
        Public Shared Function Copy(Source As _Collection(Of Byte), Destination As IO.Stream, Length As Integer) As Integer
            SyncLock Source.Lock
                If Not Destination.CanWrite Then Return 0
                If Length > Source.Count Then Length = Source.Count
                If Length <= 0 Then Return 0
                If Source.Position + Length > Source.Size Then
                    Destination.Write(Source.Buffer, Source.Position, Source.TotalSpace - Source.Position)
                    Destination.Write(Source.Buffer, 0, Length - Source.TotalSpace + Source.Position)
                Else
                    Destination.Write(Source.Buffer, Source.Position, Length)
                End If
                Source.Position = (Source.Position + Length) Mod Source.TotalSpace
                Source.Length -= Length
                Return Length
            End SyncLock
        End Function
    End Class
End Namespace