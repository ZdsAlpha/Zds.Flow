Namespace Machinery.Misc
    Public Class Collector(Of T)
        Inherits Updatables.SyncTimer
        Implements IConverter(Of T, T())
        Private Output As T()
        Private ReadOnly Queue As New Collections.SafeRound(Of T)
        Public Property Sink As ISink(Of T()) Implements ISource(Of T()).Sink
        Public Property QueueSize As Integer
            Get
                Return Queue.Size
            End Get
            Set(value As Integer)
                Queue.SetSize(value, True)
            End Set
        End Property
        Protected Overrides Sub Tick(ByRef Time As TimeSpan)
            MyBase.Tick(Time)
            Dim _Sink = Sink
            If _Sink Is Nothing Then Return
            If Queue.Length = 0 AndAlso Output Is Nothing Then Return
            If Output Is Nothing Then
                SyncLock Queue.Lock
                    Output = Queue.ToArray()
                    Queue.Clear()
                End SyncLock
            End If
            If Output IsNot Nothing AndAlso _Sink.Receive(Output) Then Output = Nothing
        End Sub
        Public Function Receive(obj As T) As Boolean Implements ISink(Of T).Receive
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