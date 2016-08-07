Public Class Config
    Public Property CfgShowHelp As Boolean
    Public Property CfgImportShowAll As Boolean
    Public Property CftSaldoDiario As Boolean
    Public Property CfgAlertaEmail As Boolean
    Public Property CfgQtdAlertasTela As Integer

    Public Sub New()
        CfgShowHelp = True
        CfgImportShowAll = True
        CftSaldoDiario = True
        CfgAlertaEmail = True
        CfgQtdAlertasTela = 10
    End Sub
End Class
