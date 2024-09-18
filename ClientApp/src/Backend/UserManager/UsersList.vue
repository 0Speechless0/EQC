<template>
    <div>
        <!--<ol class="breadcrumb">
    <li class="breadcrumb-item">-->
        <!--<a href="/Users/Update" title="後台管理">後台管理</a>-->
        <!--後台管理
        </li>
        <li class="breadcrumb-item active" aria-current="page" title="使用者管理">
            使用者管理
        </li>
    </ol>
    <h1>使用者管理</h1>-->

        <div class="row ">
            <div v-if="!isLastLevel && ( isOutSource || (!isOutSource && (isAdmin==true || isEQCAdmin==true) ))" class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
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
                        <!--<th>所屬單位</th>-->
                        <th>姓名</th>
                        <th>帳號</th>
                        <!--<th>電子郵件</th>-->
                        <th>機關/單位</th>
                        <th>手機</th>
                        <th>角色</th>
                        <th>簽名檔</th>
                        <th>編輯</th>
                        <th v-if="!isLastLevel && ( isOutSource || (!isOutSource && (isAdmin==true || isEQCAdmin==true) ))">刪除</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-bind:key="index" v-for="(item,index) in users">
                        <td style="text-align: center;">{{index+1}}</td>
                        <!--<td style="text-align: center;">{{item.UnitName}}</td>-->
                        <td style="text-align: center;">{{item.DisplayName}}</td>
                        <td style="text-align: center;">{{item.UserNo}}</td>
                        <!--<td>{{item.Email}}</td>-->
                        <td style="text-align: center;">{{item.UnitName1}}{{item.UnitName2}}{{item.UnitName3}}</td>
                        <td style="text-align: center;">{{item.Mobile}}</td>
                        <td style="text-align: center;">{{item.RoleName}}</td>
                        <td style="text-align: center;">
                            <a v-if="item.SignatureFileCount > 0" v-bind:href="'/Users/SignatureFileDownload?userSeq='+item.Seq" target="_blank">簽名檔</a>
                        </td>
                        <td style="text-align: center;">
                            <!--<a href="javascript:void(0)" @click="onEdit(item)" class="a-blue underl" title="編輯">編輯</a>-->
                            <a @click="onEdit(item)" v-if="!item.edit" href="#" class="btn btn-color11-2 btn-xs m-1" title="編輯"><i class="fas fa-pencil-alt"></i>  編輯</a>
                        </td>
                        <td style="text-align: center;" v-if="!isLastLevel && ( isOutSource || (!isOutSource && (isAdmin==true || isEQCAdmin==true) )) && item.Seq != userInfo.Seq">
                            <!--<a href="javascript:void(0)" @click="onDelete(item.Seq)" class="a-blue underl a-red" ti="" a-redtle="刪除">刪除</a>-->
                            <a @click="onDelete(item.Seq)" href="#" class="btn btn-color9-1 btn-xs m-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                        </td>
                    </tr>
                    <tr v-if="users==null||users.length==0">
                        <td colspan="9" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>

        </div>
        <!-- 編輯視窗 -->
        <button id="openEditModal" data-toggle="modal" data-target="#view01" style="display:none"> 開啟編輯視窗 </button>
        <div class="modal fade" id="view01" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog modal-xl modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title font-weight-bold">編輯人員管理</h6>
                        <button id="closeEditModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- -->
                        <div class="table-responsive">
                            <table border="0" class="table table2 min910" style="width: 95%!important;">
                                <tbody>
                                    <tr>
                                        <th>帳號</th>
                                        <td colspan="3">
                                            <label v-if="userEdit.Seq==0 && !isLastLevel && isOutSource" type="text" class="d-inline col-6">{{userInfo.UserNo}}</label>
                                            <input v-if="userEdit.Seq==0" v-model="userEdit.UserNo" type="text" class="form-control d-inline col-6">
                                            <label v-if="userEdit.Seq>0" type="text">{{userEdit.UserNo}}</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>密碼</th>
                                        <td>
                                            <input v-model="userEdit.PassWord" type="password" class="form-control col-6">
                                        </td>
                                        <th>確認密碼</th>
                                        <td>
                                            <input v-if="userEdit.Seq==0" v-model="userEdit.PassWordRV" type="password" class="form-control col-6">
                                            <input v-if="userEdit.Seq>0" v-model="userEdit.PassWordRV" type="password" class="form-control col-6">
                                        </td>
                                    </tr>
                                    <tr v-if="!isLastLevel && isOutSource">
                                        <th>單位</th>
                                        <td>
                                            <div class="form-row">
                                                <div class="col-12 col-sm-12 mt-12">
                                                    {{userInfo.UnitName1}}
                                                </div>
                                                <div v-if="userInfo.UnitName2 != null" class="col-12 col-sm-12 mt-12">
                                                    {{userInfo.UnitName2}}
                                                </div>
                                                <div v-if="userInfo.UnitName3 != null" class="col-12 col-sm-12 mt-12">
                                                    {{userInfo.UnitName3}}
                                                </div>
                                            </div>
                                        </td>
                                        <th>職稱</th>
                                        <td>
                                            <select class="form-control" v-model="userEdit.PositionSeq">
                                                <option v-bind:key="index" v-for="(item,index) in positions" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                            <!-- <input class="form-control" type="text" v-model="posText" /> -->
                                        </td>
                                    </tr>
                                    <tr v-else>
                                        <th>單位</th>
                                        <td>
                                            <div v-if="isAdmin==true || isEQCAdmin==true" class="form-row">
                                                <div class="col-12 col-sm-12 mt-12">
                                                    <select class="form-control" v-model="userEdit.UnitSeq1" @change="onChangeEditUnit1">
                                                        <option v-bind:key="index" v-for="(item,index) in unitsEdit1" v-bind:value="item.Value">{{item.Text}}</option>
                                                    </select>
                                                </div>
                                                <div v-if="unitsEdit2.length>1" class="col-12 col-sm-12 mt-12">
                                                    <select class="form-control" v-model="userEdit.UnitSeq2" @change="onChangeEditUnit2">
                                                        <option v-bind:key="index" v-for="(item,index) in unitsEdit2" v-bind:value="item.Value">{{item.Text}}</option>
                                                    </select>
                                                </div>
                                                <div v-if="unitsEdit3.length>1" class="col-12 col-sm-12 mt-12">
                                                    <select class="form-control" v-model="userEdit.UnitSeq3" @change="onChangeEditUnit3">
                                                        <option v-bind:key="index" v-for="(item,index) in unitsEdit3" v-bind:value="item.Value">{{item.Text}}</option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div v-if="isAdmin==false && isEQCAdmin==false">
                                                <label type="text">{{userEdit.UnitName1}}{{userEdit.UnitName2}}{{userEdit.UnitName3}}</label>
                                            </div>
                                        </td>
                                        <th>職稱</th>
                                        <td>
                                            <select class="form-control" v-model="userEdit.PositionSeq">
                                                <option v-bind:key="index" v-for="(item,index) in positions" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>姓名</th>
                                        <td>
                                            <input v-model="userEdit.DisplayName" type="text" class="form-control col-6">
                                        </td>

                                        <th>連絡電話</th>
                                        <td>
                                            <input v-model="userEdit.TelRegion" type="text" class="form-control d-inline col-2">
                                            <input v-model="userEdit.Tel" type="text" class="form-control d-inline col-5"> 分機
                                            <input v-model="userEdit.TelExt" type="text" class="form-control d-inline col-3">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>手機</th>
                                        <td>
                                            <input v-model="userEdit.Mobile" type="text" class="form-control col-6">
                                        </td>

                                        <th>電子信箱</th>
                                        <td>
                                            <input v-model="userEdit.Email" type="email" class="form-control">
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>上傳簽名檔</th>
                                        <td colspan="3">
                                            <div>
                                                <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                                                    <div v-if="!file" :class="['dropZone ', dragging ? 'dropZone-over' : '']"
                                                            @dragestart="dragging = true"
                                                            @dragenter="dragging = true"
                                                            @dragleave="dragging = false">
                                                        <div class="dropZone-info align-self-center" @drag="onFileChange">
                                                            <span class="dropZone-title" style="margin-top:0px;">拖拉檔案至此區塊 或 點擊此處</span>
                                                        </div>
                                                        <input type="file" @change="onFileChange" />
                                                    </div>
                                                    <div v-if="file">
                                                        <div class="dropZone-uploaded">
                                                            <div class="dropZone-uploaded-info">
                                                                <span class="dropZone-title">選取的檔案: {{ file.name }}</span>
                                                                <div class="uploadedFile-info">
                                                                    <button @click="removeFile" type="button" class="col-1 btn btn-shadow btn-color9-1 " style="width: 120px !important;">
                                                                        <i class="fas fa-times"></i> 取消
                                                                    </button>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <th>角色權限</th>
                                        <td v-if="!isLastLevel && isOutSource">
                                            {{userInfo.RoleName}}
                                        </td>
                                        <td v-else>
                                            <select v-if="isAdmin==true || isEQCAdmin==true" class="form-control" v-model="userEdit.RoleSeq">
                                                <option v-bind:key="index" v-for="(item,index) in rolesDefalut" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                            <div v-if="isAdmin==false && isEQCAdmin==false">
                                                <label type="text">{{userEdit.RoleName}}</label>
                                            </div>

                                            <select v-if="isAdmin==true || isEQCAdmin==true" class="form-control" v-model="userEdit.RoleSeq2">
                                                <option :value="0" selected> --無-- </option>
                                                <option v-bind:key="index" v-for="(item,index) in rolesNotDefalut" v-bind:value="item.Value">{{item.Text}}</option>
                                            </select>
                                            <div v-if="isAdmin==false && isEQCAdmin==false">
                                                <label type="text">{{userEdit.RoleName2}}</label>
                                            </div>
                                        </td>
                                        <th>是否啟用</th>
                                        <td>
                                            <div v-if="isAdmin==true || isEQCAdmin==true" class="custom-control custom-radio custom-control-inline">
                                                <input v-model="userEdit.IsEnabled" v-bind:value="true" type="radio" class="custom-control-input" id="enable_Yes" name="radio">
                                                <label class="custom-control-label text-color1" for="enable_Yes">是</label>
                                            </div>
                                            <div v-if="isAdmin==true || isEQCAdmin==true" class="custom-control custom-radio custom-control-inline">
                                                <input v-model="userEdit.IsEnabled" v-bind:value="false" type="radio" class="custom-control-input" id="enable_No" name="radio">
                                                <label class="custom-control-label text-color1" for="enable_No">否</label>
                                            </div>
                                            <div v-if="isAdmin==false && isEQCAdmin==false">
                                                <label type="text">是</label>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="4">
                                            <div class="row justify-content-center">
                                                <a href="javascript:void(0)" @click="onSave" role="button" class="btn btn-color11-3 btn-xs mx-1">
                                                    <i class="fas fa-save"></i>&nbsp;儲存
                                                </a>
                                                <a href="javascript:void(0)" @click="onCancel" role="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal" aria-label="Close">
                                                    <i class="fas fa-times"></i>&nbsp;關閉
                                                </a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <!-- -->
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import moment from 'moment';
    import {userStore as store} from "./userStore";
    import {ref, computed, onMounted} from "vue";

    // Suppress the warnings
    moment.suppressDeprecationWarnings = true;

    export default {
        computed: {
            editableFields() {
                return this.fields.filter(field => {
                    return field.editable == false
                })
            },
            users()
            {
                return this.userStore.userList;
            },
            userInfo()
            {
                return this.userStore.userInfo;
            },
            isOutSource()
            {
                return this.userStore.userInfo.RoleSeq == 4 || 
                        this.userStore.userInfo.RoleSeq == 5 || 
                        this.userStore.userInfo.RoleSeq== 6 || 
                        this.userStore.userInfo.RoleSeq == 7
            },
            rolesNotDefalut()
            {
                return this.roles.filter(e => e.IsSelected  == false);
            },
            rolesDefalut()
            {
                return this.roles.filter(e => e.IsSelected  == true);
            }
        
        },

        data: function () {
            return {
                Role: null,
                // userStore : {},
                subUnitSeq : 0,
                nameSearch: null,
                isAdmin: false,
                isEQCAdmin: false,
                userSeq : null,
                subUnit : ["", "", ""],
                currentPage: 1,
                perPage: 10,
                totalRows: 0,
                posText : "",
                unitOptions : [],
                // 人員列表
                // 單位下拉-第一層
                units1: [],
                // 單位下拉-第二層
                units2: [],
                // 單位下拉-第三層
                units3: [],
                // 單位下拉-第一層(值)
                unitSeq1: '0',
                // 單位下拉-第二層(值)
                unitSeq2: '0',
                // 單位下拉-第三層(值)
                unitSeq3: '0',
                // 單位下拉-第一層(編輯用)
                unitsEdit1: [],
                // 單位下拉-第二層(編輯用)
                unitsEdit2: [],
                // 單位下拉-第三層(編輯用)
                unitsEdit3: [],
                // 編輯的人員資料
                userEdit: {},
                // 職稱下拉
                positions: [],
                // 角色下拉
                roles: [],
                //簽名檔
                file: null,
                files: new FormData(),
                dragging: false
            };
        },

        emits : ["onUserListChange"],
        methods: {

            // 取得單位下拉選單1
            getUnitList1() {
                const self = this;
                let params = {};
                window.myAjax.post('/Unit/GetUnitListV2', {subUnit:["", ""]})
                    .then(resp => {
                        self.units1 = resp.data;
                        self.unitSeq1 = self.units1[0].Value;
                        //self.units1.splice(0, 0, { Text: '--全部--', Value: '0' });
                        let obj = new Object();
                        obj.target = new Object();

                        obj.target.value = self.unitSeq1;
                        self.onChangeUnit1(obj);
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得單位下拉選單2
            getUnitList2(unitSeq = 0) {
                const self = this;
                let params = { parentSeq: unitSeq };
                window.myAjax.post('/Unit/GetUnitList', params)
                    .then(resp => {
                        self.units2 = resp.data;
                        self.units2.splice(0, 0, { Text: '--全部--', Value: '0' });
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得單位下拉選單3
            getUnitList3() {
                const self = this;
                let params = { parentSeq: self.unitSeq2 };
                window.myAjax.post('/Unit/GetUnitList', params)
                    .then(resp => {
                        self.units3 = resp.data;
                        self.units3.splice(0, 0, { Text: '--全部--', Value: '0' });
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得單位下拉選單1
            getEditUnitList1() {
                const self = this;
                let params = {};
                self.unitsEdit1 = [];
                window.myAjax.post('/Unit/GetUnitList')
                    .then(resp => {
                        self.unitsEdit1 = resp.data;
                        self.userEdit.UnitSeq1 = self.unitsEdit1[0].Value;
                        //self.units1.splice(0, 0, { Text: '--全部--', Value: '0' });
                        let obj = new Object();
                        obj.target = new Object();
                        obj.target.value = self.userEdit.UnitSeq1;
                        self.onChangeEditUnit1(obj);
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得單位下拉選單2
            getEditUnitList2() {
                const self = this;
                let params = { parentSeq: self.userEdit.UnitSeq1 };
                // 清空下拉選單
                self.unitsEdit2 = [];
                if (self.userEdit.UnitSeq1 != '0') {
                    window.myAjax.post('/Unit/GetUnitList', params)
                        .then(resp => {
                            self.unitsEdit2 = resp.data;
                            self.unitsEdit2.splice(0, 0, { Text: '--請選擇--', Value: '0' });
                        })
                        .then(err => {
                            //console.log(err);
                        });
                }
            },
            // 取得單位下拉選單3
            getEditUnitList3() {
                const self = this;
                let params = { parentSeq: self.userEdit.UnitSeq2 };
                // 清空下拉選單
                self.unitsEdit3 = [];
                if (self.userEdit.UnitSeq2 != '0') {
                    window.myAjax.post('/Unit/GetUnitList', params)
                        .then(resp => {
                            self.unitsEdit3 = resp.data;
                            self.unitsEdit3.splice(0, 0, { Text: '--請選擇--', Value: '0' });
                        })
                        .then(err => {
                            //console.log(err);
                        });
                }
            },
            // 取得職稱下拉選單
            getPositionList() {
                const self = this;
                let params = {};
                window.myAjax.get('/Users/GetPositionList')
                    .then(resp => {
                        self.positions = resp.data;
                        self.positions.splice(0, 0, { Text: '--請選擇--', Value: '0' });
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },
            // 取得角色下拉選單
            getRoleList() {
                const self = this;
                let params = {};
                window.myAjax.get('/Role/GetRoleList')
                    .then(resp => {
                        self.roles = resp.data;
                        self.roles.splice(0, 0, { Text: '--請選擇--', Value: '0' });
                    })
                    .then(err => {
                        //console.log(err);
                    });
            },

            // 選擇單位第一層
            onChangeUnit1(value) {
                // const self = this;
                // this.nameSearch  = null;
                // self.unitSeq1 = value.target.value;
                // self.unitSeq2 = '0';
                // self.unitSeq3 = '0';

                // if(this.Role == 3) self.unitSeq1 = this.userInfo.UnitSeq1;
                
                // self.getList(self.unitSeq1);

                // self.getUnitList2(self.unitSeq1);
            },
            // 選擇單位第二層
            onChangeUnit2(value) {
                this.nameSearch = null;
                const self = this;
                self.unitSeq2 = value.target.value;
                self.unitSeq3 = '0';
                // 選擇「全部」使用上一層Seq
                let unitSeq = value.target.value == '0' ? self.unitSeq1 : value.target.value;



                self.getList(unitSeq);
                
                self.getUnitList3();
            },
            // 選擇單位第三層
            onChangeUnit3(value) {
                const self = this;
                self.unitSeq3 = value.target.value;
                // 選擇「全部」使用上一層Seq
                let unitSeq = value.target.value == '0' ? self.unitSeq2 : value.target.value;
                self.getList(unitSeq);
            },
            // 選擇單位第一層(編輯)
            onChangeEditUnit1(value) {
                const self = this;
                self.userEdit.UnitSeq1 = value.target.value;
                self.userEdit.UnitSeq2 = '0';
                self.userEdit.UnitSeq3 = '0';
                self.getEditUnitList2();
            },
            // 選擇單位第二層(編輯)
            onChangeEditUnit2(value) {
                const self = this;
                self.userEdit.UnitSeq2 = value.target.value;
                self.userEdit.UnitSeq3 = '0';
                self.getEditUnitList3();
            },
            // 選擇單位第三層(編輯)
            onChangeEditUnit3(value) {
                const self = this;
                self.userEdit.UnitSeq3 = value.target.value;
            },
            // 編輯
            onEdit(item) {
                const self = this;
                if (item == 0 || item == null) {
                    self.userEdit = {};
                    self.userEdit.Seq = 0;
                    self.userEdit.UnitSeq1 = '1';
                    self.userEdit.UnitSeq2 = '0';
                    self.userEdit.UnitSeq3 = '0';
                    self.userEdit.PositionSeq = '0';
                    self.userEdit.RoleSeq = '0';
                    self.userEdit.RoleSeq2 = '0';
                    self.userEdit.IsEnabled = false;
                } else {
                    self.userEdit = JSON.parse(JSON.stringify(item));
                    self.userEdit.PassWordRV = self.userEdit.PassWord;
                }
                self.removeFile();
                self.getEditUnitList2();
                self.getEditUnitList3();
                document.getElementById("openEditModal").click();
            },
            onCancel() {
                document.getElementById("closeEditModal").click();
            },
            // 人員資料編輯
            onSave() {
                const self = this;
                //if (self.userEdit.Seq == 0) {
                if (self.userEdit.PassWord != self.userEdit.PassWordRV) {
                    alert("請再次確認密碼!");
                return false;
                }
                if (self.userEdit.UserNo == null || self.userEdit.UserNo == ''
                    || self.userEdit.PassWord == null || self.userEdit.PassWord == '') {
                    alert("帳號密碼不能是空的!");
                    return false;
                }
                let params = JSON.parse(JSON.stringify(self.userEdit));
                console.log(params);
                if(this.isOutSource) {
                    if(this.userEdit.Seq == 0 )
                        params.UserNo = this.userInfo.UserNo + params.UserNo;
                    params.RoleSeq = this.userInfo.RoleSeq;
                    params.UnitSeq1 = this.userInfo.UnitSeq1;
                    params.UnitSeq2 = this.userInfo.UnitSeq2;
                    params.UnitSeq3 = this.userInfo.UnitSeq3;
                    params.RoleName = this.userInfo.RoleName;
                    params.UnitName1 = this.userInfo.UnitName1;
                    params.UnitName2 = this.userInfo.UnitName2;
                    params.UnitName3 = this.userInfo.UnitName3;
                    params.IsEnabled = true;
                    // params.DisplayName =  this.posText + ( params.DisplayName ?? ""); 
                }
                else {

                    //}
                    if (self.userEdit.UnitSeq == 0) {
                        alert("請選擇「單位」!");
                        return false;
                    }
                    if (self.userEdit.PositionSeq == 0) {
                        alert("請選擇「職稱」!");
                        return false;
                    }
                    if (self.userEdit.RoleSeq == 0) {
                        alert("請選擇「角色權限」!");
                        return false;
                    }

                }
                params.UnitSeq1 = params.UnitSeq1 == '0' ? null : params.UnitSeq1;
                params.UnitSeq2 = params.UnitSeq2 == '0' ? null : params.UnitSeq2;
                params.UnitSeq3 = params.UnitSeq3 == '0' ? null : params.UnitSeq3;

                window.myAjax.post('/Users/Save', params)
                    .then(resp => {
                        if (resp.data.IsSuccess) {
                            if (resp.data.Data > 0) {
                                //上傳簽名檔
                                self.files.append("userSeq", resp.data.Data);
                                const files = self.files;
                                window.myAjax.post('/Users/SignatureFileUpload', files)
                                    .then(resp2 => {
                                        if (resp2.data.IsSuccess) {
                                            Vue.prototype.common.showResultMessage(resp2);
                                            self.onCancel();
                                        } else {
                                            alert("上傳簽名檔 失敗!!");
                                        }
                                        this.getList();
                                        self.refreshUserList();
                                    })
                                    .catch(err => {
                                        //console.log(err);
                                    });
                            }
                        } else {
                            Vue.prototype.common.showResultMessage(resp);
                            self.onCancel();
                        }
                    })
                    .catch(err => {
                        //console.log(err);
                    });
            },
            // 刪除使用者
            onDelete(seq) {
                const self = this;
                if (confirm("確定要刪除?")) {
                    let params = { 'seq': seq }
                    window.myAjax.post('/Users/DeleteUser', params)
                        .then(resp => {
                            Vue.prototype.common.showResultMessage(resp);
                            if (resp.data.IsSuccess) {
                                self.refreshUserList();
                            }
                        })
                        .catch(err => {
                            //console.log(err);
                        });
                }
            },
            // 刷新列表
            refreshUserList() {
                const self = this;
                let unitSeq = self.unitSeq3;
                if (unitSeq == '0') {
                    unitSeq = self.unitSeq2;
                }
                if (unitSeq == '0') {
                    unitSeq = self.unitSeq1;
                }
                    this.getList();
            },
            onFileChange(e) {
                // 判斷拖拉上傳或點擊上傳的 event
                var files = e.target.files || e.dataTransfer.files;

                // 預防檔案為空檔
                if (!files.length) {
                    this.dragging = false;
                    return;
                }

                this.createFile(files[0]);
            },
            createFile(file) {
                this.file = file;
                this.dragging = false;
                this.files.append("file", this.file, this.file.name);
            },
            removeFile() {
                this.file = '';
                this.files = new FormData();
            },
            getList()
            {
                this.userStore.getList();
            }
        },
        setup()
        {
            const userStore = ref(store);
            const isLastLevel  = computed(() => userStore.value.isLastLevel);
            onMounted(() => {

            });

            return {
                userStore,
                isLastLevel
            }
        },
        mounted() {

            const self = this;
            // 資料初始化
            // self.Role = parseInt(localStorage.getItem('Role') );

            self.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
            self.isEQCAdmin = localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
            self.userSeq = localStorage.getItem("userSeq") ;
            this.Role = localStorage.getItem("Role"); 
            console.log("fffAA", this.isLastLevel);
            this.getRoleList();
            this.getPositionList();
                self.items = null;
            // 取得編輯時的第一層單位
            this.getEditUnitList1();

        },
    }
</script>
