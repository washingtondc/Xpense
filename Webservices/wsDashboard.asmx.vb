Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

<System.Web.Script.Services.ScriptService()> _
Public Class wsResumo
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function getSaldoMes(mes As Integer, ano As Integer) As Single
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim SaldoTotal As Single
        SaldoTotal = ContaRepository.getSaldoTotalMes(UsuCod, ano, mes)

        Return SaldoTotal
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getValorReceber(mes As Integer, ano As Integer) As Single
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim ValorReceber As Single
        ValorReceber = LancamentoRepository.getValorReceber(UsuCod, mes, ano)

        Return ValorReceber
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getValorPagar(mes As Integer, ano As Integer) As Single
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim ValorPagar As Single
        ValorPagar = LancamentoRepository.getValorPagar(UsuCod, mes, ano)

        Return ValorPagar
    End Function
End Class