Imports System.Web.SessionState
Imports System.IO
Imports NLog

Public Class Global_asax
    Inherits System.Web.HttpApplication
    Dim threadManutencao As New Threading.Thread(AddressOf teste)
    Dim logger As Logger = LogManager.GetCurrentClassLogger()

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        'threadManutencao.Start()
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Dim ex As Exception = Server.GetLastError()
        logger.Error(ex, ex.Message, Nothing)

        Server.ClearError()
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        threadManutencao.Abort()
    End Sub

    Private Sub teste()
        While (True)
            Dim sw As New StreamWriter("Xpense.log", True)
            sw.WriteLine(Now)
            sw.Close()
            Threading.Thread.Sleep(5000)
        End While

    End Sub

End Class