Imports Zds.Flow.Collections

Namespace Machinery.Internals
    Public Class SyncConverter(Of Input, Output)
        Implements IConverter(Of Input, Output)
        Public Property Sink As ISink(Of Output) Implements ISource(Of Output).Sink
        Public Property Convert As ConvertDelegate
        Public Property Buffer As IQueue(Of Input)
        Public Property Dropping As Boolean Implements ISource(Of Output).Dropping
        Public Property Recursive As Boolean Implements ISink(Of Input).Recursive
        Private HasValue As Boolean
        Private Value As Input
        Private HasConverted As Boolean
        Private Converted As Output
        Public Function Receive(obj As Input) As Boolean Implements ISink(Of Input).Receive
            If Buffer Is Nothing Then Return Nothing
            Return Buffer.Enqueue(obj)
        End Function
        Public Sub Activate()
            Dim _Sink = Sink
            Dim _Buffer = Buffer
            If _Buffer IsNot Nothing AndAlso Not HasValue Then HasValue = _Buffer.Dequeue(Value)
            Do
                If HasConverted Then
                    If Dropping Then
                        If _Sink IsNot Nothing Then _Sink.Receive(Converted)
                        HasConverted = False
                        Converted = Nothing
                    Else
                        If _Sink IsNot Nothing AndAlso _Sink.Receive(Converted) Then
                            HasConverted = False
                            Converted = Nothing
                        End If
                    End If
                End If
                If HasValue AndAlso Convert(Value, Converted) Then
                    HasConverted = True
                    HasValue = False
                    Value = Nothing
                Else
                    Exit Do
                End If
                If _Buffer IsNot Nothing Then HasValue = _Buffer.Dequeue(Value)
            Loop While Recursive
        End Sub
        Sub New()
            Buffer = New Collection(Of Input)(4096)
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