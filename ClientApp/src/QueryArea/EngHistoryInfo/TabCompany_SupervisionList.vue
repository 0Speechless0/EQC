<template>
    <div>
        <h5 class="insearch mt-0 py-2">督導紀錄</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>排序</strong></th>
                        <th class="text-right"><strong>年度</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>督導紀錄</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td class="text-right">{{item.EngYear}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.execUnitName}}</td>
                        <td>本案於 {{item.SuperviseDateStr}} 督導，督導總分{{item.CommitteeAverageScore}}分，扣點數:{{item.DeductPoint}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq', 'engItem'],
        watch: {
            engMainSeq: function (val) {
                if (this.engMainSeq > -1) this.getList();
            }
        },
        data: function () {
            return {
                items: [],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        methods: {
            getList() {
                this.items = [];
                window.myAjax.post('/EngHistoryInfo/GetSupervisionList', {
                    taxId: this.engItem.BuildContractorTaxId,
                    bName: this.engItem.BuildContractorName,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
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
            console.log('mounted 督導紀錄');
            this.getList();
        }
    }
</script>