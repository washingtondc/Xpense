(function () {
    angular.module('Xpense').service('fileUpload', ['$http', fileUpload]);

    function fileUpload($http) {
        this.uploadFileToUrl = function (file, path) {
            var fd = new FormData();
            fd.append('file', file);
            




            fd.append("path", path);



            var Requisicao = {
                url: "Webservices/wsUpload.asmx/uploadFile",
                method: 'POST',
                headers: { 'Content-Type': undefined },
                data: fd
            };

            return $http(Requisicao);
        }
    }
})();