Imports Zds.Flow.Updatables

Namespace Machinery.Timers
    Public Class SyncSink(Of Input)
        Inherits SyncTimer
        Implements ISink(Of Input)
        Private _Sink As Core.SyncSink(Of Input)
        Public Property SinkDelegate As Core.Sink(Of Input).SinkDelegate
        Public Property Recursive As Boolean
            Get
                Return _Sink.Recursive
            End Get
            Set(value As Boolean)
                _Sink.Recursive = value
            End Set
        End Property
        Public Property Queue As Collections.IQueue(Of Input)
            Get
                Return _Sink.Queue
            End Get
            Set(value As Collections.IQueue(Of Input))
                _Sink.Queue = value
            End Set
        End Property
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            If IsDestroyed Then Return False
            Return _Sink.Receive(obj)
        End Function
        Protected Overrides Sub Tick(ByRef Time As TimeSpan)
            MyBase.Tick(Time)
            _Sink.Activate()
        End Sub
        Private Function InternalSink(Input As Input) As Boolean
            If IsDestroyed Then Return False
            Try
                Return Sink(Input)
            Catch ex As Exception
                Handle(ex)
                Return False
            End Try
        End Function
        Protected Overridable Function Sink(Input As Input) As Boolean
            If SinkDelegate IsNot Nothing Then Return SinkDelegate(Input) Else Return False
        End Function
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            _Sink.Destroy()
        End Sub
        Sub New()
            _Sink = New Core.SyncSink(Of Input)
            _Sink.Sink = AddressOf InternalSink
        End Sub
        Sub New(Sink As Core.Sink(Of Input).SinkDelegate)
            Me.New()
            SinkDelegate = Sink
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            _Sink = New Core.SyncSink(Of Input)
            _Sink.Sink = AddressOf InternalSink
        End Sub
        Sub New(Updater As Updaters.IUpdater, Sink As Core.Sink(Of Input).SinkDelegate)
            Me.New(Updater)
            SinkDelegate = Sink
        End Sub
    End Class
End Namespace