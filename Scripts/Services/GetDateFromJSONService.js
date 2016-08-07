(function () {
    angular.module("Core").service("GetDateFromJSONService", GetDateFromJSONService);

    function GetDateFromJSONService() {
        this.getDateFromJSON = function (JSONDate) {
            if (typeof JSONDate === 'string') {
                var data = new Date(parseInt(JSONDate.substr(6)));

                return data;
            } else {
                return JSONDate;
            }
        }
    }
})();