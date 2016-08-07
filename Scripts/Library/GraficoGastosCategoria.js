function DrawGraficoGastosCategoria(series) {
    $('#GastosCategoria').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotShadow: true
        },
        title: {
            text: ''
        },
        tooltip: {
            pointFormat: '{series.Categoria} <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                showInLegend: true,
                dataLabels: {
                    enabled: false
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            floating: true
        },
        series: [{
            type: 'pie',
            name: 'Gastos Categoria',
            data: series,
            point: {
                events: {
                    click: function (event) {
                        window.location.href = '#/Lancamentos/' + this.CatCod;
                    }
                }
            }
        }]
    });
}
