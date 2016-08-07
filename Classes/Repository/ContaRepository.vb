Imports System.Data.SqlClient
Imports NLog

Public Class ContaRepository
    Private Shared log As Logger = LogManager.GetCurrentClassLogger()

    ''' <summary>
    ''' Salva os dados no banco
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function Save(objConta As Conta) As Conta
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim parametros() As SqlParameter

        If objConta.ConCod = 0 Then
            ReDim parametros(4)
            objConta.ConCod = getSequencialConta()
            _sql = "INSERT INTO CON VALUES(@ConCod,@UsuCod,@ConDes,@ConFix,@BankID)"
            parametros(0) = New SqlParameter("@ConCod", objConta.ConCod)
            parametros(1) = New SqlParameter("@UsuCod", objConta.UsuCod)
            parametros(2) = New SqlParameter("@ConDes", objConta.ConDes)
            parametros(3) = New SqlParameter("@ConFix", objConta.ConFix)
            If Not IsNothing(objConta.Banco) Then
                parametros(4) = New SqlParameter("@BankID", objConta.Banco.ID)
            Else
                parametros(4) = New SqlParameter("@BankID", 0)
            End If

            Database.Execute(_sql, parametros)
        Else
            ReDim parametros(3)
            _sql = "UPDATE CON SET ConDes=@ConDes, BankID=@BankID WHERE ConCod=@ConCod AND UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("@ConCod", objConta.ConCod)
            parametros(1) = New SqlParameter("@UsuCod", objConta.UsuCod)
            parametros(2) = New SqlParameter("@ConDes", objConta.ConDes)
            parametros(3) = New SqlParameter("@BankID", objConta.Banco.ID)
            Database.Execute(_sql, parametros)
        End If

        'SALDO ATUAL
        GravaSaldo(objConta.ConCod, Month(Today), Year(Today), objConta.ConSal)

        Return objConta
    End Function

    ''' <summary>
    ''' Carrega os dados da conta
    ''' </summary>
    ''' <param name="ConCod">Código da conta</param>
    ''' <remarks></remarks>
    Public Shared Function getConta(UsuCod As Integer, ByVal ConCod As Integer) As Conta
        Dim objConta As Conta = Nothing
        Dim Database As New clsDatabase
        Dim dtConta As New DataTable
        Dim parametros(1) As SqlParameter

        Dim _sql As String = "SELECT * FROM CON WHERE UsuCod=@UsuCod AND ConCod=@ConCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        parametros(1) = New SqlParameter("@ConCod", ConCod)

        dtConta = Database.Query(_sql, parametros)

        If dtConta.Rows.Count > 0 Then
            objConta = New Conta
            objConta.UsuCod = CInt(dtConta.Rows(0)("UsuCod"))
            objConta.ConCod = CInt(dtConta.Rows(0)("ConCod"))
            objConta.ConDes = dtConta.Rows(0)("ConDes").ToString
            objConta.ConSal = getSaldoConta(objConta.ConCod, Year(Today), Month(Today), UsuCod)
            objConta.ConFix = CBool(dtConta.Rows(0)("ConFix"))
            objConta.Banco = getBanco(CInt(dtConta.Rows(0)("BankID")))
        End If

        Return objConta
    End Function

    Public Shared Function getByBankID(UsuCod As Integer, BankID As Integer) As Conta
        Dim objConta As Conta = Nothing
        Dim Database As New clsDatabase
        Dim dtConta As New DataTable
        Dim parametros(1) As SqlParameter

        Dim _sql As String = "SELECT * FROM CON WHERE UsuCod=@UsuCod AND BankID=@BankID"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        parametros(1) = New SqlParameter("@BankID", BankID)

        dtConta = Database.Query(_sql, parametros)

        If dtConta.Rows.Count > 0 Then
            Dim ConCod As Integer = CInt(dtConta.Rows(0)("ConCod"))
            objConta = getConta(UsuCod, ConCod)
        End If

        Return objConta
    End Function

    ''' <summary>
    ''' Excluir a conta
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub delete(ConCod As Integer, UsuCod As Integer)
        Dim Database As New clsDatabase
        Dim Lancamentos As List(Of Lancamento)
        Dim parametros(0) As SqlParameter
        Dim _sql As String
        Dim objConta As Conta = ContaRepository.getConta(UsuCod, ConCod)

        'TODO: Aplicar transaction
        If Not objConta.ConFix Then
            'EXCLUIR OS LANÇAMENTOS
            Lancamentos = loadLancamentos(objConta)
            For Each obj As Lancamento In Lancamentos
                LancamentoRepository.excluirLancamento(obj)
            Next

            'EXCLUIR SALDOS
            _sql = "DELETE FROM SAL WHERE ConCod=@ConCod"
            parametros(0) = New SqlParameter("@ConCod", objConta.ConCod)
            Database.Execute(_sql, parametros)

            'EXCLUIR A CONTA
            _sql = "DELETE FROM CON WHERE ConCod=@ConCod"
            parametros(0) = New SqlParameter("@ConCod", objConta.ConCod)
            Database.Execute(_sql, parametros)

            objConta = Nothing
        End If

    End Sub

    ''' <summary>
    ''' Obtém o próximo código de conta
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getSequencialConta() As Integer
        Dim Database As New clsDatabase

        Dim _sql As String = "SELECT MAX(ConCod) FROM CON"

        Try
            Return CInt(Database.ExecuteScalar(_sql)) + 1
        Catch ex As Exception
            Return 1
        End Try
    End Function

    ''' <summary>
    ''' Obtém o saldo da conta no mês especificado
    ''' </summary>
    ''' <param name="ano">Ano do saldo</param>
    ''' <param name="mes">Mês do saldo</param>
    ''' <returns>Saldo</returns>
    ''' <remarks>se não tiver registro de saldo para o mês, retorna o último saldo atualizado</remarks>
    Public Shared Function getSaldoConta(ConCod As Integer, ByVal ano As Integer, ByVal mes As Integer, UsuCod As Integer) As Single
        Dim Database As New clsDatabase
        Dim Saldo As Object
        Dim UltimoDiaMes = Date.DaysInMonth(ano, mes)
        Dim DataFimMes As New Date(ano, mes, UltimoDiaMes)
        Dim parametros(2) As SqlParameter

        Dim _sql As String = "SELECT TOP(1) SaldoMes + AjusteManual FROM SAL WHERE ConCod=@ConCod AND UsuCod=@UsuCod AND SalDatAtu<=@DataFimMes ORDER BY SalAno DESC,SalMes DESC"
        parametros(0) = New SqlParameter("@ConCod", ConCod)
        parametros(1) = New SqlParameter("@DataFimMes", DataFimMes)
        parametros(2) = New SqlParameter("@UsuCod", UsuCod)

        Saldo = Database.ExecuteScalar(_sql, parametros)
        If Not IsDBNull(Saldo) Then
            Return CSng(Saldo)
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Retorna um vetor com o saldo do mes e o valor ajustado manualmente
    ''' </summary>
    ''' <param name="ConCod"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <returns></returns>
    ''' <remarks>Se não existir o registro, retorna nothing</remarks>
    Public Shared Function getSaldoMes(ConCod As Integer, ByVal ano As Integer, ByVal mes As Integer) As Single
        Dim Database As New clsDatabase
        Dim dtSaldo As New DataTable
        Dim parametros(2) As SqlParameter
        Dim _sql As String

        _sql = "SELECT SaldoMes FROM SAL WHERE ConCod=@ConCod AND SalAno=@SalAno AND SalMes=@SalMes"
        parametros(0) = New SqlParameter("@ConCod", ConCod)
        parametros(1) = New SqlParameter("@SalAno", ano)
        parametros(2) = New SqlParameter("@SalMes", mes)

        dtSaldo = Database.Query(_sql, parametros)
        If dtSaldo.Rows.Count > 0 Then
            Return CSng(dtSaldo.Rows(0)(0))
        Else
            CriaRegistroSaldo(ConCod, ano, mes)
            Return 0
        End If
    End Function

    Private Shared Sub CriaRegistroSaldo(ConCod As Integer, ano As Integer, mes As Integer)
        Dim Database As New clsDatabase
        Dim parametros(5) As SqlParameter
        Dim _sql As String = "INSERT INTO SAL VALUES(@mes,@ano,@ConCod,@SaldoMes,@DataAtual,@AjusteManual)"

        parametros(0) = New SqlParameter("@mes", mes)
        parametros(1) = New SqlParameter("@ano", ano)
        parametros(2) = New SqlParameter("@ConCod", ConCod)
        parametros(3) = New SqlParameter("@SaldoMes", 0)
        parametros(4) = New SqlParameter("@DataAtual", Today)
        parametros(5) = New SqlParameter("@AjusteManual", 0)

        Database.Execute(_sql, parametros)
    End Sub

    Public Shared Function getContasUsuario(UsuCod As Integer) As List(Of Conta)
        Dim Contas As New List(Of Conta)
        Dim Database As New clsDatabase
        Dim dtContas As New DataTable
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT * FROM CON WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        dtContas = Database.Query(_sql, parametros)

        For Each linha As DataRow In dtContas.Rows
            Dim objConta As Conta = getConta(UsuCod, CInt(linha("ConCod")))

            Contas.Add(objConta)
        Next

        Return Contas
    End Function

    ''' <summary>
    ''' Retorna a quantidade total de contas cadastrados no sistema
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getQtdTotalContas() As Integer
        Dim Database As New clsDatabase

        Dim _sql As String = "SELECT count(*) FROM CON"
        Return CInt(Database.ExecuteScalar(_sql))
    End Function

    ''' <summary>
    ''' Retorna o saldo de todas as contas do usuário
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function getSaldoTotalMes(ByVal UsuCod As Integer, ByVal ano As Integer, ByVal mes As Integer) As Single
        Dim saldoTotal As Single = 0
        Dim Contas As New List(Of Conta)

        Contas = getContasUsuario(UsuCod)

        For Each objConta As Conta In Contas
            saldoTotal += getSaldoConta(objConta.ConCod, ano, mes, UsuCod)
        Next

        Return saldoTotal
    End Function

    ''' <summary>
    ''' Credita um valor na conta
    ''' </summary>
    ''' <param name="mes">Mês a creditar</param>
    ''' <param name="ano">Ano a creditar</param>
    ''' <param name="valor">Valor que deve ser somado</param>
    ''' <remarks></remarks>
    Public Shared Sub Creditar(ConCod As Integer, ByVal mes As Integer, ByVal ano As Integer, ByVal valor As Single, UsuCod As Integer)
        Dim saldoAtual As Single = getSaldoConta(ConCod, ano, mes, UsuCod)

        saldoAtual += valor
        GravaSaldo(ConCod, mes, ano, saldoAtual)
    End Sub

    ''' <summary>
    ''' Debita um valor da conta
    ''' </summary>
    ''' <param name="mes">Mês a debitar</param>
    ''' <param name="ano">Ano a debitar</param>
    ''' <param name="valor">Valor a subtrair</param>
    ''' <remarks></remarks>
    Public Shared Sub Debitar(ConCod As Integer, ByVal mes As Integer, ByVal ano As Integer, ByVal valor As Single, UsuCod As Integer)
        Dim saldoAtual As Single = getSaldoConta(ConCod, ano, mes, UsuCod)

        saldoAtual -= valor
        GravaSaldo(ConCod, mes, ano, saldoAtual)
    End Sub

    ''' <summary>
    ''' Cria o registro de saldo da conta para o mês, caso não exista
    ''' </summary>
    ''' <param name="mes"></param>
    ''' <param name="ano"></param>
    ''' <param name="saldoAtual"></param>
    ''' <remarks></remarks>
    Public Shared Sub GravaSaldo(ConCod As Integer, ByVal mes As Integer, ByVal ano As Integer, ByVal saldoAtual As Single)
        Dim Database As New clsDatabase
        Dim parametros(4) As SqlParameter
        Dim valorAjustar As Single

        Dim SaldoLancamentosMes As Single = getSaldoMes(ConCod, ano, mes)

        valorAjustar = saldoAtual - SaldoLancamentosMes
        Dim _sql As String = "UPDATE SAL SET AjusteManual=@AjusteManual,SalDatAtu=@DataAtual WHERE ConCod=@ConCod AND SalMes=@mes AND SalAno=@ano"
        parametros(0) = New SqlParameter("@mes", mes)
        parametros(1) = New SqlParameter("@ano", ano)
        parametros(2) = New SqlParameter("@ConCod", ConCod)
        parametros(3) = New SqlParameter("@AjusteManual", valorAjustar)
        parametros(4) = New SqlParameter("@DataAtual", Today)

        Database.Execute(_sql, parametros)
    End Sub

    ''' <summary>
    ''' Carrega TODOS os lançamentos da conta
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared Function loadLancamentos(ByRef objConta As Conta) As List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim dtLancamentos As New DataTable
        Dim Lancamentos As New List(Of Lancamento)
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT LanAno,LanSeq FROM Lancamentos WHERE ConCod=@ConCod"
        parametros(0) = New SqlParameter("@ConCod", objConta.ConCod)
        dtLancamentos = Database.Query(_sql, parametros)

        For Each linhaLancamento As DataRow In dtLancamentos.Rows
            Dim LanAno As Integer = CInt(linhaLancamento("LanAno"))
            Dim LanSeq As Integer = CInt(linhaLancamento("LanSeq"))
            Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)
            Lancamentos.Add(objLancamento)
            objLancamento = Nothing
        Next

        Return Lancamentos
    End Function

    ''' <summary>
    ''' Efetua a transferência de valores entre contas
    ''' </summary>
    ''' <param name="ContaOrigem">Código da conta de origem</param>
    ''' <param name="ContaDestino">Código da conta de destino</param>
    ''' <param name="valor">Valor a transferir</param>
    ''' <returns>Sucesso ou falha</returns>
    ''' <remarks></remarks>
    Shared Function fazerTransferencia(ByVal ContaOrigem As Integer, ByVal ContaDestino As Integer, ByVal valor As Single, UsuCod As Integer) As Boolean
        Try
            Using scope As New Transactions.TransactionScope
                Try
                    Debitar(ContaOrigem, Month(Today), Year(Today), valor, UsuCod)
                    Creditar(ContaDestino, Month(Today), Year(Today), valor, UsuCod)

                    scope.Complete()

                    Return True
                Catch ex As Exception
                    log.Error(ex.Message)
                End Try
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Shared Function getBanco(BankID As Integer) As Banco
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter
        Dim dtBanco As New DataTable
        Dim objBanco As New Banco

        Dim _sql As String = "SELECT Nome,ID FROM Bancos WHERE ID=@BankID"
        parametros(0) = New SqlParameter("@BankID", BankID)
        dtBanco = Database.Query(_sql, parametros)

        If dtBanco.Rows.Count > 0 Then
            objBanco.Nome = dtBanco.Rows(0)("Nome").ToString
            objBanco.ID = CInt(dtBanco.Rows(0)("ID"))
        End If

        Return objBanco
    End Function

    Shared Function getBancos() As List(Of Banco)
        Dim Database As New clsDatabase
        Dim dtBancos As New DataTable
        Dim bankList As New List(Of Banco)

        Dim _sql As String = "SELECT Nome,ID FROM Bancos"
        dtBancos = Database.Query(_sql)

        If dtBancos.Rows.Count > 0 Then
            For Each linha As DataRow In dtBancos.Rows
                Dim objBanco As New Banco
                objBanco.Nome = linha("Nome").ToString
                objBanco.ID = CInt(linha("ID"))

                bankList.Add(objBanco)
            Next
        End If

        Return bankList
    End Function
End Class