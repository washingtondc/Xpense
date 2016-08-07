Imports System.Web.Services

<System.Web.Script.Services.ScriptService()> _
Public Class wsUsuario
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function SalvarUsuario(ByVal objUsuario As Usuario) As Boolean
        If Not UsuarioRepository.Save(objUsuario) Is Nothing Then
            Session("UsuCod") = objUsuario.UsuCod
            Session("UsuNom") = objUsuario.UsuNom
            Session("Avatar") = objUsuario.Avatar
            
            Return True
        Else
            Return False
        End If
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function getLoggedUserData() As Usuario
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Return UsuarioRepository.getUsuario(UsuCod)
    End Function

    <WebMethod()> _
    Public Function getUsuarios() As List(Of Usuario)
        Return UsuarioRepository.getUsuarios()
    End Function

    <WebMethod()> _
    Public Function isUsernameAvailable(Username As String) As Boolean
        Return UsuarioRepository.isUsernameAvailable(Username)
    End Function

    <WebMethod()> _
    Public Function RecuperarSenha(email As String) As Boolean
        Debug.Write("ok")
        Return True
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function resetAccount() As Boolean
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim objDatabase As New clsDatabase

            'EXCLUIR CARTÕES
            Dim Cartoes As List(Of Cartao) = CartaoRepository.GetCartoes(UsuCod)
            For Each Cartao As Cartao In Cartoes
                CartaoRepository.delete(Cartao, UsuCod)
            Next

            'EXCLUIR CATEGORIAS
            Dim Categorias As List(Of Categoria) = CategoriaRepository.getCategoriasUsuario(UsuCod)
            For Each Categoria As Categoria In Categorias
                CategoriaRepository.delete(Categoria, 0)
            Next

            'EXCLUIR CONTAS
            Dim Contas As List(Of Conta) = ContaRepository.getContasUsuario(UsuCod)
            For Each Conta As Conta In Contas
                ContaRepository.delete(Conta.ConCod, UsuCod)
            Next

            Return True
    End Function
End Class