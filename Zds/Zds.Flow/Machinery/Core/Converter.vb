Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public MustInherit Class Converter(Of Input, Output)
        Implements IConverter(Of Input, Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Convert As ConvertDelegate
        Public Property Buffer As IQueue(Of Input)
        Public Property Dropping As Boolean = True Implements ISource(Of Output).Dropping
        Public Property Recursive As Boolean = True Implements ISink(Of Input).Recursive
        Public Property MustConvert As Boolean = False Implements IConverter(Of Input, Output).MustConvert
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            If Buffer Is Nothing Then Return Nothing
            Return Buffer.Enqueue(obj)
        End Function
        Public MustOverride Sub Activate() Implements IConverter(Of Input, Output).Activate
        Sub New()
            Buffer = New Round(Of Input)()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            Me.Buffer = Buffer
        End Sub
        Sub New(Process As ConvertDelegate)
            Me.New()
            Me.Convert = Process
        End Sub
        Sub New(Buffer As IQueue(Of Input), Process As ConvertDelegate)
            Me.New(Buffer)
            Me.Convert = Process
        End Sub

        Public Delegate Function ConvertDelegate(Input As Input, ByRef Output As Output) As Boolean
    End Class
End Namespace