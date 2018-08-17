Namespace Collections
    <DebuggerStepThrough>
    Public Class Round(Of T)
        Implements IRound(Of T), IQueue(Of T), IStack(Of T), IStream(Of T), Interfaces.IInput(Of T), Interfaces.IOutput(Of T)
        Protected _Buffer As T()
        Protected _Position As Integer
        Protected _Length As Integer
        Public ReadOnly Property Length As Integer Implements IRound(Of T).Length
            Get
                Return _Length
            End Get
        End Property
        Public ReadOnly Property Size As Integer Implements IRound(Of T).Size
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
        Public Overridable Function SetSize(Size As Integer, Forced As Boolean) As Boolean Implements IRound(Of T).SetSize
            If Size < 0 Then Return False
            If Not Forced AndAlso Size < _Length Then Return False
            Dim Buffer = New T(Size - 1) {}
            CopyTo(Buffer, 0, 0, Size)
            _Buffer = Buffer
            _Position = 0
            If Size < _Length Then _Length = Size
            Return True
        End Function
        Public Overridable Function SetLength(Length As Integer, AutoResize As Boolean) As Boolean Implements IRound(Of T).SetLength
            If Length < 0 Then Return False
            If Length > _Buffer.Length Then
                If AutoResize Then
                    SetSize(Length, False)
                Else
                    Return False
                End If
            End If
            ExtendLast(Length)
            Return True
        End Function
        Public Overridable Function CopyTo(Destination As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer Implements IRound(Of T).CopyTo
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
        Public Overridable Function CopyFrom(Source As T(), SourceIndex As Integer, DestinationIndex As Integer, Length As Integer) As Integer Implements IRound(Of T).CopyFrom
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
        Public Overridable Function ExtendFirst(Count As Integer) As Integer Implements IRound(Of T).ExtendFirst
            If Count <= 0 Then Return 0
            If Count > FreeSpace Then Count = FreeSpace
            _Position = Modulo(_Position - Count, _Buffer.Length)
            _Length += Count
            Return Count
        End Function
        Public Overridable Function ExtendLast(Count As Integer) As Integer Implements IRound(Of T).ExtendLast
            If Count <= 0 Then Return 0
            If Count > FreeSpace Then Count = FreeSpace
            _Length += Count
            Return Count
        End Function
        Public Overridable Function ShrinkFirst(Count As Integer) As Integer Implements IRound(Of T).ShrinkFirst
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
        Public Overridable Function ShrinkLast(Count As Integer) As Integer Implements IRound(Of T).ShrinkLast
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
        Public Overridable Function AddFirst(Element As T, Overwrite As Boolean) As Boolean Implements IRound(Of T).AddFirst
            If _Length = _Buffer.Length Then
                If Overwrite Then
                    If ShrinkLast(1) = 0 Then Return False
                Else
                    Return False
                End If
            End If
            _Position = Modulo(_Position - 1, _Buffer.Length)
            _Buffer(_Position) = Element
            _Length += 1
            Return True
        End Function
        Public Overridable Function AddLast(Element As T, Overwrite As Boolean) As Boolean Implements IRound(Of T).AddLast
            If _Length = _Buffer.Length Then
                If Overwrite Then
                    If ShrinkFirst(1) = 0 Then Return False
                Else
                    Return False
                End If
            End If
            _Buffer((_Position + Length) Mod _Buffer.Length) = Element
            _Length += 1
            Return True
        End Function
        Public Overridable Function AddFirst(Source As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer Implements IRound(Of T).AddFirst
            If Start < 0 Or Length <= 0 Then Return 0
            If Start + Length > Source.Length Then Length = Source.Length - Start
            If Overwrite Then
                ShrinkLast(Length - FreeSpace)
                If Length > _Buffer.Length Then Length = _Buffer.Length
            End If
            Length = ExtendFirst(Length)
            Return CopyFrom(Source, Start, 0, Length)
        End Function
        Public Overridable Function AddLast(Source As T(), Start As Integer, Length As Integer, Overwrite As Boolean) As Integer Implements IRound(Of T).AddLast
            If Start < 0 Or Length <= 0 Then Return 0
            If Start + Length > Source.Length Then Length = Source.Length - Start
            If Overwrite Then
                ShrinkFirst(Length - FreeSpace)
                If Length > _Buffer.Length Then
                    Start = (Start + Length) - _Buffer.Length
                    Length = _Buffer.Length
                End If
            End If
            Length = ExtendLast(Length)
            Return CopyFrom(Source, Start, _Length - Length, Length)
        End Function
        Public Overridable Function RemoveFirst(ByRef Element As T) As Boolean Implements IRound(Of T).RemoveFirst
            If _Length = 0 Then Return False
            Element = _Buffer(_Position)
            _Buffer(_Position) = Nothing
            _Position = (_Position + 1) Mod _Buffer.Length
            _Length -= 1
            Return True
        End Function
        Public Overridable Function RemoveLast(ByRef Element As T) As Boolean Implements IRound(Of T).RemoveLast
            If _Length = 0 Then Return False
            Element = _Buffer((_Position + Length) Mod _Buffer.Length)
            _Buffer((_Position + Length) Mod _Buffer.Length) = Nothing
            _Length -= 1
            Return True
        End Function
        Public Overridable Function RemoveFirst(Destination As T(), Start As Integer, Length As Integer) As Integer Implements IRound(Of T).RemoveFirst
            Dim count = CopyTo(Destination, 0, Start, Length)
            Return ShrinkFirst(count)
        End Function
        Public Overridable Function RemoveLast(Destination As T(), Start As Integer, Length As Integer) As Integer Implements IRound(Of T).RemoveLast
            If Length > Me.Length Then Length = Me.Length
            Dim count = CopyTo(Destination, Me.Length - Length, Start, Length)
            Return ShrinkLast(count)
        End Function
        Public Overridable Function ElementAt(Index As Integer, ByRef Element As T) As Boolean Implements IRound(Of T).ElementAt
            If Index < 0 Or Index >= _Length Then Return False
            Element = _Buffer((_Position + +Index) Mod Size)
            Return True
        End Function

        'Stream
        Public Overridable Function Write(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Write
            Return AddLast(Elements, Start, Length, False)
        End Function
        Public Overridable Function Read(Elements As T(), Start As Integer, Length As Integer) As Integer Implements IStream(Of T).Read
            Return RemoveFirst(Elements, Start, Length)
        End Function

        'Queue
        Public Overridable Function Enqueue(ByRef Element As T) As Boolean Implements IQueue(Of T).Enqueue, Interfaces.IInput(Of T).Enqueue
            Return AddLast(Element, False)
        End Function
        Public Overridable Function Dequeue(ByRef Element As T) As Boolean Implements IQueue(Of T).Dequeue, Interfaces.IOutput(Of T).Output
            Return RemoveFirst(Element)
        End Function

        'Stack
        Public Overridable Function Push(ByRef Element As T) As Boolean Implements IStack(Of T).Push
            Return AddLast(Element, False)
        End Function
        Public Overridable Function Pop(ByRef Element As T) As Boolean Implements IStack(Of T).Pop
            Return RemoveLast(Element)
        End Function

        'Others
        Public Overridable Sub Clear() Implements IRound(Of T).Clear
            ShrinkLast(Integer.MaxValue)
        End Sub
        Public Overridable Function ToArray() As T() Implements IRound(Of T).ToArray
            Dim array(Length - 1) As T
            CopyTo(array, 0, 0, array.Length)
            Return array
        End Function

        Public Shared Property DefaultSize As Integer = 1024
        Sub New()
            Me.New(DefaultSize)
        End Sub
        Sub New(Size As Integer)
            _Buffer = New T(Size - 1) {}
        End Sub

        'Shared Methods
        Private Shared Function Modulo(Dividend As Integer, Divisor As Integer) As Integer
            Return (Dividend Mod Divisor + Divisor) Mod Divisor
        End Function
    End Class
End Namespace