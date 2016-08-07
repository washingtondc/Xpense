(function () {
    angular.module('Xpense').filter("jsDate", jsDate);

    function jsDate() {
        return function (x) {
            if (typeof (x) === 'string') {
                return new Date(parseInt(x.substr(6)));
            }
        };
    }
})();