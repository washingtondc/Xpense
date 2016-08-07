Public Class clsCriptografia
    Public Shared Function Encode(texto As String) As String
        Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim hashedBytes As Byte()
        Dim encoder As New System.Text.UTF8Encoding()
        Dim saida As String = String.Empty

        hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(texto))

        For Each y In hashedBytes
            saida &= y
        Next y

        Return saida
    End Function
End Class
