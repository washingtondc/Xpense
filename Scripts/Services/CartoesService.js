(function () {
    angular.module("Xpense").service("CartoesService", CartoesService);

    function CartoesService($http) {
        this.loadCartoes = function () {
            var Requisicao = {
                url: 'Webservices/wsCartoes.asmx/getCartoesUsuario',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.getCartao = function (CarCod) {
            var Requisicao = {
                url: 'Webservices/wsCartoes.asmx/getCartao',
                method: 'POST',
                data: { CarCod: CarCod }
            };

            return $http(Requisicao);
        }

        this.SalvarCartao = function (Cartao) {
            var Requisicao = {
                url: 'Webservices/wsCartoes.asmx/SalvarCartao',
                method: 'POST',
                data: { objCartao: Cartao }
            };

            return $http(Requisicao);
        };

        this.excluirCartao = function (CarCod) {
            var Requisicao = {
                url: 'Webservices/wsCartoes.asmx/excluirCartao',
                method: 'POST',
                data: { CarCod: CarCod }
            };

            return $http(Requisicao);
        };
    }
})();