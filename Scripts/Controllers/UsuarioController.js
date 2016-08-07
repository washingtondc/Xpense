(function () {
    angular.module('Xpense').controller('UsuarioController', UsuarioController);

    function UsuarioController($scope, UsuarioService, $location, ToastService, GetDateFromJSONService, fileUpload, USER_INFO, EmailService) {
        $scope.usuario = {};
        $scope.myFile = undefined;
        $scope.EmailRecuperarSenha = '';

        $scope.SelectFile = function () {
            angular.element('#AvatarFileInput').trigger('click');
        };

        $scope.getLoggedUserData = function () {
            UsuarioService.getLoggedUserData().then(function (r) {
                $scope.usuario = r.data.d;
            });
        }

        $scope.Salvar = function () {
            var file = $scope.myFile;

            if (file !== undefined) {
                fileUpload.uploadFileToUrl(file, "../Images/Avatar/").then(function (r) {
                    $scope.usuario.Avatar = $scope.usuario.UsuCod + file.name;
                    SalvarDados();
                });
            } else {
                SalvarDados();
            }
        }

        /*
        Função usada para criar uma nova conta
        */
        function CriarConta() {
            UsuarioService.Salvar($scope.usuario).then(function (r) {
                if (r.data.d === true) {
                    USER_INFO.Save($scope.usuario);
                    $scope.usuario = {};
                    // Redireciona para o aplicativo
                    window.location.href = "App.html#/Dashboard";
                } else {
                    ToastService.errorMessage('Falha no cadastro.');
                }
            })
        }

        /*
        Função usada para atualizar os dados do usuário
        */
        function SalvarDados() {
            $scope.usuario.DataCadastro = GetDateFromJSONService.getDateFromJSON($scope.usuario.DataCadastro);
            $scope.usuario.DataDesativado = GetDateFromJSONService.getDateFromJSON($scope.usuario.DataDesativado);
            $scope.usuario.LastLogin = GetDateFromJSONService.getDateFromJSON($scope.usuario.LastLogin);

            UsuarioService.Salvar($scope.usuario).then(function (r) {
                if (r.data.d === true) {
                    USER_INFO.Save($scope.usuario);
                    USER_INFO.Get();
                    ToastService.successMessage('Dados atualizados com sucesso.');
                } else {
                    ToastService.errorMessage('Falha ao gravar os dados.');
                }
            })
        }

        // Enviar email para usuário resetar a senha
        $scope.RecuperarSenha = function () {
            UsuarioService.RecuperarSenha($scope.EmailRecuperarSenha).then(function () {
                ToastService.successMessage('Email enviado');
            });
        }

        $scope.resetAccount = function () {
            swal({
                title: "Atenção", text: "Deseja resetar sua conta?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Sim",
                cancelButtonText: "Não"
            }, function (isConfirm) {
                if (isConfirm) {
                    UsuarioService.resetAccount().then(function (r) {
                        ToastService.successMessage('Seus dados foram resetados com sucesso');
                    });
                }
            });
        }
    }
})();