<template>
    <div>
        <!--<ol class="breadcrumb">
            <li class="breadcrumb-item">-->
                <!--<a href="/Users/Update" title="後台管理">後台管理</a>-->
                <!--後台管理
            </li>
            <li class="breadcrumb-item active" aria-current="page" title="功能權限管理">
                功能權限管理
            </li>
        </ol>
        <h1>功能權限管理</h1>-->
        <form class="form-group">
            <div class="form-row">

            </div>
        </form>
        <div class="d-flex justify-content-start"> 

            <div class="col-12 col-sm-3 mt-3">
                    <select class="form-control" @change="onChangeSystemType">
                        <option v-bind:key="index" v-for="(item,index) in systemTypeList" v-bind:value="item.Value">{{item.Text}}</option>
                    </select>
            </div>
            <div class="col-12 col-sm-3 mt-3"> </div>
            <div class="col-12 col-sm-3 mt-3">

            <i  @click="(RoleListPage -1)*RoleListPerPage >= 0 ? RoleListPage -- : null" class="fas fa-angle-left btn btn-color9-1  btn-s m-1" style="
                /* background-color: red; */
            "></i>
                            <i @click="RoleList.length   >  (RoleListPage + 1)*RoleListPerPage ? RoleListPage ++ : null" class="fas fa-angle-right btn btn-color9-1  btn-s m-1" style="
                                /* background-color: red; */
                            "></i>
            </div>

        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th>項次</th>
                        <th style="width:150px">功能選單 </th>

                        <th v-for="(r, index2) in RolePagination" :key="index2">{{r.Name}}</th>

                        <!-- <th>署管理者</th>
                        <th>各局管理者</th>
                        <th>施工廠商</th>
                        <th>委外監造</th>
                        <th>委外設計</th>
                        <th>委員</th>
                        <th>各局執行者</th> -->
                    </tr>
                </thead>
                <tbody>
                    <tr v-bind:key="index" v-for="(item,index) in menuRoleList">
                        <td style="text-align: center;">{{index+1}}</td>
                        <td style="text-align: center;">
                            {{item.Name}}
                        </td>
                        <td style="text-align: center;" v-for="(r, index2) in RolePagination" :key="index2">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.Seq, r.Seq, !item.Roles.has(r.Seq) )" v-bind:id="'role'+index+','+index2" :checked='item.Roles.has(r.Seq)' >
                                <label class="custom-control-label" v-bind:for="'role'+index+','+index2">
                                </label>
                            </div>
                        </td>
                        <!-- <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,2,item.Role2)" v-bind:id="'role2'+index" v-model="item.Role2" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role2'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,3,item.Role3)" v-bind:id="'role3'+index" v-model="item.Role3" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role3'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,4,item.Role4)" v-bind:id="'role4'+index" v-model="item.Role4" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role4'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,5,item.Role5)" v-bind:id="'role5'+index" v-model="item.Role5" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role5'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,6,item.Role6)" v-bind:id="'role6'+index" v-model="item.Role6" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role6'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,7,item.Role7)" v-bind:id="'role7'+index" v-model="item.Role7" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role7'+index">
                                </label>
                            </div>
                        </td>
                        <td style="text-align: center;">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" @change="onChangeRoleCheck(item.MenuSeq,20,item.Role20)" v-bind:id="'role20'+index" v-model="item.Role20" v-bind:value="true">
                                <label class="custom-control-label" v-bind:for="'role20'+index">
                                </label>
                            </div>
                        </td> -->
                    </tr>
                    <tr v-if="menuRoleList==null||menuRoleList.length==0">
                        <td colspan="12" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
            <!--<div style="width:99%;" class="row justify-content-center">
                <b-pagination :total-rows="totalRows"
                              :per-page="perPage"
                              v-model="currentPage">
                </b-pagination>
            </div>-->
        </div>
    </div>
</template>
<script>
    import moment from 'moment';

    // Suppress the warnings
    moment.suppressDeprecationWarnings = true;

    export default {
        computed: {
            editableFields() {
                return this.fields.filter(field => {
                    return field.editable == false
                })
            },
            RolePagination()
            {
                return this.RoleList.slice(this.RoleListPage*this.RoleListPerPage, (this.RoleListPage +1)*this.RoleListPerPage );
            }
        },
        data: function () {
            return {
                currentPage: 1,
                perPage: 10,
                totalRows: 0,
                RoleListPage : 0,
                RoleListPerPage : 10,
                RoleList : [],
                // 權限列表
                menuRoleList: [],
                // 系統下拉
                systemTypeList: [],
                // 系統別
                systemTypeSeq: '0',
            };
        },
        methods: {
            // 取得單位下拉選單
            getSystemTypeList() {
                const self = this;
                let params = {};
                window.myAjax.get('/Menu/GetSystemTypeList')
                    .then(resp => {
                        self.systemTypeList = resp.data;
                        self.systemTypeList.splice(0, 0, { Text: '--請選擇--', Value: '0' });
                    })
                    .then(err => {
                        console.log(err);
                    });
            },
            getRoleList()
            {
                window.myAjax.post("Role/GetAll")
                    .then(resp => {

                        this.RoleList = resp.data.l;
                    })
            },
            // 取得權限列表
            getList() {
                const self = this;
                let params = { systemTypeSeq: self.systemTypeSeq };
                window.myAjax.get('/Menu/GetList', { params: params })
                    .then(resp => {
                        self.menuRoleList = resp.data.l;

                        self.menuRoleList.forEach(e => {

                            e.Roles = new Set(e.RoleSeqs)
                        })
                    })
                    .then(err => {
                        console.log(err);
                    });
            },
            // 選擇系統別
            onChangeSystemType(value) {
                const self = this;
                self.systemTypeSeq = value.target.value;
                self.getList();
            },
            // 權限儲存
            onChangeRoleCheck(menuSeq, roleSeq, checked) {
                const self = this;
                let params = { menuSeq: menuSeq, roleSeq: roleSeq, chk: checked };
                window.myAjax.post('/Menu/Save', params)
                    .then(resp => {
                        //Vue.prototype.common.showResultMessage(resp);
                        if (resp.data.IsSuccess) {
                            self.getList();
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            const self = this;
            // 資料初始化
            self.getSystemTypeList();
            this.getRoleList();
        },
        watch: {

        }
    }
</script>