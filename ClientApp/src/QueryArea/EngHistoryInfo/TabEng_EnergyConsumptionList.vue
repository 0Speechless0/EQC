<template>
    <div>
        <h5 class="insearch mt-0 py-2">能耗歷程曲線</h5>
        <highcharts id="containerEquipment" ref="equipment" :options="optionsEquipment"></highcharts>
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq'],
        watch: {
            engMainSeq: function (val) {
                if (this.engMainSeq > -1) this.getChartEquipmen();
            }
        },
        data: function () {
            return {
                optionsEquipment: {
                    chart: { type: 'line', height: 450, },
                    title: { text: "機具能耗曲線" },
                    xAxis: { categories: [] },
                    yAxis: { title: { text: "機具碳排量" }, },
                    legend: { align: 'center', verticalAlign: 'bottom', borderWidth: 0 },
                    series: []
                },
                chartEquipmen: {},
            };
        },
        methods: {
            initEquipmen() {
                this.optionsEquipment.xAxis.categories = this.chartEquipmen.categories;
                this.optionsEquipment.series = this.chartEquipmen.series;
                document.getElementById('containerEquipment').style.removeProperty('overflow');
            },
            //機具出工
            getChartEquipmen() {
                this.chartEquipmen = {};
                this.optionsEquipment.series = [];
                window.myAjax.post('/EPCProgressReport/GetChartEquipmenKgCo2e', { id: this.engMainSeq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartEquipmen = resp.data.item;
                            this.initEquipmen();
                        } else
                            this.initEquipmen();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted 能耗歷程曲線');
            this.getChartEquipmen();
        }
    }
</script>