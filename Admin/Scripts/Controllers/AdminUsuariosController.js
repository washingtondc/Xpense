(function() {
    angular.module('XpenseAdmin').controller('AdminUsuariosController', AdminUsuariosController);

    function AdminUsuariosController($scope, UsuarioService) {
        $scope.Usuarios = [];

        $scope.init = function () {
            UsuarioService.getUsuarios().then(function (r) {
                $scope.Usuarios = r.data.d;
            })
        }
    }
})();