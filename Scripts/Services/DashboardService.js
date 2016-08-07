(function () {
    angular.module('Xpense').service('DashboardService', DashboardService);

    function DashboardService($http) {
        this.getSaldoAtual = function (ano, mes) {
            var Requisicao = {
                url: 'Webservices/wsDashboard.asmx/getSaldoMes',
                method: 'POST',
                data: "{mes:" + mes + ",ano:" + ano + "}"
            };

            return $http(Requisicao);
        };

        this.getValorPagar = function (ano, mes) {
            var Requisicao = {
                url: 'Webservices/wsDashboard.asmx/getValorPagar',
                method: 'POST',
                data: "{mes:" + mes + ",ano:" + ano + "}"
            };

            return $http(Requisicao);
        };

        this.getValorReceber = function (ano, mes) {
            var Requisicao = {
                url: 'Webservices/wsDashboard.asmx/getValorReceber',
                method: 'POST',
                data: "{mes:" + mes + ",ano:" + ano + "}"
            };

            return $http(Requisicao);
        };

        this.getGastosCategoria = function (ano, mes) {
            var Requisicao = {
                url: 'Webservices/wsGrafico.asmx/gastosPorCategoria',
                method: 'POST',
                data: "{mes:" + mes + ",ano:" + ano + "}"
            };

            return $http(Requisicao);
        };

        this.getSaldoPorMes = function (ano) {
            var Requisicao = {
                url: 'Webservices/wsGrafico.asmx/SaldoPorMes',
                method: 'POST',
                data: "{ano:" + ano + "}"
            };

            return $http(Requisicao);
        };
    }
})();