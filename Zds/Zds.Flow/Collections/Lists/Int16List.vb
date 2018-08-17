Namespace Collections
    Public Class Int16List
        Private Array As UInt64()()
        Public ReadOnly Property Length As UInt32
        Default Public Property Bit(Index As Int16) As Boolean
            Get
                Return Contains(Index)
            End Get
            Set(value As Boolean)
                If value Then Add(Index) Else Remove(Index)
            End Set
        End Property
        Default Public Property Bit(Index As UInt16) As Boolean
            Get
                Return Contains(Index)
            End Get
            Set(value As Boolean)
                If value Then Add(Index) Else Remove(Index)
            End Set
        End Property
        Private Function Add(Number As Byte()) As Boolean
            If Array Is Nothing Then Array = New UInt64(Byte.MaxValue)() {}
            If Array(Number(1)) Is Nothing Then Array(Number(1)) = New UInt64(3) {}
            Dim Remainder As Integer
            Dim Quotient = Math.DivRem(Number(0), 64, Remainder)
            Dim Value As Boolean = (Array(Number(1))(Quotient) >> Remainder) And 1UL
            If Not Value Then Array(Number(1))(Quotient) = Array(Number(1))(Quotient) Or (1UL << Remainder)
            Return Not Value
        End Function
        Private Function Remove(Number As Byte()) As Boolean
            If Array Is Nothing Then Return False
            If Array(Number(1)) Is Nothing Then Return False
            Dim Remainder As Integer
            Dim Quotient = Math.DivRem(Number(0), 64, Remainder)
            Dim Value As Boolean = (Array(Number(1))(Quotient) >> Remainder) And 1UL
            If Value Then Array(Number(1))(Quotient) = Array(Number(1))(Quotient) And Not (1UL << Remainder)
            Return Value
        End Function
        Private Function Contains(Number As Byte()) As Boolean
            If Array Is Nothing Then Return False
            If Array(Number(1)) Is Nothing Then Return False
            Dim Remainder As Integer
            Dim Quotient = Math.DivRem(Number(0), 64, Remainder)
            Return (Array(Number(1))(Quotient) >> Remainder) And 1UL
        End Function
        Public Function Add(Number As Int16)
            If Add(GetBytes(Number)) Then
                _Length += 1
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Add(Number As UInt16)
            If Add(GetBytes(Number)) Then
                _Length += 1
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Remove(Number As Int16)
            If Remove(GetBytes(Number)) Then
                _Length -= 1
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Remove(Number As UInt16)
            If Remove(GetBytes(Number)) Then
                _Length -= 1
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Contains(Number As Int16) As Boolean
            Return Contains(GetBytes(Number))
        End Function
        Public Function Contains(Number As UInt16) As Boolean
            Return Contains(BitConverter.GetBytes(Number))
        End Function
        Public Sub Clear()
            _Length = 0
            Array = Nothing
        End Sub

        Public Shared Function GetBytes(Number As Int16) As Byte()
            Dim Bytes = BitConverter.GetBytes(Number)
            If Not BitConverter.IsLittleEndian Then Bytes = Bytes.Reverse().ToArray()
            Return Bytes
        End Function
        Public Shared Function GetBytes(Number As UInt16) As Byte()
            Dim Bytes = BitConverter.GetBytes(Number)
            If Not BitConverter.IsLittleEndian Then Bytes = Bytes.Reverse().ToArray()
            Return Bytes
        End Function
    End Class
End Namespace