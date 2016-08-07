(function () {
    angular.module("Xpense")
        .controller("CartaoDialogController", CartaoDialogController);

    function CartaoDialogController($scope, $mdDialog, Cartao, CartoesModel, CartoesService, ToastService, CartoesConstant) {
        $scope.Cartao = angular.copy(Cartao);
        $scope.CartoesConstant = CartoesConstant;

        // Funções chamadas pelos botões da janela modal
        $scope.hide = function () {
            $mdDialog.hide();
        };
        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        // Salvar os dados do cartão
        $scope.Salvar = function () {
            if ($scope.Cartao.CarLim) {
                limite = $scope.Cartao.CarLim;
                if (typeof limite === 'string') {
                    $scope.Cartao.CarLim = limite.replace('.', '').replace(',', '.');
                }
            }

            // Corrige as datas das faturas se tiver
            if ($scope.Cartao.FaturaAtual !== undefined) {
                $scope.Cartao.FaturaAtual.Data = GetDateFromJSONService.getDateFromJSON($scope.Cartao.FaturaAtual.Data);
                $scope.Cartao.ProximaFatura.Data = GetDateFromJSONService.getDateFromJSON($scope.Cartao.ProximaFatura.Data);
            }
            
            CartoesService.SalvarCartao($scope.Cartao)
            .then(function (retorno) {
                if (retorno.data.d !== null) {
                    CartoesModel.put(retorno.data.d);
                    ToastService.successMessage('Dados gravados com sucesso.');
                    $mdDialog.hide();
                }
            });
        };
    }
})();