Imports System.Web.Services
Imports System.Transactions

<System.Web.Script.Services.ScriptService()> _
Public Class wsAdmin
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function clearDB() As Boolean
        Dim _sql As String
        Dim objDatabase As New clsDatabase

        Try

            Using tran As New TransactionScope()
                'EXCLUIR CARTÕES
                _sql = "DELETE FROM CAR"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR CATEGORIAS
                _sql = "DELETE FROM CAT"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR CONTAS
                _sql = "DELETE FROM CON"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR FATURAS
                _sql = "DELETE FROM FAT"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR LANÇAMENTOS NO CARTÃO
                _sql = "DELETE FROM LancamentoCartao"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR LANÇAMENTOS 
                _sql = "DELETE FROM Lancamentos"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR PARCELAS 
                _sql = "DELETE FROM Parcelas"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR PARCELAS EXCLUÍDAS 
                _sql = "DELETE FROM ParcelasExcluidas"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR SALDOS 
                _sql = "DELETE FROM SAL"
                objDatabase.ExecuteScalar(_sql)

                'EXCLUIR USUÁRIOS 
                _sql = "DELETE FROM USU WHERE UsuAdmin=0"
                objDatabase.ExecuteScalar(_sql)
                tran.Complete()
                Return True
            End Using
        Catch ex As TransactionAbortedException
            Return False
        Catch ex As ApplicationException
            Return False
        End Try
    End Function


    <WebMethod()> _
    Public Function getQtdTotalCategorias() As Integer
        Return CategoriaRepository.getQtdTotalCategorias
    End Function

    <WebMethod()> _
    Public Function getQtdTotalLancamentos() As Integer
        Return LancamentoRepository.getQtdTotalLancamentos
    End Function

    <WebMethod()> _
    Public Function getQtdTotalUsuarios() As Integer
        Return UsuarioRepository.getQtdTotalUsuarios
    End Function

    <WebMethod()> _
    Public Function getQtdTotalContas() As Integer
        Return ContaRepository.getQtdTotalContas
    End Function
End Class