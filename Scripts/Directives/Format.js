(function () {
    angular.module('Xpense').directive('format', ['$filter', format]);

    function format($filter) {
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                if (!ctrl) return;

                var format = {
                    prefix: '',
                    centsSeparator: ',',
                    thousandsSeparator: ''
                };

                ctrl.$parsers.unshift(function (value) {
                    elem.priceFormat(format);
                    return elem[0].value;
                });

                ctrl.$formatters.unshift(function (value) {
                    elem[0].value = ctrl.$modelValue * 100;
                    elem.priceFormat(format);
                    return elem[0].value;
                })
            }
        };
    }
})();