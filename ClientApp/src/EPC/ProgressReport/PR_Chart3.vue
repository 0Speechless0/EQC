<template>
    <div>
        <highcharts id="containerEquipment" ref="equipment" :options="optionsEquipment"></highcharts>
        <br />
        <highcharts id="containerPerson" ref="person" :options="optionsPerson"></highcharts>
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        data: function () {
            return {
                chartEquipmen: {},
                chartPerson: {},
                optionsEquipment: {
                    chart: { type: 'line', height: 450, },
                    title: { text: "機具出工變化圖" },
                    xAxis: { categories: [] },
                    yAxis: { title: { text: "數量" }, },
                    legend: { align: 'center', verticalAlign: 'bottom', borderWidth: 0 },
                    series: []
                },
                optionsPerson: {
                    chart: { type: 'line', height: 450, },
                    title: { text: "人員出工變化圖" },
                    xAxis: { categories: [] },
                    yAxis: { title: { text: "數量" }, },
                    legend: { align: 'center', verticalAlign: 'bottom', borderWidth: 0 },
                    series: []
                },
            };
        },
        methods: {
            initEquipmen() {
                this.optionsEquipment.xAxis.categories = this.chartEquipmen.categories;
                this.optionsEquipment.series = this.chartEquipmen.series;
                document.getElementById('containerEquipment').style.removeProperty('overflow');
            },
            initPerson() {
                this.optionsPerson.xAxis.categories = this.chartPerson.categories;
                this.optionsPerson.series = this.chartPerson.series;
                document.getElementById('containerPerson').style.removeProperty('overflow');
            },
            //機具出工
            getChartEquipmen() {
                window.myAjax.post('/EPCProgressReport/GetChartEquipmen', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartEquipmen = resp.data.item;
                            this.getChartPerson();
                            this.initEquipmen();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //人員出工
            getChartPerson() {
                window.myAjax.post('/EPCProgressReport/GetChartPerson', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartPerson = resp.data.item;
                            this.initPerson();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 機具/人員出工變化圖');
            this.getChartEquipmen();
        }
    }
</script>