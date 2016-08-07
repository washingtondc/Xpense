(function () {
    angular.module("Xpense").service("CategoriasService", CategoriasService);

    function CategoriasService($http) {
        this.getCategorias = function () {
            var Requisicao = {
                url: 'Webservices/wsCategorias.asmx/getCategoriasUsuario',
                method: 'POST',
                data: {}
            };

            return $http(Requisicao);
        };

        this.SalvarCategoria = function (Categoria) {
            var Requisicao = {
                url: 'Webservices/wsCategorias.asmx/SalvarCategoria',
                method: 'POST',
                data: { ObjCategoria: Categoria }
            };

            return $http(Requisicao);
        };

        this.excluirCategoria = function (CatCod,CatCodDestino) {
            var Requisicao = {
                url: 'Webservices/wsCategorias.asmx/excluirCategoria',
                method: 'POST',
                data: { CatCod: CatCod, CatCodDestino: CatCodDestino }
            };

            return $http(Requisicao);
        };
    }
})();