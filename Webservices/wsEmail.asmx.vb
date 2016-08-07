Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Net.Mail

<System.Web.Script.Services.ScriptService()> _
Public Class wsEmail
    Inherits System.Web.Services.WebService

    Private smtp As SmtpClient

    <WebMethod(EnableSession:=True)> _
    Public Function sendEmail(destinatario As String, assunto As String, mensagem As String) As Boolean
        ' Logar no Servidor SMTP
        'TODO: Configurar servidor SMTP na administracao
        smtp = New SmtpClient("smtp server")
        smtp.Credentials = New System.Net.NetworkCredential("Usuario", "Senha")
        smtp.Port = 587

        If validaEmail(destinatario) Then
            'cria uma mensagem
            Dim mail As MailMessage = New MailMessage()

            'define os endereços
            'mail.From = new MailAddress(RemEnd, RemNom);

            ' Para
            mail.To.Add(New MailAddress(destinatario))

            'define o conteúdo
            mail.Subject = assunto
            mail.Body = mensagem
            mail.IsBodyHtml = True

            'envia a mensagem
            smtp.Send(mail)
        Else
            Return False
        End If
    End Function

    Private Function validaEmail(email As String) As Boolean
        Try
            Dim mail = New System.Net.Mail.MailAddress(email)
            Return True
        Catch
            Return False
        End Try
    End Function

End Class