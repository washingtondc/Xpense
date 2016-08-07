(function() {
    angular.module('XpenseAdmin').controller('AdminDashboardController', AdminDashboardController);

    function AdminDashboardController($scope, $rootScope, adminService) {
        $scope.cifrao = 'R$';
        $scope.qtdTotalLancamentos = 0;
        $scope.qtdTotalContas = 0;
        $scope.qtdTotalUsuarios = 0;
        $scope.qtdTotalCategorias = 0;


        $scope.init = function () {
            getQtdTotalLancamentos();
            getQtdTotalCategorias();
            getQtdTotalUsuarios();
            getQtdTotalContas();
        }

        $scope.clearDB = function () {
            adminService.clearDB().then(function () {
                ToastService.successMessage('Procedimento executado com sucesso.');
            });
        }

        function getQtdTotalLancamentos() {
            adminService.getQtdTotalLancamentos().then(function (r) {
                $scope.qtdTotalLancamentos = r.data.d;
            });
        }

        function getQtdTotalCategorias() {
            adminService.getQtdTotalCategorias().then(function (r) {
                $scope.qtdTotalCategorias = r.data.d;
            });
        }

        function getQtdTotalUsuarios() {
            adminService.getQtdTotalUsuarios().then(function (r) {
                $scope.qtdTotalUsuarios = r.data.d;
            });
        }

        function getQtdTotalContas() {
            adminService.getQtdTotalContas().then(function (r) {
                $scope.qtdTotalContas = r.data.d;
            });
        }
    }
})();