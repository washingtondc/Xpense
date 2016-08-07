(function () {
    angular.module('Xpense').controller('LoginController', LoginController);

    function LoginController($scope, LoginService, $cookies, $window, USER_INFO, ToastService) {
        $scope.senha = '';
        $scope.memorizar = false;
        $scope.login = $cookies.get('login');
        $scope.USER_INFO = USER_INFO.Get();

        if ($scope.login != undefined) {
            $scope.memorizar = true;
        }

        $scope.logoff = function () {
            LoginService.logoff().then(function (r) {
                if (r.data.d === true) {
                    $window.location.href = "index.html";
                }
            });
        }

        $scope.efetuarLogin = function (login, senha, memorizar) {
            LoginService.Login(login, senha, memorizar)
                .then(function (retorno) {
                    if (retorno.data.d !== null) {
                        objUsuario = retorno.data.d;

                        USER_INFO.Save(objUsuario);

                        window.location.href = 'App.html#/Dashboard';
                    } else {
                        ToastService.errorMessage('Login ou senha inválidos');
                        $scope.senha = '';
                    }
                }, function (e) {
                    console.log(JSON.stringify(e));
                    ToastService.errorMessage('Erro na autenticação');
                });
        }
    }
})();