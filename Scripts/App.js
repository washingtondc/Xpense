
angular.module('Xpense', ['ngMaterial', 'ngMessages', 'focus-if', 'ngRoute', 'ngCookies', 'Core', 'ngAnimate'])
.run(function ($rootScope) {
    $rootScope.Loading = 0;

    $rootScope.MonthNames = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
});


angular.module('Xpense').config(['$compileProvider', function ($compileProvider) {
    $compileProvider.debugInfoEnabled(false);
}]);

