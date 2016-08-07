(function () {
    angular.module('Xpense')
    .controller('ContasDialogController', ContasDialogController);

    function ContasDialogController($scope, $mdDialog, Conta, ContasService, ContasModel, ToastService) {
        var mesAtual = new Date().getMonth();
        $scope.Conta = angular.copy(Conta);
        $scope.Bancos = [];
        $scope.Transferencia = { ContaOrigem: 0, ContaDestino: 0, Valor: 0 };
        $scope.Saldo = { Ano: new Date().getUTCFullYear(), Mes: new Date().getUTCMonth() + 1, Valor: $scope.Conta.ConSal };
        
        $scope.hide = function () {
            $mdDialog.hide();
        };
        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        $scope.Salvar = function () {
            if (typeof $scope.Conta.ConSal === 'string') {
                $scope.Conta.ConSal = parseFloat($scope.Conta.ConSal.replace(',', '.'));
            }

            ContasService.Salvar($scope.Conta)
            .then(function (retorno) {
                ContasModel.put(retorno.data.d);
                ToastService.successMessage('Dados gravados com sucesso.');
                $mdDialog.hide();
            });
        }

        $scope.GravarSaldo = function () {
            if (typeof ($scope.Saldo.Valor) === 'string') {
                $scope.Saldo.Valor = parseFloat($scope.Saldo.Valor.replace(",", "."));
            }

            $scope.Saldo.Mes = new Date().getMonth() + 1;

            ContasService.GravarSaldo($scope.Conta.ConCod, $scope.Saldo.Ano, $scope.Saldo.Mes, $scope.Saldo.Valor)
            .then(function (r) {
                $scope.loadContas();
                ToastService.successMessage('Saldo atualizado.');
                $mdDialog.hide();
            });
        }

        $scope.EfetuarTransferencia = function () {
            var valorTransferir = parseFloat($scope.Transferencia.Valor.replace(",", "."));
            ContasService.Transferir($scope.Transferencia.ContaOrigem, $scope.Transferencia.ContaDestino, valorTransferir)
            .then(function (retorno) {
                $scope.loadContas();
                ToastService.successMessage('Transferência efetuada com sucesso.');
                $mdDialog.hide();
            });
        }

        function getBancos() {
            ContasService.getBancos().then(function (r) {
                if (r.data.d) {
                    $scope.Bancos = r.data.d;
                }
            });
        }

        // Carrega a lista de bancos
        getBancos();
    }
})();