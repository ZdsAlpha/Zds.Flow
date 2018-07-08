Namespace Collections
    Public Class Collection(Of T)
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
        Public Shared Function Copy(Source As IO.Stream, Destination As Collection(Of Byte), Length As Integer) As Integer
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
        Public Shared Function Copy(Source As Collection(Of Byte), Destination As IO.Stream, Length As Integer) As Integer
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