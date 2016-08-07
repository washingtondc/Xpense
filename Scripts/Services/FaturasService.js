(function () {
    angular.module("Xpense").service("FaturasService", FaturasService);

    function FaturasService($http) {
        this.loadLancamentos = function (CarCod, Ano, Mes) {
            var Requisicao = {
                url: 'Webservices/wsFatura.asmx/getLancamentosFatura',
                method: 'POST',
                data: { CarCod: CarCod, AnoFatura: Ano, MesFatura: Mes }
            };

            return $http(Requisicao);
        };

        this.getValorPendente = function (CarCod, Ano, Mes) {
            var Requisicao = {
                url: 'Webservices/wsFatura.asmx/getValorPendente',
                method: 'POST',
                data: { CarCod: CarCod, AnoFatura: Ano, MesFatura: Mes }
            };

            return $http(Requisicao);
        };

        this.pagarFatura = function (CarCod, AnoFatura, MesFatura, ConCod, ValorPagar) {
            var Requisicao = {
                url: 'Webservices/wsFatura.asmx/pagarFatura',
                method: 'POST',
                data: { CarCod: CarCod, AnoFatura: AnoFatura, MesFatura: MesFatura, ConCod: ConCod, ValorPagar: ValorPagar }
            };

            return $http(Requisicao);
        }
    }
})();