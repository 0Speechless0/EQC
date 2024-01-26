<template>
    <div>
        <div>
            <highcharts id="container" :options="options"></highcharts>
        </div>
        <div>
            <p class="text-R">
                預定進度計算方式：總合(各單項之預定完成數量*單價=預定完成金額)*100/契約金額=預定進度(%)<br>
                實際進度計算方式：總合(各單項之本日完成數量*單價=本日完成金額)*100/契約金額=本日進度(%)
            </p>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['chartData'],
        watch: {
            chartData: function () {
                this.init();
            }
        },
        data: function () {
            return {
                options: {
                    chart: { type: 'line', height: 450 },
                    title: { text: "S進度曲線" },
                    tooltip: { valueDecimals: 2, valueSuffix: ' %' },
                    xAxis: { categories: [] },
                    yAxis: { title: { text: "累計完成(%)" }, },
                    legend: { align: 'center', verticalAlign: 'bottom', borderWidth: 0 },
                    series: [ { name:'預定累計完成', data: [] }, { name: '累計完成', data: [] } ]
                },
            };
        },
        methods: {
            //取得紀錄
            init() {
                this.options.xAxis.categories = this.chartData.categories;
                this.options.series = this.chartData.series;
            },
        },
        mounted() {
            console.log('mounted() S進度曲線');
            this.init();
            document.getElementById('container').style.removeProperty('overflow');
        }
    }
</script>
