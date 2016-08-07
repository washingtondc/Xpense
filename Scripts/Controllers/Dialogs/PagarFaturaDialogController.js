(function () {
    angular.module('Xpense')
    .controller('PagarFaturaDialogController', PagarFaturaDialogController);

    function PagarFaturaDialogController($scope, valor, CarCod, ano, mes, FaturasService, ContasService, $mdDialog) {
        $scope.Contas = [];
        $scope.ConCod = 0;
        $scope.CarCod = CarCod;
        $scope.ano = ano;
        $scope.mes = mes;
        $scope.ValorFatura = valor;

        var LoadContas = function () {
            ContasService.loadContas()
            .then(function (retorno) {
                $scope.Contas = retorno.data.d;
            });
        }

        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        // Carregar as contas do usuário
        LoadContas();

        $scope.EfetuarPagamento = function () {
            if (typeof $scope.ValorFatura === 'string') {
                $scope.ValorFatura = parseFloat($scope.ValorFatura.replace(',', '.'));
            }

            FaturasService.pagarFatura($scope.CarCod, $scope.ano, $scope.mes, $scope.ConCod, $scope.ValorFatura)
                .then(function () {
                    ToastService.successMessage('Fatura paga.');
                    $mdDialog.cancel();
                });
        }
    }
})();