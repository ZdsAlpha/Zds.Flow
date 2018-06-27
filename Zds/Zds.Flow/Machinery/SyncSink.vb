Imports Zds.Flow.Updatables

Namespace Machinery
    Public Class SyncSink(Of Input)
        Inherits SyncObject
        Implements ISink(Of Input)
        Private _Sink As Internals.SyncSink(Of Input)
        Public Property SinkDelegate As Internals.SyncSink(Of Input).SinkDelegate
        Public Property Recursive As Boolean Implements ISink(Of Input).Recursive
            Get
                Return _Sink.Recursive
            End Get
            Set(value As Boolean)
                _Sink.Recursive = value
            End Set
        End Property
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Return _Sink.Receive(obj)
        End Function
        Protected Overrides Sub SyncUpdate()
            MyBase.SyncUpdate()
            _Sink.Activate()
        End Sub
        Private Function InternalSink(Input As Input) As Boolean
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
        Sub New()
            _Sink = New Internals.SyncSink(Of Input)
            _Sink.Sink = AddressOf InternalSink
        End Sub
        Sub New(Sink As Internals.SyncSink(Of Input).SinkDelegate)
            Me.New()
            SinkDelegate = Sink
        End Sub
        Sub New(Updater As Updaters.Updater)
            MyBase.New(Updater)
            _Sink = New Internals.SyncSink(Of Input)
            _Sink.Sink = AddressOf InternalSink
        End Sub
        Sub New(Updater As Updaters.Updater, Sink As Internals.SyncSink(Of Input).SinkDelegate)
            MyBase.New(Updater)
            SinkDelegate = Sink
        End Sub
    End Class
End Namespace