Imports Zds.Flow.Updatables

Namespace Machinery.Updatables
    Public Class SyncSource(Of Output)
        Inherits SyncObject
        Implements ISource(Of Output)
        Private _Source As Core.SyncSource(Of Output)
        Public Property GenerateDelegate As Core.SyncSource(Of Output).GenerateDelegate
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
            Get
                Return _Source.Sink
            End Get
            Set(value As ISink(Of Output))
                _Source.Sink = value
            End Set
        End Property
        Public Property Dropping As Boolean Implements ISource(Of Output).Dropping
            Get
                Return _Source.Dropping
            End Get
            Set(value As Boolean)
                _Source.Dropping = value
            End Set
        End Property
        Protected Overrides Sub SyncUpdate()
            MyBase.SyncUpdate()
            _Source.Activate()
        End Sub
        Private Function InternalGenerate(ByRef Output As Output) As Boolean
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
        Sub New()
            _Source = New Core.SyncSource(Of Output)
            _Source.Generate = AddressOf InternalGenerate
        End Sub
        Sub New(Generate As Core.SyncSource(Of Output).GenerateDelegate)
            Me.New()
            GenerateDelegate = Generate
        End Sub
        Sub New(Updater As Updaters.IUpdater)
            MyBase.New(Updater)
            _Source = New Core.SyncSource(Of Output)
            _Source.Generate = AddressOf InternalGenerate
        End Sub
        Sub New(Updater As Updaters.IUpdater, Generate As Core.SyncSource(Of Output).GenerateDelegate)
            Me.New(Updater)
            GenerateDelegate = Generate
        End Sub
    End Class
End Namespace