<template>
    <div>
        <h5 class="insearch mt-0 py-2"> 年度：{{engReport.RptYear}} &nbsp;&nbsp;&nbsp;&nbsp;案由：{{engReport.RptName}}</h5>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <tbody>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>年度</th>
                        <td colspan="5">
                            <input v-model.trim="engReport.RptYear" maxlength="3" type="text" class="form-control" v-bind:disabled="disabledG == 1">
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center" v-if="isAdmin" >
                                <button v-if="disabledG == 1" @click="checkState(6)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-else v-on:click.stop=" onSave('RptYear',0)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editG==1" @click="disabledG=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>案由</th>
                        <td colspan="5">
                            <input v-model.trim="engReport.RptName" maxlength="30" type="text" class="form-control" v-bind:disabled="disabledF == 1">
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center">
                                <button v-if="editF==0&&engReport.IsEditF==1" @click="checkState(5)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-if="editF==1" v-on:click.stop="onSave('RptYearRptName',5)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editF==1" @click="editF=0;disabledF=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <a href="/Content/images/工程提案命名原則.jpg" title="命名規則" target="_blank">命名規則</a>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <tbody>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>緣起及範圍<br><br><font color="blue">(提案課室)</font></th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.OriginAndScope" rows="7" class="form-control" v-bind:disabled="disabledA == 1" placeholder="1.緣起：請敘明待解決問題或預期達到目的，以呈現提案必要性及需求性。2.範圍：○○溪○○堤段"></textarea>
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center">
                                <button v-if="editA==0&&engReport.IsEditA==1" @click="checkState(1)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-if="editA==1" v-on:click.stop="onSave('OriginAndScope',1)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editA==1" @click="editA=0;disabledA=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                            {{engReport.OriginAndScopeUserName}}<br>{{engReport.OriginAndScopeTWDT}}
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>相關報告成果</th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.RelatedReportResults" rows="7" class="form-control" v-bind:disabled="disabledB == 1" placeholder="1.風險評估□否：□是：高、中、低風險對策：低水保護配合河道整理2.防汛熱點□否：□是：3.淹水潛勢□否：□是：4.易致災河段□否：□是：106年0601豪雨造成基礎淘空5.□治理計畫□規劃(檢討)報告：(1)公告時間：75年10月八掌溪治理基本計畫(2)保護標準：50年防洪頻率(3)計畫流量：3600cms(4)計畫洪水位：(5)計畫堤頂高：(6)計畫流速：(7)待建設施：□堤防、□護岸、□保護工、□其他：6.其他成果報告：(請摘錄與本計畫相關)"></textarea>
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center">
                                <button v-if="editB==0&&engReport.IsEditB==1" @click="checkState(2)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-if="editB==1" v-on:click.stop="onSave('RelatedReportResults',1)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editB==1" @click="editB=0;disabledB=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                            {{engReport.RelatedReportResultsUserName}}<br>{{engReport.RelatedReportResultsTWDT}}<br>

                            <div class="custom-control custom-radio custom-control-inline" style="display: flex;">
                                <input v-bind:disabled="disabledB==1" v-model="engReport.RelatedReportResultsReviewState" v-bind:value="false" type="radio" name="YNB" id="YNB01" class="custom-control-input">
                                <label for="YNB01" class="custom-control-label">免覆核</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-bind:disabled="disabledB==1" v-model="engReport.RelatedReportResultsReviewState" v-bind:value="true" type="radio" checked name="YNB" id="YNB02" class="custom-control-input">
                                <label for="YNB02" class="custom-control-label">請「<font color="blue">規劃科</font>」覆核</label>
                            </div>
                            <input class="form-control" :disabled="true" v-if="disabledB==1||engReport.RelatedReportResultsReviewState == false" :value="engReport.RelatedReportResultsAssignReviewUserName"/>
                            <select v-else  v-model="engReport.RelatedReportResultsAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in selectReviewUserBOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>


                            {{engReport.RelatedReportResultsUpdateReviewUserName}}<br>{{engReport.RelatedReportResultsReviewTWDT}}
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>既有設施維護管理情形</th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.FacilityManagement" rows="7" class="form-control" v-bind:disabled="disabledC == 1" placeholder="1.歷年水利建造物檢查□定期檢查：□正常：□改善情形：□不定期檢查：106年0601豪雨造成基礎淘空□正常：□改善情形：2.涉河道高灘地管理：□否：□是：現況高灘地影響河道流路，工程設計時應納入考量3.其他："></textarea>
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center">
                                <button v-if="editC==0&&engReport.IsEditC==1" @click="checkState(3)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-if="editC==1" v-on:click.stop="onSave('FacilityManagement',1)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editC==1" @click="editC=0;disabledC=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                            {{engReport.FacilityManagementUserName}}<br>{{engReport.FacilityManagementTWDT}}<br>

                            <div class="custom-control custom-radio custom-control-inline" style="display: flex;">
                                <input v-bind:disabled="disabledC==1" v-model="engReport.FacilityManagementReviewState" v-bind:value="false" type="radio" name="YNC" id="YNC01" class="custom-control-input">
                                <label for="YNC01" class="custom-control-label">免覆核</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-bind:disabled="disabledC==1" v-model="engReport.FacilityManagementReviewState" v-bind:value="true" type="radio" checked name="YNC" id="YNC02" class="custom-control-input">
                                <label for="YNC02" class="custom-control-label">請「<font color="blue">管理科</font>」覆核</label>
                            </div>
                            <input class="form-control" :disabled="true" v-if="disabledC==1||engReport.FacilityManagementReviewState == false" :value="engReport.FacilityManagementAssignReviewUserName"/>
                            <select v-else v-model="engReport.FacilityManagementAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in selectReviewUserCOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>



                            <!--<b-checkbox v-bind:disabled="disabledC == 1" v-model="engReport.FacilityManagementReviewState" @change="checkReviewStateC=!checkReviewStateC">建議<font color="blue">管理科</font>覆核</b-checkbox>
                        <select v-bind:disabled="disabledC == 1" v-model="engReport.FacilityManagementAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in selectReviewUserCOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>-->
                            

                            {{engReport.FacilityManagementUpdateReviewUserName}}<br>{{engReport.FacilityManagementReviewTWDT}}
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>提案範圍用地概述</th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.ProposalScopeLand" rows="7" class="form-control" v-bind:disabled="disabledD == 1" placeholder="1.是否涉及都市計畫變更□否：□是：預計何時完成都變2.是否已將計畫範圍內公私有地情形，工務科□否：原因□是：3.其他："></textarea>
                        </td>
                        <td style="width: 200px;">
                            <div class=" justify-content-center">
                                <button v-if="editD==0&&engReport.IsEditD==1" @click="checkState(4)" class="btn btn-color11-3 btn-xs sharp mx-1" title="編輯"><i class="fas fa-pencil-alt"></i> 編輯</button>
                                <button v-if="editD==1" v-on:click.stop="onSave('ProposalScopeLand',1)" class="btn btn-color11-2 btn-xs sharp mx-1" title="儲存"><i class="fas fa-save"></i> 儲存</button>
                                <button v-if="editD==1" @click="editD=0;disabledD=1;" class="btn btn-color11-4 btn-xs mx-1" title="取消"><i class="fas fa-times"></i></button>
                            </div>
                            {{engReport.ProposalScopeLandUserName}}<br>{{engReport.ProposalScopeLandTWDT}}<br>

                            <div class="custom-control custom-radio custom-control-inline" style="display: flex;">
                                <input v-bind:disabled="disabledD==1" v-model="engReport.ProposalScopeLandReviewState" v-bind:value="false" type="radio" name="YND" id="YND01" class="custom-control-input">
                                <label for="YND01" class="custom-control-label">免覆核</label>
                            </div>
                            <div class="custom-control custom-radio custom-control-inline">
                                <input v-bind:disabled="disabledD==1" v-model="engReport.ProposalScopeLandReviewState" v-bind:value="true" type="radio" checked name="YND" id="YND02" class="custom-control-input">
                                <label for="YND02" class="custom-control-label">請「<font color="blue">資產科</font>」覆核</label>
                            </div>
                            <input class="form-control" :disabled="true" v-if="disabledD==1||engReport.ProposalScopeLandReviewState == false" :value="engReport.ProposalScopeLandAssignReviewUserName"/>
                            <select v-else  v-model="engReport.ProposalScopeLandAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in selectReviewUserDOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>



                            <!--<b-checkbox v-bind:disabled="disabledD == 1&&engReport.IsEditA==0" v-model="engReport.ProposalScopeLandReviewState" @change="checkReviewStateD=!checkReviewStateD">建議<font color="blue">資產科</font>覆核</b-checkbox>
                        <select v-bind:disabled="disabledD == 1" v-model="engReport.ProposalScopeLandAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                            <option v-for="option in selectReviewUserDOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>-->
                            

                            {{engReport.ProposalScopeLandUpdateReviewUserName}}<br>{{engReport.ProposalScopeLandReviewTWDT}}

                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>附件上傳</h5>
        <a href="#" title="說明" target="_blank" data-toggle="modal" data-target="#prepare_edit01" >說明&nbsp;</a>
        <p style="color: red; padding-top: 20px;" >*請上傳 jpg、png 格式</p>
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
        <div class="table-responsive" >
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
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A1')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.LocationMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A1')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.LocationMapFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A1')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>空拍照(建議比例尺大於1/5000)</strong></td>
                        <td>{{engReport.AerialPhotographyFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A2')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.AerialPhotographyFileName!=''" v-on:click.stop="download(engReport.Seq,'A2')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.AerialPhotographyFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A2')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><span class="small-red">&nbsp;*</span><strong>現場照片</strong></td>
                        <td>{{engReport.ScenePhotoFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A3')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.ScenePhotoFileName!=''" v-on:click.stop="download(engReport.Seq,'A3')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.ScenePhotoFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A3')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><strong>基地地藉圖(建議比例尺大於1/5000)</strong></td>
                        <td>{{engReport.BaseMapFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3"  >
                                <input v-on:change="fileChange($event,'A4')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.BaseMapFileName!=''" v-on:click.stop="download(engReport.Seq,'A4')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.BaseMapFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A4')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><strong>工程平面配置圖(建議比例尺大於1/1000)</strong></td>
                        <td>{{engReport.EngPlaneLayoutFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A5')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.EngPlaneLayoutFileName!=''" v-on:click.stop="download(engReport.Seq,'A5')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.EngPlaneLayoutFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A5')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><strong>縱斷面圖</strong></td>
                        <td>{{engReport.LongitudinalSectionFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A6')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.LongitudinalSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A6')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.LongitudinalSectionFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A6')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-left" style="text-align: left !important;"><strong>標準斷面圖 (建議比例尺水平大於1/1000、垂直大於1/500)</strong></td>
                        <td>{{engReport.StandardSectionFileName}}</td>
                        <td style="display: flex;">
                            <label v-if="engReport.IsEditFile==1 " class="btn btn-shadow btn-color11-3">
                                <input v-on:change="fileChange($event,'A7')" id="inputFile" type="file" name="file" multiple="" style="display:none;"><i class="fas fa-upload"></i>
                            </label>
                            <button v-if="engReport.StandardSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A7')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                            <button v-if="engReport.IsEditFile==1&&engReport.StandardSectionFileName!=''" v-on:click.stop="delAttachment(engReport.Seq,'A7')" role="button" class="btn btn-color11-4 btn-x sharp mx-1"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <h5>簽核流程</h5>
        <div class="table-responsive " >
            <table border="0" class="table table1 min910">
                <thead class="insearch">
                    <tr>
                        <th>簽核流程</th>
                        <th>簽辦</th>
                        <th>簽核人</th>
                        <th>簽核日期</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in approves" v-bind:key="item.Seq">
                        <td>{{item.ApprovalWorkFlow}}</td>
                        <td>{{item.ApproverName}}</td>
                        <td>
                            <button v-if="isAdmin&&item.ApproveUser==null" v-on:click.stop="onSavaApprovalOne(item.Seq)" role="button" class="btn btn-color11-2 btn-xs mx-1">
                                <i class="fas fa-save">簽核(數位簽章)</i>
                            </button>
                            {{item.ApproveUser}}
                        </td>
                        <td>{{item.ApproveTimeStr}}</td>
                    </tr>
                    <tr v-if="approves==null||approves.length==0">
                        <td colspan="4" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!--<canvas v-show="engReportApprove.Seq>0" ref="mycanvas" id="mycanvas" style="background: rgb(238, 238, 238) none repeat scroll 0% 0%; width: 400px; height: 200px;" width="400" height="200"></canvas>
    <div v-if="engReportApprove.Seq>0">
        <button @click="onConvertToImageClick" id="convertToImage" class="btn btn-color11-3 btn-xs mx-1">轉圖</button>
        <button id="clear" class="btn btn-color9-1 btn-xs mx-1">清除</button>
    </div>
    <div v-show="engReportApprove.Signature.length>0" id="image" style="display: block; width: 400px; height: 200px; max-width: 400px; max-height: 200px; ">
        <img src="">
    </div>-->
        <div class="row justify-content-center mt-5">
            <div class="d-flex">
                <button v-if="engReport.IsSavaApproval==true && ( engReportApprove.ApprovalModuleListSeq == 1|| engReportApprove.Approved)" v-on:click.stop="onSavaApproval()" role="button" class="btn btn-color11-2 btn-xs mx-1">
                    <i class="fas fa-save">&nbsp;{{engReportApprove.ApprovalModuleListSeq==1?'送簽':'簽核(數位簽章)'}}</i>
                </button>
                
            </div>
            <div class="d-flex">
                <button v-on:click.stop="back()" role="button" class="btn btn-color9-1 btn-xs mx-1"> 回上頁</button>
                <button @click.prevent="onDownloadNeedAssessment" class="btn btn-color11-1 btn-block ml-4">
                        <i class="fas fa-download"></i>產製提案需求評估表
                    </button>
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
                targetId: null,
                file: null,//{ name: null, size: null },
                files: new FormData(),

                //使用者單位資訊
                isAdmin: false,
                userUnit: null,
                userUnitName: '',
                userUnitSub: null,
                userUnitSubName: '',

                disabledA: 1,
                disabledB: 1,
                disabledC: 1,
                disabledD: 1,
                disabledF: 1,
                disabledG: 1,

                editA: 0,
                editB: 0,
                editC: 0,
                editD: 0,
                editF: 0,
                engReport: { LocationMapFileName: '', AerialPhotographyFileName: '', ScenePhotoFileName: '', BaseMapFileName: '', EngPlaneLayoutFileName: '', LongitudinalSectionFileName: '', StandardSectionFileName: ''},
                approves: [],
                engReportApprove: { Signature: '', Seq: -1 },

                selectReviewUserBOptions: [],//規劃科覆核人員
                selectReviewUserCOptions: [],//管理科覆核人員
                selectReviewUserDOptions: [],//資產科覆核人員

                //checkReviewStateB: 0,
                checkReviewStateC: 0,
                checkReviewStateD: 0,
            };
        },
        methods: {
                        //下載
            onDownloadNeedAssessment() {
                window.comm.dnFile('/EngReport/DownloadNeedAssessmentVF?year=' + this.engReport.RptYear + '&unit=' + this.engReport.ExecUnitSeq + '&subUnit=' + '-1' + '&rptType=' + '1' + '&seq=' + this.engReport.Seq);
            },
            checkState(itemNo) {
                if (itemNo == 1) {
                    this.disabledA = (this.disabledA + 1) % 2;
                    this.editA = 1;
                    if (this.strEmpty(this.engReport.OriginAndScope))
                        this.engReport.OriginAndScope = "1.緣起：請敘明待解決問題或預期達到目的，以呈現提案必要性及需求性。2.範圍：○○溪○○堤段";
                }
                else if (itemNo == 2) {
                    this.disabledB = (this.disabledB + 1) % 2;
                    this.editB = 1;
                    if (this.strEmpty(this.engReport.RelatedReportResults))
                        this.engReport.RelatedReportResults = "1.風險評估□否：□是：高、中、低風險對策：低水保護配合河道整理2.防汛熱點□否：□是：3.淹水潛勢□否：□是：4.易致災河段□否：□是：106年0601豪雨造成基礎淘空5.□治理計畫□規劃(檢討)報告：(1)公告時間：75年10月八掌溪治理基本計畫(2)保護標準：50年防洪頻率(3)計畫流量：3600cms(4)計畫洪水位：(5)計畫堤頂高：(6)計畫流速：(7)待建設施：□堤防、□護岸、□保護工、□其他：6.其他成果報告：(請摘錄與本計畫相關)";
                }
                else if (itemNo == 3) {
                    this.disabledC = (this.disabledC + 1) % 2;
                    this.editC = 1;
                    if (this.strEmpty(this.engReport.FacilityManagement))
                        this.engReport.FacilityManagement = "1.歷年水利建造物檢查□定期檢查：□正常：□改善情形：□不定期檢查：106年0601豪雨造成基礎淘空□正常：□改善情形：2.涉河道高灘地管理：□否：□是：現況高灘地影響河道流路，工程設計時應納入考量3.其他：";
                }
                else if (itemNo == 4) {
                    this.disabledD = (this.disabledD + 1) % 2;
                    this.editD = 1;
                    if (this.strEmpty(this.engReport.ProposalScopeLand))
                        this.engReport.ProposalScopeLand = "1.是否涉及都市計畫變更□否：□是：預計何時完成都變2.是否已將計畫範圍內公私有地情形，提供工務課□否：原因□是：3.其他：";
                }
                else if (itemNo == 5) {
                    this.disabledF = (this.disabledF + 1) % 2;
                    this.editF = 1;
                    if (this.strEmpty(this.engReport.RptYear))
                        this.engReport.RptYear = "";
                    if (this.strEmpty(this.engReport.RptName))
                        this.engReport.RptName = "";
                }
                else if (itemNo == 6){
                    this.disabledG = 0; 
                }
            },
            getItem() {
                this.engReport = {};
                window.myAjax.post('/ERNeedAssessment/GetEngReport', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReport = resp.data.item;
                            //this.checkReviewStateB = this.engReport.RelatedReportResultsReviewState;
                            //this.checkReviewStateC = this.engReport.FacilityManagementReviewState;
                            //this.checkReviewStateD = this.engReport.ProposalScopeLandReviewState;
                            //this.setCanvas();
                            this.getApprove(resp.data.item)
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getApproveList() {
                this.engReport = {};
                window.myAjax.post('/ERNeedAssessment/GetEngReportApproveList', { engReportSeq: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.approves = resp.data.items;
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getApprove(engReport = {}) {
                this.engReport = engReport;
                window.myAjax.post('/ERNeedAssessment/GetEngReportApprove', { engReportSeq: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReportApprove = resp.data.item;
                            this.engReportApprove.Approved  = resp.data.Approved;
                            //if (document.getElementById('image') != null)
                            //    document.getElementById('image').innerHTML = "<img src='" + this.engReportApprove.Signature + "' style='width: 400px; height: 200px;'/>";
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存
            onSave(str, isSend = 0) {
                let NAType = 0;

                if (str == 'OriginAndScope') { NAType = 1; }
                if (str == 'RelatedReportResults') { NAType = 2;  }
                if (str == 'FacilityManagement') { NAType = 3; 
                    // this.engReport.FacilityManagementReviewState = this.checkReviewStateC; 
                }
                if (str == 'ProposalScopeLand') { NAType = 4; 
                    // this.engReport.ProposalScopeLandReviewState = this.checkReviewStateD; 
                }
                if (str == 'RptYearRptName') { NAType = 5; }
                if (str == 'RptYear') { NAType = 6; }
                window.myAjax.post('/ERNeedAssessment/UpdateEngReportForNA', { m: this.engReport, naType: NAType })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            //alert(resp.data.msg);
                            this.editA = 0;
                            this.editB = 0;
                            this.editC = 0;
                            this.editD = 0;
                            this.editF = 0;
                            this.disabledA = 1;
                            this.disabledB = 1;
                            this.disabledC = 1;
                            this.disabledD = 1;
                            this.disabledF = 1;
                            this.disabledG = 1;
                            //this.editSeq = -99;
                            this.getItem();
                            //if (uItem.Seq == -1) this.onNewRecord();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            download(id,fileNo) {
                window.myAjax.get('/ERNeedAssessment/Download?id=' + id + '&fileNo=' + fileNo, { responseType: 'blob' })
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
            //上傳-檔案異動事件
            fileChange(event,type) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                var uploadfiles = new FormData();
                uploadfiles.append("file", files[0], files[0].name);
                uploadfiles.append("Seq", this.engReport.Seq);
                uploadfiles.append("fileType", type);
                this.upload(uploadfiles);
            },
            //上傳
            upload(uploadfiles) {
                window.myAjax.post('/ERNeedAssessment/UploadAttachment', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.getItem();
                        }
                        //alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //刪除 附件
            delAttachment(id, fileNo) {
                window.myAjax.post('/ERNeedAssessment/DelAttachment', { Seq: id, fileNo: fileNo })
                    .then(resp => {
                        this.getItem();
                        alert(resp.data.message);
                        console.log(resp);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存送簽或簽核
            onSavaApproval() {


                if ( (
                    this.strEmpty(this.engReport.OriginAndScope) || this.strEmpty(this.engReport.RelatedReportResults)
                    || this.strEmpty(this.engReport.FacilityManagement) || this.strEmpty(this.engReport.ProposalScopeLand)
                    || this.strEmpty(this.engReport.LocationMapFileName)
                    || this.strEmpty(this.engReport.AerialPhotographyFileName)
                    || this.strEmpty(this.engReport.ScenePhotoFileName)
                    ) 
                    //|| this.strEmpty(this.engReport.BaseMapFileName)
                    //|| this.strEmpty(this.engReport.EngPlaneLayoutFileName)
                    //|| this.strEmpty(this.engReport.LongitudinalSectionFileName)
                    //|| this.strEmpty(this.engReport.StandardSectionFileName)
                    )
                {
                    alert('此頁有標注*的資料皆為必填!!');
                    return;
                }
                else if(
                    (!this.engReport.RelatedReportResultsReviewTime  && this.engReport.RelatedReportResultsReviewState ) ||
                    (!this.engReport.FacilityManagementReviewTime  && this.engReport.FacilityManagementReviewState) ||
                    (!this.engReport.ProposalScopeLandReviewTime && this.engReport.ProposalScopeLandReviewState) 
                )
                {
                    alert('需要先覆核');
                    return ;
                }
                window.myAjax.post('/ERNeedAssessment/UpdateEngReportForNAApproval', { m: this.engReport, era: this.engReportApprove })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getApproveList();
                            this.getApprove();
                            this.getItem();
                            alert(resp.data.msg);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //儲存送簽或簽核
            onSavaApprovalOne(eraSeq) {
                if (this.strEmpty(this.engReport.OriginAndScope) || this.strEmpty(this.engReport.RelatedReportResults)
                    || this.strEmpty(this.engReport.FacilityManagement) || this.strEmpty(this.engReport.ProposalScopeLand)
                    || this.strEmpty(this.engReport.LocationMapFileName)
                    || this.strEmpty(this.engReport.AerialPhotographyFileName)
                    || this.strEmpty(this.engReport.ScenePhotoFileName)
                    //|| this.strEmpty(this.engReport.BaseMapFileName)
                    //|| this.strEmpty(this.engReport.EngPlaneLayoutFileName)
                    //|| this.strEmpty(this.engReport.LongitudinalSectionFileName)
                    //|| this.strEmpty(this.engReport.StandardSectionFileName)
                ) {
                    alert('此頁有標注*的資料皆為必填!!');
                    return;
                }
                this.engReportApprove.Seq = eraSeq;
                window.myAjax.post('/ERNeedAssessment/UpdateEngReportForNAApproval', { m: this.engReport, era: this.engReportApprove })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getApproveList();
                            this.getApprove();
                            this.getItem();
                            //alert(resp.data.msg);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            back() {
                //window.history.go(-1);
                window.location = "/ERNeedAssessment/Index";
            },
            //規劃科覆核人員
            async getReviewUserBOption() {
                this.selectReviewUserBOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetReviewUserList', { id: this.targetId, UnitSubName: '規劃科' });
                this.selectReviewUserBOptions = data;
            },
            //管理科覆核人員
            async getReviewUserCOption() {
                this.selectReviewUserCOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetReviewUserList', { id: this.targetId, UnitSubName: '管理科' });
                this.selectReviewUserCOptions = data;
            },
            //資產科覆核人員
            async getReviewUserDOption() {
                this.selectReviewUserDOptions = [];
                const { data } = await window.myAjax.post('/EngReport/GetReviewUserList', { id: this.targetId, UnitSubName: '資產科' });
                this.selectReviewUserDOptions = data;
            },
            //onConvertToImageClick() {
            //    console.log("convertToImage")
            //    var image = canvas.toDataURL("image/png");
            //    this.engReportApprove.Signature = image;
            //    document.getElementById('image').innerHTML = "<img src='" + this.engReportApprove.Signature + "' style='width: 400px; height: 200px;' />";
            //},
            //setCanvas() {
            //    canvas = document.getElementById('mycanvas');
            //    canvas = this.$refs.mycanvas;
            //    ctx = canvas.getContext("2d");
            //    // 抗鋸齒
            //    // ref: https://www.zhihu.com/question/37698502
            //    let width = 400;//canvas.width,
            //    let height = 200;//canvas.height;
            //    if (window.devicePixelRatio) {
            //        canvas.style.width = width + "px";
            //        canvas.style.height = height + "px";
            //        canvas.height = height * window.devicePixelRatio;
            //        canvas.width = width * window.devicePixelRatio;
            //        ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
            //    }
            //    canvas.addEventListener('mousedown', function (evt) {
            //        var mousePos = getMousePos(canvas, evt);
            //        ctx.beginPath();
            //        ctx.moveTo(mousePos.x, mousePos.y);
            //        evt.preventDefault();
            //        canvas.addEventListener('mousemove', mouseMove, false);
            //    });

            //    canvas.addEventListener('mouseup', function () {
            //        canvas.removeEventListener('mousemove', mouseMove, false);
            //    }, false);
            //    canvas.addEventListener('touchstart', function (evt) {
            //        // console.log('touchstart')
            //        // console.log(evt)
            //        var touchPos = getTouchPos(canvas, evt);
            //        ctx.beginPath(touchPos.x, touchPos.y);
            //        ctx.moveTo(touchPos.x, touchPos.y);
            //        evt.preventDefault();
            //        canvas.addEventListener('touchmove', touchMove, false);
            //    });

            //    canvas.addEventListener('touchend', function () {
            //        // console.log("touchend")
            //        canvas.removeEventListener('touchmove', touchMove, false);
            //    }, false);


            //    // clear
            //    document.getElementById('clear').addEventListener('click', function () {
            //        // console.log("reset")
            //        ctx.clearRect(0, 0, canvas.width, canvas.height);
            //    }, false);


            //// convertToImage
            //    /*document.getElementById('convertToImage').addEventListener('click', function () {
            //        console.log("convertToImage")
            //        var image = canvas.toDataURL("image/png");
            //        this.checkTable.Signature = image;
            //        document.getElementById('image').innerHTML = "<img src='" + this.checkTable.Signature + "' alt='from canvas'/>";
            //        //$('#').html("<img src='" + image + "' alt='from canvas'/>");
            //    }, false);*/
            //// 簽名製作
            //}
        },
        async mounted() {
            console.log('mounted() 需求評估-填報 ' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (!urlParams.has('id')) window.history.back();
            this.isAdmin = localStorage.getItem('isAdmin') == 'True'  ||  localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
            this.targetId = parseInt(urlParams.get('id'), 10);
            // console.log(this.targetId);
            if (Number.isInteger(this.targetId)) {
                if (this.selectReviewUserBOptions.length == 0) this.getReviewUserBOption();
                if (this.selectReviewUserCOptions.length == 0) this.getReviewUserCOption();
                if (this.selectReviewUserDOptions.length == 0) this.getReviewUserDOption();
                this.getApproveList();
                this.getApprove();
                this.getItem();
                //this.setCanvas();
                //return;
            }


        },
        setup()
        {
            const ItemSeqMap =  ref(ItemSeqMapStore.getMap("Users/GetSelfUnitList"));
            console.log("ItemSeqMap", ItemSeqMap);
            return {
                ItemSeqMap
            }
        }
    
    }

    //// 簽名製作
    //var canvas = document.getElementById('mycanvas');
    //var ctx;
    //// mouse
    //function getMousePos(canvas, evt) {
    //    var rect = canvas.getBoundingClientRect();
    //    return {
    //        x: evt.clientX - rect.left,
    //        y: evt.clientY - rect.top
    //    };
    //}

    //function mouseMove(evt) {
    //    var mousePos = getMousePos(canvas, evt);
    //    ctx.lineCap = "round";
    //    ctx.lineWidth = 2;
    //    ctx.lineJoin = "round";
    //    ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
    //    ctx.shadowColor = 'black';// 邊緣顏色
    //    ctx.lineTo(mousePos.x, mousePos.y);
    //    ctx.stroke();
    //}
    //// touch
    //function getTouchPos(canvas, evt) {
    //    var rect = canvas.getBoundingClientRect();
    //    return {
    //        x: evt.touches[0].clientX - rect.left,
    //        y: evt.touches[0].clientY - rect.top
    //    };
    //}

    //function touchMove(evt) {
    //    // console.log("touchmove")
    //    var touchPos = getTouchPos(canvas, evt);
    //    // console.log(touchPos.x, touchPos.y)

    //    ctx.lineWidth = 2;
    //    ctx.lineCap = "round"; // 繪制圓形的結束線帽
    //    ctx.lineJoin = "round"; // 兩條線條交匯時，建立圓形邊角
    //    ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
    //    ctx.shadowColor = 'black'; // 邊緣顏色
    //    ctx.lineTo(touchPos.x, touchPos.y);
    //    ctx.stroke();
    //}
</script>