<template>
    <div>
        <div class="row justify-content">
            <button v-show="QisShow" v-on:click="onInquire" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                <i class="fas fa-search"></i> 回清單
            </button>
            <button v-on:click="onCreatNew" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                <i class="fas fa-plus"></i> 新增
            </button>

            <!-- div class="col-6 col-lg-2 mt-3">

        </div>
        <div class="col-6 col-lg-2 mt-3">

        </div -->
        </div>
        <div style="color:red">下方預設顯示已新增的抽查紀錄，如果找不到，請點選"新增"，新增抽查紀錄單</div>
        <form @submit.prevent>
            <div class="justify-content-between" v-show="!QisShow">
                <h2>{{ constCheckItem.ItemName }} 紀錄查詢與編修</h2>
                <comm-pagination ref="pagination" :pageRecordList="[5]" :recordTotal="recordTotal"
                    v-on:onPaginationChange="onPaginationChange"></comm-pagination>
                <table class="table table-responsive-md table-hover">
                    <thead class="insearch">
                        <tr>
                            <th><strong>分項工程</strong></th>
                            <th><strong>檢查日期</strong></th>
                            <th><strong>地點</strong></th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in selectRecItemOption" v-bind:key="item.Seq">
                            <td class="text-left">{{ item.ItemName }} <span style="color:blue" v-if="item.IsFromMobile">(APP新增)</span> </td>
                            <td>{{ item.chsCheckDate }}</td>
                            <td>{{ item.CCRPosDesc }}</td>
                            <td>
                                <button v-on:click="getRecHeader(item.Seq)" class="btn btn-color11-3 btn-xs sharp mx-1"
                                    title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button v-if="userInfo &&userInfo.RoleSeq == 1 " v-on:click="removeRec(item.Seq)" class="btn btn-color11-4 btn-xs sharp mx-1"
                                    title="刪除"><i class="fas fa-trash"></i></button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-show="!NisShow">
                <div class="">
                    <hr class="my-4">
                    <h2 v-if="QisShow">{{ constCheckItem.ItemName }} 抽查紀錄新增</h2>
                    <table v-show="QisShow || !isNew" class="table table2 min720 my-0" border="0">
                        <tbody>
                            <tr>
                                <th style="width:110px">分項工程</th>
                                <td>
                                    <div class="form-inline">
                                        <select v-model="si.EngConstructionSeq" @change="onEngConstructionChange"
                                            v-bind:disabled="!QisShow" class="form-control mb-2 mr-sm-2">
                                            <option v-for="option in engConstructions" v-bind:value="option.Value"
                                                v-bind:key="option.Value">
                                                {{ option.Text }}
                                            </option>
                                        </select>
                                        <label class="m-1">檢查日期</label>
                                        <b-input-group class="mydatewidth">
                                            <input v-bind:value="chsRecDate" @change="onDateChange(chsRecDate, $event)"
                                                type="text" class="form-control" placeholder="yyy/mm/dd"
                                                v-bind:disabled="!QisShow ">
                                            <b-form-datepicker v-model="si.CCRCheckDate" :hide-header="1 == 1" button-only
                                                right size="sm" @context="onDatePicketChange($event)"
                                                v-bind:disabled=" si.FormConfirm > 0 "
                                                >
                                            </b-form-datepicker>
                                        </b-input-group>&nbsp;
                                        <select v-if="fDebug" v-model="si.CCRCheckType1" @change="onCheckTypeChange"
                                            class="form-control mb-2 mr-sm-2" disabled>
                                            <option value="1">施工抽查</option>
                                            <option value="2">設備運轉測試</option>
                                            <option value="3">職業安全</option>
                                            <option value="4">生態保育</option>
                                        </select>
                                        <label class="m-1">階段</label>
                                        <select v-if="constCheckMode == 1 || constCheckMode == 4" v-model="si.CCRCheckFlow"
                                            @change="onCheckFlowChange" class="form-control mb-2 mr-sm-2"
                                            v-bind:disabled="!QisShow">
                                            <option value="1">施工前</option>
                                            <option value="2">施工中</option>
                                            <option value="3">施工後</option>
                                        </select>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>檢查位置</th>
                                <td>
                                    <div class="form-inline">
                                        <input v-model="si.CCRPosDesc" @change="onResultChange" type="text"
                                            class="form-control col-10" placeholder="輸入位置描述" v-bind:disabled="!QisShow">
                                        <button v-if="isNew && QisShow" v-on:click="onNewRec" role="button"
                                            class="btn btn-outline-secondary btn-xs mx-1 col-1">
                                            新增
                                        </button>
                                    </div>

                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div v-show="!isNew">
                    <div class="row">
                        <div class="col col-lg-4">
                            <label class="m-1 small-green row">
                                <div class="col-12 pl-0">
                                    * 下方為自動帶出抽查管理標準的項目
                                </div>
                                <div class="col-12 pl-0">
                                    * 點 <button class="btn btn-outline-secondary btn-xs sharp m-1" disabled><i
                                            class="fas fa-copy"></i></button> 後，可帶入前次抽查紀錄，需修改資料後方可儲存
                                </div>

                            </label>
                        </div>
                        <div class="col col-lg-3 p-3" v-if="Object.keys(SuggestionList).length > 0">
                            <button type="button" class="btn btn-info" @click="openSuggestionModal">改善建議</button>
                        </div>

                    </div>

                    <div class="table-responsive mb-4">
                        <table class="table table1 min910 my-0 tableLayoutFixed" border="1">
                            <thead>
                                <tr>
                                    <th style="width:50px"> 

                                    </th>
                                    <th style="width:150px">管理項目</th>
                                    <th style="width:200px">抽查標準（定量定性）</th>
                                    <th style="width:200px" v-if="!isNew">
                                        <button :disabled="( si.FormConfirm > 0)"  @click="getRecResultHistory"
                                            class="btn btn-outline-secondary btn-xs sharp m-1" title="複製前次抽查紀錄"><i
                                                class="fas fa-copy"></i></button>
                                        實際抽查情形
                                    </th>
                                    <th v-if="!isNew" style="width:120px">抽查結果</th>
                                    <!-- th v-if="!isNew">照片</th -->
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in items" v-bind:key="item.ControllStSeq">
                                    <td v-if="item.rowSpan > 0" v-bind:rowspan="item.rowSpan">
                                    <button :disabled="( si.FormConfirm > 0)" class="btn btn-color11-4 btn-xs mx-1" @click="DeleteRecResultControlSt(item)" >
                                            <i data-v-c6f9d7a2="" class="fas fa-trash"></i>
                                        </button>

                                    </td>
                                    <td v-if="item.rowSpan > 0" v-bind:rowspan="item.rowSpan">{{ item.CheckItem1 }}</td>
                                    <td v-if="item.CheckFields >= 0 && item.rowSpanStd1 > 0" v-bind:rowspan="item.rowSpanStd1" >
                                        <span v-for="(ele,index) in item.itemStandardDivs" :key="index">
                                            <span v-if="ele.value.length > 0">
                                                    {{ ele.value }}
                                            </span>

                                            <input  v-else class="form-control" :disabled="( si.FormConfirm > 0) || !item.editable" v-model="item.StandardValues[ele.key ]" style="border: #d8c4d0 solid;"/>
                                        </span>
                                        <!-- <button
                                            v-if="item.itemType == 4 && si.FormConfirm == 0 && (item.CCRRealCheckCond == null || item.CCRRealCheckCond.length == 0)"
                                            @click="openSetStd(item)" data-toggle="modal" data-target="#setStdModal"
                                            role="button" class="btn btn-color11-3 btn-xs mx-1">
                                            <i class="fas fa-pen"></i>
                                        </button> -->
                                    </td>
                                    <td v-if="!isNew && item.rowSpan > 0" v-bind:rowspan="item.rowSpan">
                                        <p style="color:red" 
                                        v-if="!item.editable"  class="p-2"
                                        
                                        >鋼筋、模板、混凝土請透過水利工程抽查APP完成影像辨識及抽查記錄 </p>
                                        <div contenteditable="true" aria-multiline="true" v-else>
                                            <textarea v-model.trim="item.CCRRealCheckCond"
                                                :disabled="( si.FormConfirm > 0) || !item.editable"
                                                v-bind:rows="item.rowSpan == 1 ? 3 : 6"
                                                v-bind:class="[item.changed ? '' : 'mustEdit']"
                                                @change="onResultChange(item)" v-on:keyup="onInputKeyup(index, item)"
                                                maxlength="150" class="form-control"></textarea>
                                        </div>
                                    </td>
                                    <td v-if="!isNew && item.rowSpan > 0" v-bind:rowspan="item.rowSpan">
                                        <div>
                                            <select v-model="item.CCRCheckResult" @change="onResultChange(item)"
                                                class="form-control"                           :disabled="( si.FormConfirm > 0) || !item.editable">
                                                <option v-for="option in getCheckResultOption(item)"
                                                    v-bind:value="option.Value" v-bind:key="option.Value">
                                                    {{ option.Text }}
                                                </option>
                                            </select>
                                            <div v-show="constCheckMode == 1 && item.CCRCheckResult == 2"
                                                class="custom-control custom-checkbox">
                                                <input v-model="item.CCRIsNCR" @change="onResultChange(item)"
                                                    v-bind:id="'check_NCR_' + index" type="checkbox"
                                                class="custom-control-input"                           :disabled="( si.FormConfirm > 0) || !item.editable">
                                                <label class="custom-control-label"
                                                    v-bind:for="'check_NCR_' + index">NCR</label>
                                            </div>
                                            <!-- button v-on:click="uploadPhotoModal(item)" v-show="si.FormConfirm<1" class="btn btn-color11-2 btn-xs mx-1">
                                            <i class="fas fa-upload"></i> 上傳照片
                                        </button -->
                                        </div>
                                        <div>
                                            <textarea v-model="item.RecResultRemark" class="form-control"
                                                maxlength="150"                           :disabled="( si.FormConfirm > 0)  || !item.editable"> </textarea>
                                        </div>
                                    </td>
                                    <!--
                                <td v-if="!isNew && item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                                    <PhotoList v-bind:engMain="engMain.Seq" v-bind:si="si" v-bind:ctlSeq="item.ControllStSeq" v-bind:ref="'photoList'+item.Seq"></!PhotoList>
                                </td>
                                -->
                                </tr>
                            </tbody>
                        </table>
                    </div>


                    <hr />
                    <label class="m-1 small-green">
                        抽驗項目如需要照片，請拍照後上傳至對應的管理項目
                    </label>
                    <div class="form-inline mb-3">
                        <label class="m-2">管理項目：</label>
                        <select v-model="selectStdItem" class="form-control my-1 mr-0 mr-sm-1">
                            <option v-for="option in checkItems" v-bind:value="option" v-bind:key="option.Seq">
                                {{ option.CheckItem1 }}
                            </option>
                        </select>
                        <button v-on:click="uploadPhotoModal(selectStdItem)" v-show="si.FormConfirm < 1"
                            class="btn btn-color11-2 btn-xs mx-1">
                            <i class="fas fa-upload"></i> 上傳照片
                        </button>
                        <div v-if="si.FormConfirm != 0" style="color:red;padding-left: 10px;">
                            抽查已確認，無法上傳照片
                        </div>
                    </div>
                    <PhotoList1 v-bind:engMain="engMain.Seq" v-bind:si="si" v-bind:ctlSeq="selectStdItem.ControllStSeq"
                        ref="photoList"></PhotoList1>
                    <div class="d-flex" v-if=" si.FormConfirm == 0">
                        <div>
                            <h5 ref="s3" >監造現場人員簽章：</h5>
                            <canvas ref="s1" 
                                style="background: rgb(238, 238, 238) none repeat scroll 0% 0%; width: 400px; height: 200px;"
                                width="400" height="200"></canvas>
                            <div>
                                <button @click="SupervisionComSignature.onConvertToImageClick('img1')" id="convertToImage" class="btn btn-color11-3 btn-xs mx-1">轉圖</button>
                                <button id="clear" class="btn btn-color9-1 btn-xs mx-1" @click="SupervisionComSignature.clear()">清除</button>
                            </div>
                            <div v-show="SupervisionComSignature.Signature.length > 0" ref="img1"
                                style="display: block; width: 400px; height: 200px; max-width: 400px; max-height: 200px; ">
                                <img  src="">
                            </div>
                        </div>
                        <div class="ml-4">
                            <h5>監造主任簽章：</h5>
                            <canvas ref="s2" id="s2"
                                style="background: rgb(238, 238, 238) none repeat scroll 0% 0%; width: 400px; height: 200px;"
                                width="400" height="200"></canvas>
                            <div>
                                <button @click="SupervisionDirectorSignature.onConvertToImageClick('img2')" id="convertToImage" class="btn btn-color11-3 btn-xs mx-1">轉圖</button>
                                <button id="clear" class="btn btn-color9-1 btn-xs mx-1" @click="SupervisionDirectorSignature.clear()">清除</button>
                            </div>
                            <div v-show="SupervisionDirectorSignature.Signature.length > 0" ref="img2"
                                style="display: block; width: 400px; height: 200px; max-width: 400px; max-height: 200px; ">
                                <img  src="">
                            </div>
                        </div>
                    </div>


                </div>
            </div>
        </form>

        <div v-if="!isNew" class="row justify-content-center mt-4">
            <button v-on:click="onUpdateRec" v-bind:disabled="si.FormConfirm > 0" role="button"
                class="btn btn-color11-4 btn-xs mx-1">
                <i class="fas fa-save"></i> 儲存
            </button>
            <button v-on:click="onFormConfirm" v-bind:disabled="si.FormConfirm > 0" role="button"
                class="btn btn-color11-3 btn-xs mx-1">
                <i class="fa fa-check"></i> 確認
            </button>
            <button v-on:click="onDelRec" v-bind:disabled="si.FormConfirm > 0 && userInfo.RoleSeq != 1" role="button"
                class="btn btn-color9-1 btn-xs mx-1">
                <i class="fas fa-trash-alt"></i> 刪除
            </button>
        </div>
        <!-- div v-if="isNew  && !isShow && QisShow" class="row justify-content-center">
        <button v-on:click="onNewRec" role="button" class="btn btn-outline-secondary btn-xs mx-1">
            <i class="fas fa-plus"></i>&nbsp;&nbsp;新增抽查
        </button>
    </div -->

        <div  class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)"
            v-bind:style="{ display: modalShow ? 'block' : 'none' }" tabindex="-1" role="dialog"
            aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">上傳照片</h5>
                    </div>
                    <div class="modal-body">
                        <input id="inputFile" type="file" name="file" multiple="" v-on:change="fileChange($event)" />
                        <div>
                            <label>照片說明</label>
                            <input v-model="photoDesc" class="form-control" />
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal"
                            v-on:click="closeModal()">Close</button>
                        <button type="button" class="btn btn-color11-2 btn-xs mx-1" v-on:click.stop="upload_api()"><i
                                class="fas fa-upload"></i> 上傳</button>
                    </div>
                </div>
            </div>
        </div>
        <!-- s20231016-->
        <div class="modal fade" id="setStdModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
            aria-labelledby="setStdModal" aria-modal="true" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="width:600px">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title" id="projectUpload">抽查標準</h6>
                    </div>
                    <div class="modal-body">
                        <textarea v-model.trim="editStdData" maxlength="150" rows="5" class="form-control"></textarea>
                    </div>
                    <div class="modal-footer">
                        <button @click="setStdClick" type="button" class="btn btn-primary">
                            確定
                        </button>
                        <button type="button" id="closeSetStdModal" class="btn btn-secondary" data-dismiss="modal"
                            aria-label="Close">
                            取消
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <ImproveSuggestionModal ref="ImproveSuggestionModal" title="改善建議" headerColor="#17a2b8">
            <template #body>
                <div class="card" v-for="([tag, suggestions], index) in Object.entries(SuggestionList) " :key="index">
                    <div class="card-title">
                        {{ tag }}
                    </div>
                    <div class="card-body">
                        <pre v-for="(suggestion, index2) in suggestions"
                            :key="index2">{{ index2 + 1 }}. {{ suggestion.Text }}</pre>
                    </div>
                </div>
            </template>
        </ImproveSuggestionModal>
    </div>
</template>
<script>
import Canvas  from "../../Common/Canvas.js";
export default {
    props: ['tenderItem', 'constCheckMode', 'constCheckItem'],
    data: function () {
        return {
            userInfo :{},
            SupervisionComSignature  : {
                Signature : ''
            },
            SupervisionDirectorSignature : {
                Signature : ''
            },
            //s20230520
            fDebug: false,
            selectRecEngConstruction: -1,
            recEngConstructions: [], //有檢驗單之分項工程
            selectCheckFlow: -1,
            selectEngConstruction: -1,
            engConstructions: [], //分項工程
            //
            fResultChange: false,
            targetItem: { Name: '' },

            isNew: true,
            keyWord: '',
            //選項
            chsRecDate: '',
            recCheckTypeOption: [],
            ItemSeqOption: [], //檢驗標準項目清單

            //表頭資料
            si: {
                Seq: -1,
                EngConstructionSeq: -1,
                CCRCheckDate: null,
                CCRCheckType1: -1,//檢驗項目
                ItemSeq: -1,//檢驗標準項目
                CCRCheckFlow: 1,////施工流程
                CCRPosLati: '',
                CCRPosLong: '',
                CCRPosDesc: '',
                FormConfirm: 0
            },
            srcSi: {
                Seq: -1,
                EngConstructionSeq: -1,
                CCRCheckDate: null,
                CCRCheckType1: -1,//檢驗項目
                ItemSeq: -1,//檢驗標準項目
                CCRCheckFlow: 1,////施工流程
                CCRPosLati: '',
                CCRPosLong: '',
                CCRPosDesc: '',
                FormConfirm: 0
            },
            //檢驗單
            searchCheckType: 1,
            searchCheckTypeOld: -1,
            selectRecItem: -1,
            selectRecItemOption: [],
            fGetRecMode: false,
            //檔案上傳
            modalShow: false,
            photoItem: null,
            targetInput: {},
            files: new FormData(),
            photofiles: new FormData(),
            photoDesc: '',
            //
            targetId: -1,
            engMain: {},
            items: [],
            //蜂窩api回傳值
            restful: '無蜂窩',
            //查詢or新增show
            isShow: true,
            QisShow: true,
            NisShow: true,
            //s20230302
            itemsHistory: [], //複製清單
            checkItems: [],
            selectStdItem: { ControllStSeq: -1 },
            //s20230530
            //分頁
            recordTotal: 0,
            pageRecordCount: 5,
            pageIndex: 1,
            //s20231016
            editStdItem: null,
            editStdData: '',
            SuggestionList: [],
            itemStandardValues : {}
        };
    },
    components: {
        PhotoList1: require('./PhotoList1.vue').default,
        ImproveSuggestionModal: require('../../components/Modal.vue').default,

    },
    methods: {
        removeRec(id)
        {
            window.myAjax.post("SamplingInspectionRec/removeRec", {id : id}).then(resp => {
                if(resp.data == true)
                    this.onSelectCheckFlowChange();
            })
        },
        renderStandardInput(item)
        {
            item.StandardCurrentIndex = 0;
            item.StandardValuesStr = item.StandardValuesStr ?? "";
            item.StandardValues = item.StandardValuesStr.split(',') ;
            item.itemStandardDivs = item.itemStandardDivs ?? [];
            let temp = null;
            let ii = 0;
            if(!item.Stand1 || item.Stand1.length == 0 ) return;
            item.Stand1.split('_').forEach( e  => {

                if(temp =="" && e == "" ) return ;
                
                item.itemStandardDivs.push( {value: e, key : ii })
                if(e == "") ii++;
                temp = e;

            })

            console.log("aaaaa",item);
        },
        async DeleteRecResultControlSt(item)
        {
            if(confirm("確定要刪除?"))
            {
                let {data : res } = await window.myAjax.post("SamplingInspectionRec/DeleteRecResultControlSt", {
                    controlStSeq : item.ControllStSeq,
                    recSeq : item.ConstCheckRecSeq,
                    span : item.rowSpan,
                    CCRCheckType1 : this.si.CCRCheckType1
                });
                if(res == true)
                {
                    this.getRecResult();
                    alert("刪除成功");
                }   
            }


        },
        async getSuggestion(recItem) {
            console.log(this.constCheckItem);
            let { data } = await window.myAjax.post("Suggestion/GetSuggestion", { function: recItem.CCRCheckType1, functionSeq: recItem.ItemSeq });
            this.SuggestionList = data;
        },
        openSuggestionModal() {
            this.$refs.ImproveSuggestionModal.show = true;
        },
        //填寫抽查標準 s20231016
        openSetStd(item) {
            this.editStdData = '';
            this.editStdItem = item;
        },
        setStdClick() { //設定抽查標準 s20231016
            this.editStdItem.CCRRealCheckCond = "(抽查標準為:" + this.editStdData + ")";
            document.getElementById('closeSetStdModal').click();
        },
        //分項工程清單 s20230520
        getEngConstruction() {
            this.engConstructions = [];
            this.selectEngConstruction = -1;
            window.myAjax.post('/SamplingInspectionRec/GetSubEngNameList1',
                {
                    engMain: this.tenderItem.Seq,
                })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engConstructions = resp.data.items;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onEngConstructionChange() {
            if (this.constCheckMode == 2 || this.constCheckMode == 3) {//設備運轉測試清單,職業安全衛生清單
                this.si.CCRCheckFlow = 1;
                this.onCheckFlowChange();
            }
        },
        //有檢驗單之分項工程清單 s20230520
        getRecEngConstruction() {
            this.recEngConstructions = [];
            this.targetId = -1;
            this.selectRecEngConstruction = -1;
            window.myAjax.post('/SamplingInspectionRec/GetRecEngConstruction',
                {
                    mode: this.constCheckMode,
                    eId: this.tenderItem.Seq,
                    rId: this.constCheckItem.Seq
                })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.recEngConstructions = resp.data.items;
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        /*/檢驗單之分項工程 s20230520
        onRecEngConstructionChange() {
            //this.NisShow = true;
            this.selectCheckFlow = -1;
            this.selectRecItemOption = [];
            this.targetId = this.selectRecEngConstruction;
            this.si.EngConstructionSeq = this.targetId;
            this.searchCheckType = this.constCheckMode;
            this.getEngItem();
        },*/
        //檢驗流程 s20230520
        onSelectCheckFlowChange() {
            //this.NisShow = true;
            this.si.Seq = -1;
            this.isNew = true;
            this.QisShow = false;
            this.NisShow = false;
            this.selectRecItemOption = [];
            window.myAjax.post('/SamplingInspectionRec/GetRecOptionByCheckType1', {
                id: this.tenderItem.Seq,
                checkTypeSeq: this.constCheckMode, //s20230628 this.searchCheckType,
                cFlow: -1, //this.selectCheckFlow
                itemSeq: this.constCheckItem.Seq, //s20230628
                pageIndex: this.pageIndex,
                perPage: this.pageRecordCount
            })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.selectRecItemOption = resp.data.items;
                        this.recordTotal = resp.data.totalRows;
                    }
                    /*if (this.selectRecItemOption.length > 0) {
                        //this.NisShow = false;
                        this.selectRecItem = this.selectRecItemOption[0].Seq;
                        this.onSelectRecItemChange();
                    }*/
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //抽查結果選項
        getCheckResultOption(item) {
            var option = [
                { Value: 1, Text: '檢查合格' }, { Value: 2, Text: '有缺失' }
            ];
            if (item.CheckItem1.indexOf('☆檢驗停留點') == -1)
                option.push({ Value: 3, Text: '無此項目' });

            return option;
        },
        //工程資訊
        getEngItem() {
            this.step = 2;
            this.engMain = {};
            window.myAjax.post('/SamplingInspectionRec/GetEngItem', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain = resp.data.item;
                        this.getRecCheckTypeOption();
                        /*if (this.constCheckMode == 2 || this.constCheckMode == 3) {//設備運轉測試清單,職業安全衛生清單
                            this.selectCheckFlow = 1;
                            this.onSelectCheckFlowChange();
                        }*/
                    } else {
                        // alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //已有檢驗單之檢驗項目
        async getRecCheckTypeOption() {
            this.recCheckTypeOption = [];
            const { data } = await window.myAjax.post('/SamplingInspectionRec/GetRecCheckTypeOption', { id: this.targetId });
            this.recCheckTypeOption = data;
        },
        //檢驗項目 變動
        onCheckTypeChange() {
            if (!this.noSaveAlert()) {
                this.si.CCRCheckType1 = this.srcSi.CCRCheckType1;
                return;
            }

            this.isNew = true;
            if (!this.fGetRecMode) {
                this.si.Seq = -1;
                this.si.ItemSeq = -1;
            }
            this.ItemSeqOption = [];
            this.items = [];
            window.myAjax.post('/SamplingInspectionRec/GetStdTypeOption', { engMain: this.tenderItem.Seq, checkType: this.si.CCRCheckType1 })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.ItemSeqOption = resp.data.item;
                        if (this.fGetRecMode) this.getRecResult();
                    }
                    this.fGetRecMode = false;
                })
                .catch(err => {
                    this.fGetRecMode = false;
                    console.log(err);
                });
        },
        //檢驗標準項目 變動
        onStdChange() {
            if (!this.noSaveAlert()) {
                this.si.ItemSeq = this.srcSi.ItemSeq;
                return;
            }

            this.si.Seq = -1;
            this.isNew = true;
            this.getStdList();
        },
        //流程變動
        onCheckFlowChange() {
            if (this.isNew) {
                this.getStdList();
                return;
            } else if (!this.noSaveAlert()) {
                this.si.CCRCheckFlow = this.srcSi.CCRCheckFlow;
                return;
            }
            this.si.Seq = -1;
            this.isNew = true;
            this.getStdList();
        },
        getStdList() {
            if (this.si.CCRCheckType1 == -1 || this.si.ItemSeq == -1) return;

            this.items = [];
            window.myAjax.post('/SamplingInspectionRec/GetControls'
                , { engMain: this.tenderItem.Seq, checkType: this.si.CCRCheckType1, stdType: this.si.ItemSeq, checkFlow: this.si.CCRCheckFlow })
                .then(resp => {
                    if (resp.data.result == 0) {

                        
                        this.items = resp.data.item;
                        
                        
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //關鍵字搜索清單
        onSearchCheckType(keyWord) {
            if (keyWord.length == 0) return;
            if (!this.noSaveAlert()) return;

            window.myAjax.post('/SamplingInspectionRec/SearchCheckTypeByKeyWord'
                , { engMain: this.tenderItem.Seq, keyWord: keyWord })
                .then(resp => {
                    if (resp.data.result == 0) {
                        var item = resp.data.item;
                        this.si.CCRCheckType1 = item.CCRCheckType1;
                        this.onCheckTypeChange();
                        this.si.ItemSeq = item.ItemSeq;
                        this.onStdChange();
                    } else {
                        alert(resp.data.message)
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onResultChange(item) {
            if (!this.isNew) {
                if (item) item.ResultItem = 1;
                this.fResultChange = true;
            }
        },
        noSaveAlert() {
            if (this.fResultChange && !this.isNew) {
                var result = confirm('目前資料將不會儲存,是否確定?');
                if (result) this.fResultChange = false;
                return result;
            }
            return true;
        },
        onDateChange(srcDate, event) {
            if (event.target.value.length == 0) {
                this.si.CCRCheckDate = '';
                return;
            }
            if (!window.comm.isExistDate(event.target.value)) {
                event.target.value = srcDate;
                alert("日期錯誤");
            } else {
                this.si.CCRCheckDate = window.comm.toYearDate(event.target.value);
                this.onResultChange();
            }
        },
        onDatePicketChange(ctx, mode) {
            if (ctx.selectedDate != null) {
                var d = ctx.selectedDate;
                var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                this.chsRecDate = dd;
                this.onResultChange();
            }
        },
        onInquire() {
            this.QisShow = false;
            this.NisShow = false;
            this.initData();
            //this.getRecEngConstruction(); //this.onSearchCheckTypeChange(1);
            this.onSelectCheckFlowChange();
        },
        onCreatNew() {
            this.QisShow = true;
            this.NisShow = false;
            this.isShow = false;
            this.isNew = true;
            //s20230520
            this.si.CCRCheckType1 = this.constCheckMode;
            this.onCheckTypeChange();
            this.si.ItemSeq = this.constCheckItem.Seq;
            this.si.CCRCheckFlow = -1;
            this.si.EngConstructionSeq = -1;
            //this.getStdList();
        },
        onNewRec() {
            if (this.si.EngConstructionSeq == -1) {
                alert('分項工程未選取');
                return;
            }
            if (this.items.length == 0) {
                alert('無檢驗項目無法新增抽查');
                return;
            }
            if (this.si.CCRPosDesc == null || this.si.CCRPosDesc.length == 0) {
                alert('請輸入檢查位置');
                return;
            } else if (this.si.CCRCheckType1 == -1 || this.si.ItemSeq == -1) {
                alert('請選取基本資料');
                return;
            }
            window.myAjax.post('/SamplingInspectionRec/NewRec', { item: this.si })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.getRecCheckTypeOption();
                        this.onSearchCheckTypeChange();
                        this.getRecHeader(resp.data.seq);
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
            //this.isNew = false;
        },
        //儲存檢驗單
        onUpdateRec() {
            if (this.si.CCRPosDesc == null || this.si.CCRPosDesc.length == 0) {
                alert('請輸入檢查位置');
                return;
            }

            if (this.itemsHistory.length > 0) {
                for (var i = 0; i < this.items.length; i++) {
                    let item = this.items[i];
                    if (item.rowSpan > 0 && !item.changed) {
                        alert('複製過的 [實際抽查情形] 資料必須全數編輯過');
                        //return;//s20230720
                    }
                }
            }
            this.si.SupervisionComSignature = this.SupervisionComSignature.Signature;
            this.si.SupervisionDirectorSignature = this.SupervisionDirectorSignature.Signature;
            console.log("ff", this.si);
            this.items.forEach(e => {
                e.StandardValuesStr = e.StandardValues.reduce((a, c) => a +"," + c, "").slice(1);
            })
            window.myAjax.post('/SamplingInspectionRec/UpdateRec', { recItem: this.si, items: this.items })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.fResultChange = false;
                        this.onSearchCheckTypeChange();
                    }
                    alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //檢驗單確認
        onFormConfirm() {
            if (this.si.Seq == -1) {
                this.isNew = true;
                return
            }
            if (confirm('資料確認後將無法修改, 是否確認此抽查單?')) {
                window.myAjax.post('/SamplingInspectionRec/FormConfirm', { engMain: this.engMain, id: this.si.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.si.FormConfirm = 1;
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        //檢驗單表頭資訊
        getRecHeader(id) {
            if (id == -1) return;
            this.targetId = -1;
            window.myAjax.post('/SamplingInspectionRec/GetRec', { recSeq: id })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.si = resp.data.item;
                        this.chsRecDate = this.si.chsCheckDate;
                        this.si.CCRCheckDate = window.comm.toYearDate(this.chsRecDate);
                        this.srcSi = JSON.parse(JSON.stringify(this.si));
                        this.fGetRecMode = true;
                        this.onCheckTypeChange();
                        //s20230525
                        this.targetId = this.si.EngConstructionSeq;
                        this.getEngItem();
                        //this.getRecResult();
                        //this.isNew = false;
                        this.getSuggestion(resp.data.item);
                        this.SupervisionComSignature.setImage(this.si.SupervisionComSignature);
                        this.SupervisionDirectorSignature.setImage(this.si.SupervisionDirectorSignature);
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //取得檢驗單紀錄
        getRecResult() {
            this.fResultChange = false;
            this.items = [];
            this.checkItems = [];
            this.itemsHistory = [];
            this.checkItems = [];
            this.selectStdItem = { ControllStSeq: -1 };

            window.myAjax.post('/SamplingInspectionRec/GetRecResult', { rec: this.si })
                .then(resp => {
                    if (resp.data.result == 0) {
                        console.log("ffff");
                        resp.data.item.forEach(e => {
                            if(
                                (this.constCheckItem.ItemName.trim().startsWith('鋼筋') || this.constCheckItem.ItemName.trim().startsWith('混凝土')
                                        || this.constCheckItem.ItemName.trim().startsWith('模板') ) && 
                                        (e.CheckItem1.trim() == '表面修飾' || e.CheckItem1.trim() == '鋼筋外觀'  || e.CheckItem1.trim() == '模板外觀'  )
                            ){
                                e.editable = false;
                            }
                            else e.editable = true;
                        })
         
                        this.isNew = false;
                        this.items = resp.data.item;
                        this.items.forEach(e => this.renderStandardInput(e))
                        for (var i = 0; i < this.items.length; i++) {
                            let item = this.items[i];
                            if (item.rowSpan > 0) {
                                this.checkItems.push(item);
                            }
                        }
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
            this.fGetRecMode = false;
        },
        //取得前一次檢驗單紀錄 s20230302
        getRecResultHistory() {
            //this.fResultChange = false;
            this.itemsHistory = [];
            window.myAjax.post('/SamplingInspectionRec/GetRecResultHistory', { rec: this.si })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.itemsHistory = resp.data.item;
                        if (this.itemsHistory.length == 0) {
                            alert('無抽查紀錄');
                        } else {
                            this.setItemData();
                        }
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
            this.fGetRecMode = false;
        },
        //s20230302 複製資料
        setItemData() {
            let targetItems = this.itemsHistory.reduce((a, c) => {
                a[c.ControllStSeq] = c;
                return a;

            }, {});
            this.items.forEach(item => {
                let targetItem = targetItems[item.ControllStSeq];
                if(!targetItem) return;

                // if (targetItem.rowSpan > 0 
                //     && !window.comm.stringEmpty(targetItem.CCRRealCheckCond) 
                //     && item.CCRRealCheckCond != targetItem.CCRRealCheckCond) {
                //     item.CCRRealCheckCond = targetItem.CCRRealCheckCond;
                //     item.changed = false;
                // } else {
                //     item.changed = true;
                // }
                item.CCRRealCheckCond = targetItem.CCRRealCheckCond;
                item.changed = true;
            })
            this.onUpdateRec();

        },
        //s20230302 是否編輯過資料
        onInputKeyup(inx, item) {
            if (this.itemsHistory.length > 0 && item.rowSpan > 0 && item.CCRRealCheckCond != this.itemsHistory[inx].CCRRealCheckCond) {
                item.changed = true;
            }
        },
        //檢驗日期變更
        onSearchCheckTypeChange(type) {
            if (!this.noSaveAlert()) {
                this.searchCheckType = this.searchCheckTypeOld;
                return;
            }
            if (this.targetId == -1) return;

            this.searchCheckTypeOld = this.searchCheckType;
            if (this.isNew) this.selectRecItem = -1;
            this.selectRecItemOption = [];
            window.myAjax.post('/SamplingInspectionRec/GetRecOptionByCheckType', {
                constructionSeq: this.targetId,
                checkTypeSeq: this.searchCheckType
            })
                .then(resp => {
                    this.selectRecItemOption = resp.data;
                    /*if (type == 1 && this.selectRecItemOption.length > 0) {
                        this.selectRecItem = this.selectRecItemOption[0].Seq;
                        this.onSelectRecItemChange()
                    }*/
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onSelectRecItemChange() {
            this.getRecHeader(this.selectRecItem);
        },
        //刪除檢驗單
        onDelRec() {
            if (this.si.Seq == -1) {
                this.isNew = true;
                return
            }
            if (confirm('是否刪除確定?')) {
                window.myAjax.post('/SamplingInspectionRec/DelRec', { id: this.si.Seq })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isNew = true;
                            this.items = [];
                            this.getRecCheckTypeOption();
                            this.onInquire();//s20230522 this.onSearchCheckTypeChange();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        //照片上傳
        uploadPhotoModal(item) {
            this.files = new FormData();
            this.photofiles = new FormData();
            this.photoItem = item;
            this.modalShow = true;
        },
        closeModal() {
            this.photoDesc = '';
            this.targetInput.value = '';
            this.modalShow = false;
        },
        fileChange(event) {
            this.targetInput = event.target;
            this.files.append("file", this.targetInput.files[0], this.targetInput.files[0].name);
        },
        upload() {
            const files = this.files;
            files.append("engMain", this.tenderItem.Seq);
            files.append("recSeq", this.photoItem.ConstCheckRecSeq);
            files.append("ctlSeq", this.photoItem.ControllStSeq);
            files.append("photoDesc", this.photoDesc);
            files.append("restful", this.restful);
            window.myAjax.post('/SamplingInspectionRec/PhotoUpload', files,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                }).then(resp => {
                    if (resp.data.result == 0) {
                        this.closeModal();
                        //this.$refs['photoList' + this.photoItem.Seq][0].refresh();
                        this.$refs['photoList'].refresh(this.selectStdItem.ControllStSeq);
                    }
                    alert(resp.data.message);
                }).catch(error => {
                    console.log(error);
                });
        },
        back() {
            //window.history.go(-1);
            window.location = "/SamplingInspectionRec";
        },
        async upload_api() {
            const photofiles = this.photofiles;
            photofiles.append("image", this.targetInput.files[0]);
            //console.log(this.targetInput.files[0].type);
            if (this.photoItem.CheckItem1.indexOf('混凝土') > -1) {//s20230302 關鍵字才作辨識
                await fetch('https://211.22.221.188:5001/api', { method: "POST", secure: false, body: photofiles })
                    .then((resp) => {
                        return resp.json();
                    }).then((resp) => {
                        if (resp.message == 1) {
                            this.restful = '有蜂窩';
                        } else {
                            this.restful = '無蜂窩';
                        }
                    })
                    .catch((error) => {
                        console.log(`Error: ${error}`);
                    })
            } else {
                this.restful = '';
            }
            this.upload();
        },
        //s20230520
        initData() {
            
            this.isNew = true;
            this.items = [];
            this.selectRecEngConstruction = -1;
            this.selectCheckFlow = -1;
            this.selectRecItemOption = [];
            this.si = {
                Seq: -1,
                EngConstructionSeq: -1,
                CCRCheckDate: null,
                CCRCheckType1: -1,//檢驗項目
                ItemSeq: -1,//檢驗標準項目
                CCRCheckFlow: 1,////施工流程
                CCRPosLati: '',
                CCRPosLong: '',
                CCRPosDesc: '',
                FormConfirm: 0
            };
            this.srcSi = {
                Seq: -1,
                EngConstructionSeq: -1,
                CCRCheckDate: null,
                CCRCheckType1: -1,//檢驗項目
                ItemSeq: -1,//檢驗標準項目
                CCRCheckFlow: 1,////施工流程
                CCRPosLati: '',
                CCRPosLong: '',
                CCRPosDesc: '',
                FormConfirm: 0
            };
            var Today = new Date();
            this.si.CCRCheckDate = Today;
            this.chsRecDate = Today.getFullYear() - 1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
        },
        //分頁 20230530
        onPaginationChange(pInx, pCount) {
            this.pageRecordCount = pCount;
            this.pageIndex = pInx;
            this.onSelectCheckFlowChange();
        },
        async getUserInfo() {
            this.userInfo = (await window.myAjax.post("Users/GetUserInfo")).data.userInfo;
        }
    },
    async mounted() {
        console.log('mounted() 抽查紀錄填報-編輯');


        
        this.getEngConstruction();
        this.onSelectCheckFlowChange();//s20230524
        this.onInquire();
        this.getUserInfo();

        console.log("TT", this.$refs);
        console.log("AA", this.$refs.s1);
        this.SupervisionComSignature = new Canvas(this.$refs.s1, this.$refs.img1);
        this.SupervisionDirectorSignature = new Canvas(this.$refs.s2, this.$refs.img2);

        /*if (this.subEngNameSeq > 0) {
            this.targetId = this.subEngNameSeq;
            this.si.EngConstructionSeq = this.targetId;
            //var Today = new Date();
            //this.si.CCRCheckDate = Today;
            //this.chsRecDate = Today.getFullYear() - 1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
            this.getEngItem();
            return;
        }*/
        //window.location = "/EPCQualityVerify";
    },
}
</script>
<style scoped>.mustEdit {
    color: red;
    font-weight: bold;
    background-color: #ffdfdf;
}</style>