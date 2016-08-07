Imports System.Web.Services

<System.Web.Script.Services.ScriptService()> _
Public Class wsFatura
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function getLancamentosFatura(CarCod As Integer, AnoFatura As Integer, MesFatura As Integer) As List(Of Lancamento)
        Return CartaoRepository.getLancamentosFatura(CarCod, AnoFatura, MesFatura)
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getValorPendente(CarCod As Integer, AnoFatura As Integer, MesFatura As Integer) As Single
        Return CartaoRepository.getValorFaturasNaoPago(CarCod, MesFatura - 1, AnoFatura)
    End Function

    ''' <summary>
    ''' Efetuar o pagamento de uma fatura
    ''' </summary>
    ''' <param name="CarCod"></param>
    ''' <param name="AnoFatura"></param>
    ''' <param name="MesFatura"></param>
    ''' <param name="ConCod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function pagarFatura(ByVal CarCod As Integer, ByVal AnoFatura As Integer, MesFatura As Integer, ConCod As Integer, ValorPagar As Single) As Boolean
        Try
            Dim UsuCod As Integer = CInt(Session("UsuCod"))
            Dim objCartao As Cartao = CartaoRepository.GetCartao(CarCod)
            CartaoRepository.pagarFatura(objCartao, AnoFatura, MesFatura, ConCod, ValorPagar, UsuCod)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function
End Class