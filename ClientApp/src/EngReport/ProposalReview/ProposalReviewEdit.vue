<template>
    <div>
        <div >
            <h5>提案審查表&nbsp;&nbsp;&nbsp;<a href="/Content/images/工程提案命名原則.jpg" title="命名規則" target="_blank" style="font-size: smaller;">命名規則</a></h5>
            <div class="setFormcontentCenter">
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程名稱<span class="small-red">&nbsp;*</span></label>
                        <input v-model="engReport.RptName" type="text" placeholder="依命名原則" class="col-12 col-md-10 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">類別<span class="small-red">&nbsp;*</span></label>
                        <select v-model="engReport.ProposalReviewTypeSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput" @change="onReviewTypeChange($event)">
                            <option v-for="option in selectProposalReviewTypeOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                    <div class="col-6 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">屬性<span class="small-red">&nbsp;*</span></label>
                        <select v-model="engReport.ProposalReviewAttributesSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in selectProposalReviewAttributesOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div v-if="editRiver==1" class="form-row" style="display: flex;">
                    <div class="col-12  form-inline my-2">
                        <label class="my-2 mx-2">河川</label>
                        <select v-model="engReport.RiverSeq1" @change="onRiverAChange($event)" class="form-control my-1 mr-0 mr-md-4" style="min-width: 200px;">
                            <option v-for="option in selectRiverAOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                        <select v-model="engReport.RiverSeq2" @change="onRiverBChange($event)" class="form-control my-1 mr-0 mr-md-4" style="min-width: 200px;">
                            <option v-for="option in selectRiverBOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                        <select v-model="engReport.RiverSeq3" class="form-control my-1 mr-0 mr-md-4 WidthasInput" style="min-width: 200px;">
                            <option v-for="option in selectRiverCOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div v-if="editDrain==1" class="form-row">
                    <div class="col-4 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">排水</label>
                        <input v-model="engReport.DrainName" type="text" id="fruit" name="fruit" list="fruits" style="min-width: 245px;">
                        <datalist id="fruits">
                            <option v-for="option in selectDrainOptions" v-bind:value="option.Text" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </datalist>
                    </div>
                </div>
                <div v-if="editCoastal==1" class="form-row">
                    <div class="col-4 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">海岸</label>
                        <input v-model="engReport.Coastal" type="text" placeholder="" class="col-4 col-md-8 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-4 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">大斷面樁號<span class="small-red">&nbsp;*</span></label>
                        <input v-model="engReport.LargeSectionChainage" type="text" placeholder="斷面OO-OO" class="col-4 col-md-8 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">所在行政區域<span class="small-red">&nbsp;*</span></label>
                        <div>
                            <select v-model="engReport.CitySeq" @change="onCityChange($event)" class="form-control my-1 mr-0 mr-sm-1">
                                <option v-for="option in cities" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                            <select v-model="engReport.TownSeq" class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4">
                                <option v-for="option in towns" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-9 form-inline my-2 ">
                        <label class="my-2 mx-2">經度<span class="small-red">*</span></label>
                        <div class="col-4 form-inline">
                            <input v-model="engReport.CoordX" class="col-12 form-control">
                        </div>
                        <label class="my-2 mx-2">緯度<span class="small-red">*</span></label>
                        <div class="col-4 form-inline">
                            <input v-model="engReport.CoordY" class="col-12 form-control">
                        </div>
                    </div>
                    <a href="https://www.google.com.tw/maps/@23.546162,120.6402133,8z?hl=zh-TW" target="_blank" title="google map" data-target="#refSampleModal" class="a-blue underl" role="button">google map</a>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程規模</label>
                        <!-- <input type="text"  class="col-12 col-md-10 form-control"> -->
                        <textarea v-model="engReport.EngineeringScale" class="col-12 form-control min1110"
                                  style="min-height: 100px;"
                                  placeholder="面積OO、長度OO、寬度OO"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">辦理緣由<span class="small-red">&nbsp;*</span></label>
                        <!-- <input type="text" placeholder="請敘明主要待解決問題、需求及預期達成目標" class="col-12 col-md-10 form-control"> -->
                        <textarea v-model="engReport.ProcessReason" class="col-12 form-control min1110"
                                  style="min-height: 100px;" placeholder="請敘明主要待解決問題、需求及預期達成目標"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程規模說明</label>
                        <!-- <input type="text"  class="col-12 col-md-10 form-control"> -->
                        <textarea v-model="engReport.EngineeringScaleMemo" class="col-12 form-control min1110"
                                  style="min-height: 100px;"
                                  placeholder="請敘明工程規模選定原則，在整體考量原則下，後續是否有分期執行需求"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">相關報告內容概述<span class="small-red">&nbsp;*</span></label>
                        <textarea v-model="engReport.RelatedReportContent" class="col-12 form-control min1110"
                                  placeholder="簡述工程範圍內相關報告內容摘要(摘錄河川、區域排水治理計畫、風險評估、水利建造物檢查、整建計畫、環境營造規劃及流域整體改善與調適規劃成果，若報告內容無涉及本工程範圍部分則免)"
                                  style="min-height: 100px;"></textarea>
                        <!-- <input type="text" > -->
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between" v-if="engReport.ProposalReviewAttributesSeq== 1">
                        <label class="my-2 mx-2">預估用地取得經費<span class="small-red">&nbsp;*</span></label>
                        <input v-model="engReport.EstimatedLandAcquisitionCosts" type="text" class="col-12 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">治理規劃布設情形<span class="small-red">&nbsp;*</span></label>
                        <textarea v-model="engReport.ManagementPlanningLayoutSituation" class="col-12 form-control min1110"
                                  placeholder=""
                                  style="min-height: 100px;"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">歷史災害描述<span class="small-red">&nbsp;*</span></label>
                        <div class="custom-control custom-radio custom-control-inline" style="display: flex;" onclick="qq('searchResult156',1)">
                            <input v-model="engReport.HistoricalCatastrophe" v-bind:value="1" v-on:click.stop="fEdit=false" type="radio" name="YNB" id="YNB01" class="custom-control-input">
                            <label for="YNB01" class="custom-control-label">有</label>
                        </div>
                        <div class="custom-control custom-radio custom-control-inline">
                            <input v-model="engReport.HistoricalCatastrophe" v-bind:value="2" v-on:click.stop="fEdit=true" type="radio" checked name="YNB" id="YNB02" class="custom-control-input" onclick="qq('searchResult156',2)">
                            <label for="YNB02" class="custom-control-label">無</label>
                        </div>
                        <textarea :disabled="engReport.HistoricalCatastrophe == 2" v-model="engReport.HistoricalCatastropheMemo" class="col-12 form-control min1110" style="min-height: 100px;" placeholder="" id="searchResult156">○○事件，淹水面積○○公頃、深度○○公尺、影響○○戶構造物損壞(基礎掏刷、堤身產生裂縫或孔洞、破堤等)</textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2 form-inline">
                            保護標的(預期工程效益)<span class="small-red">&nbsp;*</span>
                        </label>
                        <textarea v-model="engReport.ProtectionTarget" class="col-12 form-control" style="min-width: 750px;min-height: 100px;" placeholder="○○縣 ○○鄉鎮市 ○○村 ○○千人，○○公頃"></textarea>
                    </div>
                </div>
                <!--<div v-if="engReport.ProposalReviewTypeSeq != 3" class="form-row">-->
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2 form-inline">
                            設計條件<span class="small-red">&nbsp;*</span>
                        </label>
                        <!-- <input class="form-control my-1  WidthasInput"> -->
                        <textarea v-model="engReport.SetConditions" class="col-12 form-control min1110"
                                  placeholder=""
                                  style="min-height: 100px;"></textarea>
                    </div>
                </div>
                <div class=" form-row" style="width: 1000px;">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">生態保育原則<span class="small-red">&nbsp;*</span></label>
                        <div class="col-12 table-responsive">
                            <table class="table table-responsive-md table-hover">
                                <tbody>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>公共工程生態檢核自評表</strong></td>
                                        <td style="width: 400px;">{{engReport.D01FileName}}</td>
                                        <td style="display: flex; width: 140px;">
                                            <label class="btn btn-shadow btn-color11-3" >
                                                <input  v-on:change="fileChange($event,'D1')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D01FileName!='' " v-on:click.stop="download(engReport.Seq,'D1')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D01FileName!='' " v-on:click.stop="delAttachment(engReport.Seq,'D1')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>P-01 提案階段工程生態背景資料表</strong></td>
                                        <td>{{engReport.D02FileName}}</td>
                                        <td style="display: flex;">
                                            <label class="btn btn-shadow btn-color11-3" >
                                                <input v-on:change="fileChange($event,'D2')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D02FileName!=''" v-on:click.stop="download(engReport.Seq,'D2')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D02FileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'D2')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>P-02 提案階段現場勘查紀錄表</strong></td>
                                        <td>{{engReport.D03FileName}}</td>
                                        <td style="display: flex;">
                                            <label class="btn btn-shadow btn-color11-3 ">
                                                <input v-on:change="fileChange($event,'D3')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D03FileName!=''" v-on:click.stop="download(engReport.Seq,'D3')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D03FileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'D3')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>P-03 提案階段民眾參與紀錄表</strong></td>
                                        <td>{{engReport.D04FileName}}</td>
                                        <td style="display: flex;">
                                            <label class="btn btn-shadow btn-color11-3" >
                                                <input v-on:change="fileChange($event,'D4')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D04FileName!=''" v-on:click.stop="download(engReport.Seq,'D4')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D04FileName!='' " v-on:click.stop="delAttachment(engReport.Seq,'D4')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>P-04 提案階段生態保育原則研擬紀錄表</strong></td>
                                        <td>{{engReport.D05FileName}}</td>
                                        <td style="display: flex;">
                                            <label class="btn btn-shadow btn-color11-3" >
                                                <input v-on:change="fileChange($event,'D5')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D05FileName!=''" v-on:click.stop="download(engReport.Seq,'D5')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D05FileName!='' " v-on:click.stop="delAttachment(engReport.Seq,'D5')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>P-05 提案工程生態檢核作業事項確認表</strong></td>
                                        <td>{{engReport.D06FileName}}</td>
                                        <td style="display: flex;">
                                            <label class="btn btn-shadow btn-color11-3">
                                                <input v-on:change="fileChange($event,'D6')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                            </label>
                                            <button v-if="engReport.D06FileName!=''" v-on:click.stop="download(engReport.Seq,'D6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                            <button v-if="engReport.D06FileName!='' " v-on:click.stop="delAttachment(engReport.Seq,'D6')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        需求碳排量(頓)<span class="small-red">&nbsp;*</span>
                        <a href="#" title="碳排連結" data-toggle="modal" data-target="#refSampleModal" class="a-blue underl" role="button">碳排連結</a>
                <p style="color: red; padding-top: 20px;">參考碳排量=概估經費(元)*回歸曲線0.38*物調(82.156/前一年的平均新指數)：{{RefCarbonEmission}}<span style="color:blue" v-if="engReport.RefCarbonEmission != engReport._RefCarbonEmission"> (未儲存) </span> </p>
          
                        <input v-model="engReport.DemandCarbonEmissions" type="text" class="col-12 form-control" value="OO">
                        <textarea v-model="engReport.DemandCarbonEmissionsMemo" class="col-12 form-control min1110"
                                  style="min-height: 100px;" placeholder="說明"></textarea>
                    </div>
                </div>
            </div>
            <h5 ref="itemsA" >概估經費 <span class="small-red">&nbsp;*</span></h5>
            <div class="table-responsive">
                <table class="table table1" id="addnew1010">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th style="width: 50px;"><strong>年度</strong></th>
                            <th><strong>項目</strong></th>
                            <th style="width: 250px;"><strong>金額(元)</strong></th>
                            <th style="text-align: center; width: 200px;" v-if="fileEditable">
                                <a v-on:click.stop="fAddItemA=true" href="##" class="btn btn-color11-3 btn-xs sharp mr-1" title="新增" ><i class="fas fa-plus"></i></a>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="fAddItemA">
                            <td></td>
                            <td><input v-model.trim="newItemA.Year" type="text" class="form-control " placeholder="" style="min-width: 150px;" maxlength="3"></td>
                            <td>
                                <select v-model.trim="newItemA.AttributesSeq" class="form-control my-1 mr-0 mr-sm-1 WidthasInput" style="min-width: 200px;">
                                    <option v-for="option in selectProposalReviewAttributesOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <td><input v-model.trim="newItemA.Price" type="text" class="form-control " placeholder=""></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onAddRecordA(newItem)" class="btn btn-color11-2 btn-xs mx-1" title="新增"><i class="fas fa-save"></i></button>
                                    <button @click="fAddItemA=false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr v-for="(item, index) in itemsA" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td>
                                <div v-if="!item.edit">{{item.Year}}</div>
                                <input v-if="item.edit" type="text" v-model.trim="item.Year" style="min-width: 150px;" maxlength="3" class="form-control" />
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.AttributesName}}</div>
                                <select v-if="item.edit" v-model.trim="item.AttributesSeq" class="form-control my-1 mr-0 mr-sm-1 WidthasInput" style="min-width: 200px;">
                                    <option v-for="option in selectProposalReviewAttributesOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.Price}}</div>
                                <input v-if="item.edit" type="text" v-model.trim="item.Price" maxlength="12" class="form-control" />
                            </td>
                            <td style="min-width: 105px;"  v-if="fileEditable">
                                <a href="#" v-if="!item.edit" v-on:click.prevent="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="onSaveRecordA(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                <a href="#" v-on:click.prevent="onDelEngA(index, item.Seq)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </td>
                        </tr>
                        <!--<tr v-if="itemsA==null||itemsA.length==0">
                    <td colspan="5" class="text-center">--無資料--</td>
                </tr>-->
                    </tbody>
                </table>
                <label style="padding-top: 10px;">合計：{{engReport.EngReportEstimatedCostPrice}}元</label>
            </div>
            <h5>在地溝通辦理情形</h5>
            <div class="table-responsive">
                <table class="table table1" id="addnew000">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th><strong>日期</strong></th>
                            <th><strong>文號</strong></th>
                            <th><strong>檔案名稱</strong></th>
                            <th style="text-align: center; width: 200px;" v-if="fileEditable">
                                <a v-on:click.stop="fAddItemB=true" href="##" class="btn btn-color11-3 btn-xs sharp mr-1" title="新增" ><i class="fas fa-plus"></i></a>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="fAddItemB">
                            <td></td>
                            <td style="text-align: center;"><input :value="newItemB.Date"  @input="e => newItemB.Date = checkDateSmaller(newItemB.Date, e)" type="date" name="bday"  class="form-control"></td>
                            <td><input v-model.trim="newItemB.FileNumber" type="text" class="form-control "></td>
                            <td>
                                <b-form-file v-model="newFileB" placeholder="未選擇任何檔案"></b-form-file>
                            </td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onAddRecordB(newItem)" class="btn btn-color11-2 btn-xs mx-1" title="新增"><i class="fas fa-save"></i></button>
                                    <button @click="fAddItemB=false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr v-for="(item, index) in itemsB" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td>
                                <div v-if="!item.edit">{{item.DateStr}}</div>
                                <input v-if="item.edit" :value="item.DateStr" @input="e => item.DateStr = checkDateSmaller(item.DateStr, e)" type="date" name="bday" class="form-control">
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.FileNumber}}</div>
                                <input v-if="item.edit" type="text" v-model.trim="item.FileNumber" class="form-control" />
                            </td>

                            <td>
                                <div v-if="!item.edit">{{item.FileName}}</div>
                                <b-form-file v-if="item.edit" v-model="editFileB" placeholder="未選擇任何檔案"></b-form-file>
                            </td>
                            <td style="min-width: 105px;"  v-if="fileEditable">
                                <a href="#" v-if="!item.edit" v-on:click.prevent="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="onSaveRecordB(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="item.edit==item.edit" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i>取消</a>
                                <button v-if="!item.edit" @click="downloadLC(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                <a href="#" v-if="!item.edit" v-on:click.prevent="onDelEngB(index, item.Seq)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </td>
                        </tr>
                        <!--<tr v-if="itemsB==null||itemsB.length==0">
                    <td colspan="5" class="text-center">--無資料--</td>
                </tr>-->
                    </tbody>
                </table>
            </div>
            <h5>在地諮詢辦理情形</h5>
            <div class="table-responsive">
                <table class="table table1" id="addnew001">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th><strong>日期</strong></th>
                            <th><strong>文號</strong></th>
                            <th><strong>檔案名稱</strong></th>
                            <th style="text-align: center; width: 200px;" v-if="fileEditable">
                                <a v-on:click.stop="fAddItemC=true" href="##" class="btn btn-color11-3 btn-xs sharp mr-1" title="新增" ><i class="fas fa-plus"></i></a>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="fAddItemC">
                            <td></td>
                            <td style="text-align: center;"><input :value="newItemC.Date" @input="e => newItemC.Date = checkDateSmaller(newItemC.Date, e)" type="date" name="bday" class="form-control"></td>
                            <td><input v-model.trim="newItemC.FileNumber" type="text" class="form-control "></td>
                            <td>
                                <b-form-file v-model="newFileC" placeholder="未選擇任何檔案"></b-form-file>
                            </td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onAddRecordC(newItem)" class="btn btn-color11-2 btn-xs mx-1" title="新增"><i class="fas fa-save"></i></button>
                                    <button @click="fAddItemC=false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr v-for="(item, index) in itemsC" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td>
                                <div v-if="!item.edit">{{item.DateStr}}</div>
                                <input v-if="item.edit" :value="item.DateStr" @input="e => item.DateStr = checkDateSmaller(item.DateStr, e)" type="date" name="bday" class="form-control">
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.FileNumber}}</div>
                                <input v-if="item.edit" type="text" v-model.trim="item.FileNumber" class="form-control" />
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.FileName}}</div>
                                <b-form-file v-if="item.edit" v-model="editFileC" placeholder="未選擇任何檔案"></b-form-file>
                            </td>
                            <td style="min-width: 105px;"  v-if="fileEditable">
                                <a href="#" v-if="!item.edit" v-on:click.prevent="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="onSaveRecordC(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="item.edit= !item.edit" class="btn btn-color9-1 btn-xs sharp mx-1" title="取消"><i class="fas fa-times"></i>取消</a>
                                <button v-if="!item.edit" @click="downloadSC(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                                <a href="#" v-if="!item.edit" v-on:click.prevent="onDelEngC(index, item.Seq)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                            </td>
                        </tr>
                        <!--<tr v-if="itemsB==null||itemsB.length==0">
                    <td colspan="5" class="text-center">--無資料--</td>
                </tr>-->
                    </tbody>
                </table>
            </div>
            <h5 v-if="engReport.ProposalReviewAttributesSeq != 1">主要工作內容</h5>
            <div v-if="engReport.ProposalReviewAttributesSeq != 1" class="table-responsive">
                <table class="table table1" id="addnew002">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th><strong>提報工作內容</strong></th>
                            <th style="width:110px;"><strong>數量</strong></th>
                            <th style="width:140px;"><strong>經費(元)</strong></th>
                            <th style="width:200px;"><strong>備註</strong></th>
                            <th style="text-align: center; width: 150px;" v-if="fileEditable">
                                <a v-on:click.stop="fAddItemD=true" href="##" class="btn btn-color11-3 btn-xs sharp mr-1" title="新增" ><i class="fas fa-plus"></i></a>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="fAddItemD">
                            <td></td>
                            <td>
                                <select v-model.trim="newItemD.RptJobDescriptionSeq" class="form-control my-1 mr-0 mr-sm-1 WidthasInput" style="min-width: 200px; width: 230px;">
                                    <option v-for="option in selectReportJobDescriptionOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                                <input v-if="newItemD.RptJobDescriptionSeq==9" v-model.trim="newItemD.OtherJobDescription" type="text" class="form-control " placeholder="" style="width:200px;">
                            </td>
                            <td><input v-model.trim="newItemD.Num" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItemD.Cost" type="text" class="form-control"></td>
                            <td><input v-model.trim="newItemD.Memo" type="text" class="form-control"></td>
                            <td style="min-width: unset;">
                                <div class="d-flex justify-content-center">
                                    <button @click="onAddRecordD(newItem)" class="btn btn-color11-2 btn-xs mx-1" title="新增"><i class="fas fa-save"></i></button>
                                    <button @click="fAddItemD=false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr v-for="(item, index) in itemsD" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td>
                                <div v-if="!item.edit">{{item.RptJobDescriptionName}}&nbsp;{{item.OtherJobDescription}}</div>
                                <select v-if="item.edit" v-model.trim="item.RptJobDescriptionSeq" class="form-control my-1 mr-0 mr-sm-1 WidthasInput" style="min-width: 200px; width: 230px;">
                                    <option v-for="option in selectReportJobDescriptionOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                        {{ option.Text }}
                                    </option>
                                </select>
                                <input v-if="item.edit && item.RptJobDescriptionSeq==9" v-model.trim="item.OtherJobDescription" type="text" class="form-control " placeholder="" style="width:200px;">
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.Num}}</div>
                                <input v-if="item.edit" v-model.trim="item.Num" type="text" class="form-control" />
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.Cost}}</div>
                                <input v-if="item.edit" v-model.trim="item.Cost" type="text" class="form-control" />
                            </td>
                            <td>
                                <div v-if="!item.edit">{{item.Memo}}</div>
                                <input v-if="item.edit" v-model.trim="item.Memo" type="text" class="form-control" />
                            </td>
                            <td style="min-width: 140px;" v-if="fileEditable">
                                <a href="#" v-if="!item.edit" v-on:click.prevent="item.edit=!item.edit" class="btn btn-color11-3 btn-xs mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</a>
                                <a href="#" v-if="!item.edit" v-on:click.prevent="onDelEngD(index, item.Seq)" class="btn btn-color9-1 btn-xs mx-1" title="刪除"><i class="fas fa-trash-alt"></i> 刪除</a>
                                <a href="#" v-if="item.edit" v-on:click.prevent="onSaveRecordD(item)" class="btn btn-color11-2 btn-xs mx-1"><i class="fas fa-save"></i> 儲存</a>
                                <button v-if="item.edit" @click="item.edit = false" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i> 取消</button>
                            </td>
                        </tr>
                        <!--<tr v-if="itemsD==null||itemsD.length==0">
                    <td colspan="6" class="text-center">--查無資料--</td>
                </tr>-->
                    </tbody>
                </table>
            </div>
            
            <div class="custom-control custom-checkbox">
                <input type="checkbox" class="custom-control-input" id="AR" v-model="engReport.IsFloodControlRecords">
                <label class="custom-control-label" for="AR">防洪記載</label>
            </div>

            <h5>附件上傳</h5>
            <a href="#" title="說明" target="_blank" data-toggle="modal" data-target="#prepare_edit01">說明&nbsp;</a>
            <p style="color: red; padding-top: 20px;">*請上傳 jpg、png 格式</p>
            <p style="color: red;">屬「用地先期作業」及「用地取得」者<br>以下僅需檢附位置圖、空拍圖、現場照片、基地地籍圖，其餘免填</p>
            <!-- 小視窗 編輯人員 -->
            <div class="modal fade" id="prepare_edit01">
                <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                    <div class="modal-content">
                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                            <div class="tab-content" style="width: 1030px;">
                                <div class="tab-pane active">
                                    <h5>附件上傳說明</h5>
                                    位置圖：以流域圖標註工程位置<br>
                                    空拍照：照片需清晰並需標註工程位置，以利辨識毗鄰地上物現況<br>
                                    現場照片：工程位置上、下游及左、右兩岸至少各1張具代表性現場照片<br>
                                    基地地藉圖：以地籍圖為底圖標註工程位置，確認用地範圍<br>
                                    工程平面配置圖：<br>
                                    一、以地形圖為底圖彙製工程平面配置圖<br>
                                    二、圖面範圍包括自治理河(渠)段上游100 公尺至治理河(渠)段下游100 公尺止或採上下游加測L/4(L=施工長度)<br>
                                    三、請標明圖例、指北、比例尺、水流方向、用地範圍線(紅線)、治理計畫線(黃線)及河川區域線(綠線)、縱、橫斷面樁號及工程位置<br>
                                    縱斷面圖：請結合工程範圍內流域條件，簡述工程配置原則，並依個案性質檢附結構穩定計算說明。<br>
                                    標準斷面圖：請標註樁號、計畫洪水位、計畫堤頂高、設計堤頂高、設計河床高、現況河床高及構造物位置
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="table-responsive">
                <table class="table table-responsive-md table-hover">
                    <thead>
                        <tr class="insearch">
                            <th class="text-left"><strong>資料名稱</strong></th>
                            <th style="width: 400px;"><strong>上傳檔案</strong></th>
                            <th style="width: 140px;" ><strong>功能</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>位置圖(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.LocationMapFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3"  v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A1')"  id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.LocationMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A1')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.LocationMapFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A1')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>空拍照(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.AerialPhotographyFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A2')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.AerialPhotographyFileName!=''" v-on:click.stop="download(engReport.Seq,'A2')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.AerialPhotographyFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A2')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>現場照片</strong></td>
                            <td>{{engReport.ScenePhotoFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A3')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.ScenePhotoFileName!=''" v-on:click.stop="download(engReport.Seq,'A3')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.ScenePhotoFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A3')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>基地地藉圖(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.BaseMapFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A4')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.BaseMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A4')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.BaseMapFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A4')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>工程平面配置圖(建議比例尺大於1/1000)</strong></td>
                            <td>{{engReport.EngPlaneLayoutFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A5')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.EngPlaneLayoutFileName!=''" v-on:click.stop="download(engReport.Seq,'A5')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.EngPlaneLayoutFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A5')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>縱斷面圖</strong></td>
                            <td>{{engReport.LongitudinalSectionFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable">
                                    <input v-on:change="fileChange($event,'A6')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.LongitudinalSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.LongitudinalSectionFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A6')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>標準斷面圖 (建議比例尺水平大於1/1000、垂直大於1/500)</strong></td>
                            <td>{{engReport.StandardSectionFileName}}</td>
                            <td style="display: flex;">
                                <label class="btn btn-shadow btn-color11-3" v-if="fileEditable"> 
                                    <input v-on:change="fileChange($event,'A6')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                                </label>
                                <button v-if="engReport.StandardSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                <button v-if="engReport.StandardSectionFileName!='' && fileEditable" v-on:click.stop="delAttachment(engReport.Seq,'A6')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <h5 v-if="engReport.IsProposalReview==1  && !FundDiff">審核意見</h5>
        <div v-if="engReport.IsProposalReview==1 && !FundDiff " style="display: flex;" >
            <div class="custom-control custom-radio custom-control-inline " style="padding-top: 10px;" >
                <input  :disabled="!(isAdmin||isEQCAdmin)" v-model="engReport.ProposalAuditOpinion"  :value="1 " type="radio" :name="tenderId +'QA'" :id=" 'QA07'" class="custom-control-input">
                <label :for="'QA07'" class="custom-control-label">同意</label>
            </div><br>
            <div class="custom-control custom-radio custom-control-inline" style="padding-top: 10px;"  >
                <input :disabled="!(isAdmin||isEQCAdmin)" v-model="engReport.ProposalAuditOpinion" :value="2" type="radio" :name="tenderId +'QA'" :id="'QA08'" class="custom-control-input">
                <label :for="'QA08'" class="custom-control-label">暫緩</label>
            </div>
            <button class="btn btn-color11-2 btn-xs sharp mx-1 m-2" @click="updateProposalReview()">
                <i class="fas fa-save"> </i>
            </button>
        </div>
        <div class="row justify-content-center mt-5">
            <div class="d-flex" >
                <button v-if="fileEditable " v-on:click.stop="onSend()" role="button" class="btn btn-color11-2 btn-xs mx-1">
                    <i class="fas fa-save">&nbsp;送簽</i>
                </button>
            </div>
            <div v-if="fileEditable" class="d-flex">
                <button v-on:click.stop="onSave()" role="button" class="btn btn-color11-2 btn-xs mx-1">
                    <i class="fas fa-save">&nbsp;暫存</i>
                </button>
            </div>
            <div class="d-flex">
                <button v-on:click.stop="back()" role="button" class="btn btn-color9-1 btn-xs mx-1"> 回上頁</button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ["tenderId"],
        data: function () {
            return {

                isAdmin: false,
                isEQCAdmin: false,

                targetId: null,
                file: null,//{ name: null, size: null },
                files: new FormData(),
                filesB: new FormData(),
                filesC: new FormData(),

                //使用者單位資訊
                userUnit: null,
                userUnitName: '',
                userUnitSub: null,
                userUnitSubName: '',

                selectProposalReviewTypeOptions: [],//類別清單
                selectProposalReviewAttributesOptions: [],//屬性清單
                selectRiverAOptions: [],//河川清單A
                selectRiverBOptions: [],//河川清單B
                selectRiverCOptions: [],//河川清單C
                selectDrainOptions: [],//排水
                selectReportJobDescriptionOptions: [],//工作內容清單
                cities: [],//行政區(縣市)清單
                towns: [],//行政區(鄉鎮)清單
                engReport: { IsProposalReview:0, D1FileName: '', D2FileName: '', D3FileName: '', D4FileName: '', D5FileName: '', D6FileName: '', LocationMapFileName: '', AerialPhotographyFileName: '', ScenePhotoFileName: '', BaseMapFileName: '', EngPlaneLayoutFileName: '', LongitudinalSectionFileName: '', StandardSectionFileName: '' },

                fEdit: false,

                fAddItemA: false,
                fAddItemB: false,
                fAddItemC: false,
                fAddItemD: false,

                newItemA: {},
                newItemB: {},
                newItemC: {},
                newItemD: {},

                itemsA: [],
                itemsB: [],
                itemsC: [],
                itemsD: [],

                newFileB: null,
                newFileC: null,

                editFileB: null,
                editFileC: null,

                editRiver: 0,
                editDrain: 0,
                editCoastal: 0,
                editPR:0
            };
        },
        watch :{
            // RefCarbonEmission : {
            //     async handler(value)
            //     {
            //         this.engReport.RefCarbonEmission = value;
            //     }
            // },
            engReport :{
                async handler(value){
                    await new Promise(re => setTimeout(re, 200));
                    if(value.IsProposalReview ==1 && value.ProposalAuditOpinion ==1)
                    {
     
                        Array.apply(null, document.querySelectorAll("input, textarea, select") )
                            .filter(e => e.id != "QA07" && e.id != 'QA08')
                            .forEach(e => e.disabled = true);

                            
                    }
                    else{
                        Array.apply(null, document.querySelectorAll("input, textarea, select") )
                            .forEach(e => e.disabled = false);
                    }
                    Array.apply(null, document.querySelectorAll("#inputFile"))
                    .forEach(e => e.disabled = false);
                }
            }
        },
        computed : {
            FundDiff()
            {
                return  this.estimateCost !=  this.itemsD.reduce((a, c) => a+ c.Cost, 0) && this.engReport.ProposalReviewAttributesSeq != 1
            },
            RefCarbonEmission()
            {
                return ( (this.engReport.RefCarbonEmissionFactor * this.estimateCost) ?? 0 ).toFixed(2);
                
            },
            estimateCost()
            {
                return this.itemsA.reduce( (a, c) => a + c.Price, 0);
            },
            fileEditable()
            {
                return this.engReport.IsProposalReview==0 || this.engReport.ProposalAuditOpinion != 1
            }
        },
        methods: {
            async updateProposalReview()
            {
                let {data :res} = await window.myAjax.post("ERProposalReview/UpdateProposalReview", { seq : this.engReport.Seq ,proposalAuditOption : this.engReport.ProposalAuditOpinion});
                if(res)
                {
                    alert("儲存成功");
                    this.getItem();
                }
            },
            checkDateSmaller(orgdate, event)
            {
                if(new Date(event.target.value) < new Date())
                    return event.target.value;
                event.target.value = orgdate;
                alert("請填寫今天以前的日期");
                return orgdate;
            },
            //行政區(縣市)
            async getCities() {
                this.cities = [];
                const { data } = await window.myAjax.post('/EngReport/GetCityList');
                this.cities = data;
            },
            onCityChange(event) {
                this.getCityTown();
            },
            //行政區(鄉鎮)
            async getCityTown() {
                if (this.engReport.CitySeq > 0) {
                    this.twons = [];
                    const { data } = await window.myAjax.post('/TenderPlan/GetTownList', { id: this.engReport.CitySeq });
                    this.towns = data;
                }
            },
            //類別
            async getProposalReviewTypeOption() {
                this.selectProposalReviewTypeOptions=[];
                const { data } = await window.myAjax.post('/EngReport/GetProposalReviewTypeList');
                this.selectProposalReviewTypeOptions = data;
                this.onReviewTypeChange();
            },
            //屬性
            async getProposalReviewAttributesOption() {
                this.selectProposalReviewAttributesOptions=[];
                const { data } = await window.myAjax.post('/EngReport/GetProposalReviewAttributesList');
                this.selectProposalReviewAttributesOptions = data;
            },
            //河川A
            async getRiverAOption() {
                this.selectRiverAOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetRiverList', { id: this.targetId, ParentSeq: 0 });
                this.selectRiverAOptions = data;
            },
            onRiverAChange(event) {
                this.getRiverBOption();
            },
            //河川B
            async getRiverBOption() {
                this.selectRiverBOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetRiverList', { id: this.targetId, ParentSeq: this.engReport.RiverSeq1 });
                this.selectRiverBOptions = data;
            },
            onRiverBChange(event) {
                this.getRiverCOption();
            },
            //河川C
            async getRiverCOption(parentSeq) {
                this.selectRiverCOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetRiverList', { id: this.targetId, ParentSeq: this.engReport.RiverSeq2 });
                this.selectRiverCOptions = data;
            },
            //排水
            async getDrainOption() {
                this.selectDrainOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetDrainList');
                this.selectDrainOptions = data;
            },
            //工作內容
            async getReportJobDescriptionOption() {
                this.selectReportJobDescriptionOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetReportJobDescriptionList');
                this.selectReportJobDescriptionOptions = data;
            },
            //取得工程資料
            getItem() {
                this.engReport = {};
                window.myAjax.post('/ERProposalReview/GetEngReport', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReport = resp.data.item;
                            if (this.strEmpty(this.engReport.LargeSectionChainage))
                                this.engReport.LargeSectionChainage = "斷面OO-OO";
                            if (this.strEmpty(this.engReport.EngineeringScale))
                                this.engReport.EngineeringScale = "面積OO、長度OO、寬度OO";
                            if (this.strEmpty(this.engReport.ProcessReason))
                                this.engReport.ProcessReason = "請敘明主要待解決問題、需求及預期達成目標";
                            if (this.strEmpty(this.engReport.EngineeringScaleMemo))
                                this.engReport.EngineeringScaleMemo = "請敘明工程規模選定原則，在整體考量原則下，後續是否有分期執行需求";
                            if (this.strEmpty(this.engReport.RelatedReportContent))
                                this.engReport.RelatedReportContent = "簡述工程範圍內相關報告內容摘要(摘錄河川、區域排水治理計畫、風險評估、水利建造物檢查、整建計畫、環境營造規劃及流域整體改善與調適規劃成果，若報告內容無涉及本工程範圍部分則免)";
                            if (this.strEmpty(this.engReport.ProtectionTarget))
                                this.engReport.ProtectionTarget = "○○縣 ○○鄉鎮市 ○○村 ○○千人，○○公頃";
                            this.getCityTown();
                            this.getRiverBOption();
                            this.getRiverCOption();
                            this.onReviewTypeChange();
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSave() {
                window.myAjax.post('/ERProposalReview/UpdateTempEngReport', { m: this.engReport})
                    .then(resp => {
                        this.saveFlag = false;
                        if (resp.data.result == 0) {
                            alert(resp.data.msg);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //送簽
            onSend() {
                if(this.estimateCost !=  this.itemsD.reduce((a, c) => a+ c.Cost, 0) && this.engReport.ProposalReviewAttributesSeq != 1)
                {
                    alert("該工程內容的經費與概估經費不同");
                    return ;
                }
                if(this.itemsA.length == 0)
                {
                    alert("概估經費需要填寫");
                    return ;
                }
                this.engReport.EstimatedLandAcquisitionCosts 
                    = this.engReport.EstimatedLandAcquisitionCosts  ?? 0 ;
                window.myAjax.post('/ERProposalReview/UpdateEngReport', { m: this.engReport, isSend: 1 })
                    .then(resp => {
                        this.saveFlag = false;
                        if (resp.data.result == 0) {
                            this.getItem();
                            alert(resp.data.msg);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            download(id, fileNo) {
                window.myAjax.get('/ERProposalReview/Download?id=' + id + '&fileNo=' + fileNo, { responseType: 'blob' })
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
            //上傳事件
            fileChange(event, type) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                uploadfiles.append("Seq", this.engReport.Seq);
                uploadfiles.append("fileType", type);
                this.upload(uploadfiles, files[0].name,type);
            },
            //上傳
            upload(uploadfiles,fileName,type) {
                window.myAjax.post('/ERProposalReview/UploadAttachment', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            if (type == 'A1') this.engReport.LocationMapFileName = fileName;
                            if (type == 'A2') this.engReport.AerialPhotographyFileName = fileName;
                            if (type == 'A3') this.engReport.ScenePhotoFileName = fileName;
                            if (type == 'A4') this.engReport.BaseMapFileName = fileName;
                            if (type == 'A5') this.engReport.EngPlaneLayoutFileName = fileName;
                            if (type == 'A6') this.engReport.LongitudinalSectionFileName = fileName;
                            if (type == 'A7') this.engReport.StandardSectionFileName = fileName;

                            if (type == 'D1') this.engReport.D01FileName = fileName;
                            if (type == 'D2') this.engReport.D02FileName = fileName;
                            if (type == 'D3') this.engReport.D03FileName = fileName;
                            if (type == 'D4') this.engReport.D04FileName = fileName;
                            if (type == 'D5') this.engReport.D05FileName = fileName;
                            if (type == 'D6') this.engReport.D06FileName = fileName;
                            //this.getItem();
                        }
                        //alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //刪除 附件
            delAttachment(id, fileNo) {
                window.myAjax.post('/ERProposalReview/DelAttachment', { Seq: id, fileNo: fileNo })
                    .then(resp => {
                        this.getItem();
                        alert(resp.data.message);
                        console.log(resp);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onNewRecord(Seq,id) {
                window.myAjax.post('/ERProposalReview/NewRecord', { Seq: Seq, id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            if (id == 1)
                                this.newItemA = resp.data.item;
                            else if (id == 2)
                                this.newItemC = resp.data.item;
                            else if (id == 3)
                                this.newItemC = resp.data.item;
                            else if (id == 4)
                                this.newItemD = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            async getSubList(Seq,id) {
                this.fAddItemA = false;
                this.fAddItemB = false;
                this.fAddItemC = false;
                this.fAddItemD = false;
                this.newFileB = null;
                this.newFileC = null;
                this.editFileB = null;
                this.editFileC = null;
                window.myAjax.post('/ERProposalReview/GetSubList', { Seq: Seq, id: id })
                    .then(resp => {
                        if (id == 1) {
                            this.itemsA = resp.data.items;
                            this.setEngReportEstimatedCostPrice();
                        }
                        else if (id == 2)
                            this.itemsB = resp.data.items;
                        else if (id == 3)
                            this.itemsC = resp.data.items;
                        else if (id == 4)
                            this.itemsD = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onAddRecordA() {
                //console.log(uItem);
                if (this.strEmpty(this.newItemA.Year) || this.strEmpty(this.newItemA.Price) || this.newItemA.AttributesSeq == null) {
                    alert('年度, 項目, 金額 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERProposalReview/UpdateEngReportEstimatedCost', { m: this.newItemA })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSubList(this.targetId, 1);
                            this.onNewRecord(this.targetId, 1);
                            this.setEngReportEstimatedCostPrice();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            fileChangeB(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                this.filesB = new FormData();
                this.filesB.append("file", files[0], files[0].name);
                this.newItemB.FileName = files[0].name;
                console.log(this.newItemB.FileName);
                //this.onAddRecordB();
            },
            onAddRecordB() {
                //console.log(uItem);
                if (this.strEmpty(this.newItemB.Date) || this.strEmpty(this.newItemB.FileNumber) || this.newFileB == null) {
                    alert('日期, 文號, 上傳檔案 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngReportSeq", this.targetId);
                uploadfiles.append("Seq", -1);
                uploadfiles.append("Date", this.newItemB.Date);
                uploadfiles.append("FileNumber", this.newItemB.FileNumber);
                uploadfiles.append("file", this.newFileB, this.newFileB.name);
                window.myAjax.post('/ERProposalReview/AddEngReportLocalCommunication', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSubList(this.targetId, 2);
                            this.onNewRecord(this.targetId, 2);
                            this.filesB = new FormData();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            fileChangeC(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                this.filesC = new FormData();
                this.filesC.append("file", files[0], files[0].name);
                this.newItemC.FileName = files[0].name;
                //this.onAddRecordC();
            },
            onAddRecordC() {
                //console.log(uItem);
                if (this.strEmpty(this.newItemC.Date) || this.strEmpty(this.newItemC.FileNumber) || this.newFileC == null) {
                    alert('日期, 文號, 上傳檔案 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngReportSeq", this.targetId);
                uploadfiles.append("Seq", -1);
                uploadfiles.append("Date", this.newItemC.Date);
                uploadfiles.append("FileNumber", this.newItemC.FileNumber);
                uploadfiles.append("file", this.newFileC, this.newFileC.name);
                window.myAjax.post('/ERProposalReview/AddEngReportOnSiteConsultation', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSubList(this.targetId, 3);
                            this.onNewRecord(this.targetId, 3);
                            this.filesC = new FormData();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onAddRecordD() {
                //console.log(uItem);
                if (this.strEmpty(this.newItemD.Num) || this.strEmpty(this.newItemD.Cost) || this.newItemD.RptJobDescriptionSeq == null) {
                    alert('提報工作內容、數量、經費(萬元) 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERProposalReview/UpdateEngReportMainJobDescription', { m: this.newItemD })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSubList(this.targetId, 4);
                            this.onNewRecord(this.targetId, 4);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSaveRecordA(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Year) || this.strEmpty(uItem.Price) || uItem.AttributesSeq == null) {
                    alert('年度, 項目, 金額 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERProposalReview/UpdateEngReportEstimatedCost', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            this.getSubList(this.targetId, 1);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSaveRecordB(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Date) || this.strEmpty(uItem.FileNumber)) {
                    alert('日期, 文號, 檔案 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngReportSeq", this.targetId);
                uploadfiles.append("Seq", uItem.Seq);
                uploadfiles.append("Date", uItem.DateStr);
                uploadfiles.append("FileNumber", uItem.FileNumber);
                uploadfiles.append("file", this.editFileB, this.editFileB.name);
                window.myAjax.post('/ERProposalReview/AddEngReportLocalCommunication', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            this.getSubList(this.targetId, 2);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSaveRecordC(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Date) || this.strEmpty(uItem.FileNumber)) {
                    alert('日期, 文號, 檔案 必須輸入!');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("EngReportSeq", this.targetId);
                uploadfiles.append("Seq", uItem.Seq);
                uploadfiles.append("Date", uItem.DateStr);
                uploadfiles.append("FileNumber", uItem.FileNumber);
                uploadfiles.append("file", this.editFileC, this.editFileC.name);
                window.myAjax.post('/ERProposalReview/AddEngReportOnSiteConsultation', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            this.getSubList(this.targetId, 3);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onSaveRecordD(uItem) {
                //console.log(uItem);
                if (this.strEmpty(uItem.Num) || this.strEmpty(uItem.Cost) || uItem.RptJobDescriptionSeq == null) {
                    alert('提報工作內容、數量、經費(萬元) 必須輸入!');
                    return;
                }
                window.myAjax.post('/ERProposalReview/UpdateEngReportMainJobDescription', { m: uItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            uItem.edit = false;
                            this.getSubList(this.targetId, 4);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelEngA(index, id) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/ERProposalReview/DelEngReportEstimatedCost', { id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.itemsA.splice(index, 1);
                            this.setEngReportEstimatedCostPrice();
                            alert(resp.data.msg);
                            console.log(resp);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelEngB(index, id) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/ERProposalReview/DelEngReportLocalCommunication', { id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.itemsB.splice(index, 1);
                            alert(resp.data.msg);
                            console.log(resp);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelEngC(index, id) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/ERProposalReview/DelEngReportOnSiteConsultation', { id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.itemsC.splice(index, 1);
                            alert(resp.data.msg);
                            console.log(resp);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDelEngD(index, id) {
                if (!confirm('是否確定刪除資料？')) return;
                window.myAjax.post('/ERProposalReview/DelEngReportMainJobDescription', { id: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.itemsD.splice(index, 1);
                            alert(resp.data.msg);
                            console.log(resp);
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            back() {
                //window.history.go(-1);
                window.location = "/ERProposalReview/Index";
            },
            onReviewTypeChange(event) {
                if (this.engReport.ProposalReviewTypeSeq == 1) {
                    this.editRiver = 1;
                    this.editDrain = 0;
                    this.editCoastal = 0;
                }
                else if (this.engReport.ProposalReviewTypeSeq == 2) {
                    this.editRiver = 0;
                    this.editDrain = 1;
                    this.editCoastal = 0;
                }
                else if (this.engReport.ProposalReviewTypeSeq == 3) {
                    this.editRiver = 0;
                    this.editDrain = 0;
                    this.editCoastal = 1;
                }
                else if (this.engReport.ProposalReviewTypeSeq > 3) {
                    this.editRiver = 1;
                    this.editDrain = 1;
                    this.editCoastal = 1;
                }
                else {
                    this.editRiver = 0;
                    this.editDrain = 0;
                    this.editCoastal = 0;
                }
                //if (this.strEmpty(this.engReport.SetConditions)) { 
                    if(!this.engReport.SetConditions)
                    {
                        if (this.engReport.ProposalReviewTypeSeq == 3) {
                        this.engReport.SetConditions = "河床谿線高OO公尺、現況河床高OO公尺、計畫洪水高OO公尺、常水位高OO公尺、計畫河寬OO公尺、現況河寬OO公尺、計畫堤頂高OO公尺、計畫流量OOcms、計畫流速OOcms、河床質D50OOCM、現況堤頂高程OO公尺、現況基腳深度OO公尺、其他";
                        } else {
                            this.engReport.SetConditions = "高潮位高程OO公尺、暴潮位高程OO公尺、計畫堤頂高程OO公尺、現況堤頂高OO公尺、現況海堤長度OO公尺、現況外坡坡度O/O、現況內坡坡度O/O、其他";
                        }
                    }

                //}
            },
            //下載-在地溝通辦理情形
            downloadLC(item) {
                window.myAjax.get('/ERProposalReview/DownloadLC?id=' + item.Seq, { responseType: 'blob' })
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
            //下載-在地諮詢辦理情形
            downloadSC(item) {
                window.myAjax.get('/ERProposalReview/DownloadSC?id=' + item.Seq, { responseType: 'blob' })
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
            setEngReportEstimatedCostPrice() {
                let costTotal = parseInt(0);
                this.itemsA.forEach(r => {
                    costTotal += parseInt(r.Price);
                });
                this.engReport.EngReportEstimatedCostPrice = costTotal;
            },
        },
        async mounted() {
            console.log('mounted() 提案審查A ' + window.location.href );
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                if (this.selectProposalReviewTypeOptions.length == 0) this.getProposalReviewTypeOption();
                if (this.selectProposalReviewAttributesOptions.length == 0) this.getProposalReviewAttributesOption();
                if (this.selectDrainOptions.length == 0) this.getDrainOption();
                if (this.selectReportJobDescriptionOptions.length == 0) this.getReportJobDescriptionOption();
                if (this.cities.length == 0) this.getCities();
                this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
                this.isEQCAdmin = localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
                this.targetId = parseInt(urlParams.get('id'), 10);
                // console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    this.getItem();
                    this.getSubList(this.targetId, 1);
                    this.getSubList(this.targetId, 2);
                    this.getSubList(this.targetId, 3);
                    this.getSubList(this.targetId, 4);
                    if (this.selectRiverAOptions.length == 0) this.getRiverAOption();
                    this.onNewRecord(this.targetId, 1);
                    this.onNewRecord(this.targetId, 2);
                    this.onNewRecord(this.targetId, 3);
                    this.onNewRecord(this.targetId, 4);
                    return;
                }
            }
            // window.history.back(); //.location = "/FrontDesk";
        }
    }
</script>
