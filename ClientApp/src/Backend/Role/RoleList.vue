<template>
    <div>
        <!--<ol class="breadcrumb">
            <li class="breadcrumb-item">
                後台管理
            </li>
            <li class="breadcrumb-item active" aria-current="page" title="角色管理">
                角色管理
            </li>
        </ol>
        <h1>角色管理</h1>-->
        <div class="row ">
            <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
                <a href="javascript:void(0)" @click="onEdit(0)" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                    <i class="fas fa-plus"></i>&nbsp;新增
                </a>
            </div>
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th>項次</th>
                        <th>角色</th>
                        <th>角色說明</th>
                        <th>是否為系統預設角色</th>
                        <th>編輯</th>
                        <th>刪除</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-bind:key="index" v-for="(item,index) in roles">
                        <td style="text-align: center;">{{item.Rows}}</td>
                        <td style="text-align: center;">{{item.Name}}</td>
                        <td>{{item.RoleDesc}}</td>
                        <td style="text-align: center;">{{item.IsDefault ? '是' : '否'}}</td>
                        <td style="text-align: center;">
                            <a href="javascript:void(0)" @click="onEdit(item)" class="btn btn-color11-2 btn-xs m-1" title="編輯"><i class="fas fa-pencil-alt"></i>  編輯</a>
                        </td>
                        <td style="text-align: center;">
                            <a v-if="item.IsDefault==false" href="javascript:void(0)" @click="onDelete(item.Seq)" class="btn btn-color9-1 btn-xs m-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                        </td>
                    </tr>
                    <tr v-if="roles==null||roles.length==0">
                        <td colspan="6" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
            <div style="width:99%;" class="row justify-content-center">
                <!--v-on:change="onPageChange"-->
                <b-pagination :total-rows="totalRows"
                              :per-page="perPage"
                              v-model="currentPage">
                </b-pagination>
                <!--<div>
                    目前的排序鍵值: <b>{{ sortBy }}</b>, 排序方式:
                    <b>{{ sortDesc ? 'Descending' : 'Ascending' }}</b>
                </div>-->
            </div>
        </div>
        <div ref="divEditDialog" style="display:none;">
            <div class="table-responsive">
                <table border="0" class="table table2" style="width: 95%!important;">
                    <tbody>
                        <tr>
                            <th>角色</th>
                            <td>
                                <input v-model="roleEdit.Name" type="text" class="form-control" style="width: 98% !important;">
                            </td>
                        </tr>
                        <tr>
                            <th>角色說明</th>
                            <td>
                                <input v-model="roleEdit.RoleDesc" type="text" class="form-control" style="width: 98% !important;">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="row justify-content-center">
                                    <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
                                        <a href="javascript:void(0)" @click="onSave" role="button" class="btn btn-color11-3 btn-xs mx-1">
                                            <i class="fas fa-save"></i> 儲存
                                        </a>
                                    </div>
                                    <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
                                        <a href="javascript:void(0)" @click="onCancel" role="button" class="btn btn-color9-1 btn-xs mx-1">
                                            <i class="fas fa-times"></i> 關閉
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios';
    import moment from 'moment';

    // Suppress the warnings
    moment.suppressDeprecationWarnings = true;

    export default {
        computed: {
            editableFields() {
                return this.fields.filter(field => {
                    return field.editable == true
                })
            }
        },
        data() {
            return {
                currentPage: 1,
                perPage: 10,
                totalRows: 0,
                // 角色列表
                roles: [],
                roleSeq: '0',
                // 編輯的角色資料
                roleEdit: {}
            };
        },
        methods: {
            // 取得角色列表
            getList() {
                var formData = new FormData();
                const self = this;
                let params = { page: self.currentPage, per_page: self.perPage };
                axios.post('/Role/GetList', params)
                    .then(resp => {
                        self.roles = resp.data.l;
                        self.totalRows = resp.data.t;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onEdit(item) {
                const self = this;
                if (item == 0 || item == null) {
                    self.roleEdit = {};
                    self.roleEdit.Seq = 0;
                } else {
                    self.roleEdit = JSON.parse(JSON.stringify(item));
                }
                Vue.prototype.common.createDialog($(self.$refs.divEditDialog), "編輯角色資料", 650, true);
            },
            onCancel() {
                const self = this;
                Vue.prototype.common.closeDialog(self.$refs.divEditDialog);
            },
            // 角色資料編輯
            onSave() {
                const self = this;
                if (self.roleEdit.Name == null || self.roleEdit.Name == '') {
                    alert("角色名稱不能是空的!");
                    return false;
                }
                let params = JSON.parse(JSON.stringify(self.roleEdit));
                axios.post('/Role/Save', params)
                    .then(resp => {
                        //console.log(resp.data);
                        Vue.prototype.common.showResultMessage(resp);
                        self.onCancel();
                        if (resp.data.IsSuccess) {
                            self.getList();
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            // 刪除角色
            onDelete(seq) {
                const self = this;
                if (confirm("確定要刪除?")) {
                    let params = { 'seq': seq }
                    axios.post('/Role/DeleteRole', params)
                        .then(resp => {
                            Vue.prototype.common.showResultMessage(resp);
                            if (resp.data.IsSuccess) {
                                self.getList();
                            }
                            console.log(resp.data);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            }
        },
        mounted() {
            const self = this;
            // 資料初始化
            self.getList();
            self.items = null;
        },
        watch: {
            // 顯示目前頁數的資料
            currentPage: {
                handler: function (value) {
                    this.getList();
                }
            }
        }
    }
</script>