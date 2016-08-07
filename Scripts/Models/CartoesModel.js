(function () {
    angular.module('Xpense').factory('CartoesModel', CartoesModel);

    function CartoesModel(GetDateFromJSONService) {
        var CartoesModel = this;
        var Cartoes = [];

        CartoesModel.set = function (pCartoes) {
            Cartoes = pCartoes;
            // Transformar as datas do formato JSON para JS
            angular.forEach(Cartoes, function (item) {
                item.FaturaAtual.Data = GetDateFromJSONService.getDateFromJSON(item.FaturaAtual.Data);
                item.ProximaFatura.Data = GetDateFromJSONService.getDateFromJSON(item.ProximaFatura.Data);
            });
        }

        CartoesModel.get = function () {
            return Cartoes;
        }

        CartoesModel.put = function (CartaoSalvar) {
            var posicao = -1;
            // Busca se o cartao já existe no array
            Cartoes.forEach(function (elemento) {
                if (elemento.CarCod === CartaoSalvar.CarCod) {
                    posicao = Cartoes.indexOf(elemento);
                }
            })
            if (posicao >= 0) {
                Cartoes[posicao] = CartaoSalvar;
            } else {
                Cartoes.push(CartaoSalvar);
            }
        }

        CartoesModel.remove = function (Cartao) {
            var index = Cartoes.indexOf(Cartao);
            Cartoes.splice(index, 1);
        }

        CartoesModel.replace = function (Cartao) {
            for (var i = 0; i < Cartoes.length; i++) {
                if (Cartoes[i].CarCod == Cartao.CarCod) {
                    Cartoes[i] = Cartao;
                }
            }
        }
        return CartoesModel;
    }
})();