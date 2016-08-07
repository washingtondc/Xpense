Public Class Categoria
    Public Property UsuCod As Integer
    Public Property CatCod As Integer
    Public Property CatNom As String
    Public Property CatDes As String
    Public Property CatCor As String
    Public Property CatOrc As Single
    Public Property Valores As Valores

    Public Sub New()
        Valores = New Valores
    End Sub
End Class
