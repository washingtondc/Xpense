(function () {
    angular.module('Xpense').directive('colorPicker', function () {
        return {
            restrict: 'E',
            replace: 'true',
            scope:
                {
                    selectedColor: "=ngModel"
                },
            controller: function ($scope) {
                $scope.colorPickerisOpen = false;
                $scope.colors = [
                    "#E91E63",
                    "#9C27B0",
                    "#2196F3",
                    "#8BC34A",
                    "#607D8B",
                    "#d50000",
                    "#76FF03",
                    "#FF9800"
                ];

                $scope.togglePicker = function () {
                    $scope.colorPickerisOpen = !$scope.colorPickerisOpen;
                }

                $scope.setColor = function (color) {
                    $scope.selectedColor = color;
                    $scope.colorPickerisOpen = false;
                }
            },
            templateUrl: '/scripts/directives/ColorPicker.html'
        };
    });
})();