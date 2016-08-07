(function () {
    angular.module('XpenseAdmin').config(routeConfig);

    function routeConfig($routeProvider, $httpProvider) {
        $routeProvider

        .when('/', {
            templateUrl: 'Views/Dashboard.html',
            controller: 'AdminDashboardController'
        })

        .when('/Dashboard', {
            templateUrl: 'Views/Dashboard.html',
            controller: 'AdminDashboardController'
        })

        .when('/Usuarios', {
            templateUrl: 'Views/Usuarios.html',
            controller: 'AdminUsuariosController'
        })

        .otherwise({ redirectTo: '/' });

    }
})();