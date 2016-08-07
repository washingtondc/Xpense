(function () {
    angular.module('Xpense').controller('DashboardController', DashboardController);

    function DashboardController($scope, $rootScope, $q, $mdSidenav, DashboardService, LancamentosService) {
        $scope.mes = new Date().getMonth() + 1;
        $scope.ano = new Date().getFullYear();
        $scope.Cartoes = [];
        $scope.SaldoAtual = 0;
        $scope.ValorPagar = 0;
        $scope.ValorReceber = 0;
        $scope.SaldoPrevisto = 0;
        $scope.cifrao = 'R$';

        $scope.init = function () {
            $rootScope.Loading++;
            // Carregar os valores do mês
            $q.all([
                DashboardService.getSaldoAtual($scope.ano, $scope.mes)
                .then(function (retorno) {
                    $scope.SaldoAtual = retorno.data.d;
                }),

                DashboardService.getValorPagar($scope.ano, $scope.mes)
                .then(function (retorno) {
                    $scope.ValorPagar = retorno.data.d;
                }),

                DashboardService.getValorReceber($scope.ano, $scope.mes)
                .then(function (retorno) {
                    $scope.ValorReceber = retorno.data.d;
                })
            ]).then(function () {
                $scope.SaldoPrevisto = $scope.SaldoAtual + $scope.ValorReceber - $scope.ValorPagar;
                $rootScope.Loading--;
            });

            GeraGraficoGastosCategoria();
            GeraGraficoSaldoMes();
            $scope.initCalendar();
        }

        $scope.initCalendar = function () {
            $rootScope.Loading++;
            LancamentosService.loadLancamentos($scope.mes, $scope.ano, 0, '', '').then(function (retorno) {
                $('#calendar').fullCalendar({
                    header: false,
                    events: function (start, end, timezone, callback) {
                        var events = [];
                        retorno.data.d.forEach(function (elemento) {
                            events.push({
                                title: elemento.LanDes,
                                start: elemento.LanDat,
                                backgroundColor: elemento.backgroundColor
                            });
                        });

                        callback(events);
                    }
                });
                $rootScope.Loading--;
            }).catch(function (error) {
                $rootScope.Loading--;
                console.log(JSON.stringify(error));
            });
        }

        function GeraGraficoGastosCategoria() {
            $rootScope.Loading++;
            DashboardService.getGastosCategoria($scope.ano, $scope.mes).then(function (r) {
                Result = r.data.d;
                var data = [];
                for (var i in Result) {
                    var obj = Result[i];
                    var serie = { name: obj.Categoria, y: obj.Gasto, CatCod: obj.CatCod };
                    data.push(serie);
                }

                DrawGraficoGastosCategoria(data);
                $rootScope.Loading--;
            });
        }

        function GeraGraficoSaldoMes() {
            $rootScope.Loading++;
            DashboardService.getSaldoPorMes($scope.ano).then(function (r) {
                Result = r.data.d;
                var data = [];

                $.each(Result, function (t) {
                    var obj = Result[t];
                    var serie = {};

                    serie.name = obj.Conta;
                    serie.data = [];
                    for (x = 1; x <= 12; x++) {
                        serie.data.push(obj.saldo[x]);
                    }
                    data.push(serie);
                });

                DrawGraficoSaldoMes(data);
                $rootScope.Loading--;
            });
        }

        $scope.toggleSidenav = function () {
            $mdSidenav('sideNav').toggle();

        }
    }
})();