(function () {
    angular.module('Xpense').controller("ImportController", ImportController);

    function ImportController($scope, $rootScope, $mdDialog, $routeParams, ImportService, LancamentosService, CategoriasService, GetDateFromJSONService, ToastService, ContasService) {
        $scope.Lancamentos = [];
        $scope.Categorias = []; // Categorias do usuário
        $scope.Contas = [];
        $scope.ContaImportar = 0;
        $scope.FileName = $routeParams.FileName;
        $scope.orderBy = 'ConDes';
        $scope.orderReverse = false;

        $scope.selectFile = function () {
            $mdDialog.show({
                templateUrl: 'Views/Dialogs/ImportarOFX.html',
                controller: 'ImportDialogController',
                clickOutsideToClose: true,
                locals: {

                }
            });
        }

        $scope.invertSelection = function () {
            angular.forEach($scope.Lancamentos, function (item) {
                item.selected = !item.selected;
            });
        }

        // Importar os lançamentos selecionados
        $scope.importOFX = function () {
            var LancamentosSelecionados = [];
            var qtdSelecionados = 0;
            $rootScope.Loading++;

            angular.forEach($scope.Lancamentos, function (item) {
                if (item.selected) {
                    LancamentosSelecionados.push(item);
                    qtdSelecionados++;
                    // REMOVER OS IMPORTADOS DA TELA
                }
            });

            if (qtdSelecionados > 0) {
                ImportService.importarLancamentos(LancamentosSelecionados).then(function () {
                    $rootScope.Loading--;
                    ToastService.successMessage('Todos importados');
                });
            } else {
                ToastService.errorMessage('Nenhum lançamento selecionado para importar');
                $rootScope.Loading--;
            }
        }

        $scope.loadOFX = function () {
            $rootScope.Loading++;
            ImportService.loadOFX($scope.FileName).then(function (r) {
                $scope.ContaImportar = r.data.d[0].ConCod;

                console.log($scope.ContaImportar);
                // Correções iniciais 
                angular.forEach(r.data.d, function (item) {
                    item.selected = item.LanSeq===0;
                    item.FatDat = GetDateFromJSONService.getDateFromJSON(item.FatDat);
                    item.LanDat = GetDateFromJSONService.getDateFromJSON(item.LanDat);
                    item.LanVal = Math.abs(item.LanVal);
                    item.LanEft = true;

                    $scope.Lancamentos.push(item);
                });
                $rootScope.Loading--;
            });
        }

        $scope.setCategoria = function (CatCod) {
            $scope.Lancamentos.forEach(function (item) {
                if (item.CatCod === 0) {
                    item.CatCod = CatCod;
                }
            })
        }

        $scope.setOrder = function (campo) {
            if (campo === $scope.orderBy) {
                $scope.orderReverse = !$scope.orderReverse;
            } else {
                $scope.orderBy = campo;
                $scope.orderReverse = false;
            }
        }

        function getCategorias() {
            CategoriasService.getCategorias()
            .then(function (retorno) {
                $scope.Categorias = retorno.data.d;
            });
        }

        function getContas() {
            ContasService.loadContas().then(function (r) {
                $scope.Contas = r.data.d;
            });
        }

        // Carrega as categorias do usuário
        getCategorias();
        getContas();
    }
})();