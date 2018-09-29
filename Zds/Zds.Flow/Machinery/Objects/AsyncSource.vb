Imports Zds.Flow.Updatables

Namespace Machinery.Objects
    Public Class AsyncSource(Of Output)
        Inherits AsyncObject
        Implements ISource(Of Output)
        Private _Source As Core.AsyncSource(Of Output)
        Public Custom Event OnDropped As Core.Source(Of Output).OnDroppedEventDelegate
            AddHandler(value As Core.Source(Of Output).OnDroppedEventDelegate)
                AddHandler _Source.OnDroppedEvent, value
            End AddHandler
            RemoveHandler(value As Core.Source(Of Output).OnDroppedEventDelegate)
                RemoveHandler _Source.OnDroppedEvent, value
            End RemoveHandler
            RaiseEvent(obj As Output)
                Throw New NotImplementedException()
            End RaiseEvent
        End Event
        Public Property GenerateDelegate As Core.Source(Of Output).GenerateDelegate
        Public Property Dropping As Boolean
            Get
                Return _Source.Dropping
            End Get
            Set(value As Boolean)
                _Source.Dropping = value
            End Set
        End Property
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
            Get
                Return _Source.Sink
            End Get
            Set(value As ISink(Of Output))
                _Source.Sink = value
            End Set
        End Property
        Protected Overrides Sub AsyncUpdate()
            MyBase.AsyncUpdate()
            _Source.Activate()
        End Sub
        Private Function InternalGenerate(ByRef Output As Output) As Boolean
            If IsDestroyed Then Return False
            Try
                Return Generate(Output)
            Catch ex As Exception
                Handle(ex)
                Return False
            End Try
        End Function
        Protected Overridable Function Generate(ByRef Output As Output) As Boolean
            If GenerateDelegate IsNot Nothing Then Return GenerateDelegate(Output) Else Return False
        End Function
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            _Source.Destroy()
        End Sub
        Sub New()
            _Source = New Core.AsyncSource(Of Output)
            _Source.Generate = AddressOf InternalGenerate
        End Sub
        Sub New(Generate As Core.Source(Of Output).GenerateDelegate)
            Me.New()
            GenerateDelegate = Generate
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            _Source = New Core.AsyncSource(Of Output)
            _Source.Generate = AddressOf InternalGenerate
        End Sub
        Sub New(Updater As Updaters.IUpdater, Generate As Core.Source(Of Output).GenerateDelegate)
            Me.New(Updater)
            GenerateDelegate = Generate
        End Sub
    End Class
End Namespace