<template>
    <div>
        <h5 class="insearch mt-0 py-2">
            工程編號：{{tenderItem.EngNo}}<br>工程名稱：{{tenderItem.EngName}}
        </h5>
        <div class="tab-content">
            <!-- 一 -->
            <div id="menu01" class="tab-pane active">
                <h5>
                    工程天數：{{tenderItem.EngPeriod}}天，工程總價：{{tenderItem.TotalBudget}}元，總碳排放量：{{co2Total}}，可拆解率：{{dismantlingRate}}%
                </h5>
                <form class="insearch mb-3 form-inline">
                    <div class="form-group">
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <select v-model="selectLevel" @change="onLevelChange" class="form-control">
                                <option v-bind:key="index" v-for="(item,index) in selectitems" v-bind:value="item.Value">{{item.Text}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group" >
                        顯示條件 : <span style="color:red" class="ml-2">
                            {{refCodeKeyWord ?? '無'}}
                        </span>
                    </div>
                </form>

                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="insearch">
                            <tr>
                                <th class="text-left"><strong>工程項目代號</strong></th>
                                <th style="width: 42px;"><strong>項次</strong></th>
                                <th><strong>項目及說明</strong></th>
                                <th style="width: 42px;"><strong>單位</strong></th>
                                <th><strong>數量</strong></th>
                                <th><strong>碳排係數</strong></th>
                                <th><strong>工項碳排放量</strong></th>
                                <th style="width: 150px;"><strong>編碼(備註)</strong></th>
                                <th><strong>擷取狀態</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                                <td class="text-left"><strong>{{item.PayItem}}</strong></td>
                                <td>{{item.ItemNo}}</td>
                                <td>{{item.Description}}</td>
                                <td class="text-right">{{item.Unit}}</td>
                                <td class="text-right">{{item.Quantity}}</td>
                                <td class="text-right">{{item.KgCo2e}}</td>
                                <td class="text-right">{{item.ItemKgCo2e}}</td>
                                <td class="text-left">{{item.Memo}}</td>
                                <td class="text-right"><span v-html="item.RStatusCodeStr"></span></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['targetId', "refCodeKeyWord"],
        data: function () {
            return {
                tenderItem: {},
                selectitems: [],
                selectLevel: '',
                co2Total: null,
                dismantlingRate: null,//可拆解率

                ceHeader: {},
                items: [],
            };
        },
        methods: {
            //清單
            getResords() {
                this.editSeq = -99;
                this.items = [];
                window.myAjax.post('/EADCarbonEmission/GetCEList', {
                    id: this.targetId,
                    keyword: this.selectLevel,
                    refCodeKeyWord : this.refCodeKeyWord
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.totalRows = resp.data.totalRows;
                            if (this.selectitems.length == 0) this.getLevel1();
                            this.co2Total = resp.data.co2Total;
                            if (this.tenderItem.TotalBudget>0)
                                this.dismantlingRate = Math.round(resp.data.co2ItemTotal*100 / this.tenderItem.TotalBudget);
                            else
                                this.dismantlingRate = null;
                            this.getCEHeader();
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            getLevel1() {
                this.selectitems = [];
                window.myAjax.post('/EQMCarbonEmission/GetLevel1List', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selectitems = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onLevelChange() {
                this.getResords();
            },
            //碳排量計算主檔
            getCEHeader() {
                this.ceHeader = { };
                window.myAjax.post('/EQMCarbonEmission/GetCEHeader', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ceHeader = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    return;
                }
                window.myAjax.post('/EQMCarbonEmission/GetEngMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.getLevel1();
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 碳排量計算1');
            //this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            this.getItem();
            /*console.log('mounted() 建立標案'+ this.userUnit+ ' '+this.userUnitSub);
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }*/
        }
    }
</script>
