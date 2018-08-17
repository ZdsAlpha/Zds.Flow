Namespace Collections
    Public Class DynamicRound(Of T)
        Inherits Round(Of T)
        Private _GrowthFactor As Single
        Private _MinSize As Integer = 1
        Private _AverageSize As Integer = DefaultSize
        Private _MaxSize As Integer = Integer.MaxValue
        Public Property GrowthFactor As Single
            Get
                Return _GrowthFactor
            End Get
            Set(value As Single)
                If value < 1 Then value = 1
                _GrowthFactor = value
            End Set
        End Property
        Public Property MinSize As Integer
            Get
                Return _MinSize
            End Get
            Set(value As Integer)
                If value < 1 Then value = 1
                If value > _MaxSize Then value = _MaxSize
                _MinSize = value
            End Set
        End Property
        Public Property AverageSize As Integer
            Get
                Return _AverageSize
            End Get
            Set(value As Integer)
                If value < _MinSize Then value = _MinSize
                _AverageSize = value
            End Set
        End Property
        Public Property MaxSize As Integer
            Get
                Return _MaxSize
            End Get
            Set(value As Integer)
                If value < 1 Then value = 1
                If value < _MinSize Then value = _MinSize
                _MaxSize = value
            End Set
        End Property

        Public Overrides Function SetSize(Size As Integer, Forced As Boolean) As Boolean
            If Size < MinSize Then Size = MinSize
            Return MyBase.SetSize(Size, Forced)
        End Function
        Public Overrides Function SetLength(Length As Integer, AutoResize As Boolean) As Boolean
            If AutoResize Then EnsureLength(Length)
            Return MyBase.SetLength(Length, AutoResize)
        End Function
        Public Overrides Function ExtendFirst(Count As Integer) As Integer
            EnsureLength(Length + Count)
            Return MyBase.ExtendFirst(Count)
        End Function
        Public Overrides Function ExtendLast(Count As Integer) As Integer
            EnsureLength(Length + Count)
            Return MyBase.ExtendLast(Count)
        End Function
        Public Overrides Function ShrinkFirst(Count As Integer) As Integer
            Dim num = MyBase.ShrinkFirst(Count)
            EnsureLength(Length - num)
            Return num
        End Function
        Public Overrides Function ShrinkLast(Count As Integer) As Integer
            Dim num = MyBase.ShrinkLast(Count)
            EnsureLength(Length - num)
            Return num
        End Function
        Public Overrides Function AddFirst(Element As T, Overwrite As Boolean) As Boolean
            If Not Overwrite Then EnsureLength(Length + 1)
            Return MyBase.AddFirst(Element, Overwrite)
        End Function
        Public Overrides Function AddLast(Element As T, Overwrite As Boolean) As Boolean
            If Not Overwrite Then EnsureLength(Length + 1)
            Return MyBase.AddLast(Element, Overwrite)
        End Function
        Public Overrides Function AddFirst(Source() As T, Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            If Start + Length > Source.Length Then Length = Source.Length - Start
            If Not Overwrite Then EnsureLength(Me.Length + Length)
            Return MyBase.AddFirst(Source, Start, Length, Overwrite)
        End Function
        Public Overrides Function AddLast(Source() As T, Start As Integer, Length As Integer, Overwrite As Boolean) As Integer
            If Start + Length > Source.Length Then Length = Source.Length - Start
            If Not Overwrite Then EnsureLength(Me.Length + Length)
            Return MyBase.AddLast(Source, Start, Length, Overwrite)
        End Function
        Public Overrides Function RemoveFirst(ByRef Element As T) As Boolean
            Dim output = MyBase.RemoveFirst(Element)
            If output Then EnsureLength(Length - 1)
            Return output
        End Function
        Public Overrides Function RemoveLast(ByRef Element As T) As Boolean
            Dim output = MyBase.RemoveLast(Element)
            If output Then EnsureLength(Length - 1)
            Return output
        End Function
        Public Overrides Function RemoveFirst(Destination() As T, Start As Integer, Length As Integer) As Integer
            Dim output = MyBase.RemoveFirst(Destination, Start, Length)
            EnsureLength(Length - output)
            Return output
        End Function
        Public Overrides Function RemoveLast(Destination() As T, Start As Integer, Length As Integer) As Integer
            Dim output = MyBase.RemoveLast(Destination, Start, Length)
            EnsureLength(Length - output)
            Return output
        End Function

        Public Function EnsureLength(Length As Integer) As Integer
            If Length <= 0 Then Length = 1
            Dim _Size As Integer = _Buffer.Length
            Dim Size As Long = Math.Ceiling(MinSize * Math.Pow(GrowthFactor, Math.Ceiling(Math.Log(Length / MinSize) / Math.Log(GrowthFactor))))
            If _Size > AverageSize And Size < AverageSize Then Size = AverageSize
            If Size < MinSize Then Size = MinSize
            If Size > MaxSize Then Size = MaxSize
            If Size <> _Size Then SetSize(Size, True)
            Return Size
        End Function

        Sub New()
            Me.New(DefaultMinSize)
        End Sub
        Sub New(MinSize As Integer)
            MyBase.New(MinSize)
            GrowthFactor = DefaultGrowthFactor
            Me.MinSize = MinSize
            AverageSize = DefaultAverageSize
            MaxSize = DefaultMaxSize
        End Sub

        Private Shared _DefaultGrowthFactor As Single = Math.E / 2
        Private Shared _DefaultMinSize As Integer = 1
        Private Shared _DefaultAverageSize As Integer = DefaultSize
        Private Shared _DefaultMaxSize As Integer = Integer.MaxValue
        Public Shared Property DefaultGrowthFactor As Single
            Get
                Return _DefaultGrowthFactor
            End Get
            Set(value As Single)
                If value < 1 Then value = 1
                _DefaultGrowthFactor = value
            End Set
        End Property
        Public Shared Property DefaultMinSize As Integer
            Get
                Return _DefaultMinSize
            End Get
            Set(value As Integer)
                If value < 1 Then value = 1
                If value > _DefaultMaxSize Then value = _DefaultMaxSize
                _DefaultMinSize = value
            End Set
        End Property
        Public Shared Property DefaultAverageSize As Integer
            Get
                Return _DefaultAverageSize
            End Get
            Set(value As Integer)
                If value < _DefaultMinSize Then value = _DefaultMinSize
                _DefaultAverageSize = value
            End Set
        End Property
        Public Shared Property DefaultMaxSize As Integer
            Get
                Return _DefaultMaxSize
            End Get
            Set(value As Integer)
                If value < 1 Then value = 1
                If value < _DefaultMinSize Then value = _DefaultMinSize
                _DefaultMaxSize = value
            End Set
        End Property
    End Class
End Namespace