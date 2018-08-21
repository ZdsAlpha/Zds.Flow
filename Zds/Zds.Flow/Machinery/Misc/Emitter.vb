Namespace Machinery.Misc
    Public Class Emitter(Of T)
        Inherits Updatables.SyncObject
        Implements IConverter(Of T(), T)
        Private Input As T()
        Private ReadOnly Queue As New Collections.SafeRound(Of T())
        Public Property Sink As ISink(Of T) Implements ISource(Of T).Sink
        Public Property MaxArraySize As Integer = 65536
        Public Property Dropping As Boolean = False
        Public Property QueueSize As Integer
            Get
                Return Queue.Size
            End Get
            Set(value As Integer)
                Queue.SetSize(value, True)
            End Set
        End Property
        Public ReadOnly Property QueueLength As Integer
            Get
                Return Queue.Length
            End Get
        End Property
        Protected Overrides Sub SyncUpdate()
            MyBase.SyncUpdate()
            Dim _Sink = Sink
            If _Sink Is Nothing Then Return
            If Queue.Length = 0 AndAlso Input Is Nothing Then Return
            If Input Is Nothing Then
                Queue.Dequeue(Input)
            ElseIf Input.Length <= MaxArraySize Then
                Dim Array As T() = Nothing
                Queue.Dequeue(Array)
                If Array IsNot Nothing AndAlso Array.Length <> 0 Then
                    If Not (Input.Length + Array.Length > MaxArraySize AndAlso Queue.Enqueue(Array)) Then
                        Dim Modified(Input.Length + Array.Length - 1) As T
                        Input.CopyTo(Modified, 0)
                        Array.CopyTo(Modified, Input.Length)
                        Input = Array
                    End If
                End If
            End If
            If Dropping Then
                For Each Obj In Input
                    _Sink.Receive(Obj)
                Next
                Input = Nothing
            Else
                Dim Map As New Collections.Int32List
                For i = 0 To Input.Length - 1
                    If _Sink.Receive(Input(i)) Then Map.Add(i)
                Next
                If Map.Length = Input.Length Then
                    Input = Nothing
                Else
                    Dim Remaining(Input.Length - Map.Length - 1) As T
                    Dim Index As Integer = 0
                    For i = 0 To Input.Length - 1
                        If Not Map.Contains(i) Then
                            Remaining(Index) = Input(i)
                            Index += 1
                        End If
                    Next
                    Input = Remaining
                End If
            End If
        End Sub
        Public Function Receive(obj() As T) As Boolean Implements ISink(Of T()).Receive
            Return Queue.Enqueue(obj)
        End Function
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
        End Sub
    End Class
End Namespace