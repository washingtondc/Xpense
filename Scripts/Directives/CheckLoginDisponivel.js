(function () {
    angular.module('Xpense').directive('checkLogin', CheckLogin);

    function CheckLogin(UsuarioService, $q) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {

                ngModel.$asyncValidators.invalidUsername = function (modelValue, viewValue) {
                    var username = viewValue;
                    var deferred = $q.defer();

                    UsuarioService.isUsernameAvailable(username).then(function (r) {
                        if (r.data.d) {
                            deferred.resolve();
                        } else {
                            deferred.reject();
                        }
                    });

                    return deferred.promise;
                }
            }
        }
    }
})();
