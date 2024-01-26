<template>
    <div>
        <table>
            <tr><td colspan="7">避免網路速度影響，僅提供下載、上傳30天日誌(報表)</td></tr>
            <tr>
                <td style="width: 80px; background: #f2f2f2; text-align:center;">日期範圍</td>
                <td class="pl-2">
                    <input v-model.trim="filterStartDate" type="date" class="form-control">
                </td>
                <td class="p-2">~</td>
                <td>
                    <input v-model.trim="filterEndDate" type="date" class="form-control">
                </td>
                <td>
                    <button @click="downloadMultiDate" type="button" class="btn btn-outline-secondary btn-sm" title="Excel範本下載">下載多日Excel範本&nbsp;<i class="fas fa-download"></i></button>
                </td>
                <td>
                    <label class="btn btn-block btn-outline-secondary btn-sm" style="margin-top: 6px">
                        <input v-on:change="uploadMultiDailyLog($event)" type="file" name="file" multiple="" style="display:none;">
                        上傳多日工作量(excel)&nbsp;<i class="fas fa-upload"></i>
                    </label>
                </td>
                <td>
                    <!-- button @click="downloadMultiDateReport" type="button" class="btn btn-outline-secondary btn-sm" title="施工日報下載">下載多日施工日報<i class="fas fa-download"></i></!button -->
                    <button @click="dnClickEvent = 1" data-target="#refExportModal" title="下載多日施工日報" class="btn btn-outline-secondary btn-sm"
                              role="button" data-toggle="modal" >下載多日施工日報<i class="fas fa-download"></i>
                    </button>
                </td>
            </tr>
        </table>
        <CalendarSetting ref="calendar" v-bind:tenderItem="tenderItem" v-bind:attrs="attrs"
                         v-on:onDayClickEvent="onCalDayClick"
                         v-on:onFromPageEvent="onCalFromPage"
                         v-on:onDataChangeEvent="onCalDataChange"></CalendarSetting>
        <h5>日誌日期:{{selectDay}}</h5>
        <div>
            <ul v-show="supDailyItem != null" class="nav nav-tabs" role="tablist">
                <li class="nav-item">
                    <a v-on:click="selectTab=''" id="tabMenu01" class="nav-link active" data-toggle="tab" href="#menu01">一、依施工計畫執行按圖施工概況</a>
                </li>
                <li v-show="supDailyItem != null && supDailyItem.Seq != -1" class="nav-item">
                    <a v-on:click="selectTab='menu02'" id="tabMenu02" class="nav-link" data-toggle="tab" href="#menu02">二、其它</a>
                </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
                <!-- 一 -->
                <div id="menu01" class="tab-pane">
                    <h5>一、依施工計畫執行按圖施工概況(含約定之重要施工項目及完成數量等)</h5>
                    <div class="form-row" role="toolbar">
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="download" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm" title="檔案下載">下載Excel範本&nbsp;<i class="fas fa-download"></i></button>
                        </div>
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <label class="btn btn-block btn-outline-secondary btn-sm" v-bind:class="{ 'disabled' : isDisabledAndCompleted}">
                                <input v-on:change="uploadMultiDailyLog($event)" v-bind:disabled="isDisabledAndCompleted" type="file" name="file" multiple="" style="display:none;">
                                上傳本日工作量(excel)&nbsp;<i class="fas fa-upload"></i>
                            </label>
                        </div>
                        
                        <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                            <button @click="fillCompleted" v-bind:disabled="isDisabledAndCompleted" type="button" class="btn btn-outline-secondary btn-sm" title="填報完成">填報完成 &nbsp;<i class="fas fa-check"></i></button>
                        </div>

                        <template v-if="supDailyItem != null && supDailyItem.Seq == -1">
                            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                                <button @click="saveRainyDay" type="button" class="btn btn-shadow btn-color11-4 btn-sm" title="雨天">雨天 &nbsp;<i class="fas fa-check"></i></button>
                            </div>
                            <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                                <button @click="saveHolidays" type="button" class="btn btn-shadow btn-color11-4 btn-sm" title="例假日">例假日 &nbsp;<i class="fas fa-check"></i></button>
                            </div>
                        </template>
                    </div>
                    <div class="row justify-content-between">
                        <div class="form-inline col-12 col-md-8 small">
                            <label class="my-1 mr-2">
                                <span class="text-R">【累計完成數量】為當月至當日之施工完成數量</span>
                            </label>
                        </div>
                        <div class="form-inline col-12 col-md-8 small">
                            <label class="my-1 mr-2">
                                <span class="text-R">【總累計完成數量】為本工程所有工期總完成數量，填寫本日完成數量時請注意總累計完成數量不可大於[契約數量]</span>
                            </label>
                        </div>
                    </div>
                    <comm-pagination ref="pagination" :recordTotal="planItems.length" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
                    <div class="table-responsive">
                        <table class="table table-responsive-md table-hover">
                            <thead class="insearch">
                                <tr>
                                    <th><strong>序號</strong>
                                        <button type="button" title="排序" @click="sortItem" class="btn btn-shadow btn-color11-4 btn-sm"><i class="fas fa-sort"></i></button>
                                    </th>
                                    <th><strong>項次</strong></th>
                                    <th><strong>施工項目</strong></th>
                                    <th style="width: 50px;" class="text-right"><strong>單位</strong></th>
                                    <th class="text-right"><strong>單價</strong></th>
                                    <th style="width: 50px;" class="text-right"><strong>契約數量</strong></th>
                                    <th class="text-right"><strong>金額</strong></th>
                                    <th style="min-width: 70px;"><strong>本日完成數量</strong></th>
                                    <th><strong>總累計完成數量</strong></th>
                                    <th style="min-width: 100px;"><strong>備註</strong></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-show="cellShow(index)" v-for="(item, index) in planItems" v-bind:key="item.OrderNo" class="bg-1-30">
                                    <td><strong>{{item.OrderNo}}</strong></td>
                                    <td><strong>{{item.PayItem}}</strong></td>
                                    <td><strong>{{item.Description}}</strong></td>
                                    <td>{{item.Unit}}</td>
                                    <td class="text-right"><strong>{{item.Price}}</strong></td>
                                    <td class="text-right"><strong>{{item.Quantity}}</strong></td>
                                    <td class="text-right"><strong>{{item.Amount}}</strong></td>
                                    <td>
                                        <input v-bind:disabled="item.DayProgress==-1" v-model.number="item.TodayConfirm" type="text" class="form-control text-right">
                                        <label v-if="isTodayOver(item)" style="color:red; font-size:small">總累量已超過契約量</label>
                                    </td>
                                    <td>{{calAccTotal(item)}}</td>
                                    <td>
                                        <input v-bind:disabled="item.DayProgress==-1" v-model="item.Memo" type="text" class="form-control">

                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div id="menu02" v-if="supDailyItem != null && supDailyItem.Seq != -1" class="tab-pane">
                    <div v-show="!isDisabledAndCompleted" class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0 mt-sm-2 mt-md-0">
                        <button @click="copyMiscData" v-bind:disabled="isDisabled" type="button" class="btn btn-outline-secondary btn-sm" title="複製前日材料,人員,機具資料">複製前日材料,人員,機具資料&nbsp;<i class="fas fa-copy"></i></button>
                    </div>
                    <h5>二、工地材料管理概況<small>(含約定之重要材料使用狀況及數量等)</small></h5>
                    <EditMaterial ref="editMaterial" v-bind:supDailyItem="supDailyItem" v-bind:canEditUser="canEditUser"></EditMaterial>
                    <h5>三、工地人員及機具管理(含約定之出工人數及機具使用情形及數量)</h5>
                    <EditPerson ref="editPerson" v-bind:supDailyItem="supDailyItem" v-bind:canEditUser="canEditUser"></EditPerson>
                    <EditEquipment ref="editEquipment" v-bind:supDailyItem="supDailyItem" v-bind:canEditUser="canEditUser"></EditEquipment>
                    <h5>四、本日施工項目是否有須依「營造業專業工程特定施工項目應置之技術士總類、比率或人數標準表」規定應置之技術士之專業工程</h5>
                    <div class="form-group my-2">
                        <div class="custom-control custom-radio">
                            <input v-model="miscItem.IsFollowSkill" type="radio" value="true" id="customRadio1" name="customRadioA" class="custom-control-input">
                            <label class="custom-control-label" for="customRadio1">是(此項如勾選，則應另外填寫「公共工程施工日誌之技術士簽章表」)</label>
                        </div>
                        <div class="custom-control custom-radio">
                            <input v-model="miscItem.IsFollowSkill" type="radio" value="false" id="customRadio2" name="customRadioA" class="custom-control-input">
                            <label class="custom-control-label" for="customRadio2">否</label>
                        </div>
                    </div>
                    <h5>五、工地職業安全衛生事項之督導、公共環境與安全之維護及其他工地行政事務</h5>
                    <div>
                        <table>
                            <tr><td>(ㄧ)施工前檢查事項：</td></tr>
                            <tr>
                                <td class="pl-4">1.實施勤前教育(含工地預防災變及危害告知)：</td>
                            </tr>
                            <tr>
                                <td class="pl-4">
                                    <div class="row pl-4">
                                        <div class="custom-control custom-radio">
                                            <input v-model="miscItem.SafetyHygieneMatters01" type="radio" value="true" id="customRadio1SafetyHygieneMatters01" name="customRadioSafetyHygieneMatters01" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio1SafetyHygieneMatters01">有</label>
                                        </div>
                                        <div class="custom-control custom-radio" style="padding-left: 2.5rem;">
                                            <input v-model="miscItem.SafetyHygieneMatters01" type="radio" value="false" id="customRadio2SafetyHygieneMatters01" name="customRadioSafetyHygieneMatters01" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio2SafetyHygieneMatters01">無</label>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="pl-4">2.確認新進勞工是否提報勞工保險(或其他商業保險)資料及安全衛生教育訓練紀錄：</td>
                            </tr>
                            <tr>
                                <td class="pl-4">
                                    <div class="row pl-4">
                                        <div class="custom-control custom-radio">
                                            <input v-model="miscItem.SafetyHygieneMatters02" type="radio" value="1" id="customRadio1SafetyHygieneMatters02" name="customRadioSafetyHygieneMatters02" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio1SafetyHygieneMatters02">有</label>
                                        </div>
                                        <div class="custom-control custom-radio" style="padding-left: 2.5rem;">
                                            <input v-model="miscItem.SafetyHygieneMatters02" type="radio" value="0" id="customRadio2SafetyHygieneMatters02" name="customRadioSafetyHygieneMatters02" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio2SafetyHygieneMatters02">無</label>
                                        </div>
                                        <div class="custom-control custom-radio" style="padding-left: 2.5rem;">
                                            <input v-model="miscItem.SafetyHygieneMatters02" type="radio" value="2" id="customRadio3SafetyHygieneMatters02" name="customRadioSafetyHygieneMatters02" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio3SafetyHygieneMatters02">無新進勞工</label>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="pl-4">檢查勞工個人防護具：</td>
                            </tr>
                            <tr>
                                <td class="pl-4">
                                    <div class="row pl-4">
                                        <div class="custom-control custom-radio">
                                            <input v-model="miscItem.SafetyHygieneMatters03" type="radio" value="true" id="customRadio1SafetyHygieneMatters03" name="customRadioSafetyHygieneMatters03" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio1SafetyHygieneMatters03">有</label>
                                        </div>
                                        <div class="custom-control custom-radio" style="padding-left: 2.5rem;">
                                            <input v-model="miscItem.SafetyHygieneMatters03" type="radio" value="false" id="customRadio2SafetyHygieneMatters03" name="customRadioSafetyHygieneMatters03" class="custom-control-input">
                                            <label class="custom-control-label" for="customRadio2SafetyHygieneMatters03">無</label>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr><td>(二)其他事項：</td></tr>
                            <tr><td class="pl-4"><input v-model="miscItem.SafetyHygieneMattersOther" maxlength="400" type="text" class="form-control"></td></tr>
                        </table>
                    </div>
                    <h5>六、施工取樣試驗紀錄</h5>
                    <textarea v-model.trim="miscItem.SamplingTest" maxlength="400" rows="5" class="form-control"></textarea>
                    <h5>七、通知協力廠商辦理事項</h5>
                    <textarea v-model.trim="miscItem.NoticeManufacturers" maxlength="400" rows="5" class="form-control"></textarea>
                    <h5>八、重要事項記錄</h5>
                    <textarea v-model.trim="miscItem.ImportantNotes" maxlength="400" rows="5" class="form-control"></textarea>
                </div>
            </div>
            <div v-if="supDailyItem != null" class="card-footer">
                <div class="row justify-content-center">
                    <div v-if="canEditUser" class="col-12 col-sm-4 col-xl-2 my-2">
                        <button v-on:click="getConstructionItem(selectDay)" role="button" class="btn btn-shadow btn-color3 btn-block"> 取消修改 </button>
                    </div>
                    <div v-if="canEditUser" class="col-12 col-sm-4 col-xl-2 my-2">
                        <button v-on:click="onSaveClick" role="button" class="btn btn-shadow btn-block btn-color11-4"> 儲存 <i class="fas fa-save"></i></button>
                    </div>
                    <div class="col-12 col-sm-4 col-xl-2 my-2 pt-1">
                        <!-- button @click="downloadDaily" v-bind:disabled="supDailyItem.Seq==-1" type="button" class="btn btn-outline-secondary btn-sm">施工日報 <i class="fas fa-download"></i></button -->
                        <button @click="dnClickEvent = 0" v-bind:disabled="supDailyItem.Seq==-1" data-target="#refExportModal" title="施工日報" class="btn btn-outline-secondary btn-sm"
                                role="button" data-toggle="modal">
                            施工日報<i class="fas fa-download"></i>
                        </button>
                    </div>
                </div>
                <div class="row justify-content-center">
                    最後更新人員時間:{{supDailyItem.DisplayName}}&nbsp;{{supDailyItem.ModifyTimeStr}}
                </div>
            </div>
        </div>
        <!-- 下載模式 shioulo 20230830 -->
        <div class="modal fade" id="refExportModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refExportModal" aria-modal="true">
            <div class="modal-dialog modal-xl modal-dialog-centered " style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title redText" id="projectUpload">下載工項設定</h6>
                    </div>
                    <div class="modal-body">
                        <div class="form-row">
                            <div class="col form-inline my-2">
                                <div class="custom-control custom-radio mx-2">
                                    <input v-model="dnDailyMode" name="dnDailyMode" value="0" type="radio" id="itemYes" class="custom-control-input">
                                    <label class="custom-control-label" for="itemYes">下載全部工項(預設)</label>
                                </div>
                                <div class="custom-control custom-radio mx-2">
                                    <input v-model="dnDailyMode" name="dnDailyMode" value="1" type="radio" id="itemNo" class="custom-control-input">
                                    <label class="custom-control-label" for="itemNo">當日工作量工項</label>
                                </div>
                            </div>
                        </div>
                        <br />
                        <h6>如果選擇下載全部的話，所有項目都出現<br />選擇僅當日工作量工項，則顯示當日有工作量的工項。</h6>
                    </div>
                    <div class="modal-footer">
                        <button v-on:click.stop="dnDailyDoc" type="button" class="btn btn-primary">
                            下載
                        </button>
                        <button type="button" id="closeExportModal" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
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
        props: ['tenderItem', 'canEditUser'],
        data: function () {
            return {
                supDailyItem: null,
                miscItem: {}, // 監造-施工日誌_雜項
                planItems: [], //依施工計畫執行按圖施工概況
                attrs: [],
                selectDay: null,
                fromDate: null,
                //分頁
                startRow: 0,
                endRow: 30,
                //
                selectTab: '',
                filterStartDate: '',
                filterEndDate: '',
                //s20230830
                dnDailyMode: "0", 
                dnClickEvent: -1,
            };
        },
        components: {
            CalendarSetting: require('./CalendarSetting.vue').default,
            EditMaterial: require('./PM_Construction_EditMaterial').default,
            EditPerson: require('./PM_Construction_EditPerson').default,
            EditEquipment: require('./PM_Construction_EditEquipment').default,
        },
        methods: {
            sortItem()
            {
                this.planItems.sort( (a, b) => (a.OrderNo- b.OrderNo) )
            },
            //複製前日材料,人員,機具資料 s20231116
            copyMiscData() {
                this.supDailyItem.ItemDate = this.supDailyItem.ItemDateStr;
                window.myAjax.post('/EPCProgressManage/CopyConstructionMiscData', { supDailyItem: this.supDailyItem })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$refs.editMaterial.getResords();
                            this.$refs.editPerson.getResords();
                            this.$refs.editEquipment.getResords();
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //日誌下載 s20230830
            dnDailyDoc() {
                if (this.dnClickEvent == 1) {
                    this.downloadMultiDateReport();
                } else if(this.dnClickEvent == 0) {
                    this.downloadDaily();
                }
            },
            //下載(多日期) 日報 s20230228
            downloadMultiDateReport() {
                if (this.filterEndDate == '' || this.filterStartDate == '') {
                    alert('請輸入日期範圍');
                    return;
                }
                if (window.comm.calDays(this.filterStartDate, this.filterEndDate) > 31) {
                    alert('日期範圍不可以超過31天');
                    return;
                }

                let action = '/EPCProgressManage/DownloadCDailyMulti?sd=' + this.filterStartDate + '&ed=' + this.filterEndDate + '&eId=' + this.tenderItem.Seq + '&eEM=' + this.dnDailyMode;
                window.comm.dnFile(action, this.dnCallBack);
            },
            //下載 施工日報
            downloadDaily() {
                let action = '/EPCProgressManage/DownloadCDaily?id=' + this.supDailyItem.Seq + '&eEM=' + this.dnDailyMode;
                window.comm.dnFile(action, this.dnCallBack);
            },
            //s20230830
            dnCallBack(result) {
                if (result) {
                    document.getElementById('closeExportModal').click();
                    this.dnDailyMode = "0";
                }
            },
            //雨天 s20230720
            saveRainyDay() {
                this.$refs.calendar.supDailyDateNote.Weather1 = "雨天";
                this.$refs.calendar.supDailyDateNote.Weather2 = "雨天";
                this.onSaveClick();
            },
            //例假日 s20230720
            saveHolidays() {
                this.$refs.calendar.supDailyDateNote.Weather1 = "例假日";
                this.$refs.calendar.supDailyDateNote.Weather2 = "例假日";
                this.onSaveClick();
            },
            //至今日累計完成數量
            isTodayOver(item) {
                if (isNaN(item.TodayConfirm))
                    return item.TotalAccConfirm > item.Quantity;
                else {
                    return (item.TodayConfirm + item.TotalAccConfirm) > item.Quantity;
                }
            },
            calAccTotal(item) {
                if (isNaN(item.TodayConfirm))
                    return item.TotalAccConfirm;
                else {
                    return Math.round((item.TodayConfirm + item.TotalAccConfirm)*10000) / 10000;
                }
            },

            //下載(多日期) Excel範本
            downloadMultiDate() {
                if (this.filterEndDate == '' || this.filterStartDate == '') {
                    alert('請輸入日期範圍');
                    return;
                }
                if (window.comm.calDays(this.filterStartDate, this.filterEndDate) > 31) {
                    alert('日期範圍不可以超過31天');
                    return;
                }

                let action = '/EPCProgressManage/DnMultiDate?sd=' + this.filterStartDate + '&ed=' + this.filterEndDate + '&eId=' + this.tenderItem.Seq;
                window.comm.dnFile(action);
            },
            //匯入(多日期)日誌(excel)
            uploadMultiDailyLog(event) {
                var files = event.target.files || event.dataTransfer.files;
                // 預防檔案為空檔
                if (!files.length) return;
                if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
                    alert('請選擇 .xlsx Excel檔案');
                    return;
                }
                var uploadfiles = new FormData();
                uploadfiles.append("id", this.tenderItem.Seq);
                uploadfiles.append("file", files[0], files[0].name);
                window.myAjax.post('/EPCProgressManage/UploadMultiDailyLog', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            if (this.selectDay != null) this.getConstructionItem(this.selectDay);
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            //下載 Excel範本
            download() {
                let action = '/EPCProgressManage/Download?id=' + this.supDailyItem.Seq;// + '&engId=' + this.supDailyItem.EngMainSeq;
                window.comm.dnFile(action);
            },
            //填報完成
            fillCompleted() {
                /* s20231116 多日誌上傳尚未處裡
                if (confirm('填報完成後將不能再修改\n是否確定? ')) {
                    setTimeout(
                        window.myAjax.post('/EPCProgressManage/DailyLogCompleted', { id: this.supDailyItem.Seq })
                            .then(resp => {
                                if (resp.data.result == 0) {
                                    this.getConstructionItem(this.selectDay);
                                }
                                alert(resp.data.msg);
                            })
                            .catch(err => {
                                console.log(err);
                            }), 10);
                }*/
            },
            //儲存
            onSaveClick() {
                if (this.selectDay == null) {
                    alert('請選取日誌日期');
                    return;
                }
                this.supDailyItem.ItemDate = this.supDailyItem.ItemDateStr;
                this.supDailyItem.Weather1 = this.$refs.calendar.supDailyDateNote.Weather1;
                this.supDailyItem.Weather2 = this.$refs.calendar.supDailyDateNote.Weather2;
                this.supDailyItem.FillinDate = this.$refs.calendar.supDailyDateNote.FillinDate;
                window.myAjax.post('/EPCProgressManage/ConstructionSave', {
                    supDailyItem: this.supDailyItem,
                    miscItem: this.miscItem,
                    planItems: this.planItems
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            if (this.supDailyItem.Seq == -1) {
                                this.getConstructionItem(this.selectDay);
                                this.getConstructionCalInfo();
                            }
                            if (this.selectTab == '') {
                                alert('請繼續填寫工地材料、工地人員、機具使用狀況');
                                //s20230908
                                this.selectTab = 'menu02';
                                document.getElementById('tabMenu01').classList.remove("active");
                                document.getElementById('menu01').classList.remove("active");
                                document.getElementById('tabMenu02').classList.add("active");
                                if (document.getElementById('menu02') != null) {
                                    document.getElementById('menu02').classList.add("active");
                                }
                            } else {
                                alert(resp.data.msg);
                            }
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getConstructionItem(sDay) {
                this.selectDay = null;
                this.supDailyItem = null;
                this.miscItem = {};
                this.planItems = [];
                this.selectTab = '';
                if (sDay == null) return;
                window.myAjax.post('/EPCProgressManage/GetConstructionItem', { id: this.tenderItem.Seq, tarDate: sDay })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.supDailyItem = resp.data.supDailyItem;
                            this.miscItem = resp.data.miscItem;
                            this.planItems = resp.data.planItems;
                            this.selectDay = sDay;
                            //s20230908
                            document.getElementById('tabMenu01').classList.add("active");
                            document.getElementById('menu01').classList.add("active");
                            document.getElementById('tabMenu02').classList.remove("active");
                            var that = this;
                            this.$refs.calendar.setSupDailyDateNote(this.supDailyItem);
                            this.$nextTick(function () {//s20230908
                                that.$refs.pagination.setPagination();

                                if (document.getElementById('menu02') != null) {
                                    document.getElementById('menu02').classList.remove("active");
                                }
                            });
                            /*setTimeout(function greet() {
                                that.$refs.pagination.setPagination();
                                
                                if (document.getElementById('menu02') != null) {
                                    document.getElementById('menu02').classList.remove("active");
                                }
                            }, 500);*/
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        alert('網路異常,無法取得日誌');
                    });
            },
            //取得紀錄
            getConstructionCalInfo() {
                window.myAjax.post('/EPCProgressManage/GetConstructionInfo', { id: this.tenderItem.Seq, fromDate: this.fromDate })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.setCalAttr(resp.data.items);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            cellShow(index) {
                if (index >= this.startRow && index < this.endRow)
                    return true;
                else
                    return false;
            },
            onPaginationChange(pInx, pCnt) {
                this.startRow = (pInx - 1) * pCnt;
                this.endRow = pInx * pCnt;
                console.log("pInx:" + pInx + " pCnt:" + pCnt);

            },
            //月曆
            setCalAttr(items) {
                this.attrs = [];
                //已填寫
                var fillin = {
                    highlight: {
                        class: 'bg-success',
                    },
                    dates: [],
                }
                //未填寫
                var stopfillin = {
                    highlight: {
                        class: 'bg-danger',
                    },
                    dates: [],
                }
                //未填寫
                var notfillin = {
                    highlight: {
                        class: 'bg-warning',
                    },
                    dates: [],
                }
                var i;
                for (i = 0; i < items.length; i++) {
                    let item = items[i];
                    if (item.Mode == 0)
                        notfillin.dates.push(Date.parse(item.DateStr));
                    else if (item.Mode == 1)
                        stopfillin.dates.push(Date.parse(item.DateStr));
                    else if (item.Mode == 2)
                        fillin.dates.push(Date.parse(item.DateStr));
                }
                this.attrs.push(notfillin);
                this.attrs.push(stopfillin);
                this.attrs.push(fillin);
            },
            onCalFromPage(item) {
                this.fromDate = item.year + '-' + item.month + '-01';
                this.getConstructionCalInfo();
            },
            onCalDayClick(day) {
                this.selectTab = '';//s20230425
                //this.planItemsTotal = 0;
                //this.getConstructionItem(day.id);
                //s20230412
                window.myAjax.post('/EPCProgressManage/CheckActiveDate', { id: this.tenderItem.Seq, tarDate: day.id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.planItemsTotal = 0;
                            this.getConstructionItem(day.id);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onCalDataChange() {
                this.getConstructionCalInfo();
            },
        },
        computed: {
            isDisabled: function () {
                if (this.supDailyItem == null || this.supDailyItem.Seq == -1) return true;
                return false;
            },
            isDisabledAndCompleted: function () {//s20231116
                if (this.supDailyItem == null || this.supDailyItem.Seq == -1 || this.supDailyItem.ItemState == 1) return true;
                return false;
            }
        },
        mounted() {
            console.log('mounted() 施工日誌');
        }
    }
</script>
<style>
    .vc-highlight {
        width: 95% !important;
    }
</style>