(function () {
    angular.module('Xpense').service('EmailService', EmailService);

    function EmailService() {
        this.SendEmail = function (destinatario, assunto, mensagem) {
            var Requisicao = {
                url: 'Webservices/wsEmail.asmx/sendEmail',
                method: 'POST',
                data: { destinatario: +destinatario, assunto: assunto, mensagem: mensagem }
            };

            return $http(Requisicao);
        }
    }

})();