Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

<System.Web.Script.Services.ScriptService()> _
Public Class wsCartoes
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function getCartao(ByVal CarCod As Integer) As Cartao
        Return CartaoRepository.GetCartao(CarCod)
    End Function

    ''' <summary>
    ''' Salvar os dados do cartão
    ''' </summary>
    ''' <param name="objCartao">Objeto cartão</param>
    ''' <returns>Sucesso ou Falha</returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    Public Function SalvarCartao(ByVal objCartao As Cartao) As Cartao
        Try
            Dim UsuCod As Integer = CInt(Session("UsuCod"))
            objCartao.UsuCod = UsuCod
            Return CartaoRepository.Save(objCartao)

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Excluir um cartão
    ''' </summary>
    ''' <param name="CarCod">Código do cartão</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function excluirCartao(ByVal CarCod As Integer) As Boolean
        Try
            Dim UsuCod As Integer = CInt(Session("UsuCod"))
            Dim objCartao As Cartao = CartaoRepository.GetCartao(CarCod)
            CartaoRepository.delete(objCartao, UsuCod)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getCartoesUsuario() As List(Of Cartao)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Return CartaoRepository.GetCartoes(UsuCod)
    End Function
End Class