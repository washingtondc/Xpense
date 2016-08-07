Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization


<System.Web.Script.Services.ScriptService()> _
Public Class wsGrafico
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)> _
    Public Function GastosPorCategoria(ByVal mes As Integer, ByVal ano As Integer) As List(Of GastosCategoria)
        Dim Categorias As New List(Of Categoria)
        Dim lista As New List(Of GastosCategoria)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))

        Categorias = CategoriaRepository.getCategoriasUsuario(UsuCod)

        For Each objCategoria As Categoria In Categorias
            objCategoria = CategoriaRepository.getValores(objCategoria, mes, ano)

            If objCategoria.Valores.SaldoConfirmado < 0 Then
                Dim objGastoCategoria As New GastosCategoria
                objGastoCategoria.CatCod = objCategoria.CatCod
                objGastoCategoria.Categoria = objCategoria.CatNom
                objGastoCategoria.Gasto = Math.Abs(objCategoria.Valores.SaldoConfirmado)
                lista.Add(objGastoCategoria)
            End If
        Next

        Return lista
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function SaldoPorMes(ByVal ano As Integer) As List(Of SaldoMes)
        Dim lista As New List(Of SaldoMes)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim Contas As List(Of Conta) = ContaRepository.getContasUsuario(UsuCod)

        For Each conta As Conta In Contas
            Dim objSaldoMes As New SaldoMes

            objSaldoMes.Conta = conta.ConDes
            For mes = 1 To 12
                Dim SaldoTotal As Single = 0

                Dim saldo As Single = ContaRepository.getSaldoConta(conta.ConCod, ano, mes, UsuCod)

                objSaldoMes.saldo.Add(saldo)
                SaldoTotal += saldo
            Next

            lista.Add(objSaldoMes)
        Next

        Return lista
    End Function
End Class