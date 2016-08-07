(function () {
    angular.module('Xpense').factory('USER_INFO', function () {

        return {
            Get: function () {
                var User = JSON.parse(window.localStorage.getItem("USER_INFO"));
                if (User !== null) {
                    return {
                        UsuCod: User.UsuCod,
                        UsuNom: User.UsuNom,
                        UsuEml: User.UsuEml,
                        Avatar: User.Avatar,
                        DataCadastro: User.DataCadastro
                    }
                } else {
                    return {
                        UsuCod: 0,
                        UsuNom: '',
                        UsuEml: '',
                        Avatar: '',
                        DataCadastro: new Date()
                    }
                }
            },

            Save: function (User) {
                window.localStorage.setItem("USER_INFO", JSON.stringify({
                    UsuCod: User.UsuCod,
                    UsuNom: User.UsuNom,
                    UsuEml: User.UsuEml,
                    Avatar: User.Avatar,
                    DataCadastro: new Date()
                }));
            }
        }
    });
})();