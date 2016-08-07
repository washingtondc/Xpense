(function () {
    angular.module("Xpense").directive("greaterThan", function () {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, element, attributes, ngModel) {
                ngModel.$validators.greaterThan = function (modelValue) {

                    if (typeof (modelValue) === 'string') {
                        valueToValidate = parseFloat(modelValue.replace(',', '.'));
                    } else {
                        valueToValidate = parseFloat(modelValue);
                    }

                    return valueToValidate > parseFloat(attributes.greaterThan);
                }
            }
        }
    });
})();