(function () {
    'use strict';

    //angular.module('Xpense').directive('notEqualTo', ['$parse', function ($parse) {

    //    var directive = {
    //        link: link,
    //        restrict: 'A',
    //        require: 'ngModel'
    //    };
    //    return directive;

    //    function link(scope, elem, attrs, ctrl) {
    //        var firstValue = $parse(attrs['notEqualTo']);

    //        var validator = function (value) {
    //            var temp = firstValue(scope),
    //            v = value !== temp;
    //            ctrl.$setValidity('match', v);
    //            return value;
    //        }

    //        ctrl.$parsers.unshift(validator);
    //        ctrl.$formatters.push(validator);
    //        attrs.$observe('notEqualTo', function () {
    //            validator(ctrl.$viewValue);
    //        });

    //    }
    //}]);

    angular.module("Xpense").directive("notEqualTo", function ($parse) {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, element, attributes, ngModel) {
                ngModel.$validators.notEqualTo = function (modelValue) {
                    var firstValue = $parse(attributes['notEqualTo']);
                    var temp = firstValue(scope);

                    return !(temp === modelValue);
                }
            }
        }
    });
})();
