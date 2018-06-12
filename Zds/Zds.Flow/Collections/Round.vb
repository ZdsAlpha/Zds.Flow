Namespace Collections
    Public Class Round(Of T)
        Implements IQueue(Of T), IStack(Of T), IStream(Of T), Interfaces.IInput(Of T), Interfaces.IOutput(Of T)
        Private Lock As Object
        Private Round As T()
        Private Start As Integer = 0
        Private Length As Integer = 0
        Public ReadOnly Property TotalSpace As Integer
        Public ReadOnly Property Elements As T()
            Get
                Return ToArray()
            End Get
        End Property
        Public ReadOnly Property Size As Integer
            Get
                Return Round.Length
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
        'Queue
        Public Function Enqueue(ByRef Element As T) As Boolean Implements IQueue(Of T).Enqueue, Interfaces.IInput(Of T).Input
            SyncLock Lock
                If Length = TotalSpace Then Return False
                Round((Start + Length) Mod TotalSpace) = Element
                Length += 1
                Return True
            End SyncLock
        End Function
        Public Function Dequeue(ByRef Element As T) As Boolean Implements IQueue(Of T).Dequeue, Interfaces.IOutput(Of T).Output
            SyncLock Lock
                If Length = 0 Then Return False
                Element = Round(Start)
                Start = (Start + 1) Mod TotalSpace
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
                Element = Round((Start + Length - 1) Mod TotalSpace)
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
                If (Me.Start + Me.Length) Mod TotalSpace + Length > Size Then
                    Array.Copy(Elements, Start, Round, (Me.Start + Me.Length) Mod TotalSpace, TotalSpace - (Me.Start + Me.Length) Mod TotalSpace)
                    Array.Copy(Elements, Start + TotalSpace - (Me.Start + Me.Length) Mod TotalSpace, Round, 0, Length - TotalSpace + (Me.Start + Me.Length) Mod TotalSpace)
                Else
                    Array.Copy(Elements, Start, Round, (Me.Start + Me.Length) Mod TotalSpace, Length)
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
                If Me.Start + Length > Size Then
                    Array.Copy(Round, Me.Start, Elements, Start, TotalSpace - Me.Start)
                    Array.Copy(Round, 0, Elements, Start + TotalSpace - Me.Start, Length - TotalSpace - Me.Start)
                Else
                    Array.Copy(Round, Me.Start, Elements, Start, Length)
                End If
                Me.Start = (Me.Start + Length) Mod TotalSpace
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
            If Start = 0 Then Exit Sub
            SyncLock Lock
                Dim Temp(Round.Length - 1) As T
                If Start + Length >= Round.Length Then
                    Array.Copy(Round, Start, Temp, 0, Round.Length - Start)
                    Array.Copy(Round, 0, Temp, Round.Length - Start, Length - Round.Length + Start)
                Else
                    Array.Copy(Round, Start, Temp, 0, Length)
                End If
                Array.Copy(Temp, 0, Round, 0, Length)
                Start = 0
            End SyncLock
        End Sub
        Public Sub Clean()
            SyncLock Lock
                For i = 0 To FreeSpace - 1
                    Round((Start + Length + i) Mod TotalSpace) = Nothing
                Next
            End SyncLock
        End Sub
        Public Sub Clear()
            SyncLock Lock
                For i = Start To Length - 1
                    Round(i Mod TotalSpace) = Nothing
                Next
                Start = 0
                Length = 0
            End SyncLock
        End Sub
        Public Function ToArray() As T()
            Dim Round As T() = Me.Round
            Dim Array(Length - 1) As T
            If Start + Length >= TotalSpace Then
                System.Array.Copy(Round, Start, Array, 0, TotalSpace - Start)
                System.Array.Copy(Round, 0, Array, TotalSpace - Start, Length - (TotalSpace - Start))
            Else
                System.Array.Copy(Round, Start, Array, 0, Length)
            End If
            Return Array
        End Function
        Sub New(Space As Integer)
            Lock = New Object
            Round = New T(Space - 1) {}
            TotalSpace = Space
        End Sub
        Public Shared Function Copy(Source As IO.Stream, Destination As Round(Of Byte), Length As Integer) As Integer
            SyncLock Destination.Lock
                If Not Source.CanRead Then Return 0
                If Length > Destination.FreeSpace Then Length = Destination.FreeSpace
                If Length <= 0 Then Return 0
                Dim Read As Integer = 0
                If (Destination.Start + Destination.Length) Mod Destination.TotalSpace + Length > Destination.Size Then
                    Read = Source.Read(Destination.Round, (Destination.Start + Destination.Length) Mod Destination.TotalSpace, Destination.TotalSpace - (Destination.Start + Destination.Length) Mod Destination.TotalSpace)
                    If Read = Destination.TotalSpace - (Destination.Start + Destination.Length) Mod Destination.TotalSpace Then
                        Read += Source.Read(Destination.Round, 0, Length - Destination.TotalSpace + (Destination.Start + Destination.Length) Mod Destination.TotalSpace)
                    End If
                Else
                    Read = Source.Read(Destination.Round, (Destination.Start + Destination.Length) Mod Destination.TotalSpace, Length)
                End If
                Destination.Length += Read
                Return Read
            End SyncLock
        End Function
        Public Shared Function Copy(Source As Round(Of Byte), Destination As IO.Stream, Length As Integer) As Integer
            SyncLock Source.Lock
                If Not Destination.CanWrite Then Return 0
                If Length > Source.Count Then Length = Source.Count
                If Length <= 0 Then Return 0
                If Source.Start + Length > Source.Size Then
                    Destination.Write(Source.Round, Source.Start, Source.TotalSpace - Source.Start)
                    Destination.Write(Source.Round, 0, Length - Source.TotalSpace + Source.Start)
                Else
                    Destination.Write(Source.Round, Source.Start, Length)
                End If
                Source.Start = (Source.Start + Length) Mod Source.TotalSpace
                Source.Length -= Length
                Return Length
            End SyncLock
        End Function
    End Class
End Namespace