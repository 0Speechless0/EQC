<template>
    <div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover table2">
                <thead class="insearch">
                    <tr>
                        <th><strong>工別</strong></th>
                        <th><strong>本日人數</strong></th>
                        <th><strong>累計人數</strong></th>
                        <th class="text-center"><strong>管理</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{item.KindName}}</td>
                        <template v-if="item.Seq != editSeq">
                            <td>{{item.TodayQuantity}}</td>
                            <td>{{item.AccQuantity}}</td>
                            <td style="min-width: unset;">
                                <div v-if="canEditUser && !isDisabled" class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td><input v-model="editRecord.TodayQuantity" class="form-control"></td>
                            <td> <!--<input v-model="editRecord.AccQuantity" type="text" class="form-control">--></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onSaveRecord(editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                    <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                    <tr v-if="canEditUser && !isDisabled">
                        <td>
                            <select v-model.trim="newItem.KindName" class="form-control">
                                <option v-for="item in selectItems" v-bind:key="item.Text" v-bind:Value="item.Value">
                                    {{item.Text}}
                                </option>
                            </select>
                        </td>
                        <td><input v-model="newItem.TodayQuantity" type="text" class="form-control"></td>
                        <td> <!--<input v-model="newItem.AccQuantity" type="text" class="form-control">--></td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord(newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['supDailyItem', 'canEditUser'],
        data: function () {
            return {
                selectItems: [],
                items: [],
                editSeq: -99,
                editRecord: {},
                newItem: {},
            };
        },
        methods: {
            onNewRecord() {
                window.myAjax.post('/ECProgressManage/NewPersonRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newItem = resp.data.item;
                            this.newItem.EC_SupDailyDateSeq = this.supDailyItem.Seq;
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
                    window.myAjax.post('/ECProgressManage/DelPersonRecord', { id: item.Seq })
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
                console.log(uItem);
                if (this.strEmpty(uItem.KindName) || uItem.TodayQuantity == null /*|| uItem.AccQuantity == null*/) {
                    alert('工別, 本日人數 必須輸入!');
                    return;
                }
                window.myAjax.post('/ECProgressManage/UpdatePersonRecords', { m: uItem })
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
                window.myAjax.post('/ECProgressManage/GetPersonRecords', { id: this.supDailyItem.Seq})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //材料清單
            getMaterialOptions() {
                this.items = [];
                window.myAjax.post('/ECProgressManage/GetPersonOptions')
                    .then(resp => {
                        this.selectItems = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        computed: {//s20231116
            isDisabled: function () {
                if (this.supDailyItem == null || this.supDailyItem.Seq == -1 || this.supDailyItem.ItemState == 1) return true;
                return false;
            }
        },
        mounted() {
            console.log('mounted 工程變更-工地人員');
            this.getMaterialOptions();
            this.getResords();
            this.onNewRecord();
        }
    }
</script>