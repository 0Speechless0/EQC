<template>
    <div>
        <h5 class="insearch mt-0 py-2">品質弱面資訊</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th><strong>排序</strong></th>
                        <th class="text-right"><strong>年度</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th class="text-right"><strong>決標經費(仟元)</strong></th>
                        <th><strong>工程地點</strong></th>
                        <th><strong>品質弱面分析</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" >
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td class="text-right">{{item.TenderYear}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td class="text-right">{{item.BidAmount}}</td>
                        <td>{{item.Location}}</td>
                        <td>{{item.getWeakStr}}</td>
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
                window.myAjax.post('/EngHistoryInfo/GetWeaknessesList', {
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
            console.log('mounted 品質弱面資訊');
            this.getList();
        }
    }
</script>