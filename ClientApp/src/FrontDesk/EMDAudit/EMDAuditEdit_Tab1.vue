<template>
    <div>
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-11 mt-3">
            </div>
            <!-- shioulo 20220519
            <div class="form-inline col-12 col-md-1 mt-3">
                <a v-on:click="goback()" href="#" class="a-blue" title="返回">返回</a>
            </div>
             -->
        </div>
        <div class="table-responsive mb-3" style="min-height:320px;">
            <table border="0" class="table table1 onepage my-0">
                <thead>
                    <tr>
                        <th rowspan="2">項次</th>
                        <th class="min200">契約詳細表項次</th>
                        <th rowspan="2">契約數量</th>
                        <th rowspan="2" class="sort">是否取樣試驗</th>
                        <th width="180px">預定送審日期</th>
                        <th width="180px">是否驗廠</th>
                        <th colspan="5">送審資料</th>
                        <th width="180px">審查日期</th>
                        <th rowspan="2" class="min75">備註(歸檔編號)</th>
                        <th rowspan="2" class="sort min75"></th>
                    </tr>
                    <tr>
                        <th>材料/設備名稱</th>
                        <th>實際送審日期</th>
                        <th>驗廠日期</th>
                        <th class="sort">協力廠商資料</th>
                        <th class="sort">型錄</th>
                        <th class="sort">相關試驗報告</th>
                        <th class="sort">樣品</th>
                        <th>其他</th>
                        <th>審查結果</th>
                    </tr>
                </thead>
                <tbody v-for="(item, index) in items" v-bind:key="item.Seq" v-show="!editFlag || item.edit">
                    <tr>
                        <td rowspan="2">{{perPage*(pageIndex-1)+index+1}}</td>
                        <td v-if="item.QRCodeImageURL==''">{{item.ItemNo}}</td>
                        <td v-if="item.QRCodeImageURL!=''">
                            {{item.ItemNo}}&nbsp;<img v-if="item.QRCodeImageURL!=''" v-bind:src="item.QRCodeImageURL" alt=""  @click="clickImg($event)" style="width: 15%; height: 15%;"/>
                            <big-img v-if="showImg" @clickit="viewImg" :imgSrc="imgSrc"></big-img>
                        </td>
                        <td rowspan="2">{{item.ContactQty}}</td>
                        <td rowspan="2" class="text-center">
                            <span v-if="item.IsSampleTest==null || item.IsSampleTest=='' || item.IsSampleTest=='0'">否</span>
                            <span v-if="item.IsSampleTest=='1'">是</span>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.chsSchAuditDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsSchAuditDate" @change="onDateInputChange($event, item, 'SchAuditDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.SchAuditDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'SchAuditDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td class="text-center">
                            <span v-if="!item.edit && item.IsFactoryInsp==false">否</span>
                            <span v-if="!item.edit && item.IsFactoryInsp==true">是</span>
                            <select v-if="item.edit" v-model.number="item.IsFactoryInsp" class="form-control">
                                <option value="true">是</option>
                                <option value="false">否</option>
                            </select>
                        </td>
                        <td rowspan="2" class="text-center">
                            <div v-if="!item.edit" class="custom-control custom-checkbox">
                                <input disabled type="checkbox" class="custom-control-input" v-bind:id="'info_'+index" v-model="item.IsAuditVendor">
                                <label class="custom-control-label" v-bind:for="'info_'+index"></label>
                            </div>
                            <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_1(item)" data-toggle="modal" data-target="#emdauditdModal_1" href="#" class="a-blue" title="檢視">檢視</a>

                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" v-bind:id="'info_'+index" v-model="item.IsAuditVendor">
                                <label class="custom-control-label" v-bind:for="'info_'+index"></label>
                            </div>
                            <a v-if="item.edit" v-on:click="openEMDAuditEditModal_1(item)" data-toggle="modal" data-target="#emdauditdModal_1" href="#" class="a-blue" title="編輯">編輯</a>
                        </td>
                        <td rowspan="2" class="text-center">
                            <div v-if="!item.edit" class="custom-control custom-checkbox">
                                <input disabled type="checkbox" class="custom-control-input" v-bind:id="'catalog_'+index" v-model="item.IsAuditCatalog">
                                <label class="custom-control-label" v-bind:for="'catalog_'+index"></label>
                            </div>
                            <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_5(item)" data-toggle="modal" data-target="#emdauditdModal_5" href="#" class="a-blue" title="檢視">檢視</a>

                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" v-bind:id="'catalog_'+index" v-model="item.IsAuditCatalog">
                                <label class="custom-control-label" v-bind:for="'catalog_'+index"></label>
                            </div>
                            <a v-if="item.edit" v-on:click="openEMDAuditEditModal_5(item)" data-toggle="modal" data-target="#emdauditdModal_5" href="#" class="a-blue" title="編輯">編輯</a>
                        </td>
                        <td rowspan="2" class="text-center">
                            <div v-if="!item.edit" class="custom-control custom-checkbox">
                                <input disabled type="checkbox" class="custom-control-input" v-bind:id="'report_'+index" v-model="item.IsAuditReport">
                                <label class="custom-control-label" v-bind:for="'report_'+index"></label>
                            </div>
                            <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_3(item)" data-toggle="modal" data-target="#emdauditdModal_3" href="#" class="a-blue" title="檢視">檢視</a>

                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" v-bind:id="'report_'+index" v-model="item.IsAuditReport">
                                <label class="custom-control-label" v-bind:for="'report_'+index"></label>
                            </div>
                            <a v-if="item.edit" v-on:click="openEMDAuditEditModal_3(item)" data-toggle="modal" data-target="#emdauditdModal_3" href="#" class="a-blue" title="編輯">編輯</a>
                        </td>
                        <td rowspan="2" class="text-center">
                            <div v-if="!item.edit" class="custom-control custom-checkbox">
                                <input disabled type="checkbox" class="custom-control-input" v-bind:id="'sample_'+index" v-model="item.IsAuditSample">
                                <label class="custom-control-label" v-bind:for="'sample_'+index"></label>
                            </div>
                            <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_4(item)" data-toggle="modal" data-target="#emdauditdModal_4" href="#" class="a-blue" title="檢視">檢視</a>

                            <div v-if="item.edit" class="custom-control custom-checkbox">
                                <input type="checkbox" class="custom-control-input" v-bind:id="'sample_'+index" v-model="item.IsAuditSample">
                                <label class="custom-control-label" v-bind:for="'sample_'+index"></label>
                            </div>
                            <a v-if="item.edit" v-on:click="openEMDAuditEditModal_4(item)" data-toggle="modal" data-target="#emdauditdModal_4" href="#" class="a-blue" title="編輯">編輯</a>
                        </td>
                        <td rowspan="2">
                            <div v-if="!item.edit">
                                <a v-on:click="showOtherAudit(item.OtherAudit)" href="##" class="a-blue" >{{ getOtherAudit(item.OtherAudit) }}</a>
                            </div>
                            <input v-if="item.edit" type="text" class="form-control" v-model="item.OtherAudit">
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.chsAuditDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsAuditDate" @change="onDateInputChange($event, item, 'AuditDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.AuditDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'AuditDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td rowspan="2" class="text-center">
                            <a v-if="!item.edit" v-on:click="openEMDAuditEditModal_6(item)" data-toggle="modal" data-target="#emdauditdModal_6" href="#" class="a-blue" title="檢視">檢視</a>
                            <a v-if="item.edit" v-on:click="openEMDAuditEditModal_6(item)" data-toggle="modal" data-target="#emdauditdModal_6" href="#" class="a-blue" title="編輯">編輯</a>
                        </td>
                        <td rowspan="2">
                            <div class="d-flex justify-content-center">
                                <button v-if="!item.edit" @click="editItem(item)" class="btn btn-color11-3 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button v-if="!item.edit && item.ItemType==0" @click="addItem(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                                <button v-if="!item.edit && item.ItemType==1" @click="delItem(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                <button v-if="item.edit" @click="saveItem(item)" class="btn btn-color11-4 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                <button v-if="item.edit" @click="cancelItem(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><a v-on:click="goPage(engMain.Seq)" href="#" class="a-blue underl" v-bind:title="item.MDName">{{item.MDName}}</a></td>
                        <td>
                            <div v-if="!item.edit">{{item.chsRealAutitDate}}</div>
                            <b-input-group v-if="item.edit">
                                <input v-bind:value="item.chsRealAutitDate" @change="onDateInputChange($event, item, 'RealAutitDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.RealAutitDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'RealAutitDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.chsFactoryInspDate}}</div>
                            <b-input-group v-if="item.edit && item.IsFactoryInsp=='true'">
                                <input v-bind:value="item.chsFactoryInspDate" @change="onDateInputChange($event, item, 'FactoryInspDate')" type="text" class="form-control" placeholder="yyy/mm/dd">
                                <b-form-datepicker v-model="item.FactoryInspDate" :hide-header="hideHeader"
                                                   button-only right class="mb-2" size="sm" @context="onDatePicketChange($event, index, 'FactoryInspDate')">
                                </b-form-datepicker>
                            </b-input-group>
                        </td>
                        <td>
                            <span v-if="!item.edit && item.AuditResult==2">否決</span>
                            <span v-if="!item.edit && item.AuditResult==1">通過</span>
                            <select v-if="item.edit" v-model.number="item.AuditResult" class="form-control">
                                <option value="1">通過</option>
                                <option value="2">否決</option>
                            </select>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style="width:99%;" class="row justify-content-center">
            <b-pagination :total-rows="totalRows"
                          :per-page="perPage"
                          v-model="pageIndex">
            </b-pagination>
        </div>

        <emdauditdmodal_1 v-on:onCloseEvent="closeEMDAuditEditModal_1"
                            :isEdit="targetItem_1.isEdit"
                          v-bind:modalId="'emdauditdModal_1'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_1" v-bind:engMain="engMain"></emdauditdmodal_1>

        <emdauditdmodal_2 v-if="LoadNo=='2'" v-on:onCloseEvent="closeEMDAuditEditModal_2"
                          v-bind:modalId="'emdauditdModal_2'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_2" v-bind:engMainSeq="engMain.Seq"></emdauditdmodal_2>

        <emdauditdmodal_3 v-if="LoadNo=='3'" v-on:onCloseEvent="closeEMDAuditEditModal_3"
                          v-bind:modalId="'emdauditdModal_3'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_3" v-bind:engMainSeq="engMain.Seq"></emdauditdmodal_3>

        <emdauditdmodal_4 v-if="LoadNo=='4'" v-on:onCloseEvent="closeEMDAuditEditModal_4"
                          v-bind:modalId="'emdauditdModal_4'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_4" v-bind:engMainSeq="engMain.Seq"></emdauditdmodal_4>

        <emdauditdmodal_5 v-if="LoadNo=='5'" v-on:onCloseEvent="closeEMDAuditEditModal_5"
                          v-bind:modalId="'emdauditdModal_5'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_5" v-bind:engMainSeq="engMain.Seq"></emdauditdmodal_5>

        <emdauditdmodal_6 v-on:onCloseEvent="closeEMDAuditEditModal_6"
                          v-bind:modalId="'emdauditdModal_6'" v-bind:emdSummary="emdSummary" v-bind:targetItem="targetItem_6"></emdauditdmodal_6>

    </div>
</template>
<script>
    import emdauditdmodalComponent_2 from './EMDAuditEdit_Tab1_2.vue';
    import emdauditdmodalComponent_3 from './EMDAuditEdit_Tab1_3.vue';
    import emdauditdmodalComponent_4 from './EMDAuditEdit_Tab1_4.vue';
    import emdauditdmodalComponent_5 from './EMDAuditEdit_Tab1_5.vue';
    import BigImg from './BigImg.vue';

    const emdauditdmodal_2 = () => ({
        component: new Promise((resolve) => {
            setTimeout(() => {
                resolve(emdauditdmodalComponent_2)
            }, 1000)
        }),
    })

    const emdauditdmodal_3 = () => ({
        component: new Promise((resolve) => {
            setTimeout(() => {
                resolve(emdauditdmodalComponent_3)
            }, 1000)
        }),
    })

    const emdauditdmodal_4 = () => ({
        component: new Promise((resolve) => {
            setTimeout(() => {
                resolve(emdauditdmodalComponent_4)
            }, 1000)
        }),
    })

    const emdauditdmodal_5 = () => ({
        component: new Promise((resolve) => {
            setTimeout(() => {
                resolve(emdauditdmodalComponent_5)
            }, 1000)
        }),
    })

    export default {
        props: ['engMain'],
        watch: {
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },
        components: {
            emdauditdmodal_1: require('./EMDAuditEdit_Tab1_1.vue').default,
            emdauditdmodal_2: emdauditdmodal_2,
            emdauditdmodal_3: emdauditdmodal_3,
            emdauditdmodal_4: emdauditdmodal_4,
            emdauditdmodal_5: emdauditdmodal_5,
            emdauditdmodal_6: require('./EMDAuditEdit_Tab1_6.vue').default,
            'big-img': BigImg
        },
        data: function () {
            return {
                hideHeader:true,
                editFlag: false,
                items: [],

                LoadNo: '',
                emdSummary: {},
                targetItem_1: { Seq: 0, VendorName: '', VendorTaxId: '', edit: false },
                targetItem_2: {},
                targetItem_3: {},
                targetItem_4: {},
                targetItem_5: {},
                targetItem_6: { Seq: 0, ArchiveNo: '' },

                //分頁
                pageIndex: 1,
                perPage: 3,
                totalRows: 0,

                showImg: false,
                imgSrc: '',
            };
        },
        methods: {
            //s20230926
            getOtherAudit(data) {
                if (data == null)
                    return "";
                else if (data.length < 10)
                    return data;
                else
                    return data.substring(0, 10)+"...";
            },
            showOtherAudit(data) {
                alert(data);
            },
            //日期變更 20230222
            onDateInputChange(event, item, mode) {
                var dateStr = event.target.value;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) {
                    this.clearDate(item, mode);
                } else {
                    dateStr = this.toYearDate(dateStr);
                    if (!this.isExistDate(dateStr)) {
                        this.clearDate(item, mode);
                    } else {
                        dateStr = dateStr.replaceAll('/', '-')
                        if (mode == 'SchAuditDate') {
                            item.SchAuditDate = dateStr;
                        } else if (mode == 'RealAutitDate') {
                            item.RealAutitDate = dateStr;
                        } else if (mode == 'FactoryInspDate') {
                            item.FactoryInspDate = dateStr;
                        } else if (mode == 'AuditDate') {
                            item.AuditDate = dateStr;
                        }
                    }
                }
            },
            clearDate(item, mode) {
                if (mode == 'SchAuditDate') {
                    item.SchAuditDate = '';
                    item.chsSchAuditDate = '';
                } else if (mode == 'RealAutitDate') {
                    item.RealAutitDate = '';
                    item.chsRealAutitDate = '';
                } else if (mode == 'FactoryInspDate') {
                    item.FactoryInspDate = '';
                    item.chsFactoryInspDate = '';
                } else if (mode == 'AuditDate') {
                    item.AuditDate = '';
                    item.chsAuditDate = '';
                }
            },
            //清單
            getList() {
                console.log('getList() ' + this.engMain.Seq);
                this.editFlag = false;
                this.items = [];
                let params = { id: this.engMain.Seq, pageIndex: this.pageIndex, perPage: this.perPage };
                window.myAjax.post('/EMDAudit/GetEMDSummaryList', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        for (let item of this.items) {
                            item.SchAuditDate = this.toYearDate(item.chsSchAuditDate);
                            item.RealAutitDate = this.toYearDate(item.chsRealAutitDate);
                            item.FactoryInspDate = this.toYearDate(item.chsFactoryInspDate);
                            item.AuditDate = this.toYearDate(item.chsAuditDate);
                            item.QRCodeImageURL = location.protocol + '//' + location.host + '/FileUploads/Eng/' + this.engMain.Seq + '/' + item.QRCodeImageURL;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            addItem(item) {//s20230308
                window.myAjax.post('/EMDAudit/AddEMDSummary', { item: item })
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
            delItem(item) {//s20230308
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/EMDAudit/DelEMDSummary', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getList();
                            } else
                                alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            cancelItem(item) {//s20230308
                this.editFlag = false;
                item.edit = this.editFlag;
            },
            editItem(item) {
                if (this.editFlag) return;
                this.editFlag = true;
                item.edit = this.editFlag;
            },
            //項目名稱, Keep儲存
            saveItem(item) {
                window.myAjax.post('/EMDAudit/UpdateEMDSummary', { item: item })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            console.log(resp.data.qrcodeImageURL);
                            item.QRCodeImageURL = location.protocol + '//' + location.host + '/FileUploads/Eng/' + this.engMain.Seq + '/' + resp.data.qrcodeImageURL;
                            item.edit = false;
                            this.editFlag = false;
                            //const resultItem = resp.data.item.Data;
                            //item.modifyDate = resultItem.modifyDate;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDateChange(event, itemA, index, mode) {
                if (event.target.value.length == 0) {
                    itemA = '';
                    if (mode == 'SchAuditDate')
                        this.items[index].SchAuditDate = '';
                    else if (mode == 'RealAutitDate')
                        this.items[index].RealAutitDate = '';
                    else if (mode == 'FactoryInspDate')
                        this.items[index].FactoryInspDate = '';
                    else if (mode == 'AuditDate')
                        this.items[index].AuditDate = '';
                    return;
                }
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = itemA;
                    alert("日期錯誤");
                } else {
                    itemA = this.toYearDate(event.target.value);
                    if (mode == 'SchAuditDate')
                        this.items[index].SchAuditDate = event.target.value;
                    else if (mode == 'RealAutitDate')
                        this.items[index].RealAutitDate = event.target.value;
                    else if (mode == 'FactoryInspDate')
                        this.items[index].FactoryInspDate = event.target.value;
                    else if (mode == 'AuditDate')
                        this.items[index].AuditDate = event.target.value;
                }
            },
            onDatePicketChange(ctx, index, mode) {
                //console.log(ctx);
                if (ctx.selectedDate != null) {
                    var d = ctx.selectedDate;
                    var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                    if (mode == 'SchAuditDate')
                        this.items[index].chsSchAuditDate = dd;
                    else if (mode == 'RealAutitDate')
                        this.items[index].chsRealAutitDate = dd;
                    else if (mode == 'FactoryInspDate')
                        this.items[index].chsFactoryInspDate = dd;
                    else if (mode == 'AuditDate')
                        this.items[index].chsAuditDate = dd;
                }
            },
            //中曆轉西元
            toYearDate(dateStr) {
                if (dateStr == null) return null;
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                //return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
                return (parseInt(dateObj[0]) + 1911).toString() + '/' + (parseInt(dateObj[1])).toString() + '/' + (parseInt(dateObj[2])).toString();
            },
            //日期檢查
            isExistDate(dateStr) {
                var dateObj = dateStr.split('/'); // yyy/mm/dd
                if (dateObj.length != 3) return false;

                var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                var theYear = parseInt(dateObj[0]);
                if (theYear != dateObj[0]) return false;
                var theMonth = parseInt(dateObj[1]);
                if (theMonth != dateObj[1]) return false;
                var theDay = parseInt(dateObj[2]);
                if (theDay != dateObj[2]) return false;
                if (new Date(theYear + 1911, 1, 29).getDate() === 29) { // 是否為閏年?
                    limitInMonth[1] = 29;
                }
                return theDay <= limitInMonth[theMonth - 1];
            },

            openEMDAuditEditModal_1(item) {
                this.emdSummary = item;
                this.targetItem_1 = item;
            },
            openEMDAuditEditModal_2(item) {
                this.emdSummary = item;
                this.targetItem_2 = item;
                this.LoadNo = '2';
            },
            openEMDAuditEditModal_3(item) {
                this.emdSummary = item;
                this.targetItem_3 = item;
                this.LoadNo = '3';
            },
            openEMDAuditEditModal_4(item) {
                this.emdSummary = item;
                this.targetItem_4 = item;
                this.LoadNo = '4';
            },
            openEMDAuditEditModal_5(item) {
                this.emdSummary = item;
                this.targetItem_5 = item;
                this.LoadNo = '5';
            },
            openEMDAuditEditModal_6(item) {
                this.emdSummary = item;
                this.targetItem_6 = item;
            },

            closeEMDAuditEditModal_1() {
                this.emdSummary = {};
                this.targetItem_1 = {};
            },
            closeEMDAuditEditModal_2() {
                this.emdSummary = {};
                this.targetItem_2 = {};
                this.LoadNo = '';
            },
            closeEMDAuditEditModal_3() {
                this.emdSummary = {};
                this.targetItem_3 = {};
            },
            closeEMDAuditEditModal_4() {
                this.emdSummary = {};
                this.targetItem_4 = {};
            },
            closeEMDAuditEditModal_5() {
                this.emdSummary = {};
                this.targetItem_5 = {};
            },
            closeEMDAuditEditModal_6() {
                this.emdSummary = {};
                this.targetItem_6 = {};
            },
            goback() {
                window.location = "/EMDAudit/EMDAuditList";
            },
            goPage(engId) {
                //window.location = "/SupervisionPlan/Edit?id=" + engId;
                window.open('/SupervisionPlan/Edit?id=' + engId, '_blank');
            },
            clickImg(e) {
                this.showImg = true;
                // 獲取當前圖片地址
                this.imgSrc = e.currentTarget.src;
            },
            viewImg() {
                this.showImg = false;
            },
        },
        async mounted() {
            console.log('mounted() 材料設備送審管制總表');
            this.getList();
        }
    }
</script>