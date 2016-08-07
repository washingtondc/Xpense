Imports System.Web.Services
Imports System.Web.Script.Services


Public Class wsUpload
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Função faz o upload do arquivo recebido via POST
    ''' Deve receber um parâmetro "PATH" contendo o caminho 
    ''' relativo da pasta onde o arquivo deve ser gravado
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function uploadFile() As String
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim serverFilePath As String = ""
        Dim Keys() As String = HttpContext.Current.Request.Files.AllKeys
        Dim DestinationFolder = HttpContext.Current.Request.Form("path")
        Dim FolderPath = Server.MapPath(DestinationFolder)

        If Right(FolderPath, 1) <> "\" Then
            FolderPath &= "\"
        End If

        If HttpContext.Current.Request.Files.AllKeys.Length > 0 Then
            Try
                Dim PostedFile As HttpPostedFile
                PostedFile = HttpContext.Current.Request.Files(Keys(0))
                Dim FileName As String = UsuCod.ToString & System.IO.Path.GetFileName(PostedFile.FileName)

                serverFilePath = FolderPath & FileName
                PostedFile.SaveAs(serverFilePath)
            Catch ex As Exception
                Return ""
            End Try
        End If

        Return serverFilePath
    End Function

End Class