(function () {
    angular.module('Xpense').service('LoginService', LoginService);

    function LoginService($http) {
        this.Login = function (login, senha, memorizar) {
            var Requisicao = {
                url: 'Webservices/wsLogin.asmx/Login',
                method: 'POST',
                data: { email: login, senha: senha, memorizar: memorizar }
            };

            return $http(Requisicao);
        };

        this.logoff = function () {
            var Requisicao = {
                url: 'Webservices/wsLogin.asmx/Logoff',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }
    }
})();