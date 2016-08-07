(function () {
    angular.module("Xpense")
    .controller("ImportDialogController", ImportDialogController);

    function ImportDialogController($scope, $mdDialog, $location, fileUpload, ImportService) {
        $scope.myFile = {};

        // Funções chamadas pelos botões da janela modal
        $scope.hide = function () {
            $mdDialog.hide();
        };

        $scope.cancel = function () {
            $mdDialog.cancel();
        };
          

        $scope.uploadFile = function () {
            var file = $scope.myFile;

            fileUpload.uploadFileToUrl(file, "../Uploads").then(function (r) {
                $mdDialog.cancel();
                $location.path("/ImportarOFX/" + file.name);
            });
        };

    }
})();