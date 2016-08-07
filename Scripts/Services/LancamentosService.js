(function () {
    angular.module('Xpense').service('LancamentosService', LancamentosService);

    function LancamentosService($http,GetDateFromJSONService) {

        this.loadLancamentos = function (Mes, Ano, CatCod, LanTip, Status) {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/getLancamentosMes',
                method: 'POST',
                data: { mes: Mes, ano: Ano, CatCod: CatCod, LanTip: LanTip, status: Status }
            };

            return $http(Requisicao);
        };

        this.getLancamento = function (LanAno, LanSeq) {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/getLancamento',
                method: 'POST',
                data: { LanAno: LanAno, LanSeq: LanSeq }
            };

            return $http(Requisicao);
        }

        this.getCartoes = function () {
            var Requisicao = {
                url: 'Webservices/wsCartoes.asmx/getCartoesUsuario',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.SalvarLancamento = function (Lancamento) {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/salvarLancamento',
                method: 'POST',
                data: { Lancamento: Lancamento, Efetivado: Lancamento.LanEft }
            };

            return $http(Requisicao);
        };

        this.reabrirLancamento = function (Lancamento) {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/reabrirLancamento',
                method: 'POST',
                data: { LanAno: Lancamento.LanAno, LanSeq: Lancamento.LanSeq, dataPagamento: Lancamento.Parcela.LanDatPag }
            };

            return $http(Requisicao);
        };

        this.excluirParcela = function (Parcela) {
            Parcela.LanEftDat = GetDateFromJSONService.getDateFromJSON(Parcela.LanEftDat);
            Parcela.LanDatPag = GetDateFromJSONService.getDateFromJSON(Parcela.LanDatPag);

            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/excluirParcela',
                method: 'POST',
                data: { objParcela: Parcela }
            };

            return $http(Requisicao);
        };

        this.excluirLancamento = function (Lancamento) {
            Lancamento.LanDat = GetDateFromJSONService.getDateFromJSON(Lancamento.LanDat);
            Lancamento.Parcela.LanDatPag = GetDateFromJSONService.getDateFromJSON(Lancamento.Parcela.LanDatPag);
            Lancamento.Parcela.LanEftDat = GetDateFromJSONService.getDateFromJSON(Lancamento.Parcela.LanEftDat);

            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/excluirLancamento',
                method: 'POST',
                data: { objLancamento: Lancamento }
            };

            return $http(Requisicao);
        };

        this.EfetivarLancamento = function (LanAno, LanSeq, dataPagamento, valor, movimentarSaldo) {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/efetivarLancamento',
                method: 'POST',
                data: { LanAno: LanAno, LanSeq: LanSeq, dataPagamento: dataPagamento, valor: valor, movimentarSaldo: movimentarSaldo }
            };

            return $http(Requisicao);
        };

        this.getResumoAnual = function () {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/getResumoAnual',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }

        this.getAlertas = function () {
            var Requisicao = {
                url: 'Webservices/wsLancamentos.asmx/getAlertasDia',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };
    }
})();