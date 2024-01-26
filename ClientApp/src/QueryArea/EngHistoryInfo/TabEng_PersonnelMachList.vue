<template>
    <div>
        <h5 class="insearch mt-0 py-2">工地人員及機具</h5>
        <comm-pagination :recordTotal="recordTotal" v-on:onPaginationChange="onPersonPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>工別</strong></th>
                        <th class="text-right"><strong>本日人數</strong></th>
                        <th class="text-right"><strong>累積人數</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in personItems" v-bind:key="item.Seq">
                        <td class="text-left">{{item.KindName}}</td>
                        <td class="text-right">{{item.TodayQuantity}}</td>
                        <td class="text-right">{{item.AccQuantity}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <comm-pagination :recordTotal="recordTotal1" v-on:onPaginationChange="onEquipmentPaginationChange"></comm-pagination>
        <div class="table-responsive" style="padding: 10px;">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th class="text-left"><strong>機具名稱</strong></th>
                        <th><strong>型號</strong></th>
                        <th class="text-right"><strong>本日使用數量</strong></th>
                        <th class="text-right"><strong>累積使用數量</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in equipmentItems" v-bind:key="item.Seq">
                        <td class="text-left">{{item.EquipmentName}}</td>
                        <td>{{item.EquipmentModel}}</td>
                        <td class="text-right">{{item.TodayQuantity}}</td>
                        <td class="text-right">{{item.AccQuantity}}</td>
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
                if (this.engMainSeq > -1) {
                    this.getPersonList();
                    this.getEquipmentList();
                }
            }
        },
        data: function () {
            return {
                personItems:[],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,

                equipmentItems:[],
                //分頁1
                recordTotal1: 0,
                pageRecordCount1: 30,
                pageIndex1: 1,
            };
        },
        methods: {
            //人員
            getPersonList() {
                this.personItems = [];
                window.myAjax.post('/EngHistoryInfo/GetPersonList', {
                    id: this.engMainSeq,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.personItems = resp.data.items;
                            this.recordTotal = resp.data.total;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //機具
            getEquipmentList() {
                this.equipmentItems = [];
                window.myAjax.post('/EngHistoryInfo/GetEquipmentList', {
                    id: this.engMainSeq,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.equipmentItems = resp.data.items;
                            this.recordTotal1 = resp.data.total;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPersonPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getPersonList();
            },
            //分頁
            onEquipmentPaginationChange(pInx, pCount) {
                this.pageRecordCount1 = pCount;
                this.pageIndex1 = pInx;
                this.getEquipmentList();
            },
        },
        async mounted() {
            console.log('mounted 負責案件清單');
            this.getPersonList();
            this.getEquipmentList();
        }
    }
</script>