<template>
    <div>
        <h5>已核定清單</h5>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md">
                <thead class="insearch">
                    <tr>
                        <th style="width:80px"><strong>工程年度</strong></th>
                        <th><strong>工程編號</strong></th>
                        <th><strong>工程名稱</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>執行單位</strong></th>
                        <th style="width:100px"><strong>負責人</strong></th>
                        <th class="text-center" style="width:180px"><strong>功能</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item) in items" v-bind:key="item.Seq">
                        <td>{{item.EngYear}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <template v-if="editSeq != item.Seq">
                            <td>{{item.ExecUnitName}}</td>
                            <td>{{item.ExecSubUnitName}}</td>
                            <td>{{item.ExecUserName}}</td>
                            <td>
                                <div class="row justify-content-center m-0">
                                    <button v-on:click.stop="editEA(item)" class="btn btn-color11-3 btn-xs mx-1" title="設定">
                                        <i class="fas fa-pencil-alt"></i> 設定
                                    </button>
                                    <button v-if="item.OrganizerUserSeq > 0" v-on:click.stop="editEng(item)" class="btn btn-color11-2 btn-xs mx-1" title="編輯">
                                        <i class="fas fa-pencil-alt"></i> 編輯
                                    </button>
                                </div>
                                <div class="row justify-content-center m-0 p-1">
                                    <button v-on:click.stop="delItem(item)" class="btn btn-color9-1 btn-xs mx-1" title="合併/刪除">
                                        合併/刪除
                                    </button>
                                </div>
                            </td>
                        </template>
                        <template v-if="editSeq == item.Seq">
                            <td>
                                <select v-model="editItem.ExecUnitSeq" @change="getExecSubUnit(editItem.ExecUnitSeq)"
                                        class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                    <option v-for="option in units" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <select v-model="editItem.ExecSubUnitSeq" @change="getUsers()" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                    <option v-for="option in execSubUnits" v-bind:value="option.Value"
                                            v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <select v-model="editItem.OrganizerUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                    <option v-for="option in users" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <div class="row justify-content-center m-0">
                                    <button v-on:click.stop="saveItem(editItem)" class="btn btn-color11-2 btn-xs mx-1" title="確定">
                                        <i class="fas fa-save"></i> 確定
                                    </button>
                                    <button @click="editSeq = -99" class="btn btn-color9-1 btn-xs mx-1" title="取消"><i class="fas fa-times"></i> 取消</button>
                                </div>
                            </td>
                        </template>
                    </tr>
                </tbody>
            </table>
        </div>
        <button v-show="false" id="btnEditEngModal"  data-toggle="modal" data-target="#refEditEngModal" role="button" class="btn btn-outline-secondary btn-xs mx-1">工程基本資料
        </button>
        <div v-if="editItem != null" class="modal fade" id="refEditEngModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refEditEngModal" aria-modal="true">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title" id="projectUpload">工程基本資料</h6>
                        <button @click="editItem = null" ref="btnCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <EngApprovalEdit v-bind:engApprovalItem="editItem" v-on:createEng="createEng" ></EngApprovalEdit>
                    </div>
                    <!-- div class="modal-footer">
                        <button type="button" class="btn btn-primary">
                            儲存
                        </button>
                        <button type="button" id="closeCopyEngModal" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
                            取消
                        </button>
                    </div -->
                </div>
            </div>
        </div>

    </div>
</template>
<script>
    export default {
        components: {
            EngApprovalEdit: require('./EngApprovalEdit.vue').default, //s20231006
        },
        data: function () {
            return {
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,

                //s20231006
                items: [],
                editSeq: -99,
                editItem: null,
                users: [],//人員清單
                units: [],//機關清單
                execSubUnits: [],//執行機關單位清單
                //s20231106
                selUnit:-1,
            };
        },
        methods: {
            //s20231006
            createEng(engMain) {
                window.myAjax.post('/TenderPlan/CreateApprovalEng', {
                    id: this.editItem.Seq, eng: engMain
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            window.myAjax.post('/TenderPlan/EditEng', { seq: resp.data.id })
                                .then(resp => {
                                    window.location.href = resp.data.Url;
                                })
                                .catch(err => {
                                    console.log(err);
                                });
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            editEng(item) {
                this.editItem = Object.assign({}, item);
                this.$nextTick(() => {
                    //console.log(show.value, content.value)
                    document.getElementById('btnEditEngModal').click();
                })
                /* s20231105 取消
                if (item.EngMainSeq > 0) {
                    window.myAjax.post('/TenderPlan/EditEng', { seq: item.EngMainSeq })
                        .then(resp => {
                            window.location.href = resp.data.Url;
                        })
                        .catch(err => {
                            console.log(err);
                        });
                } else {
                    this.editItem = Object.assign({}, item);
                    this.$nextTick(() => {
                        //console.log(show.value, content.value)
                        document.getElementById('btnEditEngModal').click();
                    })
                }*/
            },
            delItem(item) {
                if (confirm('是否確定刪除該核定工程資料?\n工程立號清單內該工程並不會刪除, 若需刪除請聯絡管理者')) {
                    window.myAjax.post('/EngApprovalImport/DelItem', {
                        id: item.Seq
                    })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getList(this.selUnit);
                            } else {
                                alert(resp.data.msg);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            saveItem(item) {
                if (item.OrganizerUserSeq <= 0) {
                    alert('請選取負責人');
                    return;
                }
                window.myAjax.post('/EngApprovalImport/UpdateUnit', {
                    item: item
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getList(this.selUnit);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            editEA(item) {
                if (this.editSeq > -99) return;
                this.editItem = Object.assign({}, item);
                this.editSeq = this.editItem.Seq;
                this.getExecSubUnit(this.editItem.ExecUnitSeq);
                this.getUsers();
            },
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList(this.selUnit);
            },
            //工程核定資料
            getList(selectUnit) {
                this.selUnit = selectUnit;
                this.editSeq = -99;
                this.editItem = null;
                this.items = [];
                window.myAjax.post('/EngApprovalImport/GetEngList', {
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex,
                    unit: selectUnit
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.pTotal;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //機關清單
            async getUnits() {
                this.units = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: -1 });
                this.units = data;
            },
            //執行機關-單位清單
            async getExecSubUnit(unitSeq) {
                this.execSubUnits = [];
                this.users = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: unitSeq });
                this.execSubUnits = data;
            },
            //人員清單
            async getUsers() {
                this.users = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.editItem.ExecUnitSeq, organizerSubUnitSeq: this.editItem.ExecSubUnitSeq });
                this.users = data;
            },
        },
        mounted() {
            console.log('mounted 工程核定資料');
            //this.getList();
            this.getUnits();
        }
    }
</script>
