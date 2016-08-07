Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Web.Script.Services

<System.Web.Script.Services.ScriptService()> _
Public Class wsLancamentos
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Salvar um lançamento
    ''' </summary>
    ''' <param name="Lancamento">Objeto lançamento preenchido</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    Public Function salvarLancamento(ByVal Lancamento As Lancamento, Efetivado As Boolean) As Lancamento
        Try
            Lancamento.UsuCod = CInt(Session("UsuCod"))
            Return LancamentoRepository.Save(Lancamento, Efetivado)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getLancamento(LanAno As Integer, LanSeq As Integer) As Lancamento
        Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)
        Return objLancamento
    End Function

    <WebMethod()> _
    Public Function excluirParcela(objParcela As Parcela) As Boolean
        Try
            Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(objParcela.LanAno, objParcela.LanSeq, 0, 0)
            LancamentoRepository.excluirParcela(objParcela)
            objLancamento.LanQtdPar -= 1

            If objLancamento.LanQtdPar <= 0 Then
                LancamentoRepository.excluirLancamento(objLancamento)
            End If

            Return True
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    <WebMethod()> _
    Public Function excluirLancamento(objLancamento As Lancamento) As Boolean
        Try
            LancamentoRepository.excluirLancamento(objLancamento)
            Return True
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    <WebMethod()> _
    Public Function efetivarLancamento(ByVal LanAno As Integer, ByVal LanSeq As Integer, ByVal dataPagamento As Date, ByVal valor As Single, ByVal movimentarSaldo As Boolean) As Boolean
        Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Try
            LancamentoRepository.Efetivar(LanAno, LanSeq, CDate(dataPagamento), objLancamento.LanTip, valor, movimentarSaldo, objLancamento.ConCod, UsuCod)
            Return True
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Reabrir um lançamento efetivado
    ''' </summary>
    ''' <param name="LanAno"></param>
    ''' <param name="LanSeq"></param>
    ''' <param name="dataPagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function reabrirLancamento(ByVal LanAno As Integer, ByVal LanSeq As Integer, ByVal dataPagamento As Date) As Boolean
        Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)

        Try
            LancamentoRepository.Reabrir(objLancamento, CDate(dataPagamento))
            Return True
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Retorna os lançamentos do dia
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    <WebMethod(EnableSession:=True)> _
    Public Function getAlertasDia() As Alertas
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Return LancamentoRepository.getLancamentosDia(Today, UsuCod)
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getLancamentosMes(ByVal mes As Integer, ByVal ano As Integer, ByVal CatCod As Integer, ByVal LanTip As String, ByVal status As String) As List(Of Lancamento)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Session("mes_lancamentos") = mes
        Session("ano_lancamentos") = ano

        Return LancamentoRepository.getLancamentosMes(mes, ano, CatCod, LanTip, status, UsuCod)
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getResumoAnual() As List(Of ResumoAnual)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim ano As Integer = Year(Today)

        Return LancamentoRepository.getResumoAnual(UsuCod, ano)
    End Function
End Class