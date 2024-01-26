 <template>
     <div>
         <h5 class="insearch mt-0 py-2">
             工程：{{tenderItem.EngName}}({{tenderItem.EngNo}})<br>標案：{{tenderItem.TenderName}}({{tenderItem.TenderNo}})
         </h5>
         <h5>開工日期：{{tenderItem.StartDateStr}}</h5>
         <h5>
             工程總天數：{{engTotayDay}}天(不含停工日) <span class="text-R small">契約工期：{{tenderItem.EngPeriod}}{{tenderItem.DurationCategory}}(不含國定假日) + 不計工期：{{noDuration}}天 + 展延工期：{{extDuration}}天</span>
             <br>含停工日總天數：{{includeStopDays}}天 <span class="text-R small">工程總天數：{{engTotayDay}}天 + 停工日：{{stopDays}}天</span>
         </h5>
         <div v-if="tenderLoaded">
             <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a v-on:click="selectTab='Chart1'" class="nav-link active" data-toggle="tab" href="#menu01">S進度曲線</a>
                 </li>
                 <li class="nav-item">
                     <a v-on:click="selectTab='Chart2'" class="nav-link" data-toggle="tab" href="#menu02">桿狀圖</a>
                 </li>
                 <li class="nav-item">
                     <a v-on:click="selectTab='Chart3'" class="nav-link" data-toggle="tab" href="#menu03">機具/人員出工變化圖</a>
                 </li>
                 <li class="nav-item">
                     <a v-on:click="selectTab='Chart4'" class="nav-link" data-toggle="tab" href="#menu04">機具能耗曲線</a>
                 </li>
             </ul>
             <div class="tab-content">
                 <div id="menu01" class="tab-pane active text-center">
                     <Chart1 v-if="selectTab=='Chart1'" v-bind:chartData="chartProgress"></Chart1>
                 </div>
                 <div id="menu02" class="tab-pane text-center">
                     <Chart2 v-if="selectTab=='Chart2'" v-bind:chartData="chart2Data"></Chart2>
                 </div>
                 <div id="menu03" class="tab-pane text-center">
                     <Chart3 v-if="selectTab=='Chart3'" v-bind:tenderItem="tenderItem"></Chart3>
                 </div>
                 <div id="menu04" class="tab-pane text-center">
                     <Chart4 v-if="selectTab=='Chart4'" v-bind:tenderItem="tenderItem"></Chart4>
                 </div>
             </div>
         </div>
     </div>
</template>
<script>
    export default {
        data: function () {
            return {
                targetId: null,
                tenderItem: {},
                tenderLoaded: false,
                selectTab: 'Chart1',
                stopDays: 0,
                noDuration: 0,
                extDuration: 0,
                //
                chartProgress: {},
                chart2Data: [],
                chartEquipmen: {},
                chartPerson: {},
            };
        },
        components: {
            Chart1: require('./PR_Chart1.vue').default,
            Chart2: require('./PR_Chart2.vue').default,
            Chart3: require('./PR_Chart3.vue').default,
            Chart4: require('./PR_Chart4.vue').default,
        },
        methods: {
            //完成進度
            getChartProgress() {
                window.myAjax.post('/EPCProgressReport/GetChartProgress', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartProgress = resp.data.item;
                            this.getChart2Data();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getChart2Data() {
                window.myAjax.post('/EPCProgressReport/GetChart2', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chart2Data = resp.data.items;
                            //this.getChartEquipmen();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            /*/機具出工
            getChartEquipmen() {
                window.myAjax.post('/EPCProgressReport/GetChartEquipmen', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartEquipmen = resp.data.item;
                            this.getChartPerson();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //人員出工
            getChartPerson() {
                window.myAjax.post('/EPCProgressReport/GetChartPerson', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.chartPerson = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },*/
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EPCProgressReport/GetTrender',{ id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.stopDays = resp.data.stopDays;
                            this.tenderLoaded = true;
                            this.getChartProgress(); //getChart2Data();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        computed: {
            //工程總天數(不含停工日) = 契約工期：120日曆天(不含國定假日) + 不計工期 + 展延工期
            engTotayDay: function () {
                return this.tenderItem.EngPeriod + this.noDuration + this.extDuration;
            },
            //含停工日總天數 = 工程總天數 + 停工天數
            includeStopDays: function () {
                return this.engTotayDay + this.stopDays;
            }
        },
        mounted() {
            console.log('mounted() 進度管理');
            this.targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq)
            this.getItem();
        }
    }
</script>
