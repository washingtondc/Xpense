Imports System.Data.SqlClient

Public Class UsuarioRepository
    Public Shared Function Save(objUsuario As Usuario) As Usuario
        Dim Database As New clsDatabase
        Dim _sql As String
        Dim parametros(7) As SqlParameter

        Try
            If objUsuario.UsuCod = 0 Then
                objUsuario.UsuCod = getUsuCod()
                _sql = "INSERT INTO USU VALUES(@UsuCod,@UsuNom,@UsuEml,@UsuSen,@BackgroundImage,@Avatar,@DataCadastro,@DataDesativado,0)"
                parametros(0) = New SqlParameter("@UsuCod", objUsuario.UsuCod)
                parametros(1) = New SqlParameter("@UsuNom", objUsuario.UsuNom)
                parametros(2) = New SqlParameter("@UsuEml", objUsuario.UsuEml)
                parametros(3) = New SqlParameter("@UsuSen", clsCriptografia.Encode(objUsuario.UsuSen))
                parametros(4) = New SqlParameter("@BackgroundImage", 1)
                parametros(5) = New SqlParameter("@Avatar", "no-avatar.jpg")
                parametros(6) = New SqlParameter("@DataCadastro", Now)
                parametros(7) = New SqlParameter("@DataDesativado", DBNull.Value)

                Database.Execute(_sql, parametros)

                'Criar conta "Carteira"
                Dim ContaCarteira As New Conta
                ContaCarteira.ConDes = "Carteira"
                ContaCarteira.ConFix = True
                ContaCarteira.ConSal = 0
                ContaCarteira.UsuCod = objUsuario.UsuCod
                ContaCarteira.Banco = Nothing
                ContaRepository.Save(ContaCarteira)

                'Criar "Config"
                Dim Config As New Config
                ConfigRepository.saveConfig(objUsuario.UsuCod, Config)
            Else
                _sql = "UPDATE USU SET UsuNom=@UsuNom,UsuEml=@UsuEml,Avatar=@Avatar WHERE UsuCod=@UsuCod"
                ReDim parametros(3)
                parametros(0) = New SqlParameter("@UsuCod", objUsuario.UsuCod)
                parametros(1) = New SqlParameter("@UsuNom", objUsuario.UsuNom)
                parametros(2) = New SqlParameter("@UsuEml", objUsuario.UsuEml)
                parametros(3) = New SqlParameter("@Avatar", objUsuario.Avatar)
                Database.Execute(_sql, parametros)
            End If

            Return objUsuario
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Validar login e senha do usuário
    ''' </summary>
    ''' <param name="email">Login</param>
    ''' <param name="senha">Senha</param>
    ''' <returns>Verdadeiro ou falso</returns>
    ''' <remarks>Considera se o usuário está desativado</remarks>
    Public Shared Function Login(ByVal email As String, ByVal senha As String) As Usuario
        Dim Usuario As Usuario = Nothing
        Dim Database As New clsDatabase
        Dim codigo As Object
        Dim parametros(1) As SqlParameter
        Dim senhaCriptografada As String = clsCriptografia.Encode(senha)

        Dim _sql As String = "SELECT UsuCod FROM USU WHERE UsuEml=@UsuEml AND UsuSen=@UsuSen AND ISNULL(DataDesativado,0)=0"
        parametros(0) = New SqlParameter("UsuEml", email)
        parametros(1) = New SqlParameter("UsuSen", senhaCriptografada)

        codigo = CInt(Database.ExecuteScalar(_sql, parametros))
        If Not IsDBNull(codigo) Then
            If CInt(codigo) > 0 Then
                Usuario = getUsuario(CInt(codigo))
            End If
        End If

        Usuario.UsuSen = ""
        Return Usuario
    End Function

    ''' <summary>
    ''' Carrega os dados do objeto
    ''' </summary>
    ''' <param name="UsuCod"></param>
    ''' <remarks></remarks>
    Public Shared Function getUsuario(ByVal UsuCod As Integer) As Usuario
        Dim Database As New clsDatabase
        Dim dtUsuario As New DataTable
        Dim objUsuario As New Usuario
        Dim parametros(0) As SqlParameter

        Dim _sql As String = "SELECT * FROM USU WHERE UsuCod=@Usucod"
        parametros(0) = New SqlParameter("@Usucod", UsuCod)
        dtUsuario = Database.Query(_sql, parametros)

        For Each linha As DataRow In dtUsuario.Rows
            objUsuario.UsuCod = CInt(linha("UsuCod"))
            objUsuario.UsuNom = CStr(linha("UsuNom"))
            objUsuario.UsuEml = CStr(linha("UsuEml"))

            If Not IsDBNull(linha("Avatar")) Then
                objUsuario.Avatar = CStr(linha("Avatar"))
            End If
            If Not IsDBNull(linha("DataDesativado")) Then
                objUsuario.DataDesativado = CDate(linha("DataDesativado"))
                objUsuario.UsuAti = False
            Else
                objUsuario.UsuAti = True
            End If
            objUsuario.DataCadastro = CDate(linha("DataCadastro"))
            objUsuario.UsuAdmin = CBool(linha("UsuAdmin"))
            If Not IsDBNull(linha("LastLogin")) Then
                objUsuario.LastLogin = CDate(linha("LastLogin"))
            End If
        Next

        Return objUsuario
    End Function

    Public Shared Function isUsernameAvailable(UsuEml As String) As Boolean
        Dim Database As New clsDatabase
        Dim parametros(0) As SqlParameter
        Dim qtdUsuarios As Integer

        Dim _sql As String = "SELECT count(*) FROM USU WHERE UsuEml=@UsuEml"
        parametros(0) = New SqlParameter("@UsuEml", UsuEml)
        qtdUsuarios = CInt(Database.ExecuteScalar(_sql, parametros))

        If qtdUsuarios > 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Retorna a lista com todos os usuários do sistema
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getUsuarios() As List(Of Usuario)
        Dim Database As New clsDatabase
        Dim dtUsuario As New DataTable
        Dim objUsuario As New Usuario
        Dim Usuarios As New List(Of Usuario)

        Dim _sql As String = "SELECT * FROM USU"
        dtUsuario = Database.Query(_sql)

        For Each linha As DataRow In dtUsuario.Rows
            Dim UsuCod As Integer = CInt(linha("UsuCod"))

            objUsuario = getUsuario(UsuCod)
            Usuarios.Add(objUsuario)
        Next

        Return Usuarios
    End Function

    ''' <summary>
    ''' Retorna a quantidade total de usuários cadastrados no sistema
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getQtdTotalUsuarios() As Integer
        Dim Database As New clsDatabase

        Dim _sql As String = "SELECT count(*) FROM USU"
        Return CInt(Database.ExecuteScalar(_sql))
    End Function

    ''' <summary>
    ''' Retorna próximo sequencial de usuário
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getUsuCod() As Integer
        Dim Database As New clsDatabase
        Dim _sql As String = "SELECT MAX(UsuCod) FROM USU"

        Try
            Return CInt(Database.ExecuteScalar(_sql)) + 1
        Catch ex As InvalidCastException
            Return 1
        End Try
    End Function

    ''' <summary>
    ''' Desativar um usuário do sistema
    ''' </summary>
    ''' <param name="UsuCod">Código do usuário a ser desativado</param>
    ''' <remarks></remarks>
    Public Shared Sub Desativar(UsuCod As Integer)
        Dim Database As New clsDatabase
        Dim parametros(1) As SqlParameter

        Dim _sql As String = "UPDATE USU SET DataDesativado=@DataAtual WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@DataAtual", Now)
        parametros(1) = New SqlParameter("@UsuCod", UsuCod)

        Database.Execute(_sql, parametros)
    End Sub

    Public Shared Sub setLastLogin(UsuCod As Integer)
        Dim Database As New clsDatabase
        Dim parametros(1) As SqlParameter

        Dim _sql As String = "UPDATE USU SET LastLogin=@LastLogin WHERE UsuCod=@UsuCod"
        parametros(0) = New SqlParameter("@LastLogin", Now)
        parametros(1) = New SqlParameter("@UsuCod", UsuCod)

        Database.Execute(_sql, parametros)
    End Sub
End Class
