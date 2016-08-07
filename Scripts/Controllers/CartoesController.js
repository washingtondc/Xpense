(function () {
    angular.module('Xpense').controller("CartoesController", CartoesController);

    function CartoesController($scope, $rootScope, $mdDialog, CartoesService, CartoesModel, $interval, ToastService, CartoesConstant) {
        $scope.Cartoes = [];
        $scope.CartoesConstant = CartoesConstant;
        $scope.orderBy = 'CarDes';
        $scope.orderReverse = false;

        $scope.loadCartoes = function () {
            $rootScope.Loading++;
            CartoesService.loadCartoes()
                .then(function (r) {
                    if (r.data.d != null && r.data.d.length > 0) {
                        CartoesModel.set(r.data.d);
                    }
                    $scope.Cartoes = CartoesModel.get();
                    $rootScope.Loading--;
                }).catch(function (r) {
                    console.log(r)
                });
        };

        $scope.deleteCartao = function (Cartao) {
            swal({
                title: "Atenção", text: "Deseja realmente excluir o cartão?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Sim",
                cancelButtonText: "Não"
            }, function (isConfirm) {
                if (isConfirm) {
                    $rootScope.Loading++;
                    CartoesService.excluirCartao(Cartao.CarCod)
                        .then(function () {
                            CartoesModel.remove(Cartao);
                            $rootScope.Loading--;
                            ToastService.successMessage('Cartão excluído.');
                        });
                }
            });
        }

        $scope.novoCartao = function () {
            var novoCartao = { CarBan: '', CarLim: 0 };
            OpenEditWindow(novoCartao);
        };

        $scope.editarCartao = function (Cartao) {
            CartoesService.getCartao(Cartao.CarCod).then(function (r) {
                OpenEditWindow(r.data.d);
            })
        };

        function OpenEditWindow(CartaoEditar) {
            $mdDialog.show({
                templateUrl: 'Views/Dialogs/Cartao.html',
                controller: 'CartaoDialogController',
                clickOutsideToClose: true,
                locals: {
                    Cartao: CartaoEditar
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
    }
})();