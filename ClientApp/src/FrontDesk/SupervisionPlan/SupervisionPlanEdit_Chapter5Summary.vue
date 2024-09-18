<template>
    <div>
        <div class="row justify-content-between">
            <!-- div class="col">
            <h2>材料設備送審管制總表</h2>
        </div -->
            <div v-if="fCanEdit" class="col-12 col-md-5 col-xl-3 mt-3">
                <button v-on:click.stop="newItem()" class="btn btn-shadow btn-outline-secondary btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增材料項目
                </button>
            </div>
        </div>
        <div class="d-flex mt-2 mb-2">
            <comm-pagination :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <button class="btn btn-color11-3 btn-xs ml-4"  v-if="!merging" @click="merging= true"><i class="fas  fa-compress-arrows-alt" > 合併項目</i></button>
            <button class="btn btn-color11-2 btn-xs ml-4"  v-else @click="onMergeItems"><i class="fas  fa-compress-arrows-alt"> 合併</i></button>
            <div class="form-inline ml-4" v-if="merging">
                <label for="i1">合併列舉&nbsp;&nbsp;</label>
                <input  type="text" class="form-control " id="i1" style="width: 200px;" v-model="mergeItemOrderA" />
                <span style="color:red" class="ml-4"> 範例 : 23, 24, 25 ...</span>
            </div>

        </div>

        <div class="table-responsive tableFixHead">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th rowspan="2" class="sort">
                            <div v-if="fCanEdit" class="custom-control custom-checkbox">
                                <input @change="onCheckAllChange($event)" type="checkbox" class="custom-control-input" id="check_All" value="true">
                                <label class="custom-control-label" for="check_All">All</label>
                            </div>
                        </th>
                        <th class="sort" rowspan="2">項次</th>
                        <th rowspan="2">契約詳細表項次</th>
                        <th rowspan="2">材料/設備名稱</th>
                        <th colspan="2"  style="border:0 !important; ">契約數量</th>
                        <th rowspan="2">是否取樣試驗</th>
                        <th rowspan="2">是否驗廠</th>
                        <th rowspan="2">協力廠商資料</th>
                        <th rowspan="2">型錄</th>
                        <th rowspan="2">相關試驗報告</th>
                        <th rowspan="2">樣品</th>
                        <th rowspan="2">其他</th>
                        <th v-if="fCanEdit" rowspan="2"></th>
                    </tr>
                    <tr>
                        <th>數量</th>
                        <th>單位</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'bg-gray':item.DataType>0}">
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input :disabled="!fCanEdit" v-model="item.DataKeep" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'sort'+index">
                                <label class="custom-control-label" v-bind:for="'sort'+index"></label>
                            </div>
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.number="item.OrderNo" @change="onInputChange(item)" type="text" class="form-control" />
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.trim="item.ItemNo" @change="onInputChange(item)" maxlength="20" type="text" class="form-control" />
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.trim="item.MDName" @change="onInputChange(item)" maxlength="60" type="text" class="form-control" />
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.number="item.ContactQty" @change="onInputChange(item)" maxlength="60" type="text" class="form-control" />
                        </td>
                        <td>
                            <input :disabled="!fCanEdit" v-model.trim="item.ContactUnit" @change="onInputChange(item)" maxlength="60" type="text" class="form-control" />
                        </td>
                        <td class="text-center">
                            <select :disabled="!fCanEdit" v-model.number="item.IsSampleTest" @change="onInputChange(item)" class="form-control">
                                <option value="true">是</option>
                                <option value="false">否</option>
                            </select>
                        </td>
                        <td class="text-center">
                            <select :disabled="!fCanEdit" v-model.number="item.IsFactoryInsp" @change="onInputChange(item)" class="form-control">
                                <option value="true">是</option>
                                <option value="false">否</option>
                            </select>
                        </td>
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input v-model="item.IsAuditVendor" :disabled="!fCanEdit" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'info_'+index">
                                <label class="custom-control-label" v-bind:for="'info_'+index"></label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input v-model="item.IsAuditCatalog" :disabled="!fCanEdit" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'catalog_'+index">
                                <label class="custom-control-label" v-bind:for="'catalog_'+index"></label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input v-model="item.IsAuditReport" :disabled="!fCanEdit" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'report_'+index">
                                <label class="custom-control-label" v-bind:for="'report_'+index"></label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="custom-control custom-checkbox">
                                <input v-model="item.IsAuditSample" :disabled="!fCanEdit" @change="onInputChange(item)" type="checkbox" class="custom-control-input" v-bind:id="'sample_'+index">
                                <label class="custom-control-label" v-bind:for="'sample_'+index"></label>
                            </div>
                        </td>
                        <td>
                            <input v-model="item.OtherAudit" :disabled="!fCanEdit" @change="onInputChange(item)" maxlength="50" type="text" class="form-control">
                        </td>
                        <td v-if="fCanEdit">
                            <!-- <button v-if="item.DataType==1" v-on:click.stop="delItem(index, item)" href="#" class="mx-1 btn-color9-1 btn btn-block" title="刪除"><i class="fas fa-trash-alt"></i></button> -->
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <comm-pagination :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <!-- div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div -->
    </div>
</template>
<script>
    export default {
        props: ['engMain'],
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        data: function () {
            return {
                fCanEdit: false,
                items: [],
                merging : false,
                targetItem: { ItemName: '', FlowCharOriginFileName: '' },
                //分頁
                pageIndex: 1,
                perPage: 30,
                totalRows: 0,
                mergeItemOrderA : [],
                mergeItemOrderB : []
            };
        },
        methods: {
            onMergeItems()
            {   
                var orderDic = {};
                this.items.forEach( e => {
                    orderDic[e.OrderNo] = e.Seq;
                })
                var str = this.mergeItemOrderA.split(',')
                    .reduce((a, c) => a + "," + orderDic[c], "" )
                console.log("fff", str, this.items);
                str = str.length > 0 ? str.slice(1) : "";
                window.myAjax.post("SupervisionPlan/MergeItemsChapter5Summary", { items : str })
                .then(resp => {
                    alert(resp.data);
                    this.getList();
                    this.merging = false;
                })
            },
            onCheckAllChange(event) {
                for (let item of this.items) {
                    item.DataKeep = event.target.checked;
                }
                this.onKeepAllChange();
            },
            //清單
            getList() {
                if (this.engMain.DocState == -1)
                    this.fCanEdit = true;
                else
                    this.fCanEdit = false;
                this.editFlag = false;
                this.items = [];
                let params = { engMain: this.engMain.Seq, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/SupervisionPlan/Chapter5', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.items.sort(function (a, b) {
                            return a.DataKeep - b.DataKeep;
                        })
                        this.totalRows = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onPaginationChange(pInx, pCnt) {//s20230818
                this.pageIndex = pInx;
                this.perPage = pCnt;
                this.getList();
            },
            isItemEmpty(item) {
                if (item.OrderNo==="") {
                    alert('[順序]須輸入資料');
                    return true;
                } else {
                    return false;
                }
            },
            // check All
            onKeepAllChange() {
                /*for (let item of this.items) {
                    if (this.isItemEmpty(item)) {
                        return;
                    }
                }*/
                window.myAjax.post('/SupervisionPlan/Chapter5SaveKeep', { items: this.items })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //項目名稱, Keep儲存
            onInputChange(item) {
                if (!this.fCanEdit)
                if (this.isItemEmpty(item)) {
                    return;
                }
                window.myAjax.post('/SupervisionPlan/Chapter5Save', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            //const resultItem = resp.data.item.Data;
                            //item.modifyDate = resultItem.modifyDate;
                        } else
                            item.DataKeep = resp.data.DataKeep;
                            // alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            newItem() {
                window.myAjax.post('/SupervisionPlan/Chapter5NewItem', { engMain: this.engMain.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            var item = resp.data.item.Data;
                            this.items.push(item);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delItem(index, item) {
                //item.edit = false;
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/SupervisionPlan/Chapter5Del', { seq: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.items.splice(index, 1);
                            } else {
                                alert(resp.data.message);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            }
        },
        async mounted() {
            console.log('mounted() 第五章 材料設備送審管制總表');
            this.getList();
        }
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
}
</style>