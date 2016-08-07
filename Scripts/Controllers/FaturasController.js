(function () {
    angular.module('Xpense')
        .controller("FaturasController", FaturasController);

    function FaturasController($scope, $rootScope, $mdDialog, FaturasService, LancamentosService, $routeParams) {
        $scope.Lancamentos = [];
        $scope.CarCod = $routeParams.CarCod;
        $scope.ano = $routeParams.AnoFatura;
        $scope.mes = $routeParams.MesFatura;
        $scope.ValorFatura = 0;
        $scope.ValorPendente = 0;
        $scope.valorTotalFatura = 0;

        $scope.init = function () {
            // LER LANÇAMENTOS ATUAIS
            $rootScope.Loading++;
            FaturasService.loadLancamentos($scope.CarCod, $scope.ano, $scope.mes)
            .then(function (retorno) {
                $scope.Lancamentos = retorno.data.d;

                calculaValorFatura();

                // LER VALOR PENDENTE
                FaturasService.getValorPendente($scope.CarCod, $scope.ano, $scope.mes)
                .then(function (retorno) {
                    $scope.ValorPendente = retorno.data.d;

                    $scope.valorTotalFatura = $scope.ValorFatura + $scope.ValorPendente;


                    $scope.pagarFatura = function (CarCod, Mes, Ano, e) {
                        $mdDialog.show({
                            templateUrl: 'Views/Dialogs/PagarFatura.html',
                            controller: 'PagarFaturaDialogController',
                            clickOutsideToClose: true,
                            locals: { // Envia valores para o scope do controler
                                valor: $scope.valorTotalFatura,
                                CarCod: $scope.CarCod,
                                ano: $scope.ano,
                                mes: $scope.mes
                            }
                        });
                        e.stopPropagation();
                    }
                });

                $rootScope.Loading--;

            }, function (err) {
                console.log('Fatura controller:' + JSON.stringify(err));
                $rootScope.Loading--;
            });
        }


        function calculaValorFatura() {
            var total = 0;

            if ($scope.Lancamentos.length) {
                for (x = 0; x < $scope.Lancamentos.length; x++) {

                    total += $scope.Lancamentos[x].LanVal;
                }
                $scope.ValorFatura = total;
            }
        }

    }
})();