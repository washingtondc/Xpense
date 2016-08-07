Imports System.Data.SqlClient

Public Class ConfigRepository
    Public Shared Function getUserConfig(UsuCod As Integer) As Config
        Dim parametros(0) As SqlParameter
        Dim _sql As String
        Dim Database As New clsDatabase
        Dim objConfig As New Config
        Dim dtConfig As New DataTable

        _sql = "SELECT * FROM CFG WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("UsuCod", UsuCod)

        dtConfig = Database.Query(_sql, parametros)

        For Each linha As DataRow In dtConfig.Rows
            objConfig.CfgAlertaEmail = CBool(linha("CfgAlertaEmail"))
            objConfig.CfgImportShowAll = CBool(linha("CfgImportShowAll"))
            objConfig.CfgQtdAlertasTela = CInt(linha("CfgQtdAlertasTela"))
            objConfig.CfgShowHelp = CBool(linha("CfgShowHelp"))
            objConfig.CftSaldoDiario = CBool(linha("CftSaldoDiario"))
        Next

        Return objConfig
    End Function

    Shared Function saveConfig(UsuCod As Integer, Config As Config) As Boolean
        Dim parametros(5) As SqlParameter
        Dim _sql As String
        Dim Database As New clsDatabase

        Try
            _sql = "UPDATE CFG SET CfgAlertaEmail=@CfgAlertaEmail,CfgImportShowAll=@CfgImportShowAll" & _
                        ",CfgQtdAlertasTela=@CfgQtdAlertasTela,CfgShowHelp=@CfgShowHelp,CftSaldoDiario=@CftSaldoDiario" & _
                        " WHERE UsuCod=@UsuCod"
            parametros(0) = New SqlParameter("CfgAlertaEmail", Config.CfgAlertaEmail)
            parametros(1) = New SqlParameter("CfgImportShowAll", Config.CfgImportShowAll)
            parametros(2) = New SqlParameter("CfgQtdAlertasTela", Config.CfgQtdAlertasTela)
            parametros(3) = New SqlParameter("CfgShowHelp", Config.CfgShowHelp)
            parametros(4) = New SqlParameter("CftSaldoDiario", Config.CftSaldoDiario)
            parametros(5) = New SqlParameter("UsuCod", UsuCod)

            Database.Execute(_sql, parametros)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
