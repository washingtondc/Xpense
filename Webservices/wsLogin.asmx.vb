Imports System.Web.Services

<System.Web.Script.Services.ScriptService()> _
Public Class wsLogin
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function Login(ByVal email As String, ByVal senha As String, ByVal memorizar As Boolean) As Usuario
        Dim objUsuario As Usuario = UsuarioRepository.Login(email, senha)
        If Not objUsuario Is Nothing Then
            UsuarioRepository.setLastLogin(objUsuario.UsuCod)

            Session("UsuCod") = objUsuario.UsuCod
            Session("UsuNom") = objUsuario.UsuNom
            Session("Avatar") = objUsuario.Avatar

            If memorizar Then
                Dim UserCookie As New HttpCookie("login", CStr(objUsuario.UsuEml))
                UserCookie.Expires = DateTime.Now.AddDays(30)
                Context.Response.Cookies.Add(UserCookie)
            End If

            Return objUsuario
        Else
            Return Nothing
        End If
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function Logoff() As Boolean
        Try
            Session.Abandon()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class