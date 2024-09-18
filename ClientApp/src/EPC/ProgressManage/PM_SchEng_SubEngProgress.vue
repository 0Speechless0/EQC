<template>
    <div class="tab-content">
        <div v-if="!fAdjPayItem">
            <div v-if="sepHeader.SPState==1" style="display: flex;">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                    <button @click.stop="fillCompleted" type="button" title="設定完成" class="btn btn-outline-secondary btn-sm" :disabled="sepHeader.SPState != 1">
                        分項工程設定完成&nbsp;<i class="fas fa-check"></i>
                    </button>
                </div>
            </div>
            <div class="table-responsive">
                <table class="table table1 min910">
                    <thead>
                        <tr>
                            <th class="sort">項次</th>
                            <th style="width: 40%;">分項工程名稱</th>
                            <th><strong>權重</strong></th>
                            <th class="text-right"><strong>金額</strong></th>
                            <th class="text-right"><strong>預定進度</strong></th>
                            <th class="text-right"><strong>實際進度</strong></th>
                            <th v-if="sepHeader.SPState<2" style="text-align: center;">
                                <strong>功能</strong>
                            </th>
                            <th style="text-align: center; ">
                                <strong>勾稽</strong>
                                <button v-if="sepHeader.SPState<2" @click="onAddSunEng" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></button>
                            </th>
                           
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.Seq" class="bg-1-30 parent selected">
                            <td><strong>{{index+1}}</strong></td>
                            <template v-if="item.Seq != editSeq">
                                <td><strong>{{item.EngName}}</strong></td>
                                <td class="text-right"><strong>{{item.Weights}}</strong></td>
                                <td class="text-right"><strong>{{item.Amount}}</strong></td>
                                <td class="text-right"><strong>{{item.SchProgress}}</strong></td>
                                <td class="text-right"><strong>{{item.ActualProgress}}</strong></td>
                                <td v-if="sepHeader.SPState<2" style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                        <button @click="onDelRecord(item)" class="btn btn-color11-4 btn-xs sharp mx-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                    </div>
                                </td>
                                <td style="text-align: center;">
                                    <button @click="onAdjPayItem(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="勾稽"><i class="fas fa-check"></i></button>
                                </td>
                            </template>
                            <template v-if="item.Seq == editSeq">
                                <td class="text-right"><input v-model.trim="editRecord.EngName" maxlength="50" type="text" class="form-control"></td>
                                <td class="text-right"><!-- input v-model.number="editRecord.Weights" type="number" class="form-control" --></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i></button>
                                        <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                    </div>
                                </td>
                                <td></td>
                            </template>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div v-if="fAdjPayItem">
            <h5 class="insearch mt-0 py-2">
                <div class="form-row">
                    <div class="col-10">
                        分項工程:&nbsp;{{adjSubEng.EngName}}
                    </div>
                    <div class="col-2">
                        <button @click="onAdjPayItemFinish" class="btn btn-outline-secondary btn-sm" title="項目勾稽完成">項目勾稽完成&nbsp;<i class="fas fa-check"></i></button>
                    </div>
                </div>
            </h5>
            <p style="color: rgb(0, 174, 255); padding-top: 15px;">已勾稽</p>
            <div class="table-responsive tableFixHead ">
                <table class="table table-responsive-md table-hover VA-middle">
                    <thead class="insearch">
                        <tr>
                            <th><strong>序號</strong></th>
                            <th><strong>項次</strong></th>
                            <th><strong>施工項目</strong></th>
                            <th><strong>編碼</strong></th>
                            <th class="text-right"><strong>單位</strong></th>
                            <th class="text-right"><strong>契約數量</strong></th>
                            <th class="text-right"><strong>單價</strong></th>
                            <th class="text-right"><strong>金額</strong></th>
                            <th v-if="sepHeader.SPState<2" style="text-align: center;">
                                <strong>功能</strong>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in subPayItems" v-bind:key="item.Seq" class="bg-1-30 parent selected">
                            <td><strong>{{index+1}}</strong></td>
                            <td><strong>{{item.PayItem}}</strong></td>
                            <td><strong>{{item.Description}}</strong></td>
                            <td><strong>{{item.Memo}}</strong></td>
                            <td class="text-right">{{item.Unit}}</td>
                            <td class="text-right">{{item.Quantity}}</td>
                            <td class="text-right">{{item.Price}}</td>
                            <td class="text-right">{{item.Amount}}</td>
                            <td v-if="sepHeader.SPState<2" style="text-align: center;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onDelSubPayItem(item)" class="btn btn-color11-4 btn-xs sharp mx-1" title="移除"><i class="fas fa-trash"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="sepHeader.SPState<2" >
                <p style="color: rgb(0, 174, 255); padding-top: 15px;">未勾稽</p>
                <form class="form-group insearch mb-3">
                    <div class="form-row">
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <select v-model="selectLevel" @change="onLevelChange" class="form-control">
                                <option v-bind:key="index" v-for="(item,index) in selectitems" v-bind:value="item.Value">{{item.Value=='' ? item.Text : item.Value+':'+item.Text}}</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <select v-model="selectLevel2" class="form-control">
                                <option v-bind:key="index" v-for="(item,index) in selectitems2" v-bind:value="item.Value">{{item.Value=='' ? item.Text : item.Value}}</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <input v-model="keyword" type="text" class="form-control">
                        </div>
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <button @click="onSearch" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-search"></i></button>
                        </div>
                    </div>
                </form>
                <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>

                <div class="table-responsive">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="insearch">
                            <tr>
                                <th><strong>序號</strong></th>
                                <th><strong>項次</strong></th>
                                <th><strong>施工項目</strong></th>
                                <th><strong>編碼</strong></th>
                                <th class="text-right"><strong>單位</strong></th>
                                <th class="text-right"><strong>契約數量</strong></th>
                                <th class="text-right"><strong>單價</strong></th>
                                <th class="text-right"><strong>金額</strong></th>
                                <th style="text-align: center;">
                                    <strong>功能</strong>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in payItems" v-bind:key="item.Seq" class="bg-1-30 parent selected">
                                <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                                <td><strong>{{item.PayItem}}</strong></td>
                                <td><strong>{{item.Description}}</strong></td>
                                <td><strong>{{item.Memo}}</strong></td>
                                <td class="text-right">{{item.Unit}}</td>
                                <td class="text-right">{{item.Quantity}}</td>
                                <td class="text-right">{{item.Price}}</td>
                                <td class="text-right">{{item.Amount}}</td>
                                <td style="text-align: center;">
                                    <div class="d-flex justify-content-center">
                                        <button @click="onAddSubPayItem(item)" class="btn btn-color11-3 btn-xs sharp mx-1" title="加入"><i class="fas fa-plus"></i></button>
                                    </div>
                                </td>
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
        props: ['tenderItem','sepHeader'],
        data: function () {
            return {
                targetId: null,
                selectitems: [],
                selectLevel: '',
                selectitems2: [],
                selectLevel2: '',
                keyword: '',
                //
                payItems: [],

                items: [],
                editSeq: -99,
                editRecord: {},
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 0,
                //設定分項工程
                fAdjPayItem: false,
                adjSubEng: {},
                subPayItems:[],
            };
        },
        methods: {
            //勾稽完成
            fillCompleted() {
                if (confirm('設定完成後將不能再修改\n是否確定? ')) {
                    window.myAjax.post('/EPCSchEngProgressSub/FillCompleted', { id: this.sepHeader.Seq, eId: this.tenderItem.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.$emit('reload');
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },

            //設定分項工程 PayItem
            onAdjPayItem(item) {
                this.subPayItems = [];
                this.fAdjPayItem = true;
                this.adjSubEng = item;
                this.getSubPayItemList();
            },
            //分項工程 PayItem 完成
            onAdjPayItemFinish() {
                this.fAdjPayItem = false;
                this.subPayItems = [];
                this.getResords();
            },
            //移除 PayItem
            onDelSubPayItem(item) {
                window.myAjax.post('/EPCSchEngProgressSub/DelSubPayItem', { id: this.adjSubEng.Seq, pId: item.Seq })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.getSubPayItemList();
                            this.onSearch();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //加入 PayItem
            onAddSubPayItem(item) {
                window.myAjax.post('/EPCSchEngProgressSub/AddSubPayItem', { id: this.adjSubEng.Seq, pId:item.Seq })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.getSubPayItemList();
                            this.onSearch();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分項工程 PayItem
            getSubPayItemList() {
                this.subPayItems = [];
                window.myAjax.post('/EPCSchEngProgressSub/GetSubPayItemList', { id: this.adjSubEng.Seq })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.subPayItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //
            //分項工程清單
            getResords() {
                this.editSeq = -99;
                this.items = [];
                window.myAjax.post('/EPCSchEngProgressSub/GetList', { id: this.tenderItem.Seq })
                    .then(resp => {
                        if (resp.data.result >= 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            
            //新增分項工程
            onAddSunEng() {
                window.myAjax.post('/EPCSchEngProgressSub/AddSunEng', { id: this.sepHeader.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除分項工程
            onDelRecord(item) {
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EPCSchEngProgressSub/DelRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存分項工程
            onSaveRecord() {
                if (this.strEmpty(this.editRecord.EngName)) {
                    alert('分項工程名稱 必須輸入!');
                    return;
                }
                /* s20230623 改為系統計算 經費/總經費
                if (this.editRecord.Weights <= 0 || this.editRecord.Weights > 100) {
                    alert('權重 必須在 1 ~ 100');
                    return;
                }*/
                window.myAjax.post('/EPCSchEngProgressSub/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //PayItem 
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getPayItemList();
            },
            onSearch() {
                this.pageIndex = 1;
                this.getPayItemList();
            },
            //清單
            getPayItemList() {
                var fLevel = this.selectLevel;
                if (this.selectLevel2 != '') fLevel = this.selectLevel2;
                this.editSeq = -99;
                this.payItems = [];
                window.myAjax.post('/EPCSchEngProgressSub/GetPayItemList', {
                    id: this.targetId,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
                    fLevel: fLevel,
                    keyword: this.keyword
                })
                .then(resp => {
                    if (resp.data.result >= 0) {
                        this.payItems = resp.data.items;
                        this.recordTotal = resp.data.totalRows;
                        if (this.selectitems.length == 0) this.getLevel1();
                    } else
                        alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
            },
            //第一階
            getLevel1() {
                this.selectLevel = '';
                this.selectitems = [];
                this.selectLevel2 = '';
                this.selectitems2 = [];
                window.myAjax.post('/EPCSchEngProgressSub/GetLevel1List', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selectitems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //第二階
            onLevelChange() {
                this.selectLevel2 = '';
                this.selectitems2 = [];
                if (this.selectLevel != '') {
                    window.myAjax.post('/EPCSchEngProgressSub/GetLevel2List', { id: this.targetId, key: this.selectLevel })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.selectitems2 = resp.data.items;
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
                //this.getResords();
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                this.getResords();
                this.getLevel1();
                this.onSearch();
            },
        },
        async mounted() {
            console.log('mounted() 前置作業-碳排清單');
            this.targetId = this.tenderItem.Seq;
            this.getItem();
        },
    }
</script>
<style scoped>
.tableFixHead          { overflow: auto; max-height: 500px;   }
table {
    border-collapse: separate;
    border-spacing: 0;
}
.table {
    margin : 0;
}
.tableFixHead thead  { position: sticky !important ; top: 0 !important ; z-index: 1 !important;     }
th {
    border : 0;
    border-bottom: #ddd solid 1px !important; 
    border-left : 0 !important;
    border-right:0 !important;
}
td {
    z-index: 0;
    position: relative;
}</style>