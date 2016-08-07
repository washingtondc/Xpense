Public Class Lancamento
    Public Property LanAno As Integer
    Public Property LanSeq As Integer
    Public Property LanDes As String
    Public Property LanVal As Single
    Public Property UsuCod As Integer
    Public Property ConCod As Integer
    Public Property LanTip As Char
    Public Property LanDat As DateTime
    Public Property LanDia As Integer
    Public Property CatCod As Integer
    Public Property CarCod As Integer
    Public Property Cartao As New Cartao
    Public Property LanFix As Boolean 'fixo?
    Public Property LanAti As Boolean 'ativo?
    Public Property LanQtdPar As Integer 'Quantidade de parcelas
    Public Property FITID As String = "" 'Identificador do banco
    Public Property LanDet As String
    Public Property Parcela As Parcela
    Public Property Categoria As Categoria
    Public Property LanAutoDebit As Boolean

    Public Sub New()
        LanAno = Year(Today)
        LanQtdPar = 1 'padrão
        Parcela = Nothing
        Categoria = Nothing
        Cartao = Nothing
        LanAti = True
        LanAutoDebit = False
    End Sub
End Class
