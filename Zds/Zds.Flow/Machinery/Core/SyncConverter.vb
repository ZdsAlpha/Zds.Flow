Imports Zds.Flow.Collections

Namespace Machinery.Core
    Public Class SyncConverter(Of Input, Output)
        Inherits Converter(Of Input, Output)
        Private HasValue As Boolean
        Private Value As Input
        Private HasConverted As Boolean
        Private Converted As Output
        Public Overrides Sub Activate()
            Do
                If IsDestroyed Then Exit Sub
                Dim _Sink = Sink
                Dim _Queue = Queue
                If _Queue IsNot Nothing AndAlso Not HasValue Then HasValue = _Queue.Dequeue(Value)
                'Converting value
                If HasValue And Not HasConverted Then
                    If Convert(Value, Converted) Then
                        HasConverted = True
                    ElseIf MustConvert And Not IsDestroyed Then
                        Exit Do
                    Else
                        Discard(Value)
                    End If
                    HasValue = False
                    Value = Nothing
                End If
                'Sinking converted value
                If HasConverted Then
                    If _Sink IsNot Nothing AndAlso Not IsDestroyed AndAlso _Sink.Receive(Converted) Then
                        HasConverted = False
                        Converted = Nothing
                    ElseIf Dropping OrElse IsDestroyed Then
                        Discard(Converted)
                        HasConverted = False
                        Converted = Nothing
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
            Loop While Recursive
        End Sub
        Public Overrides Sub Destroy()
            MyBase.Destroy()
            HasValue = False
            Discard(Value)
            Value = Nothing
            HasConverted = False
            Discard(Converted)
            Converted = Nothing
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