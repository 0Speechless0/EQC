<template>
    <div>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item"><a v-on:click="selectTab(1)" data-toggle="tab" href="#menu01" class="nav-link active">所屬機關弱面分析</a></li>
            <li class="nav-item"><a v-on:click="selectTab(2)" data-toggle="tab" href="#menu02" class="nav-link">縣市政府弱面分析</a></li>
            <li class="nav-item"><a v-on:click="selectTab(3)" data-toggle="tab" href="#menu03" class="nav-link">其他補助弱面分析</a></li>
        </ul>
        <div class="tab-content">
            <div id="menu01" class="tab-pane active">
                <div class="tup"></div>
                <div class="row">
                    <div class="col-12" style="padding: 0 0 0 15px;">
                        <div class="card whiteBG mb-4 pattern-F colorset_G">
                            <div class="chartitle">所屬機關弱面分析</div>
                            <div style="margin: 0 0 0 27px;">件數</div>
                            <highcharts id="unitEngAmountContainer" :options="unitEngAmountOptions"></highcharts>
                            <div style="text-align:center; margin:-12px 0 10px 0;"></div>
                        </div>
                    </div>
                    <div class="col-8">
                        <div class="card whiteBG mb-4 pattern-F colorset_G">
                            <div class="chartitle">十四項指標</div>
                            <highcharts id="weaknessContainer" :options="weaknessOptions"></highcharts>
                        </div>
                    </div>
                    <div class="col-4">
                        <div class="card2 whiteBG mb-4 pattern-F colorset_G">
                            <div class="wcolud">廠商弱面指標關鍵字雲</div>
                            <div class="tup2" style="text-align:center;">依廠商的弱面指標最多的項目顯示文字雲</div>
                            <highcharts id="keywordContainer" :options="keywordOptions"></highcharts>
                        </div>
                    </div>
                </div>
                <div class="tab-pane active" style="display: flex;">
                    <div class="col-4" style="padding: 0 0 0 15px;">
                        <div class="card whiteBG mb-4 pattern-F colorset_G">
                            <div class="chartjs-size-monitor"><div class="chartjs-size-monitor-expand"><div class=""></div></div><div class="chartjs-size-monitor-shrink"><div class=""></div></div></div>
                            <div class="chartitle">弱面比重</div>
                            <highcharts id="weaknessOrientedContainer" :options="weaknessOrientedOptions"></highcharts>
                        </div>
                    </div>
                    <div class="col-4" style="padding: 0 0 0 15px;">
                        <div class="card whiteBG mb-4 pattern-F colorset_G">
                            <div class="chartjs-size-monitor"><div class="chartjs-size-monitor-expand"><div class=""></div></div><div class="chartjs-size-monitor-shrink"><div class=""></div></div></div>
                            <div class="chartitle">標案構面</div>
                            <highcharts id="engOrientedContainer" :options="engOrientedOptions"></highcharts>
                        </div>
                    </div>
                    <div class="col-4" style="padding: 0 0 0 15px;">
                        <div class="card whiteBG mb-4 pattern-F colorset_G">
                            <div class="chartjs-size-monitor"><div class="chartjs-size-monitor-expand"><div class=""></div></div><div class="chartjs-size-monitor-shrink"><div class=""></div></div></div>
                            <div class="chartitle">廠商構面</div>
                            <highcharts id="companyOrientedContainer" :options="companyOrientedOptions"></highcharts>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {
    data: function () {
        return {
            mode: 1,
            filterUnit: '',
            unitEngAmountOptions: {//所屬機關案件分析
                chart: { type: 'column', height: 250 },
                title: { text: "" },
                //tooltip: { valueDecimals: 2, valueSuffix: '' },
                xAxis: { gridLineWidth: 1, categories: [] },
                yAxis: { title: { text: "" }, },
                legend: { align: 'center', verticalAlign: 'bottom', borderWidth: 0 },
                series: [{ name: '案件', color: '#009f8c', data: [] }],
                plotOptions: {
                    series: {
                        cursor: 'pointer',
                        events: { click: this.onUnitClick }
                    }
                }
            },
            //十四項指標
            weaknessOptions: {
                chart: { type: 'pie', height: 400 },
                title: { text: "" },
                plotOptions: {
                    pie: {
                        /*dataLabels: {
                            enabled: true,
                            distance: -50,
                            style: { fontWeight: 'bold', color: 'white' }
                        },*/
                        startAngle: -90,
                        endAngle: 90,
                        center: ['50%', '75%'],
                        size: '100%'
                    }
                },
                series: [{ name: '件數', innerSize: '40%', data: [] }]
            },
            //弱面比重
            weaknessOrientedOptions: {//所屬機關案件分析
                chart: { type: 'pie', height: 350 },
                title: { text: "" },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: { enabled: false },
                        showInLegend: true
                    }
                },
                legend: {
                    align: 'left',
                    layout: 'horizontal',
                    verticalAlign: 'top',
                },
                series: [{ name: '件數', data: [] }]
            },
            //標案構面
            engOrientedOptions: {//所屬機關案件分析
                chart: { type: 'pie', height: 350 },
                title: { text: "" },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: { enabled: false },
                        showInLegend: true
                    }
                },
                legend: {
                    align: 'left',
                    layout: 'horizontal',
                    verticalAlign: 'top',
                },
                series: [{ name: '件數', data: [] }]
            },
            //廠商構面
            companyOrientedOptions: {//所屬機關案件分析
                chart: { type: 'pie', height: 350 },
                title: { text: "" },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: { enabled: false },
                        showInLegend: true
                    }
                },
                legend: {
                    align: 'left',
                    layout: 'horizontal',
                    verticalAlign: 'top',
                },
                series: [{ name: '件數', data: [] }]
            },
            //廠商關鍵字
            keywordOptions: {
                chart: { height: 400 },
                title: { text: "" },
                series: [{
                    name: '件數',
                    type: 'wordcloud',
                    data: [{ name: 'word', weight: 1 }, { name: 'word2', weight: 10 }]
                }]
            },
        };
    },
    components: {
        //EngList: require('./EngList.vue').default,
    },
    methods: {
        selectTab(m) {
            this.mode = m;
            this.filterUnit = '';
            this.unitEngAmountOptions.xAxis.categories = [];
            this.unitEngAmountOptions.series[0].data = [];
            this.getUnitEngAmount();
        },
        //機關案件分析
        getUnitEngAmount() {
            window.myAjax.post('/EADWeaknessAnalysis/GetUnitEngAmount', { mode: this.mode })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.unitEngAmountOptions.xAxis.categories = resp.data.categories;
                        this.unitEngAmountOptions.series[0].data = resp.data.data;
                        this.getWeakness();
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onUnitClick(event) {
            this.filterUnit = event.point.category;
            //console.log(this.filterUnit);
            this.getWeakness();
        },
        //十四項指標
        getWeakness() {
            window.myAjax.post('/EADWeaknessAnalysis/GetWeakness', { mode: this.mode, unit: this.filterUnit })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.weaknessOptions.series[0].data = resp.data.data;
                        this.getWeaknessOriented();
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //構面分析
        getWeaknessOriented() {
            this.weaknessOrientedOptions.series[0].data = [];
            this.engOrientedOptions.series[0].data = [];
            this.companyOrientedOptions.series[0].data = [];
            window.myAjax.post('/EADWeaknessAnalysis/GetWeaknessOriented', { mode: this.mode, unit: this.filterUnit })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.weaknessOrientedOptions.series[0].data = resp.data.weaknessOriented;
                        this.engOrientedOptions.series[0].data = resp.data.engOriented;
                        this.companyOrientedOptions.series[0].data = resp.data.companyOriented;
                        this.getContractorWeakness();
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //廠商關鍵字
        getContractorWeakness() {
            this.keywordOptions.series[0].data = [];
            window.myAjax.post('/EADWeaknessAnalysis/GetContractorWeakness', { mode: this.mode, unit: this.filterUnit })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.keywordOptions.series[0].data = resp.data.keywordOptions;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        }
    },
    mounted() {
        console.log('mounted() 品質管制弱面分析');
        this.selectTab(1);
        //document.getElementById('unitEngAmountContainer').style.removeProperty('overflow');
    }
}
</script>
<style scoped>
    .chartitle {
        color: #D23E52;
        text-align: center;
        font-size: 1.4rem;
        font-weight: bold;
        line-height: 1.5;
        margin: 15px 0 15px 0;
    }
    .wcolud {
        padding: 15px 0 5px 0;
        text-align: center;
        font-size: 1.4rem;
        font-weight: bold;
    }
    /* 半圓形 */
    .highcharts-figure,
    .highcharts-data-table table {
        min-width: 500px;
        max-width: 800px;
        margin: 1em auto;
    }

    #container1 {
        height: 430px;
    }

    .highcharts-data-table table {
        font-family: Verdana, sans-serif;
        border-collapse: collapse;
        border: 1px solid #ebebeb;
        margin: 10px auto;
        text-align: center;
        width: 100%;
        max-width: 500px;
    }

    .highcharts-data-table caption {
        padding: 1em 0;
        font-size: 1.2em;
        color: #555;
    }

    .highcharts-data-table th {
        font-weight: 600;
        padding: 0.5em;
    }

    .highcharts-data-table td,
    .highcharts-data-table th,
    .highcharts-data-table caption {
        padding: 0.5em;
    }

    .highcharts-data-table thead tr,
    .highcharts-data-table tr:nth-child(even) {
        background: #f8f8f8;
    }

    .highcharts-data-table tr:hover {
        background: #f1f7ff;
    }
    /* 半圓形 */
</style>