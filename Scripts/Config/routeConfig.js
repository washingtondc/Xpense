(function () {
    angular.module('Xpense').config(routeConfig);

    function routeConfig($routeProvider, $httpProvider) {
        $routeProvider

        .when('/', {
            templateUrl: 'Views/login.html',
            controller: 'LoginController'
        })

        .when('/Dashboard', {
            templateUrl: 'Views/Dashboard.html',
            controller: 'DashboardController'
        })

        .when('/CriarConta', {
            templateUrl: 'Views/CriarConta.html',
            controller: 'UsuarioController'
        })

        .when('/RecuperarSenha', {
            templateUrl: 'Views/RecuperarSenha.html',
            controller: 'UsuarioController'
        })

        .when('/Perfil', {
            templateUrl: 'Views/Perfil.html',
            controller: 'UsuarioController'
        })

        .when('/Config', {
            templateUrl: 'Views/Config.html',
            controller: 'ConfigController'
        })

        .when('/Categorias', {
            templateUrl: 'Views/Categorias.html',
            controller: 'CategoriasController'
        })

        .when('/Lancamentos/:CatCod?', {
            templateUrl: 'Views/Lancamentos.html',
            controller: 'LancamentosController'
        })

        .when('/LancamentosAlerta', {
            templateUrl: 'Views/LancamentosAlerta.html',
            controller: 'LancamentosController'
        })

        .when('/Lancamentos/:Status?/:LanTip?', {
            templateUrl: 'Views/Lancamentos.html',
            controller: 'LancamentosController'
        })

        .when('/Contas', {
            templateUrl: 'Views/Contas.html',
            controller: 'ContasController'
        })

        .when('/Cartoes', {
            templateUrl: 'Views/Cartoes.html',
            controller: 'CartoesController'
        })

        .when('/Anual', {
            templateUrl: 'Views/Anual.html',
            controller: 'LancamentosController'
        })

        .when('/Fatura/:CarCod/:AnoFatura/:MesFatura', {
            templateUrl: 'Views/Fatura.html',
            controller: 'FaturasController'
        })

        .when('/ImportarOFX/:FileName', {
            templateUrl: 'Views/ImportarOFX.html',
            controller: 'ImportController'
        })

        .otherwise({ redirectTo: '/' });

    }
})();
