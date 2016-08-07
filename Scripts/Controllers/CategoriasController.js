(function () {
    angular.module("Xpense").controller("CategoriasController", CategoriasController);

    function CategoriasController($scope, $rootScope, $mdDialog, CategoriasService, CategoriasModel) {
        $scope.Categorias = CategoriasModel.get();
        $scope.orderBy = 'CatNom';
        $scope.orderReverse = false;

        function getCategorias() {
            $rootScope.Loading++;
            CategoriasService.getCategorias()
                .then(function (r) {
                    if (r.data.d != null && r.data.d.length > 0) {
                        CategoriasModel.set(r.data.d);
                        $scope.Categorias = CategoriasModel.get();
                    }
                    $rootScope.Loading--;
                });
        };

        $scope.novaCategoria = function () {
            OpenEditWindow({ CatTip: 'S', CatDes: '', CatOrc: 0, CatCor: '' });
        };

        $scope.editarCategoria = function (Categoria) {
            OpenEditWindow(Categoria);
        };

        $scope.excluirCategoria = function (Categoria, $event) {
            $mdDialog.show({
                targetEvent: $event,
                templateUrl: 'Views/Dialogs/ExcluirCategoria.html',
                controller: 'CategoriaDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o scope do controler
                    Categoria: Categoria
                }
            });
        };

        function OpenEditWindow(CategoriaEditar) {
            $mdDialog.show({
                templateUrl: 'Views/Dialogs/Categoria.html',
                controller: 'CategoriaDialogController',
                clickOutsideToClose: true,
                locals: {
                    Categoria: CategoriaEditar
                }
            });
        }

        $scope.setOrder = function (campo) {
            if (campo === $scope.orderBy) {
                $scope.orderReverse = !$scope.orderReverse;
            } else {
                $scope.orderBy = campo;
                $scope.orderReverse = false;
            }
        }

        getCategorias('');
    }
})();