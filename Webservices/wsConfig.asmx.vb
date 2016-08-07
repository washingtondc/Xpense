Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

<System.Web.Script.Services.ScriptService()> _
Public Class wsConfig
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function getUserConfig() As Config
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Return ConfigRepository.getUserConfig(UsuCod)
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function saveConfig(Config As Config) As Boolean
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Return ConfigRepository.saveConfig(UsuCod, Config)
    End Function
End Class