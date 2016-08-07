(function () {
    angular.module('XpenseAdmin', ['ngMaterial', 'ngMessages', 'focus-if', 'ngRoute', 'ngCookies', 'Xpense', 'Core'])
    .run(function ($rootScope) {
        $rootScope.Loading--;
        $rootScope.MainMenu = [
            { Name: 'Dashboard', Icon: 'dashboard', URL: 'Dashboard' },
            { Name: 'Usuários', Icon: 'event_note', URL: 'Usuarios' }
        ]
    })
})();

//angular.module('XpenseAdmin').config(['$compileProvider', function ($compileProvider) {
//    $compileProvider.debugInfoEnabled(false);
//}]);

