(function () {
    angular.module("Xpense").service("ContasService", ContasService);

    function ContasService($http) {
        this.loadContas = function () {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/getContasUsuario',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.excluirConta = function (ConCod) {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/excluirConta',
                method: 'POST',
                data: { ConCod: ConCod }
            };

            return $http(Requisicao);
        };

        this.Salvar = function (Conta) {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/SalvarConta',
                method: 'POST',
                data: { Conta: Conta }
            };

            return $http(Requisicao);
        };

        this.Transferir = function (ContaOrigem, ContaDestino, Valor) {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/fazerTransferencia',
                method: 'POST',
                data: { ContaOrigem: ContaOrigem, ContaDestino: ContaDestino, Valor: Valor }
            };

            return $http(Requisicao);
        };

        this.GravarSaldo = function (ConCod, ano, mes, valor) {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/GravarSaldo',
                method: 'POST',
                data: { ConCod: ConCod, ano: ano, mes: mes, valor: valor }
            };

            return $http(Requisicao);
        }

        this.getBancos = function () {
            var Requisicao = {
                url: 'Webservices/wsContas.asmx/getBancos',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }
    }
})();