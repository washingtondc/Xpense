(function () {
    angular.module('Xpense').factory('LancamentosModel', LancamentosModel);

    function LancamentosModel(GetDateFromJSONService) {
        var LancamentosModel = this;
        var Lancamentos = [];

        LancamentosModel.set = function (pLancamentos) {
            Lancamentos = pLancamentos;
            // Transformar as datas do formato JSON para JS
            angular.forEach(Lancamentos, function (item) {
                if (item.Parcela !== undefined) {
                    if (item.Parcela.LanDatPag !== null) {
                        item.LanDat = GetDateFromJSONService.getDateFromJSON(item.LanDat);
                        item.Parcela.LanDatPag = GetDateFromJSONService.getDateFromJSON(item.Parcela.LanDatPag);
                    }
                }
            });
        }

        LancamentosModel.get = function () {
            return Lancamentos;
        }

        LancamentosModel.put = function (Lancamento) {
            // Corrigir datas
            Lancamento.Parcela.LanDatPag = GetDateFromJSONService.getDateFromJSON(Lancamento.Parcela.LanDatPag);
            Lancamento.LanDat = GetDateFromJSONService.getDateFromJSON(Lancamento.LanDat);

            var posicao = -1;
            // Busca se o cartao já existe no array
            Lancamentos.forEach(function (elemento) {
                if (elemento.LanAno === Lancamento.LanAno && elemento.LanSeq === Lancamento.LanSeq) {
                    posicao = Lancamentos.indexOf(elemento);
                }
            })
            if (posicao >= 0) {
                Lancamentos[posicao] = Lancamento;
            } else {
                Lancamentos.push(Lancamento);
            }
        }

        LancamentosModel.remove = function (Lancamento) {
            var index = Lancamentos.indexOf(Lancamento);
            Lancamentos.splice(index, 1);
        }

        return LancamentosModel;
    }
})();