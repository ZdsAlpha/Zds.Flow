Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class SyncConverter(Of Input, Output)
        Inherits Converter(Of Input, Output)
        Private HasValue As Boolean
        Private Value As Input
        Private HasConverted As Boolean
        Private Converted As Output
        Public Overrides Sub Activate()
            Dim _Sink = Sink
            Dim _Buffer = Queue
            If _Buffer IsNot Nothing AndAlso Not HasValue Then HasValue = _Buffer.Dequeue(Value)
            Do
                'Sinking converted value
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
                'Converting input value
                If HasValue And Not HasConverted Then
                    If Convert(Value, Converted) Then
                        HasConverted = True
                        HasValue = False
                        Value = Nothing
                    ElseIf Not MustConvert Then
                        HasValue = False
                        Value = Nothing
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
                If _Buffer IsNot Nothing Then HasValue = _Buffer.Dequeue(Value)
            Loop While Recursive
        End Sub
        Sub New()
            MyBase.New()
        End Sub
        Sub New(Buffer As IQueue(Of Input))
            MyBase.New(Buffer)
        End Sub
        Sub New(Process As ConvertDelegate)
            MyBase.New(Process)
        End Sub
        Sub New(Buffer As IQueue(Of Input), Process As ConvertDelegate)
            MyBase.New(Buffer, Process)
        End Sub
    End Class
End Namespace