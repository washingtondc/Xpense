(function () {
    angular.module('Xpense')
    .controller('EfetivarDialogController', EfetivarDialogController);

    function EfetivarDialogController($scope, Lancamento, LanDatPag, LancamentosService, ContasService, LancamentosModel, $mdDialog, ToastService) {
        $scope.Lancamento = Lancamento;
        $scope.LanDatPag = LanDatPag;
        $scope.ValorEfetivar = Lancamento.LanVal;
        $scope.AtualizarSaldo = true;

        $scope.cancel = function () {
            $mdDialog.cancel();
        };

        $scope.Salvar = function () {
            var LanAno = $scope.Lancamento.LanAno;
            var LanSeq = $scope.Lancamento.LanSeq;

            LancamentosService.EfetivarLancamento(LanAno, LanSeq, LanDatPag, $scope.ValorEfetivar, $scope.AtualizarSaldo)
                .then(function () {

                    // Atualiza objeto na coleção
                    $scope.Lancamento.LanEft = true;
                    $scope.Lancamento.Parcela.LanEftDat = new Date();
                    $scope.Lancamento.Parcela.LanEftVal = $scope.ValorEfetivar;

                    ToastService.successMessage('Lançamento efetivado.');

                    $mdDialog.hide();
                });
        }
    }
})();