(function () {
    angular.module("Xpense").controller("ContasController", ContasController);

    function ContasController($scope, $rootScope, $mdDialog, ContasService, ContasModel, ToastService) {
        $scope.Contas = [];
        $scope.SaldoTotal;
        $scope.orderBy = 'ConDes';
        $scope.orderReverse = false;

        $scope.loadContas = function () {
            $rootScope.Loading++;
            ContasService.loadContas()
            .then(function (r) {
                if (r.data.d != null && r.data.d.length > 0) {
                    ContasModel.set(r.data.d);
                    $scope.Contas = ContasModel.get();
                    calculaSaldo();
                }
                $rootScope.Loading--;
            });
        }

        // Soma o saldo das contas
        function calculaSaldo() {
            var total = 0;
            for (x = 0; x < $scope.Contas.length; x++) {
                total += $scope.Contas[x].ConSal;
            }
            $scope.SaldoTotal = total;
        }

        $scope.deleteConta = function (Conta) {
            swal({
                title: "Atenção", text: "Todos os lançamentos dessa conta serão excluídos também. Deseja realmente excluir a conta?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Sim",
                cancelButtonText: "Não"
            }, function (isConfirm) {
                if (isConfirm) {
                    $rootScope.Loading++;
                    ContasService.excluirConta(Conta.ConCod)
                    .then(function (r) {

                        ContasModel.remove(Conta);
                        calculaSaldo();
                        $rootScope.Loading--;
                        ToastService.successMessage("Conta excluída com sucesso");
                    }, function () {
                        alert('Ocorreu um erro.');
                        $rootScope.Loading--;
                    });
                }
            });
        }

        $scope.transferir = function () {
            $mdDialog.show({
                templateUrl: 'Views/Dialogs/Transferencia.html',
                scope: $scope,
                preserveScope: true,
                controller: 'ContasDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o scope do controller
                    Conta: {}
                }
            });
        }

        $scope.criarConta = function () {
            OpenEditWindow({ ConSal: 0 });
        }

        $scope.editarConta = function (Conta) {
            OpenEditWindow(Conta);
        }

        $scope.ajustarSaldo = function (Conta, event) {
            $mdDialog.show({
                targetEvent: event,
                templateUrl: 'Views/Dialogs/AjustarSaldo.html',
                scope: $scope,
                preserveScope: true,
                controller: 'ContasDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o scope do controler
                    Conta: Conta
                }
            });
        }

        function OpenEditWindow(ContaEditar) {
            $mdDialog.show({
                templateUrl: 'Views/Dialogs/Conta.html',
                controller: 'ContasDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o controller do dialog
                    Conta: ContaEditar
                }
            }).then(function () {
                calculaSaldo();
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