<template>
    <div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover table2">
                <thead class="insearch">
                    <tr>
                        <th><strong>機具名稱</strong></th>
                        <th><strong>型號</strong></th>
                        <th><strong>本日使用數量</strong></th>
                        <th><strong>累計使用數量</strong></th>
                        <th><strong>本日使用時數(小時)</strong></th>
                        <th><strong>碳排量(kgCO2e)</strong></th>
                        <th><strong>累計使用時數(小時)</strong></th>
                        <th class="text-center"><strong>管理</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{item.EquipmentName}}</td>
                        <template v-if="item.Seq != editSeq">
                            <td>{{item.EquipmentModel}}</td>
                            <td>{{item.TodayQuantity}}</td>
                            <td>{{item.AccQuantity}}</td>
                            <td>{{item.TodayHours}}</td>
                            <td>{{item.KgCo2eAmount}}</td>
                            <td>{{item.AccHours}}</td>
                            <td style="min-width: unset;">
                                <div v-if="canEditUser && !isDisabled" class="d-flex justify-content-center">
                                    <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                    <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                </div>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSeq">
                            <td>{{item.EquipmentModel}} <!--<input v-model="editRecord.EquipmentModel" class="form-control">--></td>
                            <td><input v-model.number="editRecord.TodayQuantity" type="number" class="form-control"></td>
                            <td></td>
                            <td><input v-model.number="editRecord.TodayHours" type="number" class="form-control"></td>
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
                    <tr v-if="canEditUser && !isDisabled">
                        <td>
                            <select v-model.trim="newItem.EquipmentName" @change="onNewKindChange($event)" class="form-control">
                                <option v-for="item in kindOptions" v-bind:key="item.Text" v-bind:Value="item.Value">
                                    {{item.Text}}
                                </option>
                            </select>
                            <input v-if="newItem.EquipmentName == '非上述'" v-model.trim="newEquipmentName" maxlength="50" type="text" class="form-control">
                        </td>
                        <td>
                            <template v-if="newItem.EquipmentName != '非上述'">
                                <select v-model="selEquipmentModel" @change="onEquipmentModelChange($event)" class="form-control">
                                    <option v-for="item in specOptions" v-bind:key="item.NameSpec" v-bind:Value="JSON.stringify(item)">
                                        {{item.NameSpec}}
                                    </option>
                                </select>
                                <span v-if="selEquipmentModel != null">{{newItem.KgCo2e}} kgCO2e/hr</span>
                            </template>
                            <input v-if="newItem.EquipmentName == '非上述'" v-model.trim="newItem.EquipmentModel" maxlength="100" type="text" class="form-control">
                        </td>
                        <td><input v-model.number="newItem.TodayQuantity" type="number" class="form-control"></td>
                        <td></td>
                        <td><input v-model.number="newItem.TodayHours" type="number" class="form-control"></td>
                        <td></td>
                        <td></td>
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
                items: [],
                editSeq: -99,
                editRecord: {},
                newItem: {},
                newEquipmentName: "",
                //
                kindOptions: [],
                specOptions: [],
                selEquipmentModel: null, //s20230502
            };
        },
        methods: {
            //設備規格 s20230502
            onEquipmentModelChange($event) {
                if (this.selEquipmentModel == null) return;
                let model = JSON.parse(this.selEquipmentModel);
                console.log(model);
                this.newItem.EquipmentModel = model.NameSpec;
                this.newItem.KgCo2e = model.KgCo2e;
            },
            onNewRecord() {
                window.myAjax.post('/ECProgressManage/NewEquipmentRecord')
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
                    window.myAjax.post('/ECProgressManage/DelEquipmentRecord', { id: item.Seq })
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
                var tarItem = Object.assign({}, uItem);
                if (tarItem.EquipmentName == '非上述') {
                    tarItem.EquipmentName = this.newEquipmentName;
                }
                if (this.strEmpty(tarItem.EquipmentName) || !window.comm.isNumber(tarItem.TodayQuantity) || !window.comm.isNumber(tarItem.TodayHours)) {
                    alert('機具名稱, 本日使用數量,本日使用時數 必須輸入!');
                    return;
                }
                
                window.myAjax.post('/ECProgressManage/UpdateEquipmentRecords', { m: tarItem })
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
                window.myAjax.post('/ECProgressManage/GetEquipmentRecords', { id: this.supDailyItem.Seq})
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //機具清單
            getEquipmentOptions() {
                this.items = [];
                window.myAjax.post('/ECProgressManage/GetEquipmentOptions')
                    .then(resp => {
                        this.kindOptions = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onNewKindChange() {
                if (this.newItem.EquipmentName == '非上述') {
                    this.specOptions = [];
                    this.newEquipmentName = "";
                    this.clearSelItem();
                    return;
                }
                this.getEquipmentSpecOptions(this.newItem.EquipmentName);
            },
            //機具規格清單
            getEquipmentSpecOptions(kind) {
                this.clearSelItem();
                this.items = [];
                window.myAjax.post('/ECProgressManage/GetEquipmentSpecOptions', { kind:kind })
                    .then(resp => {
                        this.specOptions = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //清除資料 s20230502
            clearSelItem() {
                this.selEquipmentModel = null;
                this.newItem.EquipmentModel = "";
                this.newItem.KgCo2e = null;
            }
        },
        computed: {//s20231116
            isDisabled: function () {
                if (this.supDailyItem == null || this.supDailyItem.Seq == -1 || this.supDailyItem.ItemState == 1) return true;
                return false;
            }
        },
        mounted() {
            console.log('mounted 工程變更-機具管理');
            this.getEquipmentOptions();
            this.getResords();
            this.onNewRecord();
        }
    }
</script>