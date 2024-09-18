<template>
    <div>
        <form @submit.prevent>
            <div v-show="isAdd && step==1">
                <h2>Step1. 匯入預算書資料(請將PCCES的預算書資料*.XML在此處匯入)</h2>
                <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                    <div v-if="!file" :class="['dropZone', dragging ? 'dropZone-over' : '']"
                         @dragestart="dragging = true" @dragenter="dragging = true" @dragleave="dragging = false">
                        <div class="dropZone-info" @drag="onChange">
                            <span class="fa fa-cloud-upload dropZone-title"></span>
                            <span class="dropZone-title">拖拉檔案至此區塊 或 點擊此處</span>
                            <div class="dropZone-upload-limit-info">
                                <!-- div>許可附屬檔名: xml</div -->
                            </div>
                        </div>
                        <input type="file" @change="onChange" />
                    </div>
                    <div v-if="file" class="form-row justify-content-center">
                        <div class="dropZone-uploaded">
                            <div class="dropZone-uploaded-info">
                                <span class="dropZone-title">選取的檔案: {{ file.name }}</span>
                                <div class="uploadedFile-info">
                                    <button @click="removeFile" type="button" class="col-2 btn btn-shadow btn-color1">
                                        取消選取
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-if="file" class="row justify-content-center my-3">
                    <div class="col-12 col-sm-4">
                        <button @click="uploadXML(0)" type="button" class="btn btn-shadow btn-color1 btn-block">
                            確認
                        </button>
                    </div>
                    <div v-if="updateXMLMode" class="col-12 col-sm-4">
                        <a v-on:click.stop="uploadXML(1)" role="button" class="btn btn-shadow btn-danger btn-block">
                            仍上傳覆蓋原有資料
                        </a>
                    </div>
                    <div class="col-12 col-sm-4">
                        <button v-on:click.stop="back()" role="button" class="btn btn-shadow btn-color1 btn-block">
                            回上頁
                        </button>
                    </div>
                </div>
            </div>
            <div v-show="step>1">
                <div v-if="fCanEdit &&( engMain.chsAwardDate == null  || engMain.chsAwardDate.length == 0 )  " class="form-row" role="toolbar">
                    <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                        <label class="btn btn-block btn-outline-secondary btn-sm" >
                            <input v-on:change="onUpdatePccesChange($event)" type="file" name="file" multiple="" style="display:none;">
                            <i class="fas fa-upload"></i>&nbsp;更新PCCES
                        </label>
                    </div>
                </div>
                <!-- hr class="my-5" -->
                <h2>Step2. 請輸入基本資料(以下資料系統會自動匯入PCCES資料，請確認)</h2>
                <div class="setFormcontentCenter">
                    <div class="form-row">
                        <div class="col-12 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程名稱<span class="small-red">&nbsp;*</span></label>
                            <input v-model.trim="engMain.EngName" type="text" placeholder="PCCES帶入"
                                   class="col-12 col-md-10 form-control">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程地點<span class="small-red">&nbsp;*</span></label>
                            <div>
                                <select v-model="engMain.CitySeq" @change="onCityChange($event)"
                                        class="form-control my-1 mr-0 mr-sm-1">
                                    <option v-for="option in cities" v-bind:value="option.Value"
                                            v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                                <select v-model="engMain.EngTownSeq"
                                        class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4">
                                    <option v-for="option in towns" v-bind:value="option.Value"
                                            v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程年度<span class="small-red">&nbsp;*</span></label>
                            <input v-model="engMain.EngYear" type="number" class="form-control" :disabled=" userRole > 2 && ( engMain.chsAwardDate != null && engMain.chsAwardDate.length != 0 )" />
                        </div>
                    </div>
                    <div class="form-row">

                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程編號<span class="small-red">&nbsp;*</span></label>
                            <input v-model.trim="engMain.EngNo" type="text" :disabled="userRole != 1"
                                   class="form-control my-1 mr-0 mr-md-4 WidthasInput" placeholder="PCCES帶入">
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">工程會標案編號                                 <span style="color:red" v-if="!this.engMain.EngYear">(請先設定年度)</span></label>
                            <div role="group" class="input-group">
                                <input v-model.trim="engMain.TenderNo" disabled type="text" class="form-control">
                                <button v-on:click="onTenderSearch()" v-bind:disabled="(this.engMain.Seq == -1) || !this.engMain.EngYear"
                                        title="標案查詢" class="btn btn-sm bg-gray" data-toggle="modal"
                                        data-target="#refTenderListModal"  >
                                    <i class="fas fa-search"></i>
                                </button>
                                <button v-on:click="onCancelTenderLink()" title="清除" class="btn btn-sm bg-gray"  :disabled="!this.engMain.EngYear">
                                    <i class="fas fa-times"></i>
                                </button>

                            </div>

                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-4  my-2 justify-content-md-between">
                            <label class="my-2 mx-2">主辦機關<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.OrganizerUnitSeq"
                                    @change="getOrganizerSubUnit(engMain.OrganizerUnitSeq)"
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in units" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                        <div class="col-8 col-md-4  my-2 justify-content-md-between">
                            <label for="a">工程會標案編號</label><input type="text" disabled
                                                                 class="form-control my-1 mr-0 mr-md-4" id="a" v-model="engMain.TenderNo">

                        </div>
                        <div class="col-8 col-md-4  my-2 justify-content-md-between">
                            <label for="a">工程會標案名稱</label><input type="text" disabled
                                                                 class="form-control my-1 mr-0 mr-md-4" id="a" v-model="engMain.TenderName">

                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">執行機關<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.ExecUnitSeq" @change="getExecSubUnit(engMain.ExecUnitSeq)"
                                    ref="refExecUnit" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in units" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">執行單位<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.ExecSubUnitSeq" @change="getUsers()"
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in execSubUnits" v-bind:value="option.Value"
                                        v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                        <!-- s20231102 系統管理者,分署管理者 才能變更-->
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">標案建立者<span class="small-red">&nbsp;*</span></label>
                            <select v-model="engMain.OrganizerUserSeq" v-bind:disabled="!(userRole==1 || userRole==3)"
                                    class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in users" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">核定金費(元) (來自工程提報)</label>
                            <input v-model="engMain.TotalBudget" v-bind:disabled="!isAdmin" @change="onTotalBudgetChange()" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="預設發包預算">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">發包預算(元) (來自預算書XML)</label>
                            <input v-model="engMain.SubContractingBudget" v-bind:disabled="!isAdmin" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="PCCES帶入">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-7 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">決標金額(千元) (來自工程會標案管理系統)</label>
                            <input v-model="engMain.BidAmount" v-bind:disabled="!isAdmin" type="text"
                                   class="form-control my-1 mr-0 mr-md-4" placeholder="來自工程會標案管理系統">
                        </div>
                    </div>
                    <!-- div class="form-row">
        <div class="col form-inline my-2">
            <label class="my-2 mx-2">採購金額(元)</label>
            <div class="custom-control custom-radio mx-2">
                <input v-model="engMain.PurchaseAmount" value="1" type="radio" id="FiftyMillion"
                       class="custom-control-input" name="ProcurementValue">
                <label class="custom-control-label" for="FiftyMillion">5000萬以上</label>
            </div>
            <div class="custom-control custom-radio mx-2">
                <input v-model="engMain.PurchaseAmount" value="2" type="radio" id="TenToFifty"
                       class="custom-control-input" name="ProcurementValue">
                <label class="custom-control-label" for="TenToFifty">1000-5000萬</label>
            </div>
            <div class="custom-control custom-radio mx-2">
                <input v-model="engMain.PurchaseAmount" value="3" type="radio" id="OneToTen"
                       class="custom-control-input" name="ProcurementValue">
                <label class="custom-control-label" for="OneToTen">100-1000萬</label>
            </div>
            <label class="my-2 mx-2">*依工程總預算判斷</label>
        </div>
    </div -->
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                            <label class="my-2 mx-2">決標日期</label>
                            <input v-if="!fCanEdit" 
                                :disabled="!(engMain.chsAwardDate == null || engMain.chsAwardDate.length == 0)"
                                v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" type="text" class="form-control my-1 mr-0 mr-md-4" placeholder="yyy/mm/dd">
                          
                            <b-input-group v-if="fCanEdit ">
                                <input  :disabled="!(engMain.chsAwardDate == null || engMain.chsAwardDate.length == 0 )" 
                                v-bind:value="engMain.chsAwardDate" ref="chsAwardDate" @change="onDateChange(engMain.chsAwardDate, $event, 'AwardDate')" type="text" class="form-control mydatewidth" placeholder="yyy/mm/dd">
                             
                                <b-form-datepicker v-show="fCanEdit  &&  AwardYearChangeStep == 1" :hide-header="hideHeader"

                                                   button-only right size="sm" @context="onDatePicketChange($event, 'AwardDate')">
                                </b-form-datepicker>

                            </b-input-group>
                            <button class="btn btn-color11-2 btn-xs  mx-1" v-show="userRole == 1 &&   AwardYearChangeStep == 0" @click="engMain.chsAwardDate = null">清除</button>

                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2">
                            <label class="my-2 mx-2">需求碳排量(kgCO2e)</label>
                            <input v-model="engMain.CarbonDemandQuantity" v-bind:disabled="engMain.OfficialApprovedCarbonQuantity" type="text"
                                   class="form-control my-1 ml-1 mr-0 mr-md-4" style="width:120px;">
                        </div>
                        <div class="col-12 col-md-6 form-inline my-2">
                            <label class="my-2 mx-2">核定碳排量(kgCO2e)</label>
                            <input v-model="engMain.ApprovedCarbonQuantity" v-bind:disabled="engMain.OfficialApprovedCarbonQuantity" type="text"
                                   class="form-control my-1 ml-1 mr-0 mr-md-4" style="width:120px;">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="col-12 col-md-6 form-inline my-2">
                            <input v-model="engMain.DredgingEng" v-bind:disabled="disabledDredgingEng" type="checkbox" class="my-2 mx-2">&nbsp; 疏濬工程
                        </div>
                    </div>
                </div>

                <hr class="my-5">
                <h2>自辦監造人員, 委外需至工程履約填寫計畫資料</h2>
                <div v-if="fCanEdit" class="form-row">
                    <div class="col-12 col-md-3 form-inline my-2">
                        <label class="my-2 mx-2">職務</label>
                        <select v-model="supervisorKind" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option value="0">監造主任</option>
                            <option value="1">現場人員</option>
                            <option value="3">設計人員</option>
                        </select>
                    </div>
                    <div class="col-12 col-md-3 form-inline my-2">
                        <label class="my-2 mx-2">單位</label>
                        <select v-model="supervisorSubUnitSeq" @change="getSupervisorUsers"
                                class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in execSubUnits" v-bind:value="option.Value"
                                    v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                    <div class="col-12 col-md-3 form-inline my-2">
                        <label class="my-2 mx-2">人員</label>
                        <select v-model="supervisorUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in supervisorUsers" v-bind:value="option.Value"
                                    v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                    <div class="col-12 col-md-1 form-inline my-2">
                        <button @click="addSupervisorUser" class="btn btn-color11-2 btn-xs sharp mx-1" title="加入">
                            <i class="fas fa-plus fa-lg"></i>
                        </button>
                    </div>
                </div>
                <div class="table-responsive">
                    <table class="table table1">
                        <thead class="insearch">
                            <tr>
                                <th><strong>職務</strong></th>
                                <th><strong>單位</strong></th>
                                <th><strong>人員</strong></th>
                                <th v-if="fCanEdit"><strong>管理</strong></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in engSupervisor" v-bind:key="item.Seq">
                                <td>{{kindCaption(item.UserKind)}}</td>
                                <td>{{item.SubUnitName}}</td>
                                <td>{{item.UserName}}</td>
                                <td v-if="fCanEdit">
                                    <div class="d-flex justify-content-center">
                                        <button @click="delEngSupervisor(item)" v-if="item.UserKind != 2"
                                                class="btn btn-color11-2 btn-xs sharp mx-1" title="移除">
                                            <i class="fas fa-trash-alt"></i>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <hr class="my-5">
                <h2>Step3. 請輸入監造計畫資料</h2>
                <div class="setFormcontentCenter">
                    <div class="form-row">
                        <div class="col form-inline my-2">
                            <label class="my-2 mx-2">是否需要機電設備</label>
                            <div class="custom-control custom-radio mx-2">
                                <input v-model="engMain.IsNeedElecDevice" value="true" type="radio" id="YesEE"
                                       class="custom-control-input" name="ElectricalEquipment">
                                <label class="custom-control-label" for="YesEE">是</label>
                            </div>
                            <div class="custom-control custom-radio mx-2">
                                <input v-model="engMain.IsNeedElecDevice" value="false" type="radio" id="NoEE"
                                       class="custom-control-input" name="ElectricalEquipment">
                                <label class="custom-control-label" for="NoEE">否</label>
                            </div>
                        </div>
                    </div>
                </div>
                <hr class="my-5">
                <h2>
                    Step4. 工程主要施工項目及數量(請自行新增資料)&nbsp;<a href="#" class="a-blue underl mx-2" title="參考範例"
                                                        data-toggle="modal" data-target="#refSampleModal">參考範例</a>
                </h2>
                <label class="mb-1 small-red">
                    * 工程主要施工項目及數量請務必填寫，⑤
                </label><br />
                <label class="mb-1 small-red">
                    * 數量請填整數
                </label>
                <div v-if="fCanEdit" class="form-row justify-content-xl-between">
                    <div class="form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2"><span class="small-red">*&nbsp;</span>工項：</label>
                        <input v-model="engConstruction.ItemName" maxlength="50" type="text"
                               class="form-control mr-0 mr-md-4">
                    </div>
                    <div class="form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2"><span class="small-red">*&nbsp;</span>數量：</label>
                        <input v-model="engConstruction.ItemQty" type="text" class="form-control mr-0 mr-md-4">
                    </div>
                    <div class="form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2"><span class="small-red">*&nbsp;</span>單位：</label>
                        <input v-model="engConstruction.ItemUnit" maxlength="10" type="text"
                               class="form-control mr-0 mr-md-4">
                        <div class="d-flex">
                            <button v-on:click.stop="addConstruction()" role="button"
                                    class="btn btn-outline-secondary btn-xs mx-1">
                                <i class="fas fa-plus"></i> 新增
                            </button>
                        </div>
                    </div>
                </div>
                <div class="table-responsive">
                    <table class="table table1" border="0">
                        <thead>
                            <tr>
                                <th class="sort">項次</th>
                                <th>分項工程</th>
                                <th>數量</th>
                                <th>單位</th>
                                <th style="width:140px">功能</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in engConstructionList" v-bind:key="item.id">
                                <td>{{index+1}}</td>
                                <td>
                                    <div v-if="!item.edit">{{item.ItemName}}</div>
                                    <input v-if="item.edit" type="text" v-model.trim="item.ItemName" maxlength="50"
                                           class="form-control" />
                                </td>
                                <td class="text-right">
                                    <div v-if="!item.edit">{{item.ItemQty}}</div>
                                    <input v-if="item.edit" type="text" v-model.trim="item.ItemQty"
                                           class="form-control" />
                                </td>
                                <td>
                                    <div v-if="!item.edit">{{item.ItemUnit}}</div>
                                    <input v-if="item.edit" type="text" v-model.trim="item.ItemUnit" maxlength="10"
                                           class="form-control" />
                                </td>
                                <td>
                                    <div v-if="fCanEdit" class="d-flex">
                                        <template v-if="!item.edit">
                                            <button v-on:click.stop="item.edit=!item.edit"
                                                    class="btn btn-color11-3 btn-xs mx-1" title="編輯">
                                                <i class="fas fa-pencil-alt"></i> 編輯
                                            </button>
                                            <button v-on:click.stop="delConstruction(index, item.Seq)"
                                                    class="btn btn-color9-1 btn-xs mx-1" title="刪除">
                                                <i class="fas fa-trash-alt"></i> 刪除
                                            </button>
                                        </template>
                                        <template v-if="item.edit">
                                            <button v-on:click.stop="saveConstruction(item)"
                                                    class="btn btn-color11-2 btn-xs mx-1">
                                                <i class="fas fa-save"></i> 儲存
                                            </button>
                                            <button v-on:click.stop="item.edit = false"
                                                    class="btn btn-color9-1 btn-xs mx-1" title="取消">
                                                <i class="fas fa-times"></i> 取消
                                            </button>
                                        </template>

                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <hr class="my-5">
                <h2>Step5. 上傳監造計畫附件(檔名須為jpg)</h2>
                <div>
                    第一章需上傳工程平面圖及標準斷面圖<br />
                    第四章需上傳主要工項預定進度表<br />
                    第六章需上傳機電設備架構圖
                </div>
                <div class="table-responsive">
                    <table v-if="fCanEdit" class="table table2" border="0">
                        <tbody>
                            <tr>
                                <th>附件圖片上傳</th>
                                <td>
                                    <div>
                                        <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
                                            <div v-if="!file" :class="['dropZone ', dragging ? 'dropZone-over' : '']"
                                                 @dragestart="dragging = true" @dragenter="dragging = true"
                                                 @dragleave="dragging = false">
                                                <div class="dropZone-info align-self-center" @drag="onChartFileChange">
                                                    <span class="dropZone-title" style="margin-top:0px;">
                                                        拖拉檔案至此區塊 或
                                                        點擊此處
                                                    </span>
                                                </div>
                                                <input type="file" @change="onChartFileChange" />
                                            </div>
                                            <div v-if="file" class="form-row justify-content-center">
                                                <div class="dropZone-uploaded">
                                                    <div class="dropZone-uploaded-info">
                                                        <span class="dropZone-title">選取的檔案: {{ file.name }}</span>
                                                        <div class="uploadedFile-info">
                                                            <button @click="removeFile" type="button"
                                                                    class="btn btn-color9-1 btn-xs mx-1">
                                                                <i class="fas fa-times"></i> 取消選取
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div v-if="file" class="row justify-content-center my-3">
                                            <button @click="uploadChart" type="button"
                                                    class="btn btn-color11-2 btn-xs mx-1">
                                                <i class="fas fa-upload"></i> 確認
                                            </button>
                                        </div>
                                    </div>

                                </td>
                            </tr>
                            <tr>
                                <th>歸屬章節</th>
                                <td>
                                    <div class="form-inline">
                                        <select v-model="selectChapter" @change="onChapterChange($event)"
                                                class="form-control my-1 mr-0 mr-sm-1">
                                            <option value="1">第一章</option>
                                            <option value="2">第二章</option>
                                            <option value="3">第三章</option>
                                            <option value="4">第四章</option>
                                            <option value="5">第五章</option>
                                            <option value="6">第六章</option>
                                        </select>
                                        <select v-model="selectFileType" class="form-control my-1 mr-0 mr-sm-1">
                                            <option value="1">圖</option>
                                            <option value="2">表</option>
                                        </select>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>說明</th>
                                <td>
                                    <input v-model="engAttachmentDescription" maxlength="255" type="text"
                                           class="form-control" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="table-responsive">
                        <table class="table table1" border="0">
                            <thead>
                                <tr>
                                    <th class="sort">版次</th>
                                    <th>時間</th>
                                    <th>圖或表名稱</th>
                                    <th>功能</th>
                                    <th>檔案</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in engAttachmentList" v-bind:key="item.Seq">
                                    <td>{{engAttachmentList.length - index}}</td>
                                    <td>{{item.modifyDate}}</td>
                                    <td>
                                        <div v-if="!item.edit">{{item.Description}}</div>
                                        <input v-if="item.edit" v-model="item.Description" maxlength="255" type="text"
                                               class="form-control" />
                                    </td>
                                    <td>
                                        <div v-if="fCanEdit" class="d-flex">
                                            <a href="#" v-if="!item.edit" v-on:click.prevent="item.edit=!item.edit"
                                               class="btn btn-color11-3 btn-xs mx-1" title="編輯">
                                                <i class="fas fa-pencil-alt"></i> 編輯
                                            </a>
                                            <a href="#" v-if="item.edit" v-on:click.prevent="saveAttachment(item)"
                                               class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                            <a href="#" v-on:click.prevent="delAttachment(index, item.Seq)"
                                               class="btn btn-color9-1 btn-xs mx-1" title="刪除">
                                                <i class="fas fa-trash-alt"></i> 刪除
                                            </a>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="d-flex">
                                            <a v-on:click.prevent="download(item)" href="#"
                                               class="btn btn-color11-1 btn-xs mx-1" title="下載">
                                                <i class="fas fa-download"></i> 下載
                                            </a>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row justify-content-center mt-5">
                    <div class="d-flex">
                        <button v-on:click="updateData()" role="button"
                                class="btn btn-color11-2 btn-xs mx-1">
                            <i class="fas fa-save">&nbsp;暫存</i>
                        </button>
                        <button v-on:click="updateData(true)" v-bind:disabled="!fCanEdit" role="button"
                                class="btn btn-color11-2 btn-xs mx-1">
                            <i class="fas fa-save">&nbsp;儲存並確認</i>
                        </button>
                    </div>
                    <div v-if="engMain.PCCESSMainSeq >0 && importShow" class="d-flex">
                        <button v-bind:disabled="!saveFlag"
                                title="匯入標案基本資料" class="btn btn-color11-2 btn-xs mx-1" role="button" data-toggle="modal"
                                data-target="#refImportModal">
                            <i class="fas fa-upload"></i>&nbsp;匯入標案基本資料
                        </button>
                    </div>
                    <div class="d-flex">
                        <button v-on:click.stop="back()" role="button" class="btn btn-color9-1 btn-xs mx-1">
                            回上頁
                        </button>
                    </div>
                </div>
                <label class="mt-5 mb-1 mx-2 small-green">* 點選儲存後，僅儲存與該主檔有關之資料</label>
                <label class="my-1 mx-2 small-red">
                    *
                    點選【匯入標案基本資料】按鈕，系統更新主檔及其相關資料表外，自動將後台預設的材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料儲存到該標案對應的資料表。
                </label>
            </div>
        </form>
        <!-- setp4 參考範例 -->
        <div class="modal fade" id="refSampleModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refSampleModal" aria-modal="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="projectUpload">參考範例</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <table class="table table1 " border="0">
                                <thead>
                                    <tr>
                                        <th>項次</th>
                                        <th>分項工程</th>
                                        <th>數量</th>
                                        <th>單位</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>1</td>
                                        <td>基礎工</td>
                                        <td>○○○○</td>
                                        <td>m</td>
                                    </tr>
                                    <tr>
                                        <td>2</td>
                                        <td>丁壩工</td>
                                        <td>○○○○</td>
                                        <td>座</td>
                                    </tr>
                                    <tr>
                                        <td>3</td>
                                        <td>石籠護坦工</td>
                                        <td>○○○○</td>
                                        <td>m</td>
                                    </tr>
                                    <tr>
                                        <td>4</td>
                                        <td>坡面工</td>
                                        <td>○○○○</td>
                                        <td>m2</td>
                                    </tr>
                                    <tr>
                                        <td>5</td>
                                        <td>擋土牆</td>
                                        <td>○○○○</td>
                                        <td>m</td>
                                    </tr>
                                    <tr>
                                        <td>6</td>
                                        <td>側溝</td>
                                        <td>○○○○</td>
                                        <td>m</td>
                                    </tr>
                                    <tr>
                                        <td>7</td>
                                        <td>水防道路</td>
                                        <td>○○○○</td>
                                        <td>m2</td>
                                    </tr>
                                    <tr>
                                        <td>8</td>
                                        <td>防潮閘門</td>
                                        <td>○○○○</td>
                                        <td>座</td>
                                    </tr>
                                    <tr>
                                        <td>9</td>
                                        <td>操作機房</td>
                                        <td>○○○○</td>
                                        <td>座</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 標案清單 shioulo 20220504 -->
        <div class="modal fade" id="refTenderListModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refTenderListModal" aria-modal="true">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title redText" id="projectUpload">
                            標案清單:&nbsp;***以下為"{{execUnitName}}"在工程會標案管系的案件清單***
                        </h6>
                        <button id="closeTenderListModal" type="button" class="close" data-dismiss="modal"
                                aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h6>如果工程已決標，需要勾稽工程會標案管理系統的標案編號，後續工程履約方能作業；如果清單中找不到工程會標案管理系統的編號，可先自行新增標案編號，後續系統會自動勾稽(但標案編號請務必登打工程會標案管理系統的正確標案編號)</h6>
                        <div class="table-responsive">
                            <table class="table table-responsive-md table-hover">
                                <thead class="insearch">
                                    <tr>
                                        <th>標案編號</th>
                                        <th>工程名稱</th>
                                        <th style="width:40px"><button @click="addPrjXMLEng" class="btn btn-color9-1 btn-xs mx-1" title="新增工程會工程"><i class="fas fa-plus"></i></button></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-if="fAddPrjXMLEng">
                                        <td><input v-model.trim="prjXMLEng.TenderNo" maxlength="30" class="form-control" type="text" /></td>
                                        <td><input v-model.trim="prjXMLEng.TenderName" maxlength="500" class="form-control" type="text" /></td>
                                        <td><button @click="savePrjXMLEng" class="btn btn-color9-1 btn-xs mx-1" title="儲存"><i class="fas fa-save"></i></button></td>
                                    </tr>
                                    <tr v-on:click.stop="onSelectTrender(item)" v-for="(item, index) in tenders"
                                        v-bind:key="item.Seq">
                                        <td>{{item.TenderNo}}</td>
                                        <td colspan="2">
                                            {{item.TenderName}}<font style="color:blue">  {{item.PrjXMLSeq==null ? '' : '(已勾稽)' }} </font>
                                            <font style="color:lightblue">  {{item.code==1 ? '' : '(暫存)' }} </font>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 匯入標案基本資料模式 shioulo 20230830 -->
        <div class="modal fade" id="refImportModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refImportModal" aria-modal="true">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title redText" id="projectUpload">
                            材料設備轉入階層設定
                        </h6>
                        <!-- button id="closeTenderListModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button -->
                    </div>
                    <div class="modal-body">
                        <div class="form-row">
                            <div class="col form-inline my-2">
                                <label class="my-2 mx-2">材料設備轉入階層:</label>
                                <div class="custom-control custom-radio mx-2">
                                    <input v-model="engImportMode" value="0" type="radio" id="engImportModeYes"
                                           class="custom-control-input" name="engImportMode">
                                    <label class="custom-control-label" for="engImportModeYes">壹一、二(預設)</label>
                                </div>
                                <div class="custom-control custom-radio mx-2">
                                    <input v-model="engImportMode" value="1" type="radio" id="engImportModeNo"
                                           class="custom-control-input" name="engImportMode">
                                    <label class="custom-control-label" for="engImportModeNo">全部工項</label>
                                </div>
                            </div>
                        </div>
                        <br />
                        <h6>系統將更新主檔及其相關資料表外<br />如材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料<br />是否確定?</h6>
                    </div>
                    <div class="modal-footer">
                        <button v-on:click.stop="createSupervisionProject()" type="button" class="btn btn-primary">
                            確定
                        </button>
                        <button type="button" id="closeImportModal" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
                            取消
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {
    watch:{
        "engMain.chsAwardDate" :{
            handler(value, oldValue)
            {
                if(this.userRole == 1 )
                {
                    if(  !(value == null || value.length ==0 ))
                    {
                        this.AwardYearChangeStep = 0;
                    }
                    else{
                        this.AwardYearChangeStep = 1;
                    }
                }
                else
                {
                    this.AwardYearChangeStep = 1;
                }
                console.log("AwardYearChangeStep", this.AwardYearChangeStep)
                if(value !=null  && value != "" )
                {
                  
                    this.engMain.EngYear = value.split("/")[0]
                }
                
            }
        }
    },
    data: function () {
        return {
            AwardYearChangeStep : 0 ,
            Role : null,
            fSendMail: false,//防止連續發送
            fSendMailSupervisorUnit: false,
            fSendMailBuildContractor: false,
            fCanEdit: false,
            saveFlag: false,
            updateXMLMode: false,//xml 工程編號存在是否覆蓋
            targetId: null,
            isAdd: false,
            file: null,//{ name: null, size: null },
            files: new FormData(),
            dragging: false,
            step: -1,

            //使用者單位資訊
            isAdmin: false,
            userUnit: null,
            userUnitName: '',
            userUnitSub: null,
            userUnitSubName: '',

            users: [],//人員清單
            units: [],//機關清單
            organizerSubUnits: [],//主辦機關單位清單
            execUnitName: '',
            execSubUnits: [],//執行機關單位清單
            cities: [],//行政區(縣市)清單
            towns: [],//行政區(鄉鎮)清單
            engMain: { OrganizerUnitSeq: -1, ExecUnitSeq: -1, citySeq: -1, EngTownSeq: -1, OrganizerSubUnitSeq: -1 },

            engConstruction: { Seq: -1, EngMainSeq: -1, ItemName: '', ItemQty: 0, ItemUnit: '', edit: false },//工程主要施工項目及數量
            engConstructionList: [],//工程主要施工項目及數量 清單

            engAttachmentList: [], //上傳監造計畫附件 清單
            selectChapter: 1, //上傳監造計畫附件 章
            selectFileType: 1,
            engAttachmentDescription: '',
            //for datepicket
            //chsStartDate: '',
            //chsSchCompDate: '',
            //chsPostCompDate: '',
            //chsApproveDate: '',
            chsAwardDate: '', //決標日期 shioulo 20220618
            hideHeader: true,
            tenders: [], //標案清單 shioulo 20220504
            //自辦監造人員
            supervisorKind: 1,
            supervisorSubUnitSeq: -1,
            supervisorUserSeq: -1,
            supervisorUsers: [],
            engSupervisor: [],
            //s20230327 可自行新增工程會工程
            fAddPrjXMLEng: false,
            prjXMLEng: { Seq: -1, TenderNo: '', TenderYear: -1, TenderName: '', ExecUnitName: '' },
            engImportMode: "0", //s20230830
            userRole:-1, //s20231006
            importShow : false
        };
    },

        methods: {

        //s20230327 可自行新增工程會工程
        addPrjXMLEng() {
            var execUnitName = ''
            if (this.$refs.refExecUnit.options.selectedIndex >= 0) {
                execUnitName = this.units[this.$refs.refExecUnit.options.selectedIndex].Text;
            }
            if (!window.comm.stringEmpty(this.execUnitName)) {

                this.prjXMLEng = { Seq: -1, TenderNo: '', TenderYear: -1, TenderName: '', ExecUnitName: '' };
                this.prjXMLEng.ExecUnitName = execUnitName;
                this.prjXMLEng.TenderYear = this.engMain.EngYear ;
                this.prjXMLEng.TenderNo = this.engMain.EngNo;
                this.prjXMLEng.TenderName = this.engMain.EngName;
                this.fAddPrjXMLEng = true;
            }
            
        },
        async checkEngItemPayItemAdded()
        {   
            let { data : ItemNameList} = await window.myAjax.post("TenderPlanV2/GetEngItemPayItemAdded", {id : this.engMain.Seq})
            this.importShow =  ItemNameList.length == 0 
        },
        savePrjXMLEng() {
            window.myAjax.post('/TenderPlan/AddPrjXMLEng', { eng: this.prjXMLEng })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.onTenderSearch();
                        this.fAddPrjXMLEng = false;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //自辦監造人員
        async getSupervisorUsers() {
            this.supervisorUsers = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.ExecUnitSeq, organizerSubUnitSeq: this.supervisorSubUnitSeq });
            this.supervisorUsers = data;
        },
        addSupervisorUser() {
            window.myAjax.post('/TenderPlan/SupervisorUserAdd', { eng: this.targetId, kind: this.supervisorKind, subUnit: this.supervisorSubUnitSeq, id: this.supervisorUserSeq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        delEngSupervisor(item) {
            window.myAjax.post('/TenderPlan/SupervisorUserDel', { eng: this.targetId, id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items;
                    } else {
                        alert(resp.data.msg);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        getEngSupervisor() {
            this.engSupervisor = [];
            window.myAjax.post('/TenderPlan/GetSupervisorUser', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engSupervisor = resp.data.items
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        kindCaption(kind) {
            if (kind == 0)
                return "監造主任";
            else if (kind == 2)
                return "標案建立者";
            else if (kind == 3)
                return "設計人員";
            else 
                return "現場人員";
        },
        //
        onExecTypeChange() {
            //console.log('onExecTypeChange()');
            if (this.userUnitName != '水利署') return;

            if (this.engMain.DesignUnitName == '') {
                this.engMain.DesignUnitName = this.userUnitSubName;
            }
            if (this.engMain.SupervisorUnitName == '') {
                this.engMain.SupervisorUnitName = this.userUnitSubName;
            }
        },
        getItem() {
            this.fCanEdit = true;
            this.step = 2;
            this.engMain = {};
            this.userRole = -1;
            window.myAjax.post('/TenderPlan/GetEngItem', { id: this.targetId })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain = resp.data.item;
                        this.userRole = resp.data.ur;
                        this.getCityTown();
                        this.getEngConstructionList();
                        this.getEngAttachmentList();
                        this.getOrganizerSubUnit(this.engMain.OrganizerUnitSeq);
                        this.getUsers();
                        this.getExecSubUnit(this.engMain.ExecUnitSeq);
                        this.getEngSupervisor();
                        this.checkEngItemPayItemAdded();
                        // this.engMain.EngYear  = this.engMain.chsAwardDate.split('/')[0]; 
                        if (this.engMain.DocState == null || this.engMain.DocState == -1) {
                            this.fCanEdit = true;
                            //this.chsStartDate = this.toYearDate(this.engMain.chsStartDate);
                            //this.chsSchCompDate = this.toYearDate(this.engMain.chsSchCompDate);
                            //this.chsPostCompDate = this.toYearDate(this.engMain.chsPostCompDate);
                            //this.chsApproveDate = this.toYearDate(this.engMain.chsApproveDate);//shioulo 20220
                            this.chsAwardDate = this.toYearDate(this.engMain.chsAwardDate);
                        }

                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        onChangeSupervisorUnit() {
            this.fSendMailSupervisorUnit = false;
        },
        onChangeBuildContractor() {
            this.fSendMailBuildContractor = false;
        },
        //xml 檔案上傳處裡
        onChange(e) {
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
                // 附檔名判斷
                //console.log(file);
                if (!file.type.match('text/xml')) {
                    alert('請選擇 xml 檔案');
                    this.dragging = false;
                    return;
                }
                this.file = file;
                this.dragging = false;

                this.files.append("file", this.file, this.file.name);
            },

            onUpdatePccesChange(e) {//s20231008 更新 pcces
                var files = e.target.files || e.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                var tarFile = files[0];
                if (!tarFile.type.match('text/xml')) {
                    alert('請選擇 xml 檔案');
                    return;
                }

                this.files = new FormData();
                this.files.append("file", tarFile, tarFile.name);
                this.uploadXML(1);
            },
            uploadXML(processMode) {
                this.files.set("processMode", processMode);
                const files = this.files;
                window.myAjax.post('/TenderPlan/UploadXML', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            alert(resp.data.message);
                            this.targetId = resp.data.item;
                            this.editEng(this.targetId);
                            window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, this.targetId);
                            //this.getItem();
                            //this.removeFile();
                        } else {
                            this.updateXMLMode = (resp.data.result == -2);
                            alert(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
        editEng(engSeq) {
            window.myAjax.post('/TenderPlan/EditEng', { seq: engSeq })
                .then(resp => {
                    console.log(resp.data.Url);
                    window.location.href = resp.data.Url;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        removeFile() {
            this.file = '';
            this.files = new FormData();
            this.updateXMLMode = false;
        },
        //取得使用者單位資訊
        getUserUnit() {
            window.myAjax.post('/TenderPlan/GetUserUnit')
                .then(resp => {
                    this.userUnit = resp.data.unit;
                    this.userUnitName = resp.data.unitName;
                    this.userUnitSub = resp.data.unitSub;
                    this.userUnitSubName = resp.data.unitSubName;

                    this.selectUnit = this.userUnit;
                    //this.onUnitChange(this.selectUnit)
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //人員清單
        async getUsers(event) {
            this.users = [];
            //const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.OrganizerUnitSeq, organizerSubUnitSeq: this.engMain.OrganizerSubUnitSeq });
            //20220707 改為執行機關聯絡人
            const { data } = await window.myAjax.post('/TenderPlan/GetUserList', { organizerUnitSeq: this.engMain.ExecUnitSeq, organizerSubUnitSeq: this.engMain.ExecSubUnitSeq });
            this.users = data;
        },
        //機關清單
        async getUnits() {
            this.units = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: -1 });
            this.units = data;
        },
        //主辦機關-單位清單
        async getOrganizerSubUnit(unitSeq) {
            this.organizerSubUnits = [];
            /*this.users = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: unitSeq });
            this.organizerSubUnits = data;*/
        },
        //執行機關-單位清單
        async getExecSubUnit(unitSeq) {
            this.execSubUnits = [];
            this.users = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetUnitList', { parentUnit: unitSeq });
            this.execSubUnits = data;
        },
        //執行機關-名稱
        getExecUnitName() {
            console.log(this.$refs.refExecUnit);
        },
        //行政區(縣市)
        async getCities() {
            this.cities = [];
            const { data } = await window.myAjax.post('/TenderPlan/GetCityList');
            this.cities = data;
        },
        onCityChange(event) {
            this.getCityTown();
        },
        //行政區(鄉鎮)
        async getCityTown() {
            if (this.engMain.CitySeq > 0) {
                this.twons = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetTownList', { id: this.engMain.CitySeq });
                this.towns = data;
            }
        },
        //工程總預算變更
        onTotalBudgetChange() {
            if (this.engMain.TotalBudget > 50000000) {
                this.engMain.PurchaseAmount = 1;
            } else if (this.engMain.TotalBudget > 10000000) {
                this.engMain.PurchaseAmount = 2;
            } else if (this.engMain.TotalBudget > 1000000) {
                this.engMain.PurchaseAmount = 3;
            } else {
                this.engMain.PurchaseAmount = 0;
            }
        },
        onDateChange(srcDate, event, mode) {


            if (event.target.value.length == 0) {
                /*if (mode == 'SchCompDate') this.chsSchCompDate = '';
                else if (mode == 'PostCompDate') this.chsPostCompDate = '';
                else if (mode == 'StartDate') this.chsStartDate = '';
                else if (mode == 'ApproveDate') this.chsApproveDate = '';
                else */if (mode == 'AwardDate') this.chsAwardDate = '';
                return;
            }
            if (!this.isExistDate(event.target.value)) {
                event.target.value = srcDate;
                alert("日期錯誤");
            } else {
                /*if (mode == 'SchCompDate') this.chsSchCompDate = this.toYearDate(event.target.value);
                else if (mode == 'PostCompDate') this.chsPostCompDate = this.toYearDate(event.target.value);
                else if (mode == 'StartDate') this.chsStartDate = this.toYearDate(event.target.value);
                else if (mode == 'ApproveDate') this.chsApproveDate = this.toYearDate(event.target.value);
                else */if (mode == 'AwardDate') this.chsAwardDate = this.toYearDate(event.target.value);
            }
        },
        onDatePicketChange(ctx, mode) {
            //console.log(ctx);


            if (ctx.selectedDate != null) {
                var d = ctx.selectedDate;
                var dd = (d.getFullYear() - 1911) + '/' + (d.getMonth() + 1) + '/' + d.getDate();
                //var y = d.getYear() - 1911;
                /*if (mode == 'SchCompDate') this.engMain.chsSchCompDate = dd;
                else if (mode == 'PostCompDate') this.engMain.chsPostCompDate = dd;
                //else if (mode == 'StartDate') this.engMain.chsStartDate = dd;
                //else if (mode == 'ApproveDate') this.engMain.chsApproveDate = dd;
                //else */if (mode == 'AwardDate') this.engMain.chsAwardDate = dd;
            }
        },
        //中曆轉西元
        toYearDate(dateStr) {
            if (dateStr == null) return null;
            var dateObj = dateStr.split('/'); // yyy/mm/dd
            return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
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
        async getEngConstructionList() {
            const { data } = await window.myAjax.post('/TenderPlan/GetECList', { engMain: this.engMain.Seq });
            this.engConstructionList = data;
        },
        //新增 工程主要施工項目及數量
        addConstruction() {
            this.engConstruction.EngMainSeq = this.engMain.Seq;
            window.myAjax.post('/TenderPlan/ConstructionAdd', { item: this.engConstruction })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engConstruction.Seq = resp.data.Seq;
                        //alert(resp.data.message);
                        this.engConstructionList.push(this.engConstruction);
                        //
                        this.engConstruction = { Seq: -1, EngMainSeq: -1, ItemName: '', ItemQty: 0, ItemUnit: '', edit: false };
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //更新 工程主要施工項目及數量
        saveConstruction(item) {
            window.myAjax.post('/TenderPlan/ConstructionUpdate', { item: item })
                .then(resp => {
                    if (resp.data.result == 0) {
                        item.edit = false;
                    } else
                        alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //刪除
        delConstruction(index, id) {
            window.myAjax.post('/TenderPlan/ConstructionDel', { seq: id })
                .then(resp => {
                    this.engConstructionList.splice(index, 1);
                    alert(resp.data.message);
                    console.log(resp);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //監造計畫附件 
        onChapterChange(event) {
            this.getEngAttachmentList();
        },
        //監造計畫附件 清單
        async getEngAttachmentList() {
            const { data } = await window.myAjax.post('/TenderPlan/EngAttachmentList', { engMain: this.engMain.Seq, chapter: this.selectChapter });
            this.engAttachmentList = data;
            if (this.engAttachmentList.length > 0) {
                this.selectFileType = this.engAttachmentList[0].FileType;
            }
        },
        //xml 檔案上傳處裡
        onChartFileChange(e) {
            // 判斷拖拉上傳或點擊上傳的 event
            var files = e.target.files || e.dataTransfer.files;
            // 預防檔案為空檔
            if (!files.length) {
                this.dragging = false;
                return;
            }

            this.createChartFile(files[0]);
        },
        createChartFile(file) {
            console.log(file);
            if(file.name.split('.')[1] != 'jpg') {
                file = null;
                alert("請上傳JPG格式");
            }
            /*if (!file.type.match('text/xml')) {// 附檔名判斷
                alert('請選擇 xml 檔案');
                this.dragging = false;
                return;
            }*/
            this.file = file;
            this.dragging = false;

            this.files.append("file", this.file, this.file.name);
        },
        //上傳圖表
        uploadChart() {
            this.files.append("engMain", this.engMain.Seq);
            this.files.append("chapter", this.selectChapter);
            this.files.append("fileType", this.selectFileType);
            this.files.append("description", this.engAttachmentDescription);
            const files = this.files;
            window.myAjax.post('/TenderPlan/EngAttachmentUpload', files,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                })
                .then(resp => {
                    if (resp.data.result == 0) {
                        alert(resp.data.message);
                        this.targetId = resp.data.item;
                        this.getEngAttachmentList();
                        this.file = '';
                        this.files = new FormData();
                    } else {
                        alert(resp.data.message);
                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        //更新 上傳監造計畫附件
        saveAttachment(item) {
            window.myAjax.post('/TenderPlan/EngAttachmentUpdate', { item: item })
                .then(resp => {
                    if (resp.data.result == 0) {
                        item.edit = false;
                    }
                    alert(resp.data.message);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //刪除 上傳監造計畫附件
        delAttachment(index, id) {
            window.myAjax.post('/TenderPlan/EngAttachmentDel', { seq: id })
                .then(resp => {
                    this.engAttachmentList.splice(index, 1);
                    alert(resp.data.message);
                    console.log(resp);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        download(item) {
            window.myAjax.get('/TenderPlan/EngAttachmentDownload?seq=' + item.Seq, { responseType: 'blob' })
                .then(resp => {
                    const blob = new Blob([resp.data]);
                    const contentType = resp.headers['content-type'];
                    if (contentType.indexOf('application/json') >= 0) {
                        //alert(resp.data);
                        const reader = new FileReader();
                        reader.addEventListener('loadend', (e) => {
                            const text = e.srcElement.result;
                            const data = JSON.parse(text)
                            alert(data.message);
                        });
                        reader.readAsText(blob);
                    } else if (contentType.indexOf('application/blob') >= 0) {
                        var saveFilename = null;
                        const data = decodeURI(resp.headers['content-disposition']);
                        var array = data.split("filename*=UTF-8''");
                        if (array.length == 2) {
                            saveFilename = array[1];
                        } else {
                            array = data.split("filename=");
                            saveFilename = array[1];
                        }
                        if (saveFilename != null) {
                            const url = window.URL.createObjectURL(blob);
                            const link = document.createElement('a');
                            link.href = url;
                            link.setAttribute('download', saveFilename);
                            document.body.appendChild(link);
                            link.click();
                        } else {
                            console.log('saveFilename is null');
                        }
                    } else {
                        alert('格式錯誤下載失敗');
                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        //
        updateData(check) {
            ///*this.engMain.OrganizerSubUnitSeq == null ||
            // this.engMain.ExecUnitSeq = null;
            if(check)
            {
                if (this.engMain.EngName == null || this.engMain.EngName.length == 0
                || this.engMain.CitySeq == null || this.engMain.EngTownSeq == null
                || this.engMain.EngNo == null || this.engMain.EngNo.length == 0
                || this.engMain.OrganizerUnitSeq == null || this.engMain.OrganizerUserSeq == null
                || this.engMain.ExecUnitSeq == null || this.engMain.ExecSubUnitSeq == null) {
                alert('* 欄位必須選填');
                return;
                }
                if (this.engMain.BuildContractorTaxId && this.engMain.BuildContractorTaxId.length > 0 && !this.checkCompanyNo(this.engMain.BuildContractorTaxId)) {
                    alert('施工廠商統編錯誤');
                    return;
                }
                if (this.engMain.SupervisorTaxid && this.engMain.SupervisorTaxid.length > 0 && !this.checkCompanyNo(this.engMain.SupervisorTaxid)) {
                    alert('監造單位統編錯誤');
                    return;
                }
            }

            //this.engMain.chsStartDate = this.$refs['chsStartDate'].value;
            //this.engMain.chsSchCompDate = this.$refs['chsSchCompDate'].value;
            //this.engMain.chsPostCompDate = this.$refs['chsPostCompDate'].value;
            //this.engMain.chsApproveDate = this.$refs['chsApproveDate'].value;
            this.engMain.chsAwardDate = this.$refs['chsAwardDate'].value;
            //shioulo 20220618
            /*if (this.engMain.chsAwardDate != null && this.engMain.chsAwardDate.length > 0 && this.engMain.PrjXMLSeq == null) {
                if (!confirm("決標日期已填寫, 但工程會標案編號 未設定")) return;
            }*/

            window.myAjax.post('/TenderPlan/UpdateTenderPlan', { engMain: this.engMain })
                .then(resp => {
                    this.saveFlag = false;
                    if (resp.data.result == 0) {
                        this.saveFlag = true;
                        //this.engMain = resp.data.item;
                        this.fSendMailSupervisorUnit = true;
                        this.fSendMailBuildContractor = true;
                        alert(resp.data.message);
                    } else {
                        alert(resp.data.message);
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        },
        createSupervisionProject() {
            if (!this.saveFlag) return;

            //if (confirm('系統將更新主檔及其相關資料表外，如材料設備清單、施工管理清單、設備運轉清單、職業安全、環境保育等資料\n\n是否確定?')) {
                window.myAjax.post('/TenderPlan/CreateSupervisionProject', { engMain: this.engMain.Seq, im: this.engImportMode })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain.DocState = 0;
                            document.getElementById('closeImportModal').click();
                        }
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            //}
        },
        //檢查公司統一編號是否正確
        checkCompanyNo(idvalue) {
            var tmp = new String("12121241");
            var sum = 0;
            var re = /^\d{8}$/;
            if (!re.test(idvalue)) {
                //alert("格式不對！");
                return false;
            }
            var i = 0;
            for (i = 0; i < 8; i++) {
                var s1 = parseInt(idvalue.substr(i, 1));
                var s2 = parseInt(tmp.substr(i, 1));
                sum += this.cal(s1 * s2);
            }
            if (!this.valid(sum)) {
                if (idvalue.substr(6, 1) == "7") return (this.valid(sum + 1));
            }
            return (this.valid(sum));
        },
        valid(n) {
            return (n % 10 == 0) ? true : false;
        },
        cal(n) {
            var sum = 0;
            while (n != 0) {
                sum += (n % 10);
                n = (n - n % 10) / 10;  // 取整數
            }
            return sum;
        },
        //施工廠商發送mail
        onSendMailBuildContractor() {
            if (!this.checkCompanyNo(this.engMain.BuildContractorTaxId)) {
                alert('施工廠商統編錯誤');
                return;
            }
            if (!this.validateEmail(this.engMain.BuildContractorEmail)) {
                alert('聯絡人mail錯誤');
                return;
            }
            if (this.engMain.BuildContractorName && this.engMain.BuildContractorName.length > 0
                && this.engMain.BuildContractorContact && this.engMain.BuildContractorContact.length > 0) {
                this.fSendMail = true;
                window.myAjax.post('/TenderPlan/SendMailToBuildContractor', { engMain: this.engMain.Seq })
                    .then(resp => {
                        alert(resp.data.message);
                        this.fSendMail = false;
                    })
                    .catch(err => {
                        console.log(err);
                        this.fSendMail = false;
                    });

            } else {
                alert('施工廠商名稱,施工廠商連絡人 資料錯誤');
            }

        },
        //監造單位發送mail
        onSendMailSupervisorUnit() {
            console.log('onSendMailSupervisorUnit()');
            if (!this.checkCompanyNo(this.engMain.SupervisorTaxid)) {
                alert('監造單位統編錯誤');
                return;
            }
            if (!this.validateEmail(this.engMain.SupervisorContact)) {
                alert('聯絡人mail錯誤');
                return;
            }
            if (this.engMain.SupervisorUnitName && this.engMain.SupervisorUnitName.length > 0
                && this.engMain.SupervisorDirector && this.engMain.SupervisorDirector.length > 0) {
                this.fSendMail = true;
                window.myAjax.post('/TenderPlan/SendMailToSupervisor', { engMain: this.engMain.Seq })
                    .then(resp => {
                        alert(resp.data.message);
                        this.fSendMail = false;
                    })
                    .catch(err => {
                        console.log(err);
                        this.fSendMail = false;
                    });

            } else {
                alert('監造單位,監造主任 資料錯誤');
            }
        },
        validateEmail(email) {
            const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(email).toLowerCase());
        },
        back() {
            //window.history.go(-1);
            window.location = "/TenderPlan/TenderPlanList";
        },
        //標案查詢 shioulo 20220504
        onTenderSearch() {
            this.tenders = [];
            if (this.engMain.ExecUnitSeq > 0) {
                if (this.$refs.refExecUnit.options.selectedIndex >= 0)
                    this.execUnitName = this.units[this.$refs.refExecUnit.options.selectedIndex].Text;
                else
                    this.execUnitName = '';

                window.myAjax.post('/TenderPlan/GetTenderList', { id: this.engMain.Seq })
                    .then(resp => {
                        this.tenders = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            } else {
                alert('沒有執行單位');
            }
        },
        //工程連結標案 shioulo 20220518
        onSelectTrender(item) {
            window.myAjax.post('/TenderPlan/SetEngLinkTender', { id: this.engMain.Seq, prj: item.Seq , code: item.code})
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain.PrjXMLSeq = item.code == 1 ?  item.Seq : -item.Seq;
                        this.engMain.TenderNo = item.TenderNo;
                        this.engMain.TenderName = item.TenderName;
                        document.getElementById("closeTenderListModal").click();
                    }
                    alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        //工程連結標案取消 shioulo 20220519
        onCancelTenderLink() {
            window.myAjax.post('/TenderPlan/CancelEngLinkTender', { id: this.engMain.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.engMain.PrjXMLSeq = null;
                        this.engMain.TenderNo = "";
                    }
                    alert(resp.data.msg);
                })
                .catch(err => {
                    console.log(err);
                });
        },
        async getUserRole()
        {
            this.Role = (await window.myAjax.post("Users/GetUserInfo")).userInfo.RoleSeq;
        }
    },
    computed: {
        // 前端擷取附檔名
        extension() {
            return (this.file) ? this.file.name.split('.').pop() : '';
        },
        getSupervisorUnitState() {
            return this.fSendMailSupervisorUnit && !this.fSendMail;
        },
        getBuildContractorState() {
            return this.fSendMailBuildContractor && !this.fSendMail;
        },
        //s20231006 疏濬工程編輯
        disabledDredgingEng() {
            if (this.userRole == 1 || this.userRole == 2)
                return false;
            else
                return true;
        }
    },
    async mounted() {
        console.log('mounted() 標案編輯 ' + window.location.href);
        let urlParams = new URLSearchParams(window.location.search);
        this.getUserRole();
        if (urlParams.has('id')) {
            if (this.units.length == 0) this.getUnits();
            if (this.cities.length == 0) this.getCities();
            //if (this.users.length == 0) this.getUsers();
            this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
            this.targetId = parseInt(urlParams.get('id'), 10);
            // console.log(this.targetId);
            if (Number.isInteger(this.targetId)) {
                if (this.targetId <= 0) {
                    this.isAdd = true;
                    this.step = 1;
                } else {
                    this.getUserUnit();
                    this.getItem();
                }
                return;
            }

        }
        window.history.back(); //.location = "/FrontDesk";
    }
}
</script>