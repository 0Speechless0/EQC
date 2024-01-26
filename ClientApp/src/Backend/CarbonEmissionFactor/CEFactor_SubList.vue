<template>
    <div>
        <h5 class="insearch mt-0 py-2">
            <div class="row">
                <div class="col-11">
                    工作項目：{{masterItem.Item}}<br>計價代碼：{{masterItem.Code}}&nbsp;&nbsp;&nbsp;&nbsp;單位：{{masterItem.Unit}}
                </div>
                <div class="ml-auto mr-1">
                    <button @click="closeSubList" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                </div>
            </div>
        </h5>
        <div class="row justify-content-between">
            <comm-pagination class="col-7" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <div v-if="unitItem.Seq > -1" class="col-3 d-flex justify-content-center small">
                <span style="margin-top: 6px">工料分析基本量:&nbsp;</span>
                <input v-model.number="unitItem.Amount" type="text" class="form-control" style="max-width: 80px">
                <button @click="onSaveRecord(unitItem)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
            </div>
            <div class="col-2 d-flex small" style="margin-top: 6px">
                工項碳排係數:&nbsp;<b>{{unitItem.Quantity}}</b>
            </div>
        </div>
        <table class="table table-responsive-md table-hover VA-middle">
            <thead class="insearch">
                <tr class="insearch">
                    <th>工料名稱</th>
                    <th>單位</th>
                    <th>數量</th>
                    <th>單位碳排量</th>
                    <th>複價</th>
                    <th>編碼(備註)</th>
                    <th>
                        <div class="d-flex">
                            <a href="" class="btn btn-color11-3 btn-xs sharp mr-1" data-toggle="modal" data-target="#prepare_edit01" title="編輯"><i class="fas fa-plus"></i></a>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                    <template v-if="item.Seq != editSeq">
                        <td>{{item.NameSpec}}</td>
                        <td>{{item.Unit}}</td>
                        <td>{{item.Amount}}</td>
                        <td v-if="item.ItemMode<90000">{{item.KgCo2e}}</td>
                        <td v-if="item.ItemMode>=90000"></td>
                        <td>{{item.Quantity}}</td>
                        <td>{{item.Memo}}</td>
                        <td style="min-width: unset;">
                            <div v-if="item.ItemMode<90001" class="d-flex justify-content-center">
                                <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button v-if="item.ItemMode<90000" @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                            </div>
                        </td>
                    </template>
                    <template v-if="item.Seq == editSeq && item.ItemMode<90000">
                        <td>{{item.NameSpec}}</td>
                        <td>{{item.Unit}}</td>
                        <td><input v-model.number="editRecord.Amount" type="text" class="form-control"></td>
                        <td>{{item.KgCo2e}}</td>
                        <td>{{item.Quantity}}</td>
                        <td>{{item.Memo}}<!-- input v-model="editRecord.Memo" maxlength="100" rows="5" class="form-control" --></td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </template>
                    <template v-if="item.Seq == editSeq && item.ItemMode==90000">
                        <td>{{item.NameSpec}}</td>
                        <td>{{item.Unit}}</td>
                        <td><input v-model.number="editRecord.Amount" type="text" class="form-control"></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </template>
                </tr>
            </tbody>
        </table>

        <!-- 小視窗 -->
        <div class="modal fade" id="prepare_edit01">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-body">
                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                            <MMFMList v-on:addSubItem="addSubItem"></MMFMList>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button data-dismiss="modal" aria-label="Close" type="button" class="btn btn-color3">關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['masterItem'],
        data: function () {
            return {
                listMode: true,
                targetId: null,
                unitItem: {Seq:-1},
                items: [],
                totalScore: -1,
                editSeq: -99,
                editRecord: {},
                newItem: {},
                //分頁
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                lastUpdate: null,
                searchStr:null
            };
        },
        components: {
            MMFMList: require('./MMFM_Main.vue').default,
        },
        methods: {
            //新增碳排項目
            addSubItem(item, amount, memo) {
                let uItem = {
                    Seq: -1,
                    CarbonEmissionFactorSeq: this.masterItem.Seq,
                    NameSpec: item.NameSpec,
                    Unit: item.Unit,
                    KgCo2e: item.KgCo2e,
                    Amount: amount,
                    Memo: memo
                };
                this.onSaveRecord(uItem);
            },
            closeSubList() {
                this.$emit('closeSubList');
            },
            search() {
                this.getResords();
            },
            onNewRecord() {
                window.myAjax.post('/CEFactor/NewSubRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除紀錄
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/CEFactor/DelSubRecord', { m: item })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
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
            //儲存
            onSaveRecord(uItem) {
                if (!window.comm.isNumber(uItem.Amount)) {
                    alert('數量 必須輸入!');
                    return;
                }
                if (uItem.Amount==0) {
                    alert('數量 必須大於0');
                    return;
                }
                window.myAjax.post('/CEFactor/UpdateSubRecord', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getResords();
                            if (uItem.Seq == -1) this.onNewRecord();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
                this.getResords();
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //紀錄清單
            getResords() {
                this.items = [];
                window.myAjax.post('/CEFactor/GetSubRecords', {
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex,
                        id: this.masterItem.Seq
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                            this.unitItem = resp.data.unitItem;
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
                this.getResords();
            },
        },
        mounted() {
            console.log('mounted() 碳排係數分析');
            this.onNewRecord();
            this.getResords();
        }
    }
</script>