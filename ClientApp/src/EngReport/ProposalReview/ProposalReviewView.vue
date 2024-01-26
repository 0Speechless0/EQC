<template>
    <div>
        <div>
            <h5>需求評估</h5>
            <div class="table-responsive">
                <table border="0" class="table table1 min910">
                    <tbody>
                        <tr>
                            <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>緣起及範圍<br><br><font color="blue">(工務課)</font></th>
                            <td colspan="5">
                                <textarea v-model.trim="engReport.OriginAndScope" rows="7" class="form-control" v-bind:disabled="true" placeholder="1.緣起：請敘明待解決問題或預期達到目的，以呈現提案必要性及需求性。2.範圍：○○溪○○堤段"></textarea>
                            </td>
                            <td style="width: 170px;">
                                {{engReport.OriginAndScopeUserName}}<br>{{engReport.OriginAndScopeTWDT}}
                            </td>
                        </tr>
                        <tr>
                            <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>相關報告成果</th>
                            <td colspan="5">
                                <textarea v-model.trim="engReport.RelatedReportResults" rows="7" class="form-control" v-bind:disabled="true" placeholder="1.風險評估□否：□是：高、中、低風險對策：低水保護配合河道整理2.防汛熱點□否：□是：3.淹水潛勢□否：□是：4.易致災河段□否：□是：106年0601豪雨造成基礎淘空5.□治理計畫□規劃(檢討)報告：(1)公告時間：75年10月八掌溪治理基本計畫(2)保護標準：50年防洪頻率(3)計畫流量：3600cms(4)計畫洪水位：(5)計畫堤頂高：(6)計畫流速：(7)待建設施：□堤防、□護岸、□保護工、□其他：6.其他成果報告：(請摘錄與本計畫相關)"></textarea>
                            </td>
                            <td style="width: 150px;">
                                {{engReport.RelatedReportResultsUserName}}<br>{{engReport.RelatedReportResultsTWDT}}<br>
                                <b-checkbox v-bind:disabled="true" v-model="engReport.RelatedReportResultsReviewState" @change="checkReviewStateB=!checkReviewStateB">建議<font color="blue">規劃課</font>覆核</b-checkbox>
                                <input class="form-control" :disabled="true"  :value="engReport.RelatedReportResultsAssignReviewUserName"/>
                                {{engReport.RelatedReportResultsUpdateReviewUserName}}<br>{{engReport.RelatedReportResultsReviewTWDT}}
                            </td>
                        </tr>
                        <tr>
                            <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>既有設施維護管理情形</th>
                            <td colspan="5">
                                <textarea v-model.trim="engReport.FacilityManagement" rows="7" class="form-control" v-bind:disabled="true" placeholder="1.歷年水利建造物檢查□定期檢查：□正常：□改善情形：□不定期檢查：106年0601豪雨造成基礎淘空□正常：□改善情形：2.涉河道高灘地管理：□否：□是：現況高灘地影響河道流路，工程設計時應納入考量3.其他："></textarea>
                            </td>
                            <td style="width: 150px;">
                                {{engReport.FacilityManagementUserName}}<br>{{engReport.FacilityManagementTWDT}}<br>
                                <b-checkbox v-bind:disabled="true" v-model="engReport.FacilityManagementReviewState" @change="checkReviewStateC=!checkReviewStateC">建議<font color="blue">管理課</font>覆核</b-checkbox>
                                <input class="form-control" :disabled="true" :value="engReport.FacilityManagementAssignReviewUserName"/>
                                {{engReport.FacilityManagementUpdateReviewUserName}}<br>{{engReport.FacilityManagementReviewTWDT}}
                            </td>
                        </tr>
                        <tr>
                            <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>提案範圍用地概述</th>
                            <td colspan="5">
                                <textarea v-model.trim="engReport.ProposalScopeLand" rows="7" class="form-control" v-bind:disabled="true" placeholder="1.是否涉及都市計畫變更□否：□是：預計何時完成都變2.是否已將計畫範圍內公私有地情形，提供工務課□否：原因□是：3.其他："></textarea>
                            </td>
                            <td style="width: 150px;">
                                {{engReport.ProposalScopeLandUserName}}<br>{{engReport.ProposalScopeLandTWDT}}<br>
                                <b-checkbox v-bind:disabled="true" v-model="engReport.ProposalScopeLandReviewState" @change="checkReviewStateD=!checkReviewStateD">建議<font color="blue">資產科</font>覆核</b-checkbox>
                                <input class="form-control" :disabled="true"  :value="engReport.ProposalScopeLandAssignReviewUserName"/>
                                {{engReport.ProposalScopeLandUpdateReviewUserName}}<br>{{engReport.ProposalScopeLandReviewTWDT}}

                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <h5>提案審查表&nbsp;&nbsp;&nbsp;<a href="./images/1676343169646.jpg" title="命名規則" target="_blank" style="font-size: smaller;">命名規則</a></h5>
            <div class="setFormcontentCenter">
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程名稱<span class="small-red">&nbsp;*</span></label>
                        <input v-bind:disabled="true" v-model="engReport.RptName" type="text" placeholder="依命名原則" class="col-12 col-md-10 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">類別<span class="small-red">&nbsp;*</span></label>
                        <select v-bind:disabled="true" v-model="engReport.ProposalReviewTypeSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput" @change="onReviewTypeChange($event)">
                            <option v-for="option in selectProposalReviewTypeOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                    <div class="col-6 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">屬性<span class="small-red">&nbsp;*</span></label>
                        <select v-bind:disabled="true" v-model="engReport.ProposalReviewAttributesSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in selectProposalReviewAttributesOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div v-if="editRiver==1" class="form-row" style="display: flex;">
                    <div class="col-12  form-inline my-2">
                        <label class="my-2 mx-2">河川<span class="small-red">&nbsp;*</span></label>
                        <select v-bind:disabled="true" v-model="engReport.RiverSeq1" @change="onRiverAChange($event)" class="form-control my-1 mr-0 mr-md-4" style="min-width: 200px;">
                            <option v-for="option in selectRiverAOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                        <select v-bind:disabled="true" v-model="engReport.RiverSeq2" @change="onRiverBChange($event)" class="form-control my-1 mr-0 mr-md-4" style="min-width: 200px;">
                            <option v-for="option in selectRiverBOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                        <select v-bind:disabled="true" v-model="engReport.RiverSeq3" class="form-control my-1 mr-0 mr-md-4 WidthasInput" style="min-width: 200px;">
                            <option v-for="option in selectRiverCOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div v-if="editDrain==1" class="form-row">
                    <div class="col-4 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">排水<span class="small-red">&nbsp;*</span></label>
                        <input v-bind:disabled="true" v-model="engReport.DrainName" type="text" id="fruit" name="fruit" list="fruits" style="min-width: 245px;">
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
                        <input v-bind:disabled="true" v-model="engReport.Coastal" type="text" placeholder="" class="col-4 col-md-8 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-4 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">大斷面樁號<span class="small-red">&nbsp;*</span></label>
                        <input v-bind:disabled="true" v-model="engReport.LargeSectionChainage" type="text" placeholder="斷面OO-OO" class="col-4 col-md-8 form-control">
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">所在行政區域<span class="small-red">&nbsp;*</span></label>
                        <div>
                            <select v-bind:disabled="true" v-model="engReport.CitySeq" @change="onCityChange($event)" class="form-control my-1 mr-0 mr-sm-1">
                                <option v-for="option in cities" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                            <select v-bind:disabled="true" v-model="engReport.TownSeq" class="form-control my-1 mr-0 mr-md-1 mr-0 mr-md-4">
                                <option v-for="option in towns" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-9 form-inline my-2 ">
                        <label class="my-2 mx-2">X座標<span class="small-red">*</span></label>
                        <div class="col-4 form-inline">
                            <input v-bind:disabled="true" v-model="engReport.CoordX" class="col-12 form-control">
                        </div>
                        <label class="my-2 mx-2">Y座標<span class="small-red">*</span></label>
                        <div class="col-4 form-inline">
                            <input v-bind:disabled="true" v-model="engReport.CoordY" class="col-12 form-control">
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程規模<span class="small-red">&nbsp;*</span></label>
                        <textarea v-bind:disabled="true" v-model="engReport.EngineeringScale" class="col-12 form-control min1110"
                                  style="min-height: 100px;"
                                  placeholder="面積OO、長度OO、寬度OO"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">辦理緣由<span class="small-red">&nbsp;*</span></label>
                        <textarea v-bind:disabled="true" v-model="engReport.ProcessReason" class="col-12 form-control min1110"
                                  style="min-height: 100px;" placeholder="請敘明主要待解決問題、需求及預期達成目標"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">工程規模說明<span class="small-red">&nbsp;*</span></label>
                        <textarea v-bind:disabled="true" v-model="engReport.EngineeringScaleMemo" class="col-12 form-control min1110"
                                  style="min-height: 100px;"
                                  placeholder="請敘明工程規模選定原則，在整體考量原則下，後續是否有分期執行需求"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">相關報告內容概述<span class="small-red">&nbsp;*</span></label>
                        <textarea v-bind:disabled="true" v-model="engReport.RelatedReportContent" class="col-12 form-control min1110"
                                  placeholder="簡述工程範圍內相關報告內容摘要(摘錄河川、區域排水治理計畫、風險評估、水利建造物檢查、整建計畫、環境營造規劃及流域整體改善與調適規劃成果，若報告內容無涉及本工程範圍部分則免)"
                                  style="min-height: 100px;"></textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">歷史災害描述<span class="small-red">&nbsp;*</span></label>
                        <div class="custom-control custom-radio custom-control-inline" style="display: flex;" onclick="qq('searchResult156',1)">
                            <input v-bind:disabled="true" v-model="engReport.HistoricalCatastrophe" v-bind:value="1" v-on:click.stop="fEdit=false" type="radio" name="YNB" id="YNB01" class="custom-control-input">
                            <label for="YNB01" class="custom-control-label">有</label>
                        </div>
                        <div class="custom-control custom-radio custom-control-inline">
                            <input v-bind:disabled="true" v-model="engReport.HistoricalCatastrophe" v-bind:value="2" v-on:click.stop="fEdit=true" type="radio" checked name="YNB" id="YNB02" class="custom-control-input" onclick="qq('searchResult156',2)">
                            <label for="YNB02" class="custom-control-label">無</label>
                        </div>
                        <textarea v-bind:disabled="true" v-model="engReport.HistoricalCatastropheMemo" class="col-12 form-control min1110" style="min-height: 100px;" placeholder="" id="searchResult156">○○事件，淹水面積○○公頃、深度○○公尺、影響○○戶構造物損壞(基礎掏刷、堤身產生裂縫或孔洞、破堤等)</textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2 form-inline">
                            保護標的<span class="small-red">&nbsp;*</span>
                        </label>
                        <textarea v-bind:disabled="true" v-model="engReport.ProtectionTarget" class="col-12 form-control" style="min-width: 750px;min-height: 100px;">○○縣 ○○鄉鎮市 ○○村 ○○千人，○○公頃</textarea>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2 form-inline">
                            設計條件<span class="small-red">&nbsp;*</span>
                        </label>
                        <textarea v-bind:disabled="true" v-model="engReport.SetConditions" class="col-12 form-control min1110" style="min-height: 100px;"></textarea>
                    </div>
                </div>
                <div class=" form-row" style="width: 1000px;">
                    <div class="col-12 form-inline my-2 justify-content-md-between">
                        <label class="my-2 mx-2">生態保育原則<span class="small-red">&nbsp;*</span></label>
                        <div class="col-12 table-responsive">
                            <table class="table table-responsive-md table-hover">
                                <tbody>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-01規劃設計階段生態背景資料表</strong></td>
                                        <td style="width: 400px;">{{engReport.D01FileName}}</td>
                                        <td style="display: flex; width: 140px;">
                                            <button v-if="engReport.D01FileName!=''" v-on:click.stop="download(engReport.Seq,'D1')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-02規劃設計階段現場勘查紀錄表</strong></td>
                                        <td>{{engReport.D02FileName}}</td>
                                        <td style="display: flex;">
                                            <button v-if="engReport.D02FileName!=''" v-on:click.stop="download(engReport.Seq,'D2')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-03規劃設計階段民眾參與紀錄表</strong></td>
                                        <td>{{engReport.D03FileName}}</td>
                                        <td style="display: flex;">
                                            <button v-if="engReport.D03FileName!=''" v-on:click.stop="download(engReport.Seq,'D3')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-04規劃設計階段生態保育原則研擬紀錄表</strong></td>
                                        <td>{{engReport.D04FileName}}</td>
                                        <td style="display: flex;">
                                            <button v-if="engReport.D04FileName!=''" v-on:click.stop="download(engReport.Seq,'D4')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-05規劃設計階段生態檢核作業事項確認表</strong></td>
                                        <td>{{engReport.D05FileName}}</td>
                                        <td style="display: flex;">
                                            <button v-if="engReport.D05FileName!=''" v-on:click.stop="download(engReport.Seq,'D5')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-left" style="text-align: left !important;"><strong>D-06工程範圍生物名錄</strong></td>
                                        <td>{{engReport.D06FileName}}</td>
                                        <td style="display: flex;">
                                            <button v-if="engReport.D06FileName!=''" v-on:click.stop="download(engReport.Seq,'D6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="col-12 col-md-6 form-inline my-2 justify-content-md-between">
                        <div class="my-2 mx-2 form-inline">
                            需求碳排量(頓)<span class="small-red">&nbsp;*</span>
                            <a href="#" title="碳排連結" data-toggle="modal" data-target="#refSampleModal" class="a-blue underl" role="button">碳排連結</a>
                            <p style="color: red; padding-top: 20px;">參考碳排量=核定經費(元)*回歸曲線0.38*物調(82.156/前一年的平均新指數)</p>
                        </div>
                        <input v-bind:disabled="true" v-model="engReport.DemandCarbonEmissions" type="text" class="col-12 form-control" value="OO">
                        <textarea v-bind:disabled="true" v-model="engReport.DemandCarbonEmissionsMemo" class="col-12 form-control min1110"
                                  style="min-height: 100px;" placeholder="說明"></textarea>
                    </div>
                </div>
            </div>
            <h5>概估經費</h5>
            <div class="table-responsive">
                <table class="table table1" id="addnew1010">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th style="width: 50px;"><strong>年度</strong></th>
                            <th><strong>項目</strong></th>
                            <th style="width: 250px;"><strong>金額(元)</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in itemsA" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td><div v-if="!item.edit">{{item.Year}}</div></td>
                            <td><div v-if="!item.edit">{{item.AttributesName}}</div></td>
                            <td><div v-if="!item.edit">{{item.Price}}</div></td>
                        </tr>
                        <tr v-if="itemsA==null||itemsA.length==0">
                            <td colspan="4" class="text-center">--無資料--</td>
                        </tr>
                    </tbody>
                </table>
                <label style="padding-top: 10px;">合計：元</label>
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
                            <th style="text-align: center; width: 200px;"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in itemsB" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td><div v-if="!item.edit">{{item.DateStr}}</div></td>
                            <td><div v-if="!item.edit">{{item.FileNumber}}</div></td>
                            <td><div v-if="!item.edit">{{item.FileName}}</div></td>
                            <td style="min-width: 105px;">
                                <button v-if="!item.edit" @click="downloadLC(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                            </td>
                        </tr>
                        <tr v-if="itemsB==null||itemsB.length==0">
                            <td colspan="5" class="text-center">--無資料--</td>
                        </tr>
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
                            <th style="text-align: center; width: 200px;"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in itemsC" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td><div v-if="!item.edit">{{item.DateStr}}</div></td>
                            <td><div v-if="!item.edit">{{item.FileNumber}}</div></td>
                            <td><div v-if="!item.edit">{{item.FileName}}</div></td>
                            <td style="min-width: 105px;">
                                <button v-if="!item.edit" @click="downloadSC(item)" class="btn btn-color11-1 btn-xs sharp mx-1" title="下載"><i class="fas fa-download"></i>下載</button>
                            </td>
                        </tr>
                        <tr v-if="itemsB==null||itemsB.length==0">
                            <td colspan="5" class="text-center">--無資料--</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <h5>主要工作內容</h5>
            <div class="table-responsive">
                <table class="table table1" id="addnew002">
                    <thead class="insearch">
                        <tr>
                            <th style="width: 50px;"><strong>項次</strong></th>
                            <th><strong>提報工作內容</strong></th>
                            <th style="width:110px;"><strong>數量</strong></th>
                            <th style="width:140px;"><strong>經費(元)</strong></th>
                            <th style="width:200px;"><strong>備註</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in itemsD" v-bind:key="item.Seq">
                            <td>{{index+1}}</td>
                            <td><div v-if="!item.edit">{{item.RptJobDescriptionName}}&nbsp;{{item.OtherJobDescription}}</div></td>
                            <td><div v-if="!item.edit">{{item.Num}}</div></td>
                            <td><div v-if="!item.edit">{{item.Cost}}</div></td>
                            <td><div v-if="!item.edit">{{item.Memo}}</div></td>
                        </tr>
                        <tr v-if="itemsD==null||itemsD.length==0">
                            <td colspan="5" class="text-center">--查無資料--</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <h5>附件上傳</h5>
            <a href="#" title="說明" target="_blank" data-toggle="modal" data-target="#prepare_edit01">說明&nbsp;</a>
            <p style="color: red; padding-top: 20px;">*請上傳 jpg、png、pdf格式</p>
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
                            <th style="width: 140px;"><strong>功能</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>位置圖(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.LocationMapFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.LocationMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A1')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>空拍照(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.AerialPhotographyFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.AerialPhotographyFileName!=''" v-on:click.stop="download(engReport.Seq,'A2')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>現場照片</strong></td>
                            <td>{{engReport.ScenePhotoFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.ScenePhotoFileName!=''" v-on:click.stop="download(engReport.Seq,'A3')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>基地地藉圖(建議比例尺大於1/5000)</strong></td>
                            <td>{{engReport.BaseMapFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.BaseMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A4')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>工程平面配置圖(建議比例尺大於1/1000)</strong></td>
                            <td>{{engReport.EngPlaneLayoutFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.EngPlaneLayoutFileName!=''" v-on:click.stop="download(engReport.Seq,'A5')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>縱斷面圖</strong></td>
                            <td>{{engReport.LongitudinalSectionFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.LongitudinalSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-left" style="text-align: left !important;"><strong>標準斷面圖 (建議比例尺水平大於1/1000、垂直大於1/500)</strong></td>
                            <td>{{engReport.StandardSectionFileName}}</td>
                            <td style="display: flex;">
                                <button v-if="engReport.StandardSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <h5 v-if="engReport.IsProposalReview==1">審核意見：{{engReport.ResolutionAuditOpinionName}}</h5>
        <div class="row justify-content-center mt-5">
            <div class="d-flex">
                <button v-on:click.stop="back()" role="button" class="btn btn-color9-1 btn-xs mx-1"> 回上頁</button>
            </div>
        </div>
    </div>
</template>
<script>
import {ItemSeqMapStore} from "../../store/ItemSeqMapStore.js";
import { ref } from "vue";
    export default {
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
                editPR:0,
            };
        },
        methods: {
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
                        if (id == 1)
                            this.itemsA = resp.data.items;
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
            fileChangeC(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;

                this.filesC = new FormData();
                this.filesC.append("file", files[0], files[0].name);
                this.newItemC.FileName = files[0].name;
                //this.onAddRecordC();
            },
            back() {
                window.history.go(-1);
                //window.location = "/ERProposalReview/Index";
            },
            onReviewTypeChange(event) {
                if (this.engReport.ProposalReviewTypeSeq == 1)
                {
                    this.editRiver = 1;
                    this.editDrain = 0;
                    this.editCoastal = 0;
                }
                else if (this.engReport.ProposalReviewTypeSeq == 2)
                {
                    this.editRiver = 0;
                    this.editDrain = 1;
                    this.editCoastal = 0;
                }
                else if (this.engReport.ProposalReviewTypeSeq > 2) {
                    this.editRiver = 1;
                    this.editDrain = 1;
                    this.editCoastal = 1;
                }
                else
                {
                    this.editRiver = 0;
                    this.editDrain = 0;
                    this.editCoastal = 0;
                }
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
        },
        async mounted() {
            console.log('mounted() 提案審查 ' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                if (this.selectProposalReviewTypeOptions.length == 0) this.getProposalReviewTypeOption();
                if (this.selectProposalReviewAttributesOptions.length == 0) this.getProposalReviewAttributesOption();
                if (this.selectDrainOptions.length == 0) this.getDrainOption();
                if (this.selectReportJobDescriptionOptions.length == 0) this.getReportJobDescriptionOption();
                if (this.cities.length == 0) this.getCities();
                this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
                this.isEQCAdmin = localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
                console.log(this.isAdmin);
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
            window.history.back(); //.location = "/FrontDesk";
        },
        setup()
        {
            const ItemSeqMap =  ref(ItemSeqMapStore.getMap("Users/GetSelfUnitList"));
            return {
                ItemSeqMap
            }
        }
    }
</script>
