<template>
    <div>
        <h5 class="insearch mt-0 py-2">工地材料管制概況</h5>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>材料名稱</strong></th>
                        <th><strong>單位</strong></th>
                        <th class="text-right"><strong>契約數量</strong></th>
                        <th class="text-right"><strong>本日完成數量</strong></th>
                        <th class="text-right"><strong>累積完成數量</strong></th>
                        <th><strong>備註</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" >
                        <td class="text-left">{{item.MaterialName}}</td>
                        <td>{{item.Unit}}</td>
                        <td class="text-right">{{item.ContractQuantity}}</td>
                        <td class="text-right">{{item.TodayQuantity}}</td>
                        <td class="text-right">{{item.AccQuantity}}</td>
                        <td>{{item.Memo}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq'],
        watch: {
            engMainSeq: function (val) {
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
                window.myAjax.post('/EngHistoryInfo/GetMaterialList', {
                    id: this.engMainSeq,
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
            console.log('mounted 工地材料管制概況');
            this.getList();
        }
    }
</script>