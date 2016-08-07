(function () {
    angular.module("Xpense").controller("ConfigController", ConfigController);

    function ConfigController($scope, $rootScope, ConfigService, ToastService) {
        $scope.config = {};

        $scope.getUserConfig = function () {
            ConfigService.getUserConfig().then(function (r) {
                $scope.config = r.data.d;
            });
        }

        $scope.saveConfig = function () {
            ConfigService.saveConfig($scope.config).then(function (r) {
                if (r.data.d === true) {
                    ToastService.successMessage('Configurações salvas com sucesso');
                } else {
                    ToastService.errorMessage('Ocorreu um erro ao salvar as alterações');
                }

            });
        }       
    }
})();