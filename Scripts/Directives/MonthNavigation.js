(function () {
    angular.module('Xpense').directive('monthNavigation', monthNavigation);

    function monthNavigation() {
        return {
            restrict: 'E',
            replace: 'true',
            controller: function ($scope) {
                $scope.meses = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
                $scope.navegaProximo = function () {
                    $scope.mes++;
                    if ($scope.mes > 12) {
                        $scope.mes = 1;
                        $scope.ano++;
                    }
                    $scope.init();
                }

                $scope.navegaAnterior = function () {
                    $scope.mes--;
                    if ($scope.mes < 1) {
                        $scope.mes = 12;
                        $scope.ano--;
                    }
                    $scope.init();
                }

                $scope.navega = function (mes) {
                    $scope.mes = mes;
                    $scope.init();
                }
            },
            templateUrl: '/scripts/directives/MonthNavigation.html'

        };

    }
})();