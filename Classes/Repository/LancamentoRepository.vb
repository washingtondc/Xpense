Imports System.Data.SqlClient
Imports NLog

Public Class LancamentoRepository
    Private Shared log As Logger = LogManager.GetCurrentClassLogger()

    Public Shared Function getLancamento(ByVal LanAno As Integer, ByVal LanSeq As Integer, ano As Integer, mes As Integer) As Lancamento
        Dim Database As New clsDatabase
        Dim dtLan As New DataTable
        Dim objLancamento As New Lancamento
        Dim parametros(1) As SqlParameter


        If LanSeq > 0 Then
            Dim _sql As String = "SELECT Lancamentos.*,LC.CarCod FROM Lancamentos LEFT JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno" & _
                " AND Lancamentos.LanSeq=LC.LanSeq WHERE Lancamentos.LanAno=@LanAno AND Lancamentos.LanSeq=@LanSeq"
            parametros(0) = New SqlParameter("@LanAno", LanAno)
            parametros(1) = New SqlParameter("@LanSeq", LanSeq)

            dtLan = Database.Query(_sql, parametros)

            If dtLan.Rows.Count > 0 Then
                objLancamento.UsuCod = CInt(dtLan.Rows(0)("UsuCod"))
                objLancamento.LanAno = CInt(dtLan.Rows(0)("LanAno"))
                objLancamento.LanSeq = CInt(dtLan.Rows(0)("LanSeq"))
                objLancamento.LanDes = CStr(dtLan.Rows(0)("LanDes"))
                objLancamento.LanVal = CSng(dtLan.Rows(0)("LanVal"))
                objLancamento.UsuCod = CInt(dtLan.Rows(0)("UsuCod"))
                objLancamento.ConCod = CInt(dtLan.Rows(0)("ConCod"))
                objLancamento.LanTip = CChar(dtLan.Rows(0)("LanTip"))
                objLancamento.LanDat = CDate(dtLan.Rows(0)("LanDat"))
                objLancamento.LanDia = objLancamento.LanDat.Day
                objLancamento.CatCod = CInt(dtLan.Rows(0)("CatCod"))
                objLancamento.LanDet = CStr(dtLan.Rows(0)("LanDet"))
                objLancamento.LanAutoDebit = CBool(dtLan.Rows(0)("LanAutoDebit"))

                'FIXO?
                If Not IsDBNull(dtLan.Rows(0)("LanFix")) Then
                    objLancamento.LanFix = CBool(dtLan.Rows(0)("LanFix"))
                Else
                    objLancamento.LanFix = False
                End If
                'ATIVO?
                If Not IsDBNull(dtLan.Rows(0)("LanAti")) Then
                    objLancamento.LanAti = CBool(dtLan.Rows(0)("LanAti"))
                Else
                    objLancamento.LanAti = False
                End If
                'CÓDIGO DO CARTÃO CREDITO
                If Not IsDBNull(dtLan.Rows(0)("CarCod")) Then
                    objLancamento.CarCod = CInt(dtLan.Rows(0)("CarCod"))
                    objLancamento.Cartao = CartaoRepository.GetCartao(objLancamento.CarCod)
                End If
            End If

            'BUSCA A PARCELA DO MÊS INFORMADO
            objLancamento.Parcela = getParcela(LanAno, LanSeq, ano, mes)

            'QUANTIDADE DE PARCELAS
            objLancamento.LanQtdPar = getQtdParcelas(objLancamento.LanAno, objLancamento.LanSeq)

            'CATEGORIA
            objLancamento.Categoria = CategoriaRepository.getCategoria(objLancamento.CatCod, objLancamento.UsuCod)
        End If

        Return objLancamento
    End Function

    ''' <summary>
    ''' Retorna a quantidade de parcelas de um lançamento
    ''' </summary>
    ''' <param name="LanAno"></param>
    ''' <param name="LanSeq"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getQtdParcelas(LanAno As Integer, LanSeq As Integer) As Integer
        Dim Database As New clsDatabase
        Dim parametros(1) As SqlParameter
        Dim _sql As String = "SELECT COUNT(*) FROM PARCELAS WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
        parametros(0) = New SqlParameter("@LanAno", LanAno)
        parametros(1) = New SqlParameter("@LanSeq", LanSeq)

        Return CInt(Database.ExecuteScalar(_sql, parametros))
    End Function

    ''' <summary>
    ''' Salva os dados do objeto no banco de dados
    ''' </summary>
    ''' <returns>Retorna o sequencial gerado</returns>
    ''' <remarks></remarks>
    Public Shared Function Save(objLancamento As Lancamento, Efetivado As Boolean) As Lancamento
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim objCartao As Cartao = Nothing

        If objLancamento.LanSeq = 0 Then 'INSERIR NOVO LANÇAMENTO
            Dim parametros(12) As SqlParameter
            'Gera código sequencia
            objLancamento.LanSeq = getLanSeq()

            _sql = "INSERT INTO Lancamentos VALUES(@LanAno,@LanSeq,@LanDes,@LanDat,@LanVal,@UsuCod,@ConCod,@LanTip,@CatCod,@LanFix,1,@FITID,@LanDet,@LanAutoDebit)"
            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
            parametros(2) = New SqlParameter("@LanDes", objLancamento.LanDes)
            parametros(3) = New SqlParameter("@LanDat", objLancamento.LanDat)
            parametros(4) = New SqlParameter("@LanVal", Replace(objLancamento.LanVal.ToString, ",", "."))
            parametros(5) = New SqlParameter("@UsuCod", objLancamento.UsuCod)
            parametros(6) = New SqlParameter("@ConCod", objLancamento.ConCod)
            parametros(7) = New SqlParameter("@LanTip", objLancamento.LanTip)
            parametros(8) = New SqlParameter("@CatCod", objLancamento.CatCod)
            parametros(9) = New SqlParameter("@LanFix", Convert.ToInt32(objLancamento.LanFix))
            parametros(10) = New SqlParameter("@FITID", objLancamento.FITID)
            parametros(11) = New SqlParameter("@LanDet", objLancamento.LanDet.ToString)
            parametros(12) = New SqlParameter("@LanAutoDebit", objLancamento.LanAutoDebit)

            Database.Execute(_sql, parametros)
        Else 'ATUALIZAR O LANÇAMENTO
            Dim parametros(11) As SqlParameter
            _sql = "UPDATE Lancamentos SET LanDes=@LanDes,LanDat=@LanDat,LanVal=@LanVal,ConCod=@ConCod,LanTip=@LanTip," & _
                "CatCod=@CatCod,LanFix=@LanFix,LanDet=@LanDet,LanAutoDebit=@LanAutoDebit WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND UsuCod=@UsuCod"

            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
            parametros(2) = New SqlParameter("@LanDes", objLancamento.LanDes)
            parametros(3) = New SqlParameter("@LanDat", objLancamento.LanDat)
            parametros(4) = New SqlParameter("@LanVal", Replace(objLancamento.LanVal.ToString, ",", "."))
            parametros(5) = New SqlParameter("@UsuCod", objLancamento.UsuCod)
            parametros(6) = New SqlParameter("@ConCod", objLancamento.ConCod)
            parametros(7) = New SqlParameter("@LanTip", objLancamento.LanTip)
            parametros(8) = New SqlParameter("@CatCod", objLancamento.CatCod)
            parametros(9) = New SqlParameter("@LanFix", Convert.ToInt32(objLancamento.LanFix))
            parametros(10) = New SqlParameter("@LanDet", objLancamento.LanDet)
            parametros(11) = New SqlParameter("@LanAutoDebit", objLancamento.LanAutoDebit)

            Database.Execute(_sql, parametros)

            'DELETA AS PARCELAS PENDENTES, CASO TENHA (Depois vai recriar)
            ReDim parametros(1)
            _sql = "DELETE FROM Parcelas WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND isnull(LanEftDat,0)=0"
            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)

            Database.Execute(_sql, parametros)

            'ATUALIZA DIA DAS PARCELAS
            Try
                ReDim parametros(2)
                _sql = "update Parcelas set LanDatPag = DATEADD(dd,@LanDat-datepart(dd,LanDatPag),LanDatPag) WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
                parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
                parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
                parametros(2) = New SqlParameter("@LanDat", objLancamento.LanDat)
                Database.Execute(_sql, parametros)
            Catch ex As Exception

            End Try

            Return objLancamento
        End If

        'LANÇAMENTO NO CARTÃO DE CRÉDITO ?
        If objLancamento.CarCod > 0 Then
            'Pegar data da fatura
            objCartao = CartaoRepository.GetCartao(objLancamento.CarCod)

            Dim parametros(2) As SqlParameter
            'INSERE REGISTRO DO CARTÃO
            _sql = "INSERT INTO LancamentoCartao VALUES(@LanAno,@LanSeq,@CarCod)"
            ReDim parametros(2)
            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
            parametros(2) = New SqlParameter("@CarCod", objLancamento.CarCod)
            Database.Execute(_sql, parametros)
        Else
            Dim parametros(1) As SqlParameter
            'REMOVE REGISTRO DO CARTÃO CASO TENHA
            _sql = "DELETE FROM LancamentoCartao WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
            ReDim parametros(1)
            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
            Database.Execute(_sql, parametros)
        End If

        'CRIAR PARCELAS
        Dim DataParcela As Date
        For x As Integer = 1 To objLancamento.LanQtdPar
            If objLancamento.CarCod > 0 Then
                DataParcela = objCartao.FaturaAtual.Data
            Else
                DataParcela = objLancamento.LanDat.AddMonths(x - 1)
            End If

            Dim objParcela As Parcela = criaParcela(objLancamento, DataParcela)
            LancamentoRepository.salvarParcela(objParcela)

            'ATUALIZAR REGISTRO DE EFETIVAÇÃO
            If Efetivado Then
                If DataParcela = objLancamento.LanDat Or DataParcela <= Today Then
                    Efetivar(objLancamento.LanAno, objLancamento.LanSeq, objLancamento.LanDat, objLancamento.LanTip, objLancamento.LanVal, True, 0, objLancamento.UsuCod)
                    objParcela.LanEftDat = Today
                    objParcela.LanEftVal = objLancamento.LanVal
                End If
            End If

            ' Insere a parcela no objeto lançamento
            objLancamento.Parcela = objParcela
        Next

        objLancamento.Categoria = CategoriaRepository.getCategoria(objLancamento.CatCod, objLancamento.UsuCod)

        Return objLancamento
    End Function

    ''' <summary>
    ''' Cria um registro de parcela para a data especificada
    ''' </summary>
    ''' <param name="DataPagamento"></param>
    ''' <remarks></remarks>
    Public Shared Function criaParcela(objLancamento As Lancamento, ByVal DataPagamento As Date) As Parcela
        Dim objParcela As New Parcela
        objParcela.LanAno = objLancamento.LanAno
        objParcela.LanSeq = objLancamento.LanSeq
        objParcela.LanDatPag = DataPagamento

        Return objParcela
    End Function

    ''' <summary>
    ''' Excluir uma parcela
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub excluirParcela(objParcela As Parcela)
        Dim Database As New clsDatabase
        Dim parametros(2) As SqlParameter
        Dim objLancamento As Lancamento = getLancamento(objParcela.LanAno, objParcela.LanSeq, 0, 0)

        Using excluirParcelaTransaction As New Transactions.TransactionScope
            Try
                'MOVIMENTAR O SALDO
                If objParcela.LanEftDat <> Nothing Then
                    If objLancamento.LanTip = "E" Then
                        ContaRepository.Debitar(objParcela.ConCod, Month(objParcela.LanEftDat), Year(objParcela.LanEftDat), objParcela.LanEftVal, objLancamento.UsuCod)
                    Else
                        ContaRepository.Creditar(objParcela.ConCod, Month(objParcela.LanEftDat), Year(objParcela.LanEftDat), objParcela.LanEftVal, objLancamento.UsuCod)
                    End If
                End If

                Dim _sql As String = "DELETE FROM Parcelas WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND LanDatPag=@LanDatPag"
                parametros(0) = New SqlParameter("@LanAno", objParcela.LanAno)
                parametros(1) = New SqlParameter("@LanSeq", objParcela.LanSeq)
                parametros(2) = New SqlParameter("@LanDatPag", objParcela.LanDatPag)

                Database.Execute(_sql, parametros)

                ReDim parametros(3)

                If objLancamento.LanFix Then
                    'Grava a parcela como excluída para o sistema não recriar
                    _sql = "INSERT INTO ParcelasExcluidas VALUES(@LanAno, @LanSeq, @LanDatPag,@LanDatExc)"
                    parametros(0) = New SqlParameter("@LanAno", objParcela.LanAno)
                    parametros(1) = New SqlParameter("@LanSeq", objParcela.LanSeq)
                    parametros(2) = New SqlParameter("@LanDatPag", objParcela.LanDatPag)
                    parametros(3) = New SqlParameter("@LanDatExc", Today)
                End If

                Database.Execute(_sql, parametros)

                excluirParcelaTransaction.Complete()
            Catch ex As Exception
                log.Error(ex.Message)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Excluir um lançamento
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub excluirLancamento(objLancamento As Lancamento)
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim parametros(1) As SqlParameter

        Try
            'EXCLUIR PARCELAS DELETADAS
            _sql = "DELETE FROM ParcelasExcluidas WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
            parametros(0) = New SqlParameter("LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("LanSeq", objLancamento.LanSeq)
            Database.Execute(_sql, parametros)

            'EXCLUIR VINCULO COM CARTÃO
            If objLancamento.CarCod > 0 Then
                _sql = "DELETE FROM LancamentoCartao WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
                parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
                parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
                Database.Execute(_sql, parametros)
            End If

            'EXCLUIR PARCELAS
            _sql = "SELECT * FROM Parcelas WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
            parametros(0) = New SqlParameter("@LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objLancamento.LanSeq)
            Dim dtParcelas As DataTable = Database.Query(_sql, parametros)

            For Each linha As DataRow In dtParcelas.Rows
                Dim AnoParcela As Integer = CDate(linha("LanDatPag")).Year
                Dim MesParcela As Integer = CDate(linha("LanDatPag")).Month
                Dim parcela As Parcela = LancamentoRepository.getParcela(objLancamento.LanAno, objLancamento.LanSeq, AnoParcela, MesParcela)

                excluirParcela(parcela)
            Next

            'DELETA O LANÇAMENTO
            _sql = "DELETE FROM Lancamentos WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
            parametros(0) = New SqlParameter("LanAno", objLancamento.LanAno)
            parametros(1) = New SqlParameter("LanSeq", objLancamento.LanSeq)
            Database.Execute(_sql, parametros)

        Catch ex As Exception
            log.Error(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Obter próximo sequencial de lançamento
    ''' </summary>
    ''' <returns>próximo sequencial</returns>
    ''' <remarks></remarks>
    Private Shared Function getLanSeq() As Integer
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter
        Try
            Dim _sql As String = "SELECT MAX(LanSeq) FROM Lancamentos WHERE LanAno=@LanAno"
            parametros(0) = New SqlParameter("@LanAno", Year(Today))
            Return CInt(Database.ExecuteScalar(_sql, parametros)) + 1
        Catch ex As Exception
            Return 1
        End Try
    End Function

    ''' <summary>
    ''' Efetivar uma parcela de um lançamento
    ''' </summary>
    ''' <param name="dataPagamento">Data de pagamento da parcela</param>
    ''' <param name="valor">Valor efetivado</param>
    ''' <param name="ContaEfetivar">Código da conta a efetivar, enviar 0 para efetivar com a conta do lançamento</param>
    ''' <remarks></remarks>
    Public Shared Sub Efetivar(LanAno As Integer, LanSeq As Integer, ByVal dataPagamento As DateTime, LanTip As String, ByVal valor As Single, ByVal movimentarSaldo As Boolean, ConCod As Integer, UsuCod As Integer)
        Dim objParcela As Parcela = getParcela(LanAno, LanSeq, dataPagamento.Year, dataPagamento.Month)

        If Not IsNothing(objParcela) Then

            Using scope As New Transactions.TransactionScope
                Try
                    objParcela.LanEftDat = Now
                    objParcela.LanEftVal = valor
                    salvarParcela(objParcela)

                    If movimentarSaldo Then
                        If LanTip = "E" Then
                            ContaRepository.Creditar(ConCod, Month(dataPagamento), Year(dataPagamento), valor, UsuCod)
                        Else
                            ContaRepository.Debitar(ConCod, Month(dataPagamento), Year(dataPagamento), valor, UsuCod)
                        End If
                    End If

                    scope.Complete()
                Catch ex As Exception
                    log.Error(ex.Message)
                End Try
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Reabrir um lançamento efetivado
    ''' </summary>
    ''' <param name="dataPagamento">Data de pagamento da parcela</param>
    ''' <remarks></remarks>
    Public Shared Sub Reabrir(objLancamento As Lancamento, ByVal dataPagamento As DateTime)
        Dim objParcela As Parcela
        Dim ConCod As Integer = objLancamento.ConCod
        Dim valorEfetivado As Single

        Using scope As New Transactions.TransactionScope
            Try
                objParcela = getParcela(objLancamento.LanAno, objLancamento.LanSeq, dataPagamento.Year, dataPagamento.Month)
                valorEfetivado = objParcela.LanEftVal

                objParcela.LanEftDat = Nothing
                objParcela.LanEftVal = 0
                salvarParcela(objParcela)

                If objLancamento.LanTip = "E" Then
                    ContaRepository.Debitar(ConCod, Month(objLancamento.LanDat), Year(objLancamento.LanDat), valorEfetivado, objLancamento.UsuCod)
                Else
                    ContaRepository.Creditar(ConCod, Month(objLancamento.LanDat), Year(objLancamento.LanDat), valorEfetivado, objLancamento.UsuCod)
                End If

                scope.Complete()
            Catch ex As Exception
                log.Error(ex.Message)
            End Try
        End Using

    End Sub

    ''' <summary>
    ''' Retorna todos os lançamentos do mês, de acordo com os filtros passados por parâmetro
    ''' </summary>
    ''' <param name="mes">Mês dos lançamentos</param>
    ''' <param name="ano">Ano dos lançamentos</param>
    ''' <param name="CatCod">Código da categoria a filtrar</param>
    ''' <param name="LanTip">Tipo de lançamento a filtrar</param>
    ''' <param name="status">E=Efetivado , P=Pendente</param>
    ''' <param name="UsuCod">Código do usuário</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function getLancamentosMes(ByVal mes As Integer, ByVal ano As Integer, ByVal CatCod As Integer, ByVal LanTip As String, ByVal status As String, ByVal UsuCod As Integer) As List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim dtLancamentos As New DataTable
        Dim strFiltro As String = ""
        Dim Lista As New List(Of Lancamento)
        Dim parametros(4) As SqlParameter

        criaParcelasDoMes(mes, ano, UsuCod)

        'PARCELAS
        _sql = "SELECT Lancamentos.CatCod,Lancamentos.LanVal,Lancamentos.LanTip,Lancamentos.LanFix,Lancamentos.LanAti," & _
            "Parcelas.LanAno, Parcelas.LanSeq, Parcelas.LanDatPag, Parcelas.LanEftDat,Parcelas.LanEftVal,LC.CarCod" & _
            " FROM Lancamentos LEFT JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
            " LEFT JOIN LancamentoCartao LC on Lancamentos.LanAno=LC.LanAno AND Lancamentos.LanSeq=LC.LanSeq" & _
            " WHERE(Year(LanDatPag) = @Ano And Month(LanDatPag) = @Mes)" & _
            " AND UsuCod=@UsuCod AND isNull(CarCod,0)=0"

        If CatCod > 0 Then
            _sql &= "CatCod=@CatCod"
        End If

        If LanTip <> "" Then
            _sql &= " AND LanTip=@LanTip"
        End If

        If status = "P" Then 'Pendentes
            _sql &= " AND isNull(LanEftDat,0)=0"
        End If

        parametros(0) = New SqlParameter("@Ano", ano)
        parametros(1) = New SqlParameter("@Mes", mes)
        parametros(2) = New SqlParameter("@UsuCod", UsuCod)
        parametros(3) = New SqlParameter("@CatCod", CatCod)
        parametros(4) = New SqlParameter("@LanTip", LanTip)

        dtLancamentos = Database.Query(_sql, parametros)

        For Each linha As DataRow In dtLancamentos.Rows
            Dim LanAno As Integer = CInt(linha("LanAno"))
            Dim LanSeq As Integer = CInt(linha("LanSeq"))

            Dim objLancamento As Lancamento = getLancamento(LanAno, LanSeq, ano, mes)
            Lista.Add(objLancamento)
        Next

        'Carregar fatura do cartão
        If LanTip = "" Or LanTip = "S" Then
            Lista.AddRange(getLancamentosFaturas(UsuCod, ano, mes, status))
        End If

        Return Lista
    End Function

    ''' <summary>
    ''' Retorna a quantidade total de usuários cadastrados no sistema
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getQtdTotalLancamentos() As Integer
        Dim Database As New clsDatabase

        Dim _sql As String = "SELECT count(*) FROM Lancamentos"
        Return CInt(Database.ExecuteScalar(_sql))
    End Function

    ''' <summary>
    ''' Retorna os lançamentos que representam as faturas do mês
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <param name="tipo">P=Pendente</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getLancamentosFaturas(UsuCod As Integer, ano As Integer, mes As Integer, tipo As String) As List(Of Lancamento)
        Dim lista As New List(Of Lancamento)
        Dim Cartoes As List(Of Cartao) = CartaoRepository.GetCartoes(UsuCod)

        For Each objCartao As Cartao In Cartoes
            Dim carrega As Boolean = True
            Dim objLancamento As New Lancamento
            Dim dataFatura As Date = New Date(ano, mes, objCartao.CarDiaPag)

            Dim ValorPago As Single = CartaoRepository.getValorPagoFatura(objCartao.CarCod, mes, ano)
            Dim valorFatura As Single = CartaoRepository.getValorFatura(objCartao.CarCod, dataFatura)

            If tipo = "P" And ValorPago > 0 Or valorFatura = 0 Then
                carrega = False
            End If

            If carrega Then
                'LANÇAMENTO
                objLancamento.LanDat = New Date(ano, mes, objCartao.CarDiaPag)
                objLancamento.LanDia = objCartao.CarDiaPag
                objLancamento.LanDes = "Fatura " & objCartao.CarNom
                objLancamento.LanVal = valorFatura
                objLancamento.CarCod = objCartao.CarCod
                objLancamento.LanAti = True
                objLancamento.LanTip = CChar("S")
                objLancamento.UsuCod = UsuCod
                objLancamento.LanFix = True
                'CARTÃO
                objLancamento.Cartao = CartaoRepository.GetCartao(objCartao.CarCod)
                'PARCELA
                objLancamento.Parcela = New Parcela
                objLancamento.Parcela.LanEftVal = ValorPago
                objLancamento.Parcela.LanDatPag = New Date(ano, mes, Day(objLancamento.LanDat))
                If ValorPago > 0 Then
                    objLancamento.Parcela.LanEftDat = objLancamento.LanDat
                End If

                lista.Add(objLancamento)
            End If
        Next

        Return lista
    End Function

    ''' <summary>
    ''' Retorna valor pendente a receber no mês especificado
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <param name="mes"></param>
    ''' <param name="ano"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorReceber(UsuCod As Integer, mes As Integer, ano As Integer) As Single
        Dim Database As New clsDatabase
        Dim valorReceber As Single = 0
        Dim _sql As String
        Dim parametros(2) As SqlParameter

        _sql = "SELECT SUM(LanVal)" & _
            " FROM Lancamentos LEFT JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
            " WHERE(Year(LanDatPag) = @Ano And Month(LanDatPag) = @Mes)" & _
            " AND UsuCod=@UsuCod AND LanTip='E' AND isnull(LanEftDat,0)=0"

        parametros(0) = New SqlParameter("@Ano", ano)
        parametros(1) = New SqlParameter("@Mes", mes)
        parametros(2) = New SqlParameter("@UsuCod", UsuCod)

        Try
            valorReceber = CSng(Database.ExecuteScalar(_sql, parametros))
        Catch ex As InvalidCastException
            valorReceber = 0
        End Try

        Return valorReceber
    End Function

    ''' <summary>
    ''' Retorna valor pendente a pagar(débitos) no mês especificado
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <param name="mes"></param>
    ''' <param name="ano"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorPagar(UsuCod As Integer, mes As Integer, ano As Integer) As Single
        Dim Database As New clsDatabase
        Dim valorPagar As Single = 0
        Dim _sql As String
        Dim parametros(2) As SqlParameter

        _sql = "SELECT SUM(LanVal)" & _
            " FROM Lancamentos LEFT JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
            " WHERE(Year(LanDatPag) = @Ano And Month(LanDatPag) = @Mes)" & _
            " AND UsuCod=@UsuCod AND LanTip='S' AND isnull(LanEftDat,0)=0"

        parametros(0) = New SqlParameter("@Ano", ano)
        parametros(1) = New SqlParameter("@Mes", mes)
        parametros(2) = New SqlParameter("@UsuCod", UsuCod)
        Try
            valorPagar = CSng(Database.ExecuteScalar(_sql, parametros))
        Catch ex As InvalidCastException
            valorPagar = 0
        End Try

        Return valorPagar
    End Function

    ''' <summary>
    ''' Retorna um objeto parcela
    ''' </summary>
    ''' <param name="LanAno"></param>
    ''' <param name="LanSeq"></param>
    ''' <param name="AnoParcela"></param>
    ''' <param name="MesParcela"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getParcela(ByVal LanAno As Integer, ByVal LanSeq As Integer, AnoParcela As Integer, MesParcela As Integer) As Parcela
        Dim Database As New clsDatabase
        Dim dtParcela As New DataTable
        Dim objParcela As Parcela = Nothing
        Dim parametros(3) As SqlParameter

        Dim _sql As String = "SELECT Parcelas.*,Lancamentos.LanTip,Lancamentos.ConCod FROM Parcelas JOIN Lancamentos ON Parcelas.LanAno=Lancamentos.LanAno " & _
            "AND Parcelas.LanSeq=Lancamentos.LanSeq WHERE Parcelas.LanAno=@LanAno AND Parcelas.LanSeq=@LanSeq" & _
            " AND MONTH(LanDatPag)=@MesParcela AND YEAR(LanDatPag)=@AnoParcela"

        parametros(0) = New SqlParameter("@LanAno", LanAno)
        parametros(1) = New SqlParameter("@LanSeq", LanSeq)
        parametros(2) = New SqlParameter("@MesParcela", MesParcela)
        parametros(3) = New SqlParameter("@AnoParcela", AnoParcela)

        dtParcela = Database.Query(_sql, parametros)

        If dtParcela.Rows.Count > 0 Then
            Try
                objParcela = New Parcela
                objParcela.LanAno = CInt(dtParcela.Rows(0)("Lanano"))
                objParcela.LanSeq = CInt(dtParcela.Rows(0)("LanSeq"))
                If Not IsDBNull(dtParcela.Rows(0)("LanEftDat")) Then
                    objParcela.LanEftDat = CDate(dtParcela.Rows(0)("LanEftDat"))
                    objParcela.LanEftVal = CSng(dtParcela.Rows(0)("LanEftVal"))
                End If

                objParcela.LanDatPag = CDate(dtParcela.Rows(0)("LanDatPag"))
                objParcela.ConCod = CInt(dtParcela.Rows(0)("ConCod"))
                objParcela.ParSeq = getPosicaoParcela(LanAno, LanSeq, objParcela.LanDatPag)
            Catch ex As Exception
                log.Error(ex.Message)
            End Try
        End If
        Return objParcela
    End Function

    Public Shared Function ParcelaFoiExcluida(ByVal LanAno As Integer, ByVal LanSeq As Integer, AnoParcela As Integer, MesParcela As Integer) As Boolean
        Dim Database As New clsDatabase
        Dim dtParcela As New DataTable
        Dim parametros(3) As SqlParameter

        Dim _sql As String = "SELECT LanAno FROM ParcelasExcluidas WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND MONTH(LanDatPag)=@Mes AND YEAR(LanDatPag)=@Ano"
        parametros(0) = New SqlParameter("@LanAno", LanAno)
        parametros(1) = New SqlParameter("@LanSeq", LanSeq)
        parametros(2) = New SqlParameter("@Mes", MesParcela)
        parametros(3) = New SqlParameter("@Ano", AnoParcela)

        dtParcela = Database.Query(_sql, parametros)

        If dtParcela.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Cria parcela para todos os lançamentos ativos no mês especificado
    ''' </summary>
    ''' <param name="mes"></param>
    ''' <param name="ano"></param>
    ''' <param name="UsuCod"></param>
    ''' <remarks></remarks>
    Public Shared Sub criaParcelasDoMes(ByVal mes As Integer, ByVal ano As Integer, UsuCod As Integer)
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim dtLancamentos As New DataTable
        Dim dtParcela As New DataTable
        Dim diasMes = Date.DaysInMonth(ano, mes)
        Dim ultimoDiaMes As New Date(ano, mes, diasMes)
        Dim parametros(1) As SqlParameter

        ' Não cria se estiver visualizando períodos anteriores ao atual
        If ultimoDiaMes > Today Then
            'Buscar os lançamentos fixos
            _sql = "SELECT LanAno,LanSeq,LanDat,LanTip FROM Lancamentos" & _
                        " WHERE LanFix=1 AND LanAti=1 AND LanDat<= @ultimoDiaMes AND UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("@ultimoDiaMes", ultimoDiaMes)
            parametros(1) = New SqlParameter("@UsuCod", UsuCod)

            dtLancamentos = Database.Query(_sql, parametros)

            For Each linha As DataRow In dtLancamentos.Rows
                ReDim parametros(3)

                Dim objParcelaMes As Parcela = LancamentoRepository.getParcela(CInt(linha("LanAno")), CInt(linha("LanSeq")), ano, mes)

                'Se não tem parcela
                If IsNothing(objParcelaMes) Then
                    If Not ParcelaFoiExcluida(CInt(linha("LanAno")), CInt(linha("LanSeq")), ano, mes) Then
                        Dim diaLancamento = CDate(linha("LanDat")).Day
                        Dim objParcela As Parcela = Nothing

                        objParcela = New Parcela
                        objParcela.LanAno = CInt(linha("LanAno"))
                        objParcela.LanSeq = CInt(linha("LanSeq"))
                        objParcela.LanDatPag = New Date(ano, mes, diaLancamento)

                        salvarParcela(objParcela)
                    End If
                End If
                dtParcela.Clear()
            Next
        End If
    End Sub

    ''' <summary>
    ''' Salva os dados do objeto no banco
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub salvarParcela(objParcela As Parcela)
        Dim Database As New clsDatabase
        Dim parametros(4) As SqlParameter
        Dim _sql As String

        Try
            Dim valorEfetivado As String

            valorEfetivado = Replace(objParcela.LanEftVal.ToString, ",", ".")
            _sql = "INSERT INTO Parcelas VALUES(@LanAno,@LanSeq,@LanDatPag,@LanDatEft,@LanValEft)"

            parametros(0) = New SqlParameter("@LanAno", objParcela.LanAno)
            parametros(1) = New SqlParameter("@LanSeq", objParcela.LanSeq)
            parametros(2) = New SqlParameter("@LanDatPag", objParcela.LanDatPag)
            If objParcela.LanEftDat <> Nothing Then
                parametros(3) = New SqlParameter("@LanDatEft", objParcela.LanEftDat)
            Else
                parametros(3) = New SqlParameter("@LanDatEft", SqlDbType.DateTime)
                parametros(3).Value = DBNull.Value
            End If
            parametros(4) = New SqlParameter("@LanValEft", valorEfetivado)
            Database.Execute(_sql, parametros)
        Catch ex As Exception
            ReDim parametros(4)
            _sql = "UPDATE Parcelas SET LanEftDat=@LanEftDat,LanEftVal=@LanEftVal,LanDatPag=@LanDatPag WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND LanDatPag=@LanDatPag"
            If objParcela.LanEftDat <> Nothing Then
                parametros(0) = New SqlParameter("@LanEftDat", objParcela.LanEftDat)
            Else
                parametros(0) = New SqlParameter("@LanEftDat", SqlDbType.DateTime)
                parametros(0).Value = DBNull.Value
            End If
            parametros(1) = New SqlParameter("@LanEftVal", objParcela.LanEftVal)
            parametros(2) = New SqlParameter("@LanDatPag", objParcela.LanDatPag)
            parametros(3) = New SqlParameter("@LanAno", objParcela.LanAno)
            parametros(4) = New SqlParameter("@LanSeq", objParcela.LanSeq)

            Database.Execute(_sql, parametros)
        End Try
    End Sub

    ''' <summary>
    ''' Retorna o número da parcela entre as parcelas do lançamento
    ''' </summary>
    ''' <param name="LanAno"></param>
    ''' <param name="LanSeq"></param>
    ''' <param name="LanDatPag"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getPosicaoParcela(LanAno As Integer, LanSeq As Integer, LanDatPag As Date) As Integer
        Dim Database As New clsDatabase
        Dim posicao As Integer
        Dim parametros(2) As SqlParameter

        Dim _sql As String = "SELECT COUNT(LanDatPag) FROM Parcelas WHERE LanAno=@LanAno AND LanSeq=@LanSeq AND LanDatPag<@LanDatPag"
        parametros(0) = New SqlParameter("@LanAno", LanAno)
        parametros(1) = New SqlParameter("@LanSeq", LanSeq)
        parametros(2) = New SqlParameter("@LanDatPag", LanDatPag)

        posicao = CInt(Database.ExecuteScalar(_sql, parametros))

        Return posicao + 1
    End Function

    ''' <summary>
    ''' Obtem os lançamentos do dia
    ''' </summary>
    ''' <param name="data">Data a obter lançamentos</param>
    ''' <returns>Datatable com os lançamentos</returns>
    ''' <remarks></remarks>
    Shared Function getLancamentosDia(ByVal data As Date, ByVal UsuCod As Integer) As Alertas
        Dim listaAlertas As New Alertas
        Dim Database As New clsDatabase
        Dim dtLancamentos As New DataTable
        Dim parametros(1) As SqlParameter
        Dim _sql As String

        Try
            'PARCELAS
            _sql = "SELECT DISTINCT Lancamentos.LanAno,Lancamentos.LanSeq,Parcelas.LanDatPag" & _
                " FROM Lancamentos LEFT JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
                " LEFT JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno AND Lancamentos.LanSeq=LC.LanSeq" & _
                " WHERE(LanDatPag <= @Data) AND UsuCod=@UsuCod AND LanEftVal=0 AND isnull(LC.CarCod,0)=0"
            parametros(0) = New SqlParameter("@Data", data)
            parametros(1) = New SqlParameter("@UsuCod", UsuCod)

            dtLancamentos = Database.Query(_sql, parametros)
            listaAlertas.QtdAlertas = dtLancamentos.Rows.Count

            For Each linha As DataRow In dtLancamentos.Rows
                Dim AnoParcela As Integer = CDate(linha("LanDatPag")).Year
                Dim MesParcela As Integer = CDate(linha("LanDatPag")).Month
                Dim objLancamento As Lancamento = getLancamento(CInt(linha("LanAno")), CInt(linha("LanSeq")), data.Year, data.Month)
                objLancamento.Parcela = getParcela(objLancamento.LanAno, objLancamento.LanSeq, AnoParcela, MesParcela)
                listaAlertas.Lancamentos.Add(objLancamento)
            Next
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        'TODO: Pegar as faturas também
        Return listaAlertas
    End Function

    Shared Function getResumoAnual(UsuCod As Integer, Ano As Integer) As List(Of ResumoAnual)
        Dim CategoriasUsuario As List(Of Categoria)
        Dim ListaResumo As New List(Of ResumoAnual)
        Dim mes As Integer

        CategoriasUsuario = CategoriaRepository.getCategoriasUsuario(UsuCod)

        For Each objCategoria As Categoria In CategoriasUsuario
            Dim ResumoAnualCategoria As New ResumoAnual

            ResumoAnualCategoria.Titulo = objCategoria.CatNom
            ResumoAnualCategoria.Descricao = objCategoria.CatDes

            'Valor do mês
            For mes = 1 To 12 'Meses
                objCategoria = CategoriaRepository.getValores(objCategoria, mes, Ano)

                If mes < Month(Today) Then
                    'Considerar o valor que foi realmente efetivado no mês
                    ResumoAnualCategoria.ValorMes(mes - 1) = objCategoria.Valores.SaldoConfirmado
                    ResumoAnualCategoria.Entradas(mes - 1) = objCategoria.Valores.EntradasConfirmado
                    ResumoAnualCategoria.Saidas(mes - 1) = objCategoria.Valores.SaidasConfirmado
                Else
                    'Considerar o valor previsto da categoria (todos os lançamentos, mesmo ainda não confirmados)
                    ', caso o valor seja maior que o orçamento
                    If objCategoria.Valores.SaldoPrevisto > objCategoria.CatOrc Then
                        ResumoAnualCategoria.ValorMes(mes - 1) = objCategoria.Valores.SaldoPrevisto
                        ResumoAnualCategoria.Entradas(mes - 1) = objCategoria.Valores.EntradasPrevisto
                        ResumoAnualCategoria.Saidas(mes - 1) = objCategoria.Valores.SaidasPrevisto
                    Else
                        ResumoAnualCategoria.ValorMes(mes - 1) = objCategoria.CatOrc
                    End If
                End If
            Next
            ListaResumo.Add(ResumoAnualCategoria)
        Next

        Return ListaResumo
    End Function

    Shared Function getByFITID(UsuCod As Integer, FITID As String) As Lancamento
        Dim Database As New clsDatabase
        Dim dtLancamento As New DataTable
        Dim parametros(1) As SqlParameter
        Dim objLancamento As Lancamento = Nothing

        Dim _sql As String = "SELECT LanAno,LanSeq FROM Lancamentos WHERE UsuCod=@UsuCod AND FITID=@FITID"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        parametros(1) = New SqlParameter("@FITID", FITID)

        dtLancamento = Database.Query(_sql, parametros)
        If dtLancamento.Rows.Count > 0 Then
            Dim linha As DataRow = dtLancamento.Rows(0)
            objLancamento = LancamentoRepository.getLancamento(CInt(linha("LanAno")), CInt(linha("LanSeq")), 0, 0)
        End If

        Return objLancamento
    End Function

End Class