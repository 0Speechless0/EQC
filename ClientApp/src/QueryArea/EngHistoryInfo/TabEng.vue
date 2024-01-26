<template>
    <div>
        <h5 class="insearch mt-0 py-2">
            <div class="width: 100%; padding-right: 15px; padding-left: 15px; margin-right: auto;">
                <div class="row" style="display:flex; padding-left: 15px; ">
                    <div style="color:#000 ; padding-right: 15px;"><label for="checkCode">輸入工程名稱或編號</label></div>
                    <div><input v-model.trim="keyword" type="text" placeholder="請輸入" class="form-control"></div>
                    <div class="row justify-content-center">
                        <div class="col-12">
                            <button @click="onSearchEng" class="btn btn-primary" type="button" style="background-color: #04b9c3; border: #04b9c3 double;">開始查詢</button>
                            <button @click="onClearSearchEng" class="btn btn-primary" type="button" style="background-color: #04b9c3; border: #04b9c3 double;">清除資料</button>
                        </div>
                    </div>
                </div>
            </div>
        </h5>
        <div>
            <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <div class="table-responsive" style="padding: 10px;">
                <table class="table table-responsive-md table-hover">
                    <thead class="insearch">
                        <tr>
                            <th><strong>排序</strong></th>
                            <th><strong>工程編號</strong></th>
                            <th><strong>工程名稱</strong></th>
                            <th><strong>執行機關</strong></th>
                            <th><strong>執行單位</strong></th>
                            <th class="text-right"><strong>碳排放量(kgCO2e)</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq" @click="onSelectEngClick(item)" v-bind:class="{'bg-1-30':(item.Seq==selEngSeq), '':(item.Seq!=selEngSeq)}">
                            <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                            <td>{{item.EngNo}}</td>
                            <td>{{item.EngName}}</td>
                            <td>{{item.execUnitName}}</td>
                            <td>{{item.execSubUnitName}}</td>
                            <td class="text-right">{{item.co2Total}}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <!-- EngList></EngList -->
        <div v-if="selectEng != null">
            <h5 class="insearch mt-0 py-2">{{selectEng.EngName}}</h5>
            <PhotoList :engMainSeq="selectEng.Seq"></PhotoList>
            <MaterialList :engMainSeq="selectEng.Seq"></MaterialList>
            <PersonnelMach :engMainSeq="selectEng.Seq"></PersonnelMach>
            <EnergyConsumptionList :engMainSeq="selectEng.Seq"></EnergyConsumptionList>
            <h5 class="insearch mt-0 py-2">材料設備送審及試驗管制清單</h5>
            <div style="margin: 0 0 0 20px;">
                <EquipmentReviewList :engMainSeq="selectEng.Seq"></EquipmentReviewList>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                keyword: '',
                items: [],
                selectEng: null,
                selEngSeq: -1,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        components: {
            //EngList: require('./TabEng_EngList.vue').default,
            PhotoList: require('./TabEng_PhotoList.vue').default,
            MaterialList: require('./TabEng_MaterialList.vue').default,
            PersonnelMach: require('./TabEng_PersonnelMachList.vue').default,
            EnergyConsumptionList: require('./TabEng_EnergyConsumptionList.vue').default,
            EquipmentReviewList: require('./TabEng_EquipmentReviewList.vue').default,
        },
        methods: {
            //選取工程
            onSelectEngClick(item) {
                this.selectEng = item;
                this.selEngSeq = this.selectEng.Seq;
            },
            //清除查詢
            onClearSearchEng() {
                this.selectEng = null;
                this.items = [];
                this.keyword = '';
                this.selEngSeq = -1;
                this.recordTotal = 0;
                this.pageIndex = 0;
            },
            //工程查詢
            onSearchEng() {
                this.selectEng = null;
                this.selEngSeq = -1;
                this.pageIndex = 1;
                this.getList();
            },
            //工程清單
            getList() {
                if (this.keyword.length == 0) {
                    alert('必須輸入資料');
                    return;
                }
                window.myAjax.post('/EngHistoryInfo/SearchEng', {
                    keyword: this.keyword,
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
            console.log('mounted 水利工程履歷管理-工程');
        }
    }
</script>