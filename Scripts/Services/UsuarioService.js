(function () {
    angular.module("Xpense").service("UsuarioService", UsuarioService);

    function UsuarioService($http) {

        this.getLoggedUserData = function () {
            var Requisicao = {
                url: 'Webservices/wsUsuario.asmx/getLoggedUserData',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }

        this.Salvar = function (usuario) {
            var Requisicao = {
                url: 'Webservices/wsUsuario.asmx/SalvarUsuario',
                method: 'POST',
                data: { objUsuario: usuario }
            };

            return $http(Requisicao);
        };

        this.getUsuarios = function () {
            var Requisicao = {
                url: '../Webservices/wsUsuario.asmx/getUsuarios',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.isUsernameAvailable = function (username) {
            var Requisicao = {
                url: '../Webservices/wsUsuario.asmx/isUsernameAvailable',
                method: 'POST',
                data: {Username:username}
            };

            return $http(Requisicao);
        };

        this.RecuperarSenha = function (email) {
            var Requisicao = {
                url: '../Webservices/wsUsuario.asmx/RecuperarSenha',
                method: 'POST',
                data: { email: email }
            };

            return $http(Requisicao);
        }

        this.resetAccount = function () {
            var Requisicao = {
                url: 'Webservices/wsUsuario.asmx/resetAccount',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }
    }
})();