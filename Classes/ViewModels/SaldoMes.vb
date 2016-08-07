Public Class SaldoMes
    Public Property Conta As String
    Public Property saldo As List(Of Single)

    Public Sub New()
        saldo = New List(Of Single)
    End Sub
End Class
