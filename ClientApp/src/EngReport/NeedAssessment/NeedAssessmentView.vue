<template>
    <div>
        <div>
            識別碼: ({{ engReport.Seq }})
        </div>
        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <tbody>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>緣起及範圍<br><br><font color="blue">(工務課)</font></th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.OriginAndScope" rows="7" class="form-control" v-bind:disabled="disabledA == 1" placeholder="1.緣起：請敘明待解決問題或預期達到目的，以呈現提案必要性及需求性。2.範圍：○○溪○○堤段"></textarea>
                        </td>
                        <td style="width: 170px;">
                            {{engReport.OriginAndScopeUserName}}<br>{{engReport.OriginAndScopeTWDT}}
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>相關報告成果</th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.RelatedReportResults" rows="7" class="form-control" v-bind:disabled="disabledB == 1" placeholder="1.風險評估□否：□是：高、中、低風險對策：低水保護配合河道整理2.防汛熱點□否：□是：3.淹水潛勢□否：□是：4.易致災河段□否：□是：106年0601豪雨造成基礎淘空5.□治理計畫□規劃(檢討)報告：(1)公告時間：75年10月八掌溪治理基本計畫(2)保護標準：50年防洪頻率(3)計畫流量：3600cms(4)計畫洪水位：(5)計畫堤頂高：(6)計畫流速：(7)待建設施：□堤防、□護岸、□保護工、□其他：6.其他成果報告：(請摘錄與本計畫相關)"></textarea>
                        </td>
                        <td style="width: 150px;">
                            {{engReport.RelatedReportResultsUserName}}<br>{{engReport.RelatedReportResultsTWDT}}<br>
                            <b-checkbox v-bind:disabled="disabledB == 1" v-model="engReport.RelatedReportResultsReviewState" @change="checkReviewStateB=!checkReviewStateB">建議<font color="blue">規劃科</font>覆核</b-checkbox>
                            <input class="form-control" :disabled="true" v-if="disabledB==1" :value="engReport.RelatedReportResultsAssignReviewUserName"/>
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
                        <td style="width: 150px;">
                            {{engReport.FacilityManagementUserName}}<br>{{engReport.FacilityManagementTWDT}}<br>
                            <b-checkbox v-bind:disabled="disabledC == 1" v-model="engReport.FacilityManagementReviewState" @change="checkReviewStateC=!checkReviewStateC">建議<font color="blue">管理科</font>覆核</b-checkbox>
                            <input class="form-control" :disabled="true" v-if="disabledC==1" :value="engReport.FacilityManagementAssignReviewUserName"/>
                            <select v-else v-model="engReport.FacilityManagementAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in selectReviewUserCOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                            {{engReport.FacilityManagementUpdateReviewUserName}}<br>{{engReport.FacilityManagementReviewTWDT}}
                        </td>
                    </tr>
                    <tr>
                        <th class="insearch" style="width: 200px; text-align:left;"><span class="small-red">*</span>提案範圍用地概述</th>
                        <td colspan="5">
                            <textarea v-model.trim="engReport.ProposalScopeLand" rows="7" class="form-control" v-bind:disabled="disabledD == 1" placeholder="1.是否涉及都市計畫變更□否：□是：預計何時完成都變2.是否已將計畫範圍內公私有地情形，工務科□否：原因□是：3.其他："></textarea>
                        </td>
                        <td style="width: 150px;">
                            {{engReport.ProposalScopeLandUserName}}<br>{{engReport.ProposalScopeLandTWDT}}<br>
                            <b-checkbox v-bind:disabled="disabledD == 1" v-model="engReport.ProposalScopeLandReviewState" @change="checkReviewStateD=!checkReviewStateD">建議<font color="blue">資產科</font>覆核</b-checkbox>
                            <input class="form-control" :disabled="true" v-if="disabledD==1" :value="engReport.ProposalScopeLandAssignReviewUserName"/>
                            <select v-else  v-model="engReport.ProposalScopeLandAssignReviewUserSeq" class="form-control my-1 mr-0 mr-md-4 WidthasInput">
                                <option v-for="option in selectReviewUserDOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                    {{ option.Text }}
                                </option>
                            </select>
                            {{engReport.ProposalScopeLandUpdateReviewUserName}}<br>{{engReport.ProposalScopeLandReviewTWDT}}

                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>附件上傳</h5>
        <a href="#" title="說明" target="_blank" data-toggle="modal" data-target="#prepare_edit01">說明&nbsp;</a>
        <p style="color: red; padding-top: 20px;">*請上傳 jpg、png 格式</p>
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
                            <button v-if="engReport.StandardSectionFileName!=''" v-on:click.stop="download(engReport.Seq,'A7')" role="button" class="btn btn-color11-1 btn-x sharp mx-1"><i class="fas fa-download"></i></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5>簽核流程</h5>
        <div class="table-responsive">
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
                        <td>{{item.ApproveUser}}</td>
                        <td>{{item.ApproveTimeStr}}</td>
                    </tr>
                    <tr v-if="approves==null||approves.length==0">
                        <td colspan="4" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
        </div>
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

                editA: 0,
                editB: 0,
                editC: 0,
                editD: 0,

                engReport: { LocationMapFileName: '', AerialPhotographyFileName: '', ScenePhotoFileName: '', BaseMapFileName: '', EngPlaneLayoutFileName: '', LongitudinalSectionFileName: '', StandardSectionFileName: ''},
                approves: [],
                engReportApprove: { Signature: '', Seq: -1 },

                selectReviewUserBOptions: [],//規劃科覆核人員
                selectReviewUserCOptions: [],//管理科覆核人員
                selectReviewUserDOptions: [],//資產科覆核人員

                checkReviewStateB: 0,
                checkReviewStateC: 0,
                checkReviewStateD: 0,
            };
        },
        methods: {
            getItem() {
                this.engReport = {};
                window.myAjax.post('/ERNeedAssessment/GetEngReport', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReport = resp.data.item;
                            this.checkReviewStateB = this.engReport.RelatedReportResultsReviewState;
                            this.checkReviewStateC = this.engReport.FacilityManagementReviewState;
                            this.checkReviewStateD = this.engReport.ProposalScopeLandReviewState;
                            //this.setCanvas();
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
            getApprove() {
                this.engReport = {};
                window.myAjax.post('/ERNeedAssessment/GetEngReportApprove', { engReportSeq: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engReportApprove = resp.data.item;
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
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
            back() {
                window.history.go(-1);
                //window.location = "/ERNeedAssessment/Index";
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
        },
        async mounted() {
            console.log('mounted() 需求評估-填報 ' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (!urlParams.has('id')) window.history.back();
            this.isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
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
            return {
                ItemSeqMap
            }
        }
    }
</script>