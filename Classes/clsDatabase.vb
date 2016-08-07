Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports NLog

Public Class clsDatabase
    Dim conexoes As ConnectionStringSettingsCollection = WebConfigurationManager.ConnectionStrings
    Public strConn As String = conexoes("strConn").ToString
    Dim logger As Logger = LogManager.GetCurrentClassLogger()

    Public Function getConnection() As SqlConnection
        Return New SqlConnection(strConn)
    End Function

    ''' <summary>
    ''' Executa um comando e retorna o código
    ''' </summary>
    ''' <param name="sql">comando SQL</param>
    ''' <remarks>código retornado pelo comando ExecuteNonQuery</remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Function Execute(ByVal sql As String) As Integer
        Dim conexao As New SqlConnection(strConn)
        Using conexao
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            Return comando.ExecuteNonQuery()
        End Using
    End Function

    ''' <summary>
    ''' Executa um comando e retorna o código
    ''' </summary>
    ''' <param name="sql">Comando SQL com parâmetros</param>
    ''' <param name="parametros">Array com os valores dos parâmetros</param>
    ''' <returns>código retornado pelo comando ExecuteNonQuery</returns>
    ''' <remarks></remarks>
    Public Function Execute(ByVal sql As String, ByVal parametros() As SqlParameter) As Integer
        Dim conexao As New SqlConnection(strConn)
        Using conexao
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            comando.Parameters.AddRange(parametros)

            Return comando.ExecuteNonQuery()
        End Using
    End Function

    ''' <summary>
    ''' Executa uma consulta e retorna a datatable com os resultados
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal sql As String) As DataTable
        Dim dt As New DataTable
        Dim conexao As New SqlConnection(strConn)

        Try
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            dt.Load(comando.ExecuteReader)

        Catch ex As Exception
            logger.Error(ex.Message)
        Finally
            conexao.Close()
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' Executa uma consulta e retorna a datatable com os resultados
    ''' </summary>
    ''' <param name="sql">comando SQL</param>
    ''' <param name="parametros">Array de parâmetros</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal sql As String, ByVal parametros As SqlParameter()) As DataTable
        Dim dt As New DataTable
        Dim conexao As New SqlConnection(strConn)

        Try
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            comando.Parameters.AddRange(parametros)
            dt.Load(comando.ExecuteReader)

        Catch ex As Exception
            logger.Error(ex.Message)
        Finally
            conexao.Close()
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' Executa um comando sql e retorna o primeiro campo dos resultados
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(ByVal sql As String) As Object
        Dim conexao As New SqlConnection(strConn)
        Try
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            Return comando.ExecuteScalar
        Catch ex As Exception
            logger.Error(ex.Message)
        Finally
            conexao.Close()
        End Try
        Return Nothing
    End Function

    Public Function ExecuteScalar(ByVal sql As String, ByVal parametros As SqlParameter()) As Object
        Dim conexao As New SqlConnection(strConn)
        Try
            conexao.Open()
            Dim comando As New SqlCommand(sql, conexao)
            comando.Parameters.AddRange(parametros)
            Return comando.ExecuteScalar
        Catch ex As Exception
            logger.Error(ex.Message)
        Finally
            conexao.Close()
        End Try
        Return Nothing
    End Function
End Class