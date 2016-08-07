Imports System.Data.SqlClient
Imports NLog

Public Class CartaoRepository
    Private Shared log As Logger = LogManager.GetCurrentClassLogger()

    ''' <summary>
    ''' Salva os dados no banco
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function Save(objCartao As Cartao) As Cartao
        Dim parametros(6) As SqlParameter
        Dim _sql As String
        Dim Database As New clsDatabase

        If objCartao.CarCod = 0 Then
            objCartao.CarCod = getCarCod(objCartao.UsuCod)

            _sql = "INSERT INTO CAR VALUES(@UsuCod,@CarCod,@CarBan,@CarLim,@CarDiaFec,@CarDiaPag,@CarNom)"

            parametros(0) = New SqlParameter("@UsuCod", objCartao.UsuCod)
            parametros(1) = New SqlParameter("@CarCod", objCartao.CarCod)
            parametros(2) = New SqlParameter("@CarBan", objCartao.CarBan)
            parametros(3) = New SqlParameter("@CarLim", objCartao.CarLim)
            parametros(4) = New SqlParameter("@CarDiaFec", objCartao.CarDiaFec)
            parametros(5) = New SqlParameter("@CarDiaPag", objCartao.CarDiaPag)
            parametros(6) = New SqlParameter("@CarNom", objCartao.CarNom)
            Database.Execute(_sql, parametros)

            'Obter todos os dados do objeto
            objCartao = GetCartao(objCartao.CarCod)
        Else
            _sql = "UPDATE CAR SET CarBan=@CarBan,CarLim=@CarLim,CarDiaFec=@CarDiaFec,CarDiaPag=@CarDiaPag," & _
                "CarNom=@CarNom WHERE UsuCod=@UsuCod AND CarCod=@CarCod"

            parametros(0) = New SqlParameter("@UsuCod", objCartao.UsuCod)
            parametros(1) = New SqlParameter("@CarCod", objCartao.CarCod)
            parametros(2) = New SqlParameter("@CarBan", objCartao.CarBan)
            parametros(3) = New SqlParameter("@CarLim", objCartao.CarLim)
            parametros(4) = New SqlParameter("@CarDiaFec", objCartao.CarDiaFec)
            parametros(5) = New SqlParameter("@CarDiaPag", objCartao.CarDiaPag)
            parametros(6) = New SqlParameter("@CarNom", objCartao.CarNom)
            Database.Execute(_sql, parametros)

            'ATUALIZA DIA DAS PARCELAS
            ReDim parametros(1)
            Dim dtLancamentosCartao As New DataTable
            _sql = "SELECT Ano,Sequenc FROM LancamentoCartao WHERE CarCod=@CarCod"
            parametros(0) = New SqlParameter("@CarCod", objCartao.CarCod)
            dtLancamentosCartao = Database.Query(_sql, parametros)

            ReDim parametros(2)
            Using dtLancamentosCartao
                For Each linha As DataRow In dtLancamentosCartao.Rows
                    Try
                        _sql = "UPDATE LAN SET LanDat = DATEADD(dd,@CarDiaPag-datepart(dd,LanDat),LanDat) WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
                        parametros(0) = New SqlParameter("@CarDiaPag", objCartao.CarDiaPag)
                        parametros(1) = New SqlParameter("@LanAno", linha("LanAno"))
                        parametros(2) = New SqlParameter("@LanSeq", linha("LanSeq"))

                        Database.Execute(_sql, parametros)
                        _sql = "UPDATE Parcelas SET LanDatPag = DATEADD(dd,@CarDiaPag-datepart(dd,LanDatPag),LanDatPag) WHERE LanAno=@LanAno AND LanSeq=@LanSeq"
                        parametros(0) = New SqlParameter("@CarDiaPag", objCartao.CarDiaPag)
                        parametros(1) = New SqlParameter("@LanAno", linha("LanAno"))
                        parametros(2) = New SqlParameter("@LanSeq", linha("LanSeq"))
                        Database.Execute(_sql, parametros)
                    Catch ex As SqlException
                        Debug.WriteLine(ex.Message)
                    End Try
                Next
            End Using
        End If

        Return objCartao
    End Function

    ''' <summary>
    ''' Carrega os dados de um cartão e preenche o objeto
    ''' </summary>
    ''' <param name="CarCod">Código do cartão a carregar</param>
    ''' <remarks></remarks>
    Public Shared Function GetCartao(ByVal CarCod As Integer) As Cartao
        Dim objCartao As Cartao = Nothing
        Dim dtCartao As New DataTable
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT * FROM CAR WHERE CarCod=@CarCod"
        parametros(0) = New SqlParameter("@CarCod", CarCod)
        dtCartao = Database.Query(_sql, parametros)

        If dtCartao.Rows.Count > 0 Then
            Dim UsuCod As Integer = CInt(dtCartao.Rows(0)("UsuCod"))
            Dim diaPagamento As Integer = CInt(dtCartao.Rows(0)("CarDiaPag"))
            Dim DataFaturaAtual = New Date(Today.Year, Today.Month, diaPagamento)
            Dim ValorPendente = getValorUsadoDoLimite(UsuCod, CarCod, DataFaturaAtual)
            Dim DataProximaFatura = DataFaturaAtual.AddMonths(1)

            objCartao = New Cartao
            objCartao.UsuCod = UsuCod
            objCartao.CarCod = CarCod
            objCartao.CarBan = CStr(dtCartao.Rows(0)("CarBan"))
            objCartao.CarLim = CSng(dtCartao.Rows(0)("CarLim"))
            objCartao.CarDiaFec = CInt(dtCartao.Rows(0)("CarDiaFec"))
            objCartao.CarDiaPag = diaPagamento
            objCartao.CarNom = CStr(dtCartao.Rows(0)("CarNom"))

            Dim FaturaAtual As New Fatura
            FaturaAtual.Data = DataFaturaAtual
            FaturaAtual.Valor = getValorFatura(CarCod, DataFaturaAtual)
            FaturaAtual.Efetivada = getValorPagoFatura(CarCod, DataFaturaAtual.Month, DataFaturaAtual.Year) > 0

            Dim ProximaFatura As New Fatura
            ProximaFatura.Data = DataProximaFatura
            ProximaFatura.Valor = getValorFatura(CarCod, DataProximaFatura)
            ProximaFatura.Efetivada = getValorPagoFatura(CarCod, DataProximaFatura.Month, DataProximaFatura.Year) > 0

            objCartao.FaturaAtual = FaturaAtual
            objCartao.ProximaFatura = ProximaFatura

            If objCartao.CarLim > 0 Then
                objCartao.Porcentagem = CInt(ValorPendente * 100 / objCartao.CarLim)
            End If
        End If

        Return objCartao
    End Function
    ''' <summary>
    ''' Obtem os cartões de um usuário
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCartoes(UsuCod As Integer) As List(Of Cartao)
        Dim Database As New clsDatabase
        Dim dtCartoes As New DataTable
        Dim Lista As New List(Of Cartao)
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT * FROM CAR WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)

        dtCartoes = Database.Query(_sql, parametros)

        If dtCartoes.Rows.Count > 0 Then
            For Each linhaBD As DataRow In dtCartoes.Rows
                Dim objCartao As Cartao = CartaoRepository.GetCartao(CInt(linhaBD("CarCod")))
                Lista.Add(objCartao)
            Next
        End If

        Return Lista
    End Function

    ''' <summary>
    ''' Obtém o próximo código do cartão
    ''' </summary>
    ''' <returns>Próximo número sequencial de cartões</returns>
    ''' <remarks></remarks>
    Private Shared Function getCarCod(UsuCod As Integer) As Integer
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT MAX(CarCod) FROM CAR WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)

        Try
            Return CInt(Database.ExecuteScalar(_sql, parametros)) + 1
        Catch ex As InvalidCastException
            Return 1
        End Try
    End Function

    ''' <summary>
    ''' Retorna a data da próxima fatura aberta
    ''' </summary>
    ''' <param name="objCartao"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getDataFaturaAberta(DiaPagamento As Integer, DiaFechamento As Integer) As Date
        Dim DataFaturaAtual As Date = New Date(Today.Year, Today.Month, DiaPagamento)

        If Today.Day <= DiaFechamento Then
            Return DataFaturaAtual
        Else
            Return DataFaturaAtual.AddMonths(1)
        End If
    End Function

    ''' <summary>
    ''' Exclui o cartão
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub delete(ObjCartao As Cartao, UsuCod As Integer)
        Dim Database As New clsDatabase
        Dim Lancamentos As New List(Of Lancamento)
        Dim _sql As String
        Dim parametros(1) As SqlParameter

        'EXCLUIR OS LANÇAMENTOS NO CARTÃO
        Lancamentos = getLancamentosCartao(ObjCartao)
        For Each objLancamento As Lancamento In Lancamentos
            LancamentoRepository.excluirLancamento(objLancamento)
        Next

        'EXCLUIR AS FATURAS
        _sql = "DELETE FROM FAT WHERE UsuCod=@UsuCod AND CarCod=@CarCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        parametros(1) = New SqlParameter("@CarCod", ObjCartao.CarCod)
        Database.Execute(_sql, parametros)

        'EXCLUIR O CARTÃO
        _sql = "DELETE FROM CAR WHERE UsuCod=@UsuCod AND CarCod=@CarCod"
        parametros(0) = New SqlParameter("@UsuCod", UsuCod)
        parametros(1) = New SqlParameter("@CarCod", ObjCartao.CarCod)
        Database.Execute(_sql, parametros)
    End Sub

    ''' <summary>
    ''' Carrega a lista de TODOS os lançamentos do cartão, independente de data ou se está pago
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared Function getLancamentosCartao(objCartao As Cartao) As List(Of Lancamento)
        Dim Lancamentos As New List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim dtLancamentos As New DataTable
        Dim parametros(0) As SqlParameter
        Dim LanAno, LanSeq As Integer

        Dim _sql As String = "SELECT Lancamentos.*,LC.CarCod FROM Lancamentos LEFT JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno" & _
                " AND Lancamentos.LanSeq=LC.LanSeq WHERE LC.CarCod=@CarCod"
        parametros(0) = New SqlParameter("CarCod", objCartao.CarCod)

        dtLancamentos = Database.Query(_sql, parametros)

        If dtLancamentos.Rows.Count > 0 Then
            For Each linha As DataRow In dtLancamentos.Rows
                LanAno = CInt(linha("LanAno"))
                LanSeq = CInt(linha("LanSeq"))
                Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, 0, 0)
                Lancamentos.Add(objLancamento)
                objLancamento = Nothing
            Next
        End If

        Return Lancamentos
    End Function

    ''' <summary>
    ''' Procedimentos para efetivar o pagamento da fatura
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub pagarFatura(objCartao As Cartao, ano As Integer, mes As Integer, ConCod As Integer, ValorPagar As Single, UsuCod As Integer)
        Dim Lancamentos As New List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim dataFatura As Date = New Date(ano, mes, objCartao.CarDiaPag)
        Dim valorTotalFatura As Single
        Dim valorPendentePagar As Single

        Lancamentos = getLancamentosFatura(objCartao.CarCod, ano, mes)

        Using scope As New Transactions.TransactionScope
            Try
                'EFETIVA TODOS OS LANÇAMENTOS SEM ALTERAR O SALDO
                For Each objLancamento As Lancamento In Lancamentos
                    LancamentoRepository.Efetivar(objLancamento.LanAno, objLancamento.LanSeq, Today, objLancamento.LanTip, objLancamento.LanVal, False, ConCod, UsuCod)
                Next

                'DEBITAR O VALOR DA CONTA
                ContaRepository.Debitar(ConCod, mes, ano, ValorPagar, UsuCod)

                'LER VALOR TODAL DA FATURA
                valorTotalFatura = CartaoRepository.getValorFatura(objCartao.CarCod, dataFatura)
                valorPendentePagar = valorTotalFatura - ValorPagar

                'GERA O REGISTRO DE PAGAMENTO DA FATURA
                'Deleta registro caso tenha
                Dim parametros(3) As SqlParameter
                Dim _sql As String = "DELETE FROM FAT WHERE UsuCod=@UsuCod AND FatMes=@FatMes AND FatAno=@FatAno AND CarCod=@CarCod"
                parametros(0) = New SqlParameter("@CarCod", objCartao.CarCod)
                parametros(1) = New SqlParameter("@UsuCod", objCartao.UsuCod)
                parametros(2) = New SqlParameter("@FatMes", mes)
                parametros(3) = New SqlParameter("@FatAno", ano)
                Database.Execute(_sql, parametros)
                'Gera novo registro
                ReDim parametros(7)
                _sql = "INSERT INTO FAT VALUES(@CarCod,@UsuCod,@FatMes,@FatAno,@FatTotVal,@FatEftVal,@FatPenVal,@FatEftDat)"
                parametros(0) = New SqlParameter("@CarCod", objCartao.CarCod)
                parametros(1) = New SqlParameter("@UsuCod", objCartao.UsuCod)
                parametros(2) = New SqlParameter("@FatMes", mes)
                parametros(3) = New SqlParameter("@FatAno", ano)
                parametros(4) = New SqlParameter("@FatTotVal", valorTotalFatura)
                parametros(5) = New SqlParameter("@FatEftVal", ValorPagar)
                parametros(6) = New SqlParameter("@FatPenVal", valorPendentePagar)
                parametros(7) = New SqlParameter("@FatEftDat", Today)

                Database.Execute(_sql, parametros)

                scope.Complete()
            Catch ex As Exception
                log.Error(ex.Message)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Obtem os lançamentos de uma fatura
    ''' </summary>
    ''' <param name="objCartao"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getLancamentosFatura(CarCod As Integer, ano As Integer, mes As Integer) As List(Of Lancamento)
        Dim Lancamentos As New List(Of Lancamento)
        Dim Database As New clsDatabase
        Dim dtLancamentos As New DataTable
        Dim parametros(1) As SqlParameter
        Dim objCartao As Cartao = CartaoRepository.GetCartao(CarCod)
        Dim dataFatura As Date = New Date(ano, mes, objCartao.CarDiaPag)

        Dim _sql As String = "SELECT Lancamentos.LanAno,Lancamentos.LanSeq,Lancamentos.LanVal,Lancamentos.LanDat,Lancamentos.LanTip,Parcelas.LanDatPag" &
                " FROM Lancamentos JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
                " JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno AND Lancamentos.LanSeq=LC.LanSeq" & _
                " WHERE Parcelas.LanDatPag=@LanDatPag AND LC.CarCod=@CarCod ORDER BY LanDat"
        parametros(0) = New SqlParameter("LanDatPag", dataFatura)
        parametros(1) = New SqlParameter("@CarCod", objCartao.CarCod)

        dtLancamentos = Database.Query(_sql, parametros)
        'TODO: Gerar lançamento para o valor pendente da fatura anterior
        Using dtLancamentos
            For Each linhaBD As DataRow In dtLancamentos.Rows
                Dim LanAno As Integer = CInt(linhaBD("LanAno"))
                Dim LanSeq As Integer = CInt(linhaBD("LanSeq"))
                Dim objLancamento As Lancamento = LancamentoRepository.getLancamento(LanAno, LanSeq, ano, mes)
                Lancamentos.Add(objLancamento)
                objLancamento = Nothing
            Next
        End Using

        Return Lancamentos
    End Function

    ''' <summary>
    ''' Retorna o valor total da fatura
    ''' </summary>
    ''' <param name="objCartao"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorFatura(CarCod As Integer, dataFatura As Date) As Single
        Dim Database As New clsDatabase
        Dim ValorFatura As Single
        Dim valorPendente As Single
        Dim parametros(1) As SqlParameter
        Dim dataFaturaAnterior = dataFatura.AddMonths(-1)

        Dim _sql As String = "SELECT SUM(Lancamentos.LanVal)" &
                " FROM Lancamentos JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
                " JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno AND Lancamentos.LanSeq=LC.LanSeq" & _
                " WHERE Parcelas.LanDatPag=@LanDatPag AND LC.CarCod=@CarCod"
        parametros(0) = New SqlParameter("@LanDatPag", dataFatura)
        parametros(1) = New SqlParameter("@CarCod", CarCod)

        Try
            ValorFatura = CSng(Database.ExecuteScalar(_sql, parametros))
        Catch ex As Exception

        End Try

        valorPendente = getValorFaturasNaoPago(CarCod, dataFaturaAnterior.Month, dataFaturaAnterior.Year)

        Return ValorFatura + valorPendente
    End Function

    ''' <summary>
    ''' Retorna o valor pendente da fatura anterior
    ''' </summary>
    ''' <param name="objCartao"></param>
    ''' <param name="ano"></param>
    ''' <param name="mes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorFaturasNaoPago(CarCod As Integer, Mes As Integer, Ano As Integer) As Single
        Dim Database As New clsDatabase
        Dim ValorNaoPago As Single
        Dim parametros(2) As SqlParameter

        Dim _sql As String = "SELECT FatPenVal FROM FAT WHERE CarCod=@CarCod AND FatAno=@FatAno AND FatMes=@FatMes"
        parametros(0) = New SqlParameter("@CarCod", CarCod)
        parametros(1) = New SqlParameter("@FatAno", Ano)
        parametros(2) = New SqlParameter("@FatMes", Mes)

        Try
            ValorNaoPago = CSng(Database.ExecuteScalar(_sql, parametros))
        Catch ex As Exception
            ValorNaoPago = 0
        End Try

        Return ValorNaoPago
    End Function

    ''' <summary>
    ''' Retorna o valor pendente a pagar no cartão, considera todos as parcelas futuras a pagar
    ''' para indicar o quanto foi utilizado do limite
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <param name="CarCod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorUsadoDoLimite(UsuCod As Integer, CarCod As Integer, dataFatura As Date) As Single
        Dim Database As New clsDatabase
        Dim ValorPendente As Single = 0
        Dim parametros(2) As SqlParameter

        Try
            Dim _sql As String = "SELECT SUM(Lancamentos.LanVal)" &
                " FROM Lancamentos JOIN Parcelas ON Lancamentos.LanAno=Parcelas.LanAno AND Lancamentos.LanSeq=Parcelas.LanSeq" & _
                " JOIN LancamentoCartao LC ON Lancamentos.LanAno=LC.LanAno AND Lancamentos.LanSeq=LC.LanSeq" & _
                " WHERE LanEftVal=0 AND LC.CarCod=@CarCod AND LanDatPag>=@DataFaturaAberta AND UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("@DataFaturaAberta", dataFatura)
            parametros(1) = New SqlParameter("@CarCod", CarCod)
            parametros(2) = New SqlParameter("@UsuCod", UsuCod)

            ValorPendente = CSng(Database.ExecuteScalar(_sql, parametros))
        Catch ex As InvalidCastException
            ValorPendente = 0
        End Try

        Return ValorPendente
    End Function

    ''' <summary>
    ''' Retorna o valor pago da fatura
    ''' </summary>
    ''' <param name="CarCod"></param>
    ''' <param name="MesFatura"></param>
    ''' <param name="AnoFatura"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValorPagoFatura(CarCod As Integer, MesFatura As Integer, AnoFatura As Integer) As Single
        Dim ValorPago As Single = 0
        Dim Database As New clsDatabase
        Dim parametros(2) As SqlParameter

        Dim _sql As String = "SELECT FatEftVal from Fat WHERE CarCod=@CarCod AND FatMes=@FatMes AND FatAno=@FatAno"
        parametros(0) = New SqlParameter("@CarCod", CarCod)
        parametros(1) = New SqlParameter("@FatMes", MesFatura)
        parametros(2) = New SqlParameter("@FatAno", AnoFatura)

        ValorPago = CSng(Database.ExecuteScalar(_sql, parametros))

        Return ValorPago
    End Function
End Class