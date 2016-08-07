(function () {
    angular.module("Xpense")
    .controller("CategoriaDialogController", CategoriaDialogController);

    function CategoriaDialogController($scope, $mdDialog, Categoria, CategoriasService, CategoriasModel,ToastService) {
        $scope.Categoria = angular.copy(Categoria);
        $scope.Operacao = 'E';
        $scope.CategoriaMover = 0;
        $scope.Categorias = [];

        // Funções chamadas pelos botões da janela modal
        $scope.hide = function () {
            $mdDialog.hide();
        };

        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        $scope.Salvar = function () {
            if (typeof $scope.Categoria.CatOrc === 'string') {
                $scope.Categoria.CatOrc = parseFloat($scope.Categoria.CatOrc.replace(",", "."));
            }

            CategoriasService.SalvarCategoria($scope.Categoria)
            .then(function (Retorno) {
                CategoriasModel.put(Retorno.data.d);
                ToastService.successMessage('Dados gravados com sucesso.');
                $mdDialog.hide();
            });
        }

        $scope.excluirCategoria = function () {
            $mdDialog.cancel();
            CategoriasService.excluirCategoria($scope.Categoria.CatCod,$scope.CategoriaMover)
                .then(function (r) {
                    CategoriasModel.remove($scope.Categoria);
                    ToastService.successMessage('Registro excluído com sucesso.');
                });
        }

        function getCategorias() {
            CategoriasService.getCategorias()
                .then(function (r) {
                    if (r.data.d != null && r.data.d.length > 0) {
                        $scope.Categorias = r.data.d;
                    }
                });
        };

        // Carrega as categorias do usuário
        getCategorias(Categoria.CatTip);
    }
})();