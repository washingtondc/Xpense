Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports EasyFarm.OFX
Imports System.IO
Imports System.Web.Script.Services

<System.Web.Script.Services.ScriptService()> _
Public Class wsImportar
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function loadOFX(fileName As String) As List(Of Lancamento)
        Dim lista As New List(Of Lancamento)
        Dim UsuCod As Integer = CInt(Session("UsuCod"))
        Dim filePath = Server.MapPath("/Uploads/") & UsuCod.ToString & System.IO.Path.GetFileName(fileName)

        If File.Exists(filePath) Then
            Dim document As New OfxDocument(New FileStream(filePath, FileMode.Open))
            Dim objConta As Conta

            objConta = ContaRepository.getByBankID(UsuCod, CInt(document.BankID))

            For Each trans As Transaction In document.Transactions
                Dim Database As New clsDatabase
                Dim objLancamento As Lancamento

                objLancamento = LancamentoRepository.getByFITID(UsuCod, trans.FITID)

                If IsNothing(objLancamento) Then
                    objLancamento = New Lancamento
                    If InStr("Credit", trans.TransType.ToString) > 0 Then
                        objLancamento.LanTip = CChar("E")
                    Else
                        objLancamento.LanTip = CChar("S")
                    End If
                    objLancamento.LanDat = getDate(trans.DatePosted)
                    objLancamento.LanVal = Math.Abs(CSng(trans.TransAmount.Replace(".", ","))) 'TODO: Dá pra melhorar isso?
                    objLancamento.LanAti = True
                    objLancamento.CarCod = 0
                    objLancamento.FITID = trans.FITID
                    objLancamento.UsuCod = UsuCod
                    objLancamento.LanDes = trans.Memo
                    objLancamento.LanDet = "Importado via OFX em:" & Today.ToShortDateString
                    If Not IsNothing(objConta) Then
                        objLancamento.ConCod = objConta.ConCod
                    End If
                End If

                lista.Add(objLancamento)
            Next
        End If

        Return lista
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function ImportarLancamentos(Lancamentos As List(Of Lancamento)) As Boolean
        For Each Lancamento As Lancamento In Lancamentos
            LancamentoRepository.Save(Lancamento, True)
        Next
        Return True
    End Function

    Private Function getDate(ByVal strData As String) As Date
        Dim dia, mes, ano As Integer

        ano = CInt(Mid(strData, 1, 4))
        mes = CInt(Mid(strData, 5, 2))
        dia = CInt(Mid(strData, 7, 2))

        Return New Date(ano, mes, dia)
    End Function
End Class