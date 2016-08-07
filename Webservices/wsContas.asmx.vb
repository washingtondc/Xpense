Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
Public Class wsContas
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Salvar os dados da conta
    ''' </summary>
    ''' <param name="Conta">Código da conta</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    Public Function SalvarConta(ByVal Conta As Conta) As Conta
        Try
            Conta.UsuCod = CInt(Session("UsuCod"))
            Return ContaRepository.Save(Conta)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Excluir uma conta
    ''' </summary>
    ''' <param name="ConCod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    Public Function excluirConta(ConCod As Integer) As Boolean
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Try
            ContaRepository.delete(ConCod, UsuCod)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Efetua a transferência do valor de uma conta para outra
    ''' </summary>
    ''' <param name="ContaOrigem">Código da conta a debitar</param>
    ''' <param name="ContaDestino">Código da conta a creditar</param>
    ''' <param name="valor">Valor da transação</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function fazerTransferencia(ByVal ContaOrigem As Integer, ByVal ContaDestino As Integer, ByVal Valor As Single) As Boolean
        If ContaOrigem > 0 And ContaDestino > 0 Then
            If ContaOrigem <> ContaDestino Then
                Dim UsuCod As Integer = CInt(Session("UsuCod"))
                Return ContaRepository.fazerTransferencia(ContaOrigem, ContaDestino, Valor, UsuCod)
                Return True
            End If
        End If
        Return False
    End Function

    <WebMethod()> _
    Public Function GravarSaldo(ByVal ConCod As Integer, ano As Integer, mes As Integer, valor As Single) As Boolean
        Try
            ContaRepository.GravaSaldo(ConCod, mes, ano, valor)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getContasUsuario() As List(Of Conta)
        Return ContaRepository.getContasUsuario(CInt(Session("UsuCod")))
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getBancos() As List(Of Banco)
        Return ContaRepository.getBancos()
    End Function
End Class