function DrawGraficoSaldoMes(series) {
    $('#SaldoMes').highcharts({
        chart: {
            plotBackgroundColor: null,
            type: 'line',
            plotShadow: false
        },
        title: {
            text: ''
        },
        xAxis: {
            categories: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
                'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez']
        },
        yAxis: {
            title: {
                text: 'Saldo (R$)'
            }
        },
        tooltip: {
            pointFormat: '<b>{point.y}</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',

                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor)
                    }
                }
            }
        },
        series: series
    });
}