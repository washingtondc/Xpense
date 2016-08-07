(function () {
    angular.module("Core").service("ToastService", ToastService);

    function ToastService($mdToast) {
        this.successMessage = function (message) {
            $mdToast.show(
              $mdToast.simple()
                .position('top right')
                .textContent(message)
                .hideDelay(5000)
            );
        };

        this.errorMessage = function (message) {
            $mdToast.show(
              $mdToast.simple()
                .position('top right')
                .textContent(message)
                .hideDelay(5000)
            );
        };
    }
})();