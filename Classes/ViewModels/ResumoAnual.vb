Public Class ResumoAnual
    Public Property Titulo As String
    Public Property Descricao As String
    Public Property Tipo As String
    Public Property ValorMes As Single()
    Public Property Entradas As Single()
    Public Property Saidas As Single()

    Public Sub New()
        ReDim ValorMes(11)
        ReDim Entradas(11)
        ReDim Saidas(11)
    End Sub
End Class