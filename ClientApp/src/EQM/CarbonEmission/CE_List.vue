<template>
    <div v-show="targetId > 0">
        <h5 class="insearch mt-0 py-2">
            工程編號：{{tenderItem.TenderNo}}({{tenderItem.EngNo}})<br>工程名稱：{{tenderItem.TenderName}}({{tenderItem.EngName}})
        </h5>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <button v-if="ceHeader.PccesXMLDateStr != ''" @click="downloadPccesTemplate(ceHeader)" class="btn btn-color11-1 btn-xs mx-1">
                    <i class="fas fa-download"></i> Pcces下載({{ceHeader.PccesXMLDateStr}})
                </button>
            </li>
            <li class="nav-item">
                <button v-if="ceHeader.PccesXMLDateStr" @click="downloadPcces(ceHeader)" class="btn btn-color11-1 btn-xs mx-1">
                    <i class="fas fa-download"></i> Pcces修正下載
                </button>
            </li>
        </ul>
        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a @click="selTab='TaPrice'" class="nav-link active" data-toggle="tab" href="#menu01">詳細價目表</a>
            </li>
            <li v-if="items.length>0" class="nav-item">
                <!-- button title="碳排量檢核表" class="btn btn-block btn-color12 btn-sm" data-toggle="modal" data-target="#prepare_edit01">碳排量檢核表</button -->
                <a @click="selTab='TabCheckTable'" class="nav-link" data-toggle="tab" href="#menu02">碳排量檢核表</a>
            </li>
            <li v-if="items.length>0" class="nav-item ml-2">
                <!-- button @click="onCarbonTradingClick" title="碳交易" class="btn btn-block btn-color12 btn-sm" style="background-color: #1aa009;" data-toggle="modal" data-target="#CarbonTrading">碳交易</button -->
                <a @click="selTab='CarbonTrading'" class="nav-link" data-toggle="tab" href="#menu03">碳交易</a>
            </li>
        </ul>
        <div class="tab-content">
            <!-- 一 -->
            <div id="menu01" class="tab-pane active">
                <h5>
                    工程總價：{{tenderItem.SubContractingBudget}}元，綠色經費：{{greenFunding}}元，綠色經費占比：{{greenFundingRate}}%
                    &nbsp;如需修改請點<button disabled class="btn btn-color11-1 btn-xs sharp mx-1"><i class="fas fa-pencil-alt"></i></button>圖示<br />
                    核定碳排量：{{tenderItem.ApprovedCarbonQuantity}}&nbsp;kgCO2e，設計排放量：{{co2Total}}&nbsp;kgCO2e，可拆解率：{{dismantlingRate}}%
                    &nbsp;如需修改請點<button disabled class="btn btn-color11-3 btn-xs sharp mx-1"><i class="fas fa-pencil-alt"></i></button>圖示
                </h5>
                <form class="form-group insearch mb-3">
                    <div class="form-row">
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <select v-model="selectLevel" @change="onLevelChange" class="form-control">
                                <option v-bind:key="index" v-for="(item,index) in selectitems" v-bind:value="item.Value">{{item.Text}}</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                            <button type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-search"></i></button>
                        </div>
                        <div v-if=" ceHeader.State == 0 || !(Role > 2 && ceHeader.State ==2 )" class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <label class="btn btn-block btn-color11-2 btn-sm">
                                <input v-on:change="fileChange($event)" id="inputFile" type="file" name="file" multiple="" style="display: none;" >
                                <i class="fas fa-upload"></i> 匯入PCCES
                            </label>
                            <!-- button @click="ImportPcces" type="button" class="btn btn-block btn-color12 btn-sm">匯入PCCES</button-->
                        </div>
                        <!-- div v-if="ceHeader.State==0" class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="onCalCarbonEmissions" type="button" class="btn btn-block btn-color12 btn-sm">計算碳排放量</button>
                        </div>
                        -->
                        <div v-if="ceHeader.State==0 && Role <= 2 " class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="onLock" type="button" class="btn btn-block btn-color12 btn-sm"><i class="fa fa-lock"></i>鎖定</button>
                        </div> 
                        <div v-else-if="ceHeader.State==2 && Role <= 2 " class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="unLock" type="button" class="btn btn-block btn-color11-1 btn-sm"><i class="fa fa-lock"></i>解鎖</button>
                        </div> 
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="downloadPayItem" type="button" class="btn btn-block btn-color11-1 btn-sm"><i class="fas fa-download"></i> 碳排放量估算表下載</button>
                        </div>
                    </div>
                </form>

                <comm-pagination ref="pagination" :recordTotal="totalRows" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
                <SuggesttionModal ref="SuggesttionModal" :title="`${editSuggestionItem.Description} 修改意見填寫`">
                                            <template #body>
                                                <input type="text" class="form-control"  v-model="editSuggestionItem.Suggestion"/>
                                                <div class="d-flex mt-3 justify-content-center">
                                                    <button class="btn btn-color11-2 btn-xs  mx-1"  @click="sendSuggestion">送出</button>
                                                </div>
                     
                                            </template>
                                        </SuggesttionModal>
                <div class="tableFixHead">
                    <table class="table table-responsive-md table-hover VA-middle">
                        <thead class="insearch">
                            <tr>
                                <th class="text-left"><strong>工程項目代號</strong></th>
                                <th style="width: 42px;"><strong>項次</strong></th>
                                <th style="width: 300px !important;"><strong>項目及說明</strong></th>
                                <th style="width: 42px;"><strong>單位</strong></th>
                                <th class="text-right"><strong>數量</strong></th>
                                <th class="text-right"><strong>金額(元)</strong></th>
                                <th class="text-right" style="max-width: 120px;"><strong>碳係數(kgCO2e)</strong></th>
                                <th class="text-right"><strong>工項碳排放量</strong></th>
                                <!-- th style="min-width: 110px;"><strong>綠色經費</strong></th>
                                <th><strong>綠色經費修改說明</strong></th -->
                                <th style="width: 100px;"><strong>編碼(備註)</strong></th>
                                <th style="width: 100px;"><strong v-if="editSeq != -99" >修改說明</strong></th>
                                <th><strong>管理</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in items" v-bind:key="item.Seq" v-bind:class="{'GreenFunding': item.GreenFundingSeq!=null }">

                                <td class="text-left"><strong>{{item.PayItem}}</strong></td>
                                <td>{{item.ItemNo}}</td>
                                <td>{{item.Description}}</td>
                                <td><span v-html="item.UnitFmt"></span></td>
                                <td class="text-right">{{item.Quantity}}</td>
                                <td class="text-right">{{item.Amount}}</td>
                                <template v-if="item.Seq != editSeq">
                                    <td class="text-right">{{item.KgCo2e}}</td>
                                    <td class="text-right">{{item.ItemKgCo2e}}</td>
                                    <!-- td class="text-right">
                                        <select v-model="item.GreenFundingSeq" disabled class="form-control">
                                            <option v-bind:key="index" v-for="(item,index) in greenFundingOptions" v-bind:value="item.Value">{{item.Text}}</option>
                                        </select>
                                    </td>
                                    <td class="text-right">{{item.GreenFundingMemo}}</td -->
                                    <td colspan="2" class="text-left">
                                        <span v-html="memoTitle(item)"></span>
                                        <br />修改說明:{{editTitle(item)}}
                                        <br />

                                        <label  v-show="item.Suggestion">
                                            水利署修改意見 :
                                        </label>


                                        <div  class="d-inline-block col-12" tabindex="0" v-b-tooltip :title="item.Suggestion ">
                                            <label>{{(item.Suggestion ?? "").slice(0, 10)}} </label> <span v-if="item.Suggestion && item.Suggestion.length > 15"> ... </span>
                                        </div>
                                    </td>
                                    <td>
                                     
                                        <div class="d-flex justify-content-center" >
                                            <button v-if="editSuggestion != index" @click="onEditSuggestion(item)" class="btn btn-color11-2 btn-xs  mx-1"   ><i class="fas fa-pencil-alt"></i> 意見</button>
                                            <!-- <button v-else @click="onEditSuggestion(index, false)" class="btn btn-color11-2 btn-xs  mx-1"   ><i class="fas fa-pencil-alt"></i> 取消 </button> -->
                                            <button  v-if=" canEdit(item) && ! (Role > 2 && ceHeader.State ==2 )" @click="onEditRecord(item)" class="btn btn-color11-3 btn-xs sharp mx-1" :disabled="ceHeader.State!=0 && ceHeader.State!=2"  ><i class="fas fa-pencil-alt"></i></button>
                                            <button  v-if=" canEdit(item) && ! (Role > 2 && ceHeader.State ==2 )" @click="onEditRecord1(item)" class="btn btn-color11-1 btn-xs sharp mx-1" data-toggle="modal" data-target="#green_edit"><i class="fas fa-pencil-alt"></i></button>
                                        </div>
                                    </td>
                                </template>
                                <template v-if="item.Seq == editSeq">
                                    <td><input v-model="editRecord.KgCo2e" type="text" class="form-control text-right"></td>
                                    <td class="text-right">{{item.ItemKgCo2e}}</td>
                                    <!-- td class="text-right"></td>
                                    <td class="text-right"></td -->
                                    <td class="text-right"><span v-html="item.RStatusCodeStr"></span></td>
                                    <td><input v-model.trim="editRecord.Memo" maxlength="100" type="text" class="form-control text-left"></td>
                                    <td>
                                        <div class="d-flex justify-content-center">
                                            <button @click.prevent="onSaveRecord(index)" class="btn btn-color11-2 btn-xs sharp mx-1"><i class="fas fa-save"></i></button>
                                            <button @click.stop="editSeq = -99" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i></button>
                                        </div>
                                    </td>
                                </template>
                            </tr>
                            <tr>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div>
                    <p style="color:red">***表列中編碼有問題的部分需修正，否則碳排量無法正確計算</p>
                    <p style="color:red">1. 不足10碼，顯示”不足10碼”，不提供修改(使用者須修改pcces)</p>
                    <p style="color:red">2. 如果是E開頭，比對最後一碼，單位的資料與最後一碼資訊不一樣者(1:時、2:天、3:月、4:式、5:年、6:趟、7:半天)，顯示”單位代碼錯誤”；不提供修改(使用者須修改pcces).(機具)</p>
                    <p style="color:red">3. 如果是L開頭，比對最後一碼，單位的資料與最後一碼資訊不一樣者(1:時、2:工、3:月、4:式、5:年)，顯示”單位代碼錯誤”；不提供修改(使用者須修改pcces).(人力)</p>
                    <p style="color:red">4. 如果不是E開頭，比對最後一碼，單位的資料與第10碼資訊不一樣者(1:M、2:M2、3:M3、4:式、5:T、6:只、7:個、8:組、9:KG)，顯示”單位代碼錯誤”；不提供修改(使用者須修改pcces)</p>
                    <p>*** 以下條件比對取原始編碼後10碼進行 ***</p>
                    <p style="color:red">5. 編碼比對後台全部符合【全部10碼】，直接帶係數，提供修改</p>
                    <p style="color:red">6. 【前5碼+細目編碼+第10碼】比對後台全部符合，直接帶係數，提供修改</p>
                    <p style="color:red">7. 【前5碼】為02931、02932</p>
                    <p style="color:red">&nbsp;&nbsp;&nbsp;7-1. 【前5碼+細目編碼+第10碼帶'0'】有比對到，直接帶係數</p>
                    <p style="color:red">&nbsp;&nbsp;&nbsp;7-2. 項目第10碼是0，【前5碼+細目編碼】比對不到，係數直接帶0</p>
                    <p style="color:red">8. 【前5碼+細目編碼】都比對到，第10碼比對不到，顯示”查無單位係數”；提供修改</p>
                    <p style="color:red">9. 【備註為不分類+前5碼】有比對到，直接帶係數</p>
                    <p style="color:red">10. 其它，顯示”查無編碼”；可提供修改</p>
                    <p style="color:red">上述有修改都要填修改說明</p>
                    <p>***              ***</p>
                    <p style="color:red">本平台自動計算碳排量功能為提供簡易快速上手使用，計算結果仍須由設計人員以專業判斷</p>
                    <p style="color:red">(尤其混凝土礦物摻料比例系統預設為50%、土石籠及石籠系統預設以石籠為主等)依實際設計內容檢核碳排係數自動帶入之正確性，</p>
                    <p style="color:red">並涉未自動帶入部分，則應在水利署函頒指引編定原則下，合理化計算及於擷取狀態欄特別註記。</p>

                    <p>***              ***</p>
                </div>
            </div>
            <!-- 碳排量檢核表 -->
            <div id="menu02" class="tab-pane">
                <template v-if="items.length>0">
                    <form class="form-group insearch">
                        <div class="form-row">
                            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                                <button @click="downloadPayItem" type="button" class="btn btn-block btn-color11-1 btn-sm"><i class="fas fa-download"></i> 碳排放量估算表下載</button>
                            </div>
                            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                                <button @click="downloadCheckTable" type="button" class="btn btn-block btn-color11-1 btn-sm"><i class="fas fa-download"></i> 簡易檢核表檔案下載</button>
                            </div>
                        </div>
                    </form>
                    <CheckTable v-if="selTab=='TabCheckTable'" v-bind:tenderItem="tenderItem" v-bind:ceHeader="ceHeader"></CheckTable>
                </template>
            </div>
            <div id="menu03" class="tab-pane">
                <div v-if="tenderItem.ApprovedCarbonQuantity == null">
                    <h5 class="card-title font-weight-bold">沒有核定的碳排量資料</h5>
                </div>
                <div v-if="items.length>0 && tenderItem.ApprovedCarbonQuantity != null && selTab=='CarbonTrading'">
                    <form class="form-group insearch">
                        <div class="form-row">
                            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                                <button @click="downloadPayItem" type="button" class="btn btn-block btn-color11-1 btn-sm"><i class="fas fa-download"></i> 碳排放量估算表下載</button>
                            </div>
                        </div>
                    </form>
                    <h5 class="card-title font-weight-bold">碳交易 核定碳排量：{{tenderItem.ApprovedCarbonQuantity}}&nbsp;kgCO2e，設計排放量：{{co2Total}}&nbsp;kgCO2e</h5>
                    <CarbonTrading ref="CarbonTradingModal" v-bind:tenderItem="tenderItem" v-bind:co2Total="co2Total" v-on:reload="reload"></CarbonTrading>
                </div>
            </div>
        </div>
        <!-- 綠色經費修改 modal -->
        <div class="modal fade" id="green_edit" tabindex="-1" ria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width: fit-content;" >
                <div class="modal-content">
                    <div class="card whiteBG mb-4 pattern-F colorset_1">
                        <div class="card-header">
                            <h3 class="card-title font-weight-bold">綠色經費</h3>
                            <button ref="btnCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">×</span>
                            </button>
                        </div>
                        <div class="tab-content">
                            <div class="table-responsive">
                                <table class="table table-responsive-md table-hover VA-middle">
                                    <tbody>
                                        <tr>
                                            <th style="width: 130px;"><strong>類型</strong></th>
                                            <td class="text-right" colspan="3">
                                                <select v-model="editRecord.GreenFundingSeq" class="form-control">
                                                    <option v-bind:key="index" v-for="(item,index) in greenFundingOptions" v-bind:value="item.Value">{{item.Text}}</option>
                                                </select>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><strong>備註說明</strong></th>
                                            <td colspan="3">
                                                <textarea v-model.trim="editRecord.GreenFundingMemo" rows="4" maxlength="100" type="text" class="form-control text-left"></textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="row justify-content-center">
                            <div class="d-flex">
                                <button @click="onSaveGreenFundingRecord(1)" role="button" class="btn btn-color11-2 btn-x mx-1">
                                    <i class="fas fa-save">&nbsp;儲存</i>
                                </button>
                            </div>
                            <div class="d-flex">
                                <button @click="onSaveGreenFundingRecord(0)" role="button" class="btn btn-color9-1 btn-x mx-1">
                                    <i class="fas fa-times">&nbsp;取消</i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import SuggesttionModal from "../../components/Modal.vue";
    export default {
        components: {
            CheckTable: require('./CE_CheckTable.vue').default,
            CarbonTrading: require('./CE_CarbonTrading.vue').default,
            SuggesttionModal : SuggesttionModal
        },
        data: function () {
            return {
                Role : {},
                targetId: null,
                tenderItem: {},
                selectitems: [],
                selectLevel: '',
                co2Total: null,
                dismantlingRate: null,//可拆解率

                ceHeader: {},
                items: [],
                editSeq: -99,
                selRecord: {},
                editRecord: {},
                //分頁
                totalRows: 0,
                editSuggestion : -1,
                isAdmin: false,
                //s20230418
                greenFundingOptions: [],
                greenFundingChange: false,
                approvedCarbonQuantity: null,
                greenFunding: null,//s20230418
                greenFundingRate: null,//s20230418
                selTab:'TaPrice',//s20230428
                editSuggestionItem : {}
            };
        },
        methods: {
            sendSuggestion()
            {
                window.myAjax.post("/EQMCarbonEmission/UpdatePayItemSuggestion", {m : this.editSuggestionItem } )
                .then(resp => {
                    if(resp.data == true) {
                        alert("成功");
                        this.$refs.SuggesttionModal.show = false;
                    }
                })
            },
            onEditSuggestion(item)
            {
                this.editSuggestionItem = item;
                this.$refs.SuggesttionModal.show = true;
            },
            //下載 節能減碳簡易檢核表 s20230428
            downloadCheckTable() {
                window.comm.dnFile('/EQMCarbonEmission/DnCheckTable?id=' + this.targetId);
            },
            //更新
            reload() {
                this.getCEHeader();
            },
            //開啟碳交易
            onCarbonTradingClick() {
                this.$refs.CarbonTradingModal.getTradeList();
            },
            //綠色經費項目 20230418
            getGreenFundingList() {
                window.myAjax.post('/EQMCarbonEmission/GreenFundingList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.greenFundingOptions = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //是否可編輯
            canEdit(item) {
                if (item.RStatusCode == 300 /*|| item.RStatusCode <= 0 || item.RStatusCode == 6*/
                    /*|| item.RStatusCode == 200 || item.RStatusCode == 99 || item.RStatusCode == 98 || item.RStatusCode == 97*/) return false;
                return true;
            },
            //編碼(備註) shioulo20230204
            memoTitle(item) {
                if (item.RStatusCode == 300) {
                    return "";
                } else if (item.RStatusCode == 200){
                    return item.RStatusCodeStr;
                } else if (item.RStatusCode == 97 || item.RStatusCode == 98 || item.RStatusCode == 99
                    || item.RStatusCode == 100 || item.RStatusCode == 101) {
                    return item.Memo;
                } else if (item.RStatusCode == 51 || item.RStatusCode == 55 || item.RStatusCode == 56
                    || item.RStatusCode == 147 || item.RStatusCode == 148
                    || item.RStatusCode == 149 || item.RStatusCode == 150 || item.RStatusCode == 151
                    || item.RStatusCode == 201 ) {
                    return item.RStatusCodeStr;// + ' <br>' + item.Memo;
                } else {
                    return item.RStatusCodeStr;
                }
            },
            editTitle(item) {
                if (item.RStatusCode < 10
                    || item.RStatusCode == 97 || item.RStatusCode == 98 || item.RStatusCode == 99
                    || item.RStatusCode == 100 || item.RStatusCode == 101
                    || item.RStatusCode == 200) {
                    return "";
                } else {
                    return item.Memo;
                }
            },
            unLock() {
                window.myAjax.post('/EQMCarbonEmission/unLockData', { id: this.targetId })
                    .then(resp => {
                        alert(resp.data.msg);
                        this.getCEHeader();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onLock() {
                window.myAjax.post('/EQMCarbonEmission/LockData', { id: this.targetId })
                    .then(resp => {
                        alert(resp.data.msg);
                        this.getCEHeader();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //檢查是否有未填寫理由
            onCheck() {
                window.myAjax.post('/EQMCarbonEmission/CheckData', { id: this.targetId })
                    .then(resp => {
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //計算碳排量
            onCalCarbonEmissions() {
                window.myAjax.post('/EQMCarbonEmission/CalCarbonEmissions', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onPaginationChange(pInx, pCnt) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCnt:" + pCnt);
                this.getResords();
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            //儲存
            onSaveRecord(index) {
                if (this.isAdmin && this.strEmpty(this.editRecord.Memo)) {//s20230307
                    alert('編碼(備註) 必須輸入!');
                    return;
                }
                else if (!this.isAdmin && (this.strEmpty(this.editRecord.KgCo2e) || this.strEmpty(this.editRecord.Memo))) {
                    alert('碳排係數, 編碼(備註) 必須輸入!');
                    return;
                }/* else if (this.editRecord.GreenFundingSeq != this.selRecord.GreenFundingSeq && this.strEmpty(this.editRecord.GreenFundingMemo)) {//s20230418
                    alert('綠色經費修改說明 必須輸入!');
                    return;
                }*/
                window.myAjax.post('/EQMCarbonEmission/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            // this.items[index] = this.editRecord;
                            this.getResords();
                        } else
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.selRecord = item;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //編輯紀錄 綠色經費 s20230428
            onEditRecord1(item) {
                this.selRecord = item;
                this.editRecord = Object.assign({}, item);
            },
            //儲存 s20230428
            onSaveGreenFundingRecord(mode) {
                /* //s20230526 取消檢查
                if (this.editRecord.GreenFundingSeq != this.selRecord.GreenFundingSeq && this.strEmpty(this.editRecord.GreenFundingMemo)) {//s20230418
                    alert('綠色經費修改說明 必須輸入!');
                    return;
                }*/
                if (mode == 0) this.editRecord.GreenFundingSeq = ''; //s20230526
                window.myAjax.post('/EQMCarbonEmission/UpdateRecord', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$refs.btnCloseModal.click();
                            this.getResords();
                        } else
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //清單
            getResords() {
                this.editSeq = -99;
                this.items = [];
                this.isAdmin = false;
                window.myAjax.post('/EQMCarbonEmission/GetList', {
                    id: this.targetId,
                    pageIndex: this.$refs.pagination.pageIndex,
                    perPage: this.$refs.pagination.pageRecordCount,
                    keyword: this.selectLevel
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.totalRows = resp.data.totalRows;
                            this.isAdmin = resp.data.admin;
                            if (this.selectitems.length == 0) this.getLevel1();
                            this.co2Total = resp.data.co2Total;
                            this.dismantlingRate = resp.data.dismantlingRate;
                            this.greenFunding = resp.data.greenFunding;
                            this.greenFundingRate = resp.data.greenFundingRate;
                            this.getCEHeader();
                        } else if (resp.data.result == -1) {
                            alert(resp.data.msg)
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //匯入PCCES
            fileChange(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('text/xml')) {
                    alert('請選擇 XML 檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.targetId);
                uploadfiles.append("file", files[0], files[0].name);
                this.upload(uploadfiles);
            },
            upload(uploadfiles) {
                window.myAjax.post('/EQMCarbonEmission/UploadXML', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getItem();//s20230410
                            //this.getLevel1();
                            //this.getResords();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //
            getLevel1() {
                this.selectitems = [];
                window.myAjax.post('/EQMCarbonEmission/GetLevel1List', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.selectitems = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onLevelChange() {
                this.getResords();
            },
            //下載
            downloadPayItem() {
                window.comm.dnFile('/EQMCarbonEmission/Download?id=' + this.targetId);
            },
            downloadPccesTemplate(item) 
            {
                window.comm.dnFile('/EQMCarbonEmission/DownloadXML?id=' + item.EngMainSeq);
            },
            downloadPcces(item) {
                window.comm.dnFile('/EQMCarbonEmission/DownloadPccesXML?id=' + item.EngMainSeq);
            },
            //碳排量計算主檔
            getCEHeader() {
                this.ceHeader = { };
                window.myAjax.post('/EQMCarbonEmission/GetCEHeader', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ceHeader = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EQMCarbonEmission/GetEngMain', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.Role = resp.data.Role;
                            this.getGreenFundingList();
                            this.getLevel1();
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 碳排量計算');
            this.targetId = window.sessionStorage.getItem(window.eqSelTrenderPlanSeq);
            this.getItem();
            /*console.log('mounted() 建立標案'+ this.userUnit+ ' '+this.userUnitSub);
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }*/
        },
    }
</script>
<style scoped>
.tableFixHead          { overflow: auto; max-height: 500px;   }
table {
    border-collapse: separate;
    border-spacing: 0;
}
.table {
    margin : 0;
}
.tableFixHead thead  { position: sticky !important ; top: 0 !important ; z-index: 1 !important;     }
th {
    border : 0;
    border-bottom: #ddd solid 1px !important; 
    border-left : 0 !important;
    border-right:0 !important;
}
td {
    z-index: 0;
    position: relative;
}
</style>