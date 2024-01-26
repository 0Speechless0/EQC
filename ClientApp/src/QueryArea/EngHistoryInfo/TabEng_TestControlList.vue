<template>
    <div>
        <h5 class="insearch mt-0 py-2">材料設備檢(試)驗管制</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive mb-3" style="min-height:320px; padding: 10px;">
            <table class="table table1 onepage my-0" border="0">
                <thead>
                    <tr>
                        <th>項次</th>
                        <th class="text-left">契約詳細表項次<br>材料/設備名稱</th>
                        <th class="text-left">抽樣日期</th>
                        <th class="text-right">進場數量</th>
                        <th class="text-right">累積進場數量</th>
                        <th class="text-right">抽樣數量</th>
                        <th class="text-right">累積抽樣數量</th>
                        <th>抽驗結果</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.ItemNo}}<br />{{item.MDName}}</td>
                        <td>{{item.chsSampleDate}}</td>
                        <td class="text-right">{{item.TestQty}}</td>
                        <td class="text-right">{{item.AccTestQty}}</td>
                        <td class="text-right">{{item.SampleQty}}</td>
                        <td class="text-right">{{item.AccSampleQty}}</td>
                        <td class="text-center">
                            <span v-if="item.TestResult==2">否決</span>
                            <span v-if="item.TestResult==1">通過</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq', 'emdSeq'],
        watch: {
            /*engMainSeq: function (val) {
                if (this.engMainSeq > -1) this.getList();
            },*/
            emdSeq: function (val) {
                if (this.engMainSeq > -1) this.getList();
            }
        },
        data: function () {
            return {
                items:[],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        methods: {
            getList() {
                this.items = [];
                window.myAjax.post('/EngHistoryInfo/GetEMDTestSummaryList', {
                    id: this.engMainSeq,
                    emdSeq: this.emdSeq,
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.total;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },           
        },
        async mounted() {
            console.log('mounted 材料設備檢(試)驗管制總表');
            this.getList();
        }
    }
</script>