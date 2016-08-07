Imports System.Data.SqlClient
Imports NLog

Public Class CategoriaRepository
    Private Shared log As Logger = LogManager.GetCurrentClassLogger()

    Public Shared Function Save(objCategoria As Categoria) As Categoria
        Dim Database As New clsDatabase
        Dim _sql As String
        If objCategoria.CatCod > 0 Then
            _sql = "UPDATE CAT SET CatDes=@CatDes,CatCor=@CatCor,CatNom=@CatNom,CatOrc=@CatOrc WHERE CatCod=@CatCod"
            Dim parametros(4) As SqlParameter
            parametros(0) = New SqlParameter("@CatDes", objCategoria.CatDes)
            parametros(1) = New SqlParameter("@CatCor", objCategoria.CatCor)
            parametros(2) = New SqlParameter("@CatCod", objCategoria.CatCod)
            parametros(3) = New SqlParameter("@CatNom", objCategoria.CatNom)
            parametros(4) = New SqlParameter("@CatOrc", objCategoria.CatOrc)
            Database.Execute(_sql, parametros)
        Else
            objCategoria.CatCod = getCatCod(objCategoria.UsuCod)
            _sql = "INSERT INTO CAT VALUES(@UsuCod,@CatCod,@CatDes,@CatCor,@CatNom,@CatOrc)"
            Dim parametros(5) As SqlParameter
            parametros(0) = New SqlParameter("@UsuCod", objCategoria.UsuCod)
            parametros(1) = New SqlParameter("@CatCod", objCategoria.CatCod)
            parametros(2) = New SqlParameter("@CatDes", objCategoria.CatDes)
            parametros(3) = New SqlParameter("@CatCor", objCategoria.CatCor)
            parametros(4) = New SqlParameter("@CatNom", objCategoria.CatNom)
            parametros(5) = New SqlParameter("@CatOrc", objCategoria.CatOrc)
            Database.Execute(_sql, parametros)
        End If

        Return objCategoria
    End Function

    ''' <summary>
    ''' Carrega os dados de uma categoria
    ''' </summary>
    ''' <param name="codigoCategoria">Código da categoria</param>
    ''' <remarks></remarks>
    Public Shared Function getCategoria(ByVal codigoCategoria As Integer, UsuCod As Integer) As Categoria
        Dim Database As New clsDatabase
        Dim dtCategoria As New DataTable
        Dim parametros(1) As SqlParameter
        Dim objCategoria As New Categoria

        Dim _sql As String = "SELECT * FROM CAT WHERE CatCod=@CatCod AND UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@CatCod", codigoCategoria)
        parametros(1) = New SqlParameter("@UsuCod", UsuCod)
        dtCategoria = Database.Query(_sql, parametros)

        Try
            Using dtCategoria
                If dtCategoria.Rows.Count > 0 Then
                    objCategoria.UsuCod = CInt(dtCategoria.Rows(0)("UsuCod"))
                    objCategoria.CatCod = CInt(dtCategoria.Rows(0)("CatCod"))
                    objCategoria.CatDes = dtCategoria.Rows(0)("CatDes").ToString
                    objCategoria.CatNom = dtCategoria.Rows(0)("CatNom").ToString
                    If Not IsDBNull(dtCategoria.Rows(0)("CatCor")) Then
                        objCategoria.CatCor = CStr(dtCategoria.Rows(0)("CatCor"))
                    End If
                    objCategoria.CatOrc = CSng(dtCategoria.Rows(0)("CatOrc"))

                    'Carrega valores previstos e confirmados do mês atual
                    objCategoria = getValores(objCategoria, Today.Month, Today.Year)
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            log.Error(ex.Message)
        End Try

        Return objCategoria
    End Function

    ''' <summary>
    ''' Retorna a quantidade total de categorias cadastrados no sistema
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getQtdTotalCategorias() As Integer
        Dim Database As New clsDatabase

        Dim _sql As String = "SELECT count(*) FROM CAT"
        Return CInt(Database.ExecuteScalar(_sql))
    End Function

    ''' <summary>
    ''' Obter próximo código de categoria
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getCatCod(UsuCod As Integer) As Integer
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter
        Try
            Dim _sql As String = "SELECT MAX(CatCod) FROM CAT WHERE UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("@UsuCod", UsuCod)

            Return CInt(Database.ExecuteScalar(_sql, parametros)) + 1
        Catch ex As InvalidCastException
            Return 1
        End Try
    End Function

    ''' <summary>
    ''' Excluir uma categoria
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub delete(objCategoria As Categoria, CatCodDestino As Integer)
        Dim Database As New clsDatabase
        Dim Lancamentos As New List(Of Lancamento)
        Dim parametros(1) As SqlParameter

        Try
            'MOVER LANÇAMENTOS ?
            If CatCodDestino > 0 Then
                MoverLancamentos(objCategoria, CatCodDestino)
            End If

            'EXCLUI OS LANÇAMENTOS
            Lancamentos = CategoriaRepository.getLancamentos(objCategoria)
            For Each obj As Lancamento In Lancamentos
                LancamentoRepository.excluirLancamento(obj)
            Next

            'EXCLUI A CATEGORIA
            Dim _sql As String = "DELETE FROM CAT WHERE CatCod=@CatCod AND UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("@CatCod", objCategoria.CatCod)
            parametros(1) = New SqlParameter("@UsuCod", objCategoria.UsuCod)
            Database.Execute(_sql, parametros)

            objCategoria = Nothing

        Catch ex As Exception
            log.Error(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Calcula os valores dos lançamentos da categoria no mês
    ''' </summary>
    ''' <param name="mes"></param>
    ''' <param name="ano"></param>
    ''' <remarks></remarks>
    Public Shared Function getValores(ByVal objCategoria As Categoria, ByVal mes As Integer, ByVal ano As Integer) As Categoria
        Dim Database As New clsDatabase
        Dim Conexao As SqlConnection = Database.getConnection
        Dim cmd As SqlCommand = Conexao.CreateCommand()
        Dim dataInicio = New Date(ano, mes, 1)
        Dim dataFim = New Date(ano, mes, Date.DaysInMonth(ano, mes))

        Conexao.Open()

        Using (Conexao)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "getValoresCategoria"
            cmd.Parameters.AddWithValue("@UsuCod", objCategoria.UsuCod)
            cmd.Parameters.AddWithValue("@DataInicio", dataInicio)
            cmd.Parameters.AddWithValue("@DataFim", dataFim)
            cmd.Parameters.AddWithValue("@Categoria", objCategoria.CatCod)
            cmd.Parameters.Add("@EntradasPrevisto", SqlDbType.Money)
            cmd.Parameters.Add("@EntradasConfirmado", SqlDbType.Money)
            cmd.Parameters.Add("@SaidasPrevisto", SqlDbType.Money)
            cmd.Parameters.Add("@SaidasConfirmado", SqlDbType.Money)
            cmd.Parameters("@EntradasPrevisto").Direction = ParameterDirection.Output
            cmd.Parameters("@EntradasConfirmado").Direction = ParameterDirection.Output
            cmd.Parameters("@SaidasPrevisto").Direction = ParameterDirection.Output
            cmd.Parameters("@SaidasConfirmado").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            If Not IsDBNull(cmd.Parameters("@EntradasPrevisto").Value) Then
                objCategoria.Valores.EntradasPrevisto = CSng(cmd.Parameters("@EntradasPrevisto").Value)
            End If

            If Not IsDBNull(cmd.Parameters("@EntradasConfirmado").Value) Then
                objCategoria.Valores.EntradasConfirmado = CSng(cmd.Parameters("@EntradasConfirmado").Value)
            End If

            If Not IsDBNull(cmd.Parameters("@SaidasPrevisto").Value) Then
                objCategoria.Valores.SaidasPrevisto = CSng(cmd.Parameters("@SaidasPrevisto").Value)
            End If

            If Not IsDBNull(cmd.Parameters("@SaidasConfirmado").Value) Then
                objCategoria.Valores.SaidasConfirmado = CSng(cmd.Parameters("@SaidasConfirmado").Value)
            End If

            objCategoria.Valores.SaldoPrevisto = objCategoria.Valores.EntradasPrevisto - objCategoria.Valores.SaidasPrevisto
            objCategoria.Valores.SaldoConfirmado = objCategoria.Valores.EntradasConfirmado - objCategoria.Valores.SaidasConfirmado
        End Using

        Return objCategoria
    End Function

    ''' <summary>
    ''' Carrega TODOS os lançamentos de uma categoria
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared Function getLancamentos(ByRef objCategoria As Categoria) As List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim dtLancamentos As New DataTable
        Dim ListaLancamentos As New List(Of Lancamento)
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT * FROM Lancamentos WHERE CatCod = @CatCod"
        parametros(0) = New SqlParameter("@CatCod", objCategoria.CatCod)
        dtLancamentos = Database.Query(_sql, parametros)

        For Each linhaLancamento As DataRow In dtLancamentos.Rows
            Dim LanAno As Integer = CInt(linhaLancamento("LanAno"))
            Dim LanSeq As Integer = CInt(linhaLancamento("LanSeq"))
            Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)
            ListaLancamentos.Add(objLancamento)
            objLancamento = Nothing
        Next

        Return ListaLancamentos
    End Function

    ''' <summary>
    ''' Mover os lançamentos da categoria para outra categoria
    ''' </summary>
    ''' <param name="categoriaDestino"></param>
    ''' <remarks></remarks>
    Public Shared Sub MoverLancamentos(objCategoria As Categoria, ByVal categoriaDestino As Integer)
        Dim Database As New clsDatabase
        Dim parametros(1) As SqlParameter

        Dim _sql As String = "UPDATE Lancamentos SET CatCod=@categoriaDestino WHERE CatCod=@CatCod"
        parametros(0) = New SqlParameter("@categoriaDestino", categoriaDestino)
        parametros(1) = New SqlParameter("@CatCod", objCategoria.CatCod)

        Database.Execute(_sql, parametros)
    End Sub

    ''' <summary>
    ''' Retorna as categorias de um usuário
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getCategoriasUsuario(UsuCod As Integer) As List(Of Categoria)
        Dim Database As New clsDatabase
        Dim dtCategorias As New DataTable
        Dim listaCategorias As New List(Of Categoria)
        Dim parametros(0) As SqlParameter
        Dim _sql As String

        parametros(0) = New SqlParameter("@UsuCod", UsuCod)

        _sql = "SELECT * FROM CAT WHERE UsuCod=@UsuCod ORDER BY CatNom"

        dtCategorias = Database.Query(_sql, parametros)

        For Each linhaBD As DataRow In dtCategorias.Rows
            Dim objCategoria As Categoria = getCategoria(CInt(linhaBD("CatCod")), UsuCod)
            listaCategorias.Add(objCategoria)
        Next

        Return listaCategorias
    End Function
End Class