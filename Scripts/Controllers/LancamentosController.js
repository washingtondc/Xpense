(function () {
    angular.module('Xpense').controller('LancamentosController', LancamentosController);

    function LancamentosController($scope, $rootScope, $mdDialog, LancamentosService, LancamentosModel, CategoriasService, $routeParams, FaturasService, GetDateFromJSONService, ToastService) {
        $scope.dataAtual = new Date();
        $scope.mes = new Date().getMonth() + 1;
        $scope.ano = new Date().getFullYear();
        $scope.Lancamentos = []; // Lançamentos do mês
        $scope.ResumoAnual = [];
        $scope.Entradas = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        $scope.Saidas = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        $scope.Saldo = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        $scope.Categorias = [];

        // Pega os parâmetros da URL
        $scope.CatCod = $routeParams.CatCod ? $routeParams.CatCod : 0;
        $scope.LanTip = $routeParams.LanTip ? $routeParams.LanTip : '';
        $scope.status = $routeParams.Status ? $routeParams.Status : '';
        $scope.alertas = [];
        $scope.qtdAlertas = 0;
        $scope.SaldoFinal = 0;
        $scope.orderBy = 'LanDia';
        $scope.orderReverse = false;

        // Carregar lançamentos
        $scope.init = function () {
            $rootScope.Loading++;
            LancamentosService.loadLancamentos($scope.mes, $scope.ano, $scope.CatCod, $scope.LanTip, $scope.status)
            .then(function (r) {
                LancamentosModel.set(r.data.d);
                $scope.Lancamentos = LancamentosModel.get();
                calculaSaldo();
                $rootScope.Loading--;
            });
        }

        // Calcula o saldo final de todos os lançamentos carregados
        function calculaSaldo() {
            var total = 0;
            for (x = 0; x < $scope.Lancamentos.length; x++) {
                if ($scope.Lancamentos[x].LanTip == 'E') {
                    total += $scope.Lancamentos[x].LanVal;
                } else {
                    total -= $scope.Lancamentos[x].LanVal;
                }
            }
            $scope.SaldoFinal = total;
        }

        $scope.getResumoAnual = function () {
            $rootScope.Loading++;
            LancamentosService.getResumoAnual()
            .then(function (retorno) {
                $scope.ResumoAnual = retorno.data.d;
                calculaEntradasSaidasAnual();
                $rootScope.Loading--;
            });
        }

        function calculaEntradasSaidasAnual() {
            angular.forEach($scope.ResumoAnual, function (categoria) {
                for (mes = 0; mes < 12; mes++) {
                    if (categoria.Tipo === 'E') {   // Entradas

                        $scope.Entradas[mes] += categoria.ValorMes[mes];
                        $scope.Saldo[mes] += categoria.ValorMes[mes];
                    } else {        //Saídas
                        $scope.Saidas[mes] += categoria.ValorMes[mes];
                        $scope.Saldo[mes] -= categoria.ValorMes[mes];
                    }
                }
            });
        }

        $scope.efetivar = function (Lancamento, LanDatPag, $event) {
            console.log(Lancamento);
            $mdDialog.show({
                targetEvent: $event,
                templateUrl: 'Views/Dialogs/EfetivarLancamento.html',
                controller: 'EfetivarDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o scope do controler
                    Lancamento: Lancamento,
                    LanDatPag: LanDatPag
                }
            });

        }

        // Reabrir um lançamento
        $scope.reabrirLancamento = function (Lancamento) {
            Lancamento.Parcela.LanDatPag = GetDateFromJSONService.getDateFromJSON(Lancamento.Parcela.LanDatPag);
            LancamentosService.reabrirLancamento(Lancamento)
            .then(function () {
                // Atualiza objeto na coleção
                var indice = $scope.Lancamentos.indexOf(Lancamento)
                $scope.Lancamentos[indice].LanEft = false;
                $scope.Lancamentos[indice].Parcela.LanEftDat = undefined;
                $scope.Lancamentos[indice].Parcela.LanEftVal = 0;
                ToastService.successMessage('Lançamento reaberto.');
            });
        }

        //Carrega o objeto Lançamento e chama a janela de edição
        $scope.editarLancamento = function (Lancamento) {
            var LancamentoNoCartao = Lancamento.CarCod > 0;
            var TituloJanela = 'Editar: ' + Lancamento.LanDes;

            OpenEditWindow(Lancamento, TituloJanela, LancamentoNoCartao);
        };

        // Excluir um lançamento
        $scope.Excluir = function (Lancamento) {
            if (Lancamento.LanQtdPar > 1 || Lancamento.LanFix) {
                swal({
                    title: "Atenção",
                    text: "Esse lançamento possui mais de uma parcela. O que deseja excluir?",
                    type: "warning", showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Excluir parcela",
                    cancelButtonText: "Excluir lançamento"
                }, function (isConfirm) {
                    if (isConfirm) {
                        // Excluir parcela
                        LancamentosService.excluirParcela(Lancamento.Parcela)
                        .then(function () {
                            LancamentosModel.remove(Lancamento);
                            ToastService.successMessage('Parcela excluída com sucesso.');
                            calculaSaldo();
                        });
                    } else {
                        // Excluir o lançamento
                        LancamentosService.excluirLancamento(Lancamento)
                        .then(function () {
                            LancamentosModel.remove(Lancamento);
                            ToastService.successMessage('Lançamento excluído com sucesso.');
                            calculaSaldo();
                        });
                    }
                });
            } else {
                swal({
                    title: "Atenção", text: "Deseja excluir o lançamento?",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Sim",
                    cancelButtonText: "Não"
                }, function (isConfirm) {
                    if (isConfirm) {
                        // Excluir o lançamento
                        LancamentosService.excluirLancamento(Lancamento)
                        .then(function () {
                            LancamentosModel.remove(Lancamento);
                            ToastService.successMessage('Lançamento excluído com sucesso.');
                            calculaSaldo();
                        });
                    }
                });
            }
        }

        //Limpa o objeto Lançamento e chama a janela de edição
        $scope.novaDespesa = function () {
            var TituloJanela = 'Nova Despesa';
            OpenEditWindow({ LanAti: true, LanTip: 'S', LanDet: '', CatCod: 0, ConCod: 0, LanVal: 0, LanDat: new Date(), Parcela: { LanEftVal: 0 } }, TituloJanela, false);
        };

        $scope.novaDespesaCartao = function () {
            var TituloJanela = 'Nova Despesa no Cartão';
            OpenEditWindow({ LanAti: true, LanTip: 'S', LanDet: '', CarCod: 0, CatCod: 0, LanVal: 0, LanDat: new Date(), Parcela: { LanEftVal: 0 } }, TituloJanela, true);
        };

        $scope.novaReceita = function () {
            var TituloJanela = 'Nova Receita';
            OpenEditWindow({ LanAti: true, LanTip: 'E', LanDet: '', LanVal: 0, LanDat: new Date(), CatCod: 0, ConCod: 0, Parcela: { LanEftVal: 0 } }, TituloJanela, false);
        };

        function OpenEditWindow(LancamentoEditar, TituloJanela, LancamentoNoCartao) {
            var LancamentoCopy = angular.copy(LancamentoEditar)
            // Transforma o formato da data
            LancamentoCopy.LanDat = GetDateFromJSONService.getDateFromJSON(LancamentoCopy.LanDat);
            // Lançamento está efetivado?
            LancamentoEditar.LanEft = LancamentoEditar.Parcela.LanEftVal>0;

            $mdDialog.show({
                templateUrl: 'Views/Dialogs/Lancamento.html',
                controller: 'LancamentoDialogController',
                clickOutsideToClose: true,
                locals: { // Envia valores para o scope do controler
                    Lancamento: LancamentoCopy,
                    TituloJanela: TituloJanela,
                    LancamentoNoCartao: LancamentoNoCartao,
                    Categorias: $scope.Categorias
                }
            });
        }

        $scope.showFatura = function (Lancamento, Ano, Mes) {
            if (Lancamento.CarCod > 0) {
                // Carrega os lançamentos da fatura
                FaturasService.loadLancamentos(Lancamento.CarCod, Ano, Mes).then(function (r) {
                    if (r.data.d !== null) {
                        Lancamento.Cartao.FaturaAtual.Lancamentos = r.data.d;
                        LancamentosModel.put(Lancamento);
                    }
                });
                // Toggle visibility
                Lancamento.Cartao.FaturaAtual.Visivel = !Lancamento.Cartao.FaturaAtual.Visivel;
            }
        }

        $scope.getAlertas = function() {
            $rootScope.Loading++;
            LancamentosService.getAlertas().then(function (r) {
                if (r.data.d != null && r.data.d.Lancamentos.length > 0) {
                    $scope.alertas = r.data.d.Lancamentos;
                    $scope.qtdAlertas = r.data.d.QtdAlertas;
                } 
                $rootScope.Loading--;
            });
        }      

        var getCategorias = function () {
            CategoriasService.getCategorias()
            .then(function (retorno) {
                $scope.Categorias = retorno.data.d;
            });
        }

        $scope.setOrder = function (campo) {
            if (campo === $scope.orderBy) {
                $scope.orderReverse = !$scope.orderReverse;
            } else {
                $scope.orderBy = campo;
                $scope.orderReverse = false;
            }
        }

        // Carrega as categorias do usuário
        getCategorias();
    }
})();