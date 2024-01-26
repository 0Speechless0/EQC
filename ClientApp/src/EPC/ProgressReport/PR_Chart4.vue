<template>
    <div>
        <highcharts id="containerEquipment" ref="equipment" :options="optionsEquipment"></highcharts>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        /*watch: {
            chartEquipmen: function () {
                this.initEquipmen();
            },
        },*/
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
                window.myAjax.post('/EPCProgressReport/GetChartEquipmenKgCo2e', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartEquipmen = resp.data.item;
                            this.initEquipmen();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 機具能耗曲線');
            this.getChartEquipmen();
        }
    }
</script>