(function() {
    angular.module("XpenseAdmin").service("adminService", adminService);

    function adminService($http) {
        this.clearDB = function () {
            var Requisicao = {
                url: '../Webservices/wsAdmin.asmx/clearDB',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.getQtdTotalLancamentos = function () {
            var Requisicao = {
                url: '../Webservices/wsAdmin.asmx/getQtdTotalLancamentos',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.getQtdTotalCategorias = function () {
            var Requisicao = {
                url: '../Webservices/wsAdmin.asmx/getQtdTotalCategorias',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.getQtdTotalUsuarios = function () {
            var Requisicao = {
                url: '../Webservices/wsAdmin.asmx/getQtdTotalUsuarios',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.getQtdTotalContas = function () {
            var Requisicao = {
                url: '../Webservices/wsAdmin.asmx/getQtdTotalContas',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };
    }
})();