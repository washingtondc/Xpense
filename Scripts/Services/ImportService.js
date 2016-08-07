(function () {
    angular.module("Xpense").service("ImportService", ImportService);

    function ImportService($http) {
        this.loadOFX = function (arquivo) {
            var Requisicao = {
                url: 'Webservices/wsImportar.asmx/loadOFX',
                method: 'POST',
                data: { fileName: arquivo }
            };

            return $http(Requisicao);
        }

        this.importarLancamentos = function (Lancamentos) {
            var Requisicao = {
                url: 'Webservices/wsImportar.asmx/ImportarLancamentos',
                method: 'POST',
                data: { Lancamentos: Lancamentos }
            };

            return $http(Requisicao);
        }
    }
})();