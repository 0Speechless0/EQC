<template>
    <div>
        <div class="row d-flex bd-highlight mb-3" style="padding-left:10px">
            <div class="d-flex pl-3 pb-2">
                <input v-model.trim="searchStr" />
                <button type="button" class="btn btn-outline-success" style="color: #fff;background-color: #6c757d;" @click="search()">查詢</button>
            </div>
            <div class="align-self-center ml-2" style="color:red">
                可輸入名稱及規格
            </div>
        </div>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover VA-middle">
                <thead class="insearch">
                    <tr>
                        <th>ID</th>
                        <th>編碼</th>
                        <th>名稱</th>
                        <th>單位碳排量</th>
                        <th>單位</th>
                        <th>備註</th>
                        <!-- th class="text-center">管理</th -->
                        <th>
                            <div class="d-flex justify-content-center">
                                <a v-on:click.stop="fAddItem=true" href="##" class="btn btn-color11-3 btn-xs sharp mx-1" title="新增"><i class="fas fa-plus"></i></a>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="fAddItem">
                        <td></td>
                        <td><input v-model.trim="newItem.ItemCode" maxlength="20" type="text" class="form-control"></td>
                        <td><input v-model.trim="newItem.NameSpec" maxlength="200" type="text" class="form-control"></td>
                        <td><input v-model.number="newItem.KgCo2e" type="number" class="form-control"></td>
                        <td><input v-model.trim="newItem.Unit" maxlength="30" type="text" class="form-control"></td>
                        <td><input v-model.trim="newItem.Memo" maxlength="100" type="text" class="form-control"></td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-save"></i></button>
                                <button @click="fAddItem=false" class="btn btn-outline-secondary btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td style="min-width: unset;">{{item.Seq}}</td>
                        <template v-if="item.Seq != editSeq">
                            <td>{{item.ItemCode}}</td>
                            <td>{{item.NameSpec}}</td>
                            <td>{{item.KgCo2e}}</td>
                            <td>{{item.Unit}}</td>
                            <td>{{item.Memo}}</td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onAddSelRecord(item)" class="btn btn-color11-3 btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                                    <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><input v-model.trim="editRecord.ItemCode" maxlength="20" type="text" class="form-control"></td>
                            <td><input v-model.trim="editRecord.NameSpec" maxlength="200" type="text" class="form-control"></td>
                            <td><input v-model.number="editRecord.KgCo2e" type="number" class="form-control"></td>
                            <td><input v-model.trim="editRecord.Unit" maxlength="30" type="text" class="form-control"></td>
                            <td><input v-model.trim="editRecord.Memo" maxlength="100" type="text" class="form-control"></td>
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
        </div>
        <div class="modal fade" v-bind:class="{'show':modelShow}" v-bind:style="{display: modelShow ? 'block' : 'none'}" id="case_01">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        建立單位：{{selItem.CreateUnit}}&nbsp;&nbsp;編號：{{selItem.ItemCode}}<br>
                        名稱及規格：{{selItem.NameSpec}}<br>單位碳排量：{{selItem.KgCo2e}}<br>單位：{{selItem.Unit}}
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col"></div>
                            <div class="col">數量:</div>
                            <div class="col"></div>
                        </div>
                        <div class="row">
                            <div class="col"></div>
                            <div class="col"><input ref="iAmount" v-model.number="addAmount" @focus="$event.target.select()" type="number" class="form-control"></div>
                            <div class="col"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button @click="onAddRecord" type="button" class="btn btn-color3">儲存</button>
                        <button @click="modelShow=false" type="button" class="btn btn-color3">關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                addAmount: 0,
                modelShow: false,
                selItem: {},
                //
                items: [],
                editSeq: -99,
                editRecord: {},
                newItem: {},
                fAddItem: false,
                //分頁
                recordTotal: 0,
                pageIndex: 1,
                pageRecordCount: 30,
                lastUpdate: null,
                searchStr: '' //s20230318
            };
        },
        methods: {
            search() {//s20230318
                this.getResords();
            },
            onAddRecord() {
                if (!window.comm.isNumber(this.addAmount)) {
                    alert('必須輸入正確數值資料');
                    this.$refs.iAmount.focus();
                    return;
                }
                if (this.addAmount == 0) {
                    alert('數量 必須大於0');
                    return;
                }
                this.$emit('addSubItem', this.selItem, this.addAmount, '自-' + this.selItem.Seq);
                this.modelShow = false;
            },
            onAddSelRecord(item) {
                this.selItem = item;
                this.modelShow = true;
                this.$nextTick(() => this.$refs.iAmount.focus());
            },
            onNewRecord() {
                window.myAjax.post('/MMFMaintenance/NewCustomizeRecord')
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
                    window.myAjax.post('/MMFMaintenance/DelCustomizeRecord', { id: item.Seq })
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
                //console.log(uItem);
                if (this.strEmpty(uItem.ItemCode) || this.strEmpty(uItem.NameSpec) || this.strEmpty(uItem.Unit) || !window.comm.isNumber(uItem.KgCo2e) ) {
                    alert('編碼,名稱,單位碳排量,單位 必須輸入!');
                    return;
                }
                window.myAjax.post('/MMFMaintenance/UpdateCustomizeRecord', { m: uItem })
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
                this.fAddItem = false;
                this.items = [];
                window.myAjax.post('/MMFMaintenance/GetCustomizeRecords', {
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex,
                    keyWord: this.searchStr
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                            this.lastUpdate = resp.data.lastUpdate;
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
            console.log('mounted() 自定義-維護功能');
            this.getResords();
            this.onNewRecord();
        }
    }
</script>