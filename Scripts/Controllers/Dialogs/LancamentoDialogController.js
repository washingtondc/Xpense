(function () {
    angular.module('Xpense')
    .controller('LancamentoDialogController', LancamentoDialogController);

    function LancamentoDialogController($scope, Lancamento, LancamentosService, ContasService, Categorias, TituloJanela, LancamentoNoCartao, LancamentosModel, $mdDialog, ToastService) {
        $scope.Lancamento = Lancamento;
        $scope.Categorias = Categorias; // Categorias do usuário
        $scope.Contas = [];
        $scope.Cartoes = [];
        $scope.TituloJanela = TituloJanela;
        $scope.LancamentoNoCartao = LancamentoNoCartao;

        if ($scope.Lancamento.LanQtdPar > 1 || $scope.Lancamento.LanFix) {
            $scope.LancamentoRepetir = true;
        } else {
            $scope.LancamentoRepetir = LancamentoNoCartao;
        }

        // Funções chamadas pelos botões da janela modal
        $scope.hide = function () {
            $mdDialog.hide();
        };
        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        $scope.Salvar = function () {
            if (typeof $scope.Lancamento.LanVal === 'string') {
                $scope.Lancamento.LanVal = parseFloat($scope.Lancamento.LanVal.replace(',', '.'));
            }

            LancamentosService.SalvarLancamento($scope.Lancamento)
            .then(function (retorno) {
                $mdDialog.hide();
                LancamentosModel.put(retorno.data.d);
                ToastService.successMessage('Dados gravados com sucesso.');
            });
        }

        var LoadContas = function () {
            ContasService.loadContas()
            .then(function (retorno) {
                $scope.Contas = retorno.data.d;
            });
        }

        var LoadCartoes = function () {
            LancamentosService.getCartoes()
            .then(function (retorno) {
                $scope.Cartoes = retorno.data.d;
            });
        }

        // Carregar as contas do usuário
        LoadContas();
        // Carregar os cartões do usuário
        LoadCartoes();
    }
})();