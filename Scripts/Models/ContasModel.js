(function () {
    angular.module('Xpense').factory('ContasModel', ContasModel);

    function ContasModel() {
        var ContasModel = this;
        var Contas = [];

        ContasModel.set = function (pContas) {
            Contas = pContas;
        }

        ContasModel.get = function () {
            return Contas;
        }

        ContasModel.put = function (Conta) {
            var posicao = -1;
            // Busca se o cartao já existe no array
            Contas.forEach(function (elemento) {
                if (elemento.ConCod === Conta.ConCod) {
                    posicao = Contas.indexOf(elemento);
                }
            })
            if (posicao >= 0) {
                Contas[posicao] = Conta;
            } else {
                Contas.push(Conta);
            }
        }

        ContasModel.remove = function (Conta) {
            var index = Contas.indexOf(Conta);
            Contas.splice(index, 1);
        }

        return ContasModel;
    }
})();