(function () {
    angular.module("Xpense").service("ConfigService", ConfigService);

    function ConfigService($http) {
        this.getUserConfig = function () {
            var Requisicao = {
                url: 'Webservices/wsConfig.asmx/getUserConfig',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        }

        this.saveConfig = function (config) {
            var Requisicao = {
                url: 'Webservices/wsConfig.asmx/saveConfig',
                method: 'POST',
                data: {Config:config}
            };

            return $http(Requisicao);
        }
    }
})();