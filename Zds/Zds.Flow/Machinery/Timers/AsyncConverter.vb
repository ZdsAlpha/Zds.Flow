Imports Zds.Flow.Updatables

Namespace Machinery.Timers
    Public Class AsyncConverter(Of Input, Output)
        Inherits AsyncTimer
        Implements IConverter(Of Input, Output)
        Private _Converter As Core.AsyncConverter(Of Input, Output)
        Public Property ConvertDelegate As Core.Converter(Of Input, Output).ConvertDelegate
        Public Property Dropping As Boolean
            Get
                Return _Converter.Dropping
            End Get
            Set(value As Boolean)
                _Converter.Dropping = value
            End Set
        End Property
        Public Property Recursive As Boolean
            Get
                Return _Converter.Recursive
            End Get
            Set(value As Boolean)
                _Converter.Recursive = value
            End Set
        End Property
        Public Property MustConvert As Boolean
            Get
                Return _Converter.MustConvert
            End Get
            Set(value As Boolean)
                _Converter.MustConvert = value
            End Set
        End Property
        Public Property Queue As Collections.IQueue(Of Input)
            Get
                Return _Converter.Queue
            End Get
            Set(value As Collections.IQueue(Of Input))
                _Converter.Queue = value
            End Set
        End Property
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
            Get
                Return _Converter.Sink
            End Get
            Set(value As ISink(Of Output))
                _Converter.Sink = value
            End Set
        End Property
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            Return _Converter.Receive(obj)
        End Function
        Protected Overrides Sub Tick(ByRef Time As TimeSpan)
            MyBase.Tick(Time)
            _Converter.Activate()
        End Sub
        Private Function InternalConvert(Input As Input, ByRef Output As Output) As Boolean
            Try
                Return Convert(Input, Output)
            Catch ex As Exception
                Handle(ex)
                Return False
            End Try
        End Function
        Protected Function Convert(Input As Input, ByRef Output As Output) As Boolean
            If ConvertDelegate IsNot Nothing Then Return ConvertDelegate(Input, Output) Else Return False
        End Function
        Sub New()
            _Converter = New Core.AsyncConverter(Of Input, Output)
            _Converter.Convert = AddressOf InternalConvert
        End Sub
        Sub New(Convert As Core.Converter(Of Input, Output).ConvertDelegate)
            Me.New()
            ConvertDelegate = Convert
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            _Converter = New Core.AsyncConverter(Of Input, Output)
            _Converter.Convert = AddressOf InternalConvert
        End Sub
        Sub New(Updater As Updaters.IUpdater, Convert As Core.Converter(Of Input, Output).ConvertDelegate)
            Me.New(Updater)
            ConvertDelegate = Convert
        End Sub
    End Class
End Namespace