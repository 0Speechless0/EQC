<template>
    <div>
        <highcharts v-if="initflag" id="container" ref="gaugeChart" :options="options"></highcharts>
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
                initflag:false,
                options: {
                    chart: { type: 'columnrange', inverted: true, height: 600, marginLeft: 250},
                    title: { text: "桿狀圖" },
                    xAxis: {
                        categories: [],
                        type: 'category',
                        labels: {
                            enabled: true,
                            formatter: function () {
                                if (this.value.length >= 20)
                                    return this.value.substring(0, 8) + " ... " + this.value.substring(this.value.length - 8, this.value.length);
                                else
                                    return this.value;
                            }
                        },
                    },
                    yAxis: {
                        title: { text: '工期起訖' },
                        labels: { format: '{value:%Y-%m-%d}' },
                    },
                    tooltip: { pointFormat: '{point.low:%Y-%m-%d} ~ {point.high:%Y-%m-%d} <br> {point.progress}' },
                    plotOptions: {
                        columnrange: {
                            dataLabels: {
                                enabled: true,
                               //format: '{y:%Y-%m--%d}',
                                formatter: function () {
                                    if (this.y == this.point.low)
                                        return new Date(this.y).toLocaleDateString() + ' ' + this.point.progress;
                                    else
                                        return new Date(this.y).toLocaleDateString();
                                }
                            }
                        },
                    },
                    legend: { enabled: false },
                    series: [{ data: [] }],
                    chartData:[]
                },
            };
        },
        methods: {
            init() {
                this.options.xAxis.categories = [];
                this.options.series[0].data = [];
                var i;
                if (this.chartData.length > 30) this.options.chart.height = this.chartData.length * 20;
                for (i = 0; i < this.chartData.length; i++) {
                    let item = this.chartData[i];
                    this.options.xAxis.categories.push(item.Description);
                    var d = [];
                    //this.options.series[0].data.push([this.toUTC(new Date(parseInt(item.minDate.substring(6, 19), 10))), this.toUTC(new Date(parseInt(item.maxDate.substring(6, 19), 10)))]);
                    this.options.series[0].data.push({
                        low: this.toUTC(new Date(parseInt(item.minDate.substring(6, 19), 10))),
                        high: this.toUTC(new Date(parseInt(item.maxDate.substring(6, 19), 10))),
                        progress: item.Progress
                    });
                }
                this.initflag = true;
                setTimeout(function () {
                    document.getElementById('container').style.removeProperty('overflow');
                }, 300);
            },
            toUTC(date) {
                return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);
            }

        },
        mounted() {
            console.log('mounted() 桿狀圖');
            this.init();
        }
    }
</script>