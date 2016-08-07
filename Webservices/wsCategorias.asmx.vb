Imports System.Web.Services
Imports System.Web.Script.Services

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
Public Class wsCategorias
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function SalvarCategoria(ByVal ObjCategoria As Categoria) As Categoria
        Try
            Dim UsuCod As Integer = CInt(Session("UsuCod"))
            ObjCategoria.UsuCod = UsuCod
            Return CategoriaRepository.Save(ObjCategoria)

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function excluirCategoria(ByVal CatCod As Integer, CatCodDestino As Integer) As Boolean
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Try
            Dim objCategoria As Categoria = CategoriaRepository.getCategoria(CatCod, UsuCod)
            If Not IsNothing(objCategoria) Then
                CategoriaRepository.delete(objCategoria, CatCodDestino)
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    <WebMethod(EnableSession:=True)> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function getCategoriasUsuario() As List(Of Categoria)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Return CategoriaRepository.getCategoriasUsuario(UsuCod)
    End Function
End Class