(function () {
    angular.module('Xpense').factory('CategoriasModel', CategoriasModel);

    function CategoriasModel() {
        var CategoriasModel = this;
        var Categorias = [];

        CategoriasModel.set = function (pCategorias) {
            Categorias = pCategorias;
        }

        CategoriasModel.get = function () {
            return Categorias;
        }

        CategoriasModel.put = function (Categoria) {
            var posicao = -1;
            // Busca se o cartao já existe no array
            Categorias.forEach(function (elemento) {
                if (elemento.CatCod === Categoria.CatCod) {
                    posicao = Categorias.indexOf(elemento);
                }
            })
            if (posicao >= 0) {
                Categorias[posicao] = Categoria;
            } else {
                Categorias.push(Categoria);
            }
        }

        CategoriasModel.remove = function (Categoria) {
            var index = Categorias.indexOf(Categoria);
            Categorias.splice(index, 1);
        }

        return CategoriasModel;
    }
})();