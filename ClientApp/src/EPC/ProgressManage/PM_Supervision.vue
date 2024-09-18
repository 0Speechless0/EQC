<template>
    <div>
        <table>
            <tr><td colspan="5">避免網路速度影響，僅提供下載31天報表</td></tr>
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
                    <!-- button @click="downloadDailyMulti" type="button" class="btn btn-outline-secondary btn-sm" title="監造報表下載">下載多日監造報表<i class="fas fa-download"></i></button -->
                    <button :disabled="downloadTaskStore.isDownloading" @click="dnClickEvent = 1" data-target="#refExportModal" title="下載多日監造日報" class="btn btn-outline-secondary btn-sm"
                            role="button" data-toggle="modal">
                        下載多日監造日報<i class="fas fa-download"></i>
                    </button>
                </td>
                <td>
                    <label class="btn btn-block btn-color11-4 btn-sm" style="margin-top: 6px">
                        <input v-on:change="uploadMultiDailyLog($event)" type="file" name="file" multiple="" style="display:none;">
                        上傳施工日誌&nbsp;<i class="fas fa-upload"></i>
                    </label>
                </td>
                <td style="color:red" v-if="downloadTaskStore.isDownloading" class="pl-2">
                        因資源超負荷使用，請等候....
                </td>
            </tr>
        </table>
        <CalendarSetting ref="calendar" v-bind:tenderItem="tenderItem" v-bind:attrs="attrs"
                         v-on:onDayClickEvent="onCalDayClick"
                         v-on:onFromPageEvent="onCalFromPage"
                         :supervision="true"
                    
                         v-on:onDataChangeEvent="onCalDataChange">
        <template #constructionBtn>
            <slot name="constructionBtn">
                
            </slot>
        </template>    
        <template #constructionDaysSetting>
        
            <slot name="constructionDaysSetting">

        </slot>
    </template>            
        </CalendarSetting>
        <h5>日誌日期:{{selectDay}}</h5>
        <div>
            <ul v-show="supDailyItem!=null" class="nav nav-tabs" role="tablist">
                <li class="nav-item">
                    <a v-on:click="selectTab=''" :class="`nav-link ${selectTab == '' ? 'active' : '' }`" id="tabMenu01" data-toggle="tab" href="#menu01">一、依施工計畫執行按圖施工概況</a>
                </li>
                <li class="nav-item">
                    <a v-on:click="selectTab='menu02'" :class="`nav-link ${selectTab == 'menu02' ? 'active' : '' }`" id="tabMenu02" data-toggle="tab" href="#menu02">二、其它</a>
                </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
                <!-- 一 -->
                <div id="menu01" v-if="selectTab == ''">
                    <h5>一、依施工計畫執行按圖施工概況(含約定之重要施工項目及完成數量等)</h5>
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
                    <div class="table-responsive tableFixHead ">
                        <table class="table table-responsive-md table-hover">
                            <thead class="insearch">
                                <tr>
                                    <th>
                                        <strong>序號</strong>
                                        <button type="button" title="排序" @click="sortItem" class="btn btn-shadow btn-color11-4 btn-sm"><i class="fas fa-sort"></i></button>
                                    </th>
                                    <th><strong>項次</strong></th>
                                    <th><strong>施工項目</strong></th>
                                    <th style="width: 50px;" class="text-right"><strong>單位</strong></th>
                                    <th class="text-right"><strong>單價</strong></th>
                                    <th style="width: 50px;" class="text-right"><strong>契約數量</strong></th>
                                    <th class="text-right"><strong>金額</strong></th>
                                    <th style="min-width: 70px;"><strong>施工日至本日完成數量</strong></th>
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
                                    <td :style="{color:item.ConstructionConfirm  != item.TodayConfirm ?  'red' : 'inherit' }">{{ item.ConstructionConfirm  }}</td>
                                    <td>
                                        <input v-bind:disabled="item.DayProgress==-1" v-model.number="item.TodayConfirm" type="text" class="form-control text-right">
                                        <label v-if="isTodayOver(item)" style="color:red; font-size:small">總累量已超過契約量</label>
                                    </td>
                                    <td>{{calAccTotal(item.TodayConfirm, item.TotalAccConfirm)}}</td>
                                    <td><input v-bind:disabled="item.DayProgress==-1" v-model="item.Memo" type="text" class="form-control"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div id="menu02" v-if="selectTab == 'menu02'">
                    <h5>二、監督依照設計圖說及核定施工圖說施工<small>(含約定之檢驗停留點及施工抽查等情形)</small></h5>
                    <textarea v-model="miscItem.DesignDrawingConst" rows="5" class="form-control"></textarea>

                    <h5>三、查核材料規格及品質<small>(含約定之檢驗停留點、材料設備管制及檢（試）驗等抽驗情形)</small></h5>
                    <div class="form-inline col-12 col-md-8 small">
                        <label class="my-1 mr-2">
                            <span class="text-R">※ 若須填寫出工人數及機具使用狀況，請監造單位至詳細價目表新增機具及人員項目後，即可在日誌填寫數量</span>
                        </label>
                    </div>
                    <textarea v-model="miscItem.SpecAndQuality" maxlength="400" rows="5" class="form-control"></textarea>

                    <!-- 材料管理 -->
                    <h5>四、工地人材料管理概況<small>(含約定之重要材料使用狀況及數量等)</small></h5>
                    <div class="table-responsive">
                        <h6>(一) 施工廠商施工前檢查事項辦理情形：</h6>
                        <div rows="5" style="padding: 0 0 0 50px;">
                            <input @click="ratioClick(miscItem, 1)" v-model="miscItem.SafetyHygieneMatters01" value="true" class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault1">
                            <label class="form-check-label" for="flexRadioDefault1">
                                完成
                            </label>
                        </div>
                        <div rows="5" style="padding: 0 0 0 50px;">
                            <input @click="ratioClick(miscItem, 2)" v-model="miscItem.SafetyHygieneMatters01" value="false" class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault2" checked="">
                            <label class="form-check-label" for="flexRadioDefault2">
                                未完成
                            </label>
                        </div>
                        <h6>(二) 其他工地安全衛生督導事項</h6>
                        <textarea v-model="miscItem.SafetyHygieneMattersOther" maxlength="400" rows="5" class="form-control"></textarea>
                    </div>

                    <h5>五、其他約定監造事項<small>(含重要事項紀錄、主辦機關指示及通知廠商辦理事項等)</small></h5>
                    <textarea v-model="miscItem.OtherMatters" maxlength="400" rows="5" class="form-control"></textarea>
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
                    <!-- div class="col-12 col-lg-4 my-2 pt-1">
                        <button @click="downloadDaily" v-bind:disabled="supDailyItem.Seq==-1" type="button" class="btn btn-outline-secondary btn-sm">監造報表 <i class="fas fa-download"></i></button>&nbsp;
                    </div -->
                    <button @click="dnClickEvent = 0" v-bind:disabled="supDailyItem.Seq==-1" data-target="#refExportModal" title="監造報表" class="btn btn-outline-secondary btn-sm"
                            role="button" data-toggle="modal">
                        監造報表<i class="fas fa-download"></i>
                    </button>
                </div>
                <div class="row justify-content-center">
                    最後更新人員時間:{{supDailyItem.DisplayName}}&nbsp;{{supDailyItem.ModifyTimeStr}}
                </div>
            </div>
        </div>
        <!-- 下載模式 shioulo 20230831 -->
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
    import downloadTaskStore  from '../../store/downloadTaskStore';
    export default {
        props: ['tenderItem', 'canEditUser'],
        setup()
        {   
            downloadTaskStore.getDonloadTaskTag();
            return {
                downloadTaskStore 
            }
        },
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
                filterStartDate: '',
                filterEndDate: '',
                isAdmin: false,
                //s20230831
                dnDailyMode: "0",
                dnClickEvent: -1,
                //s20230908
                selectTab: '',
                
            };
        },
        components: {
            CalendarSetting: require('./CalendarSetting.vue').default,
        },
        methods: {
            async ratioClick(miscItem, index)
            {
                console.log("fff", miscItem.SafetyHygieneMatters01);


                if( (index == 1 && miscItem.SafetyHygieneMatters01 == 'true') || (index == 2 && miscItem.SafetyHygieneMatters01 == 'false') )
                {
                    miscItem.SafetyHygieneMatters01 = null;
                }
                else if(index == 1)
                    miscItem.SafetyHygieneMatters01 = true;
                else miscItem.SafetyHygieneMatters01 = false;
  
            },
            sortItem()
            {
                this.planItems.sort( (a, b) => (a.OrderNo- b.OrderNo) )
            },
            //日誌下載 s20230831
            dnDailyDoc() {
                if (this.dnClickEvent == 1) {
                    this.downloadDailyMulti();
                } else if (this.dnClickEvent == 0) {
                    this.downloadDaily();
                }
                window.closeModal("#refExportModal");
            },
            //下載 多日期 監造報表 s20230228
            async downloadDailyMulti() {
                if (this.filterEndDate == '' || this.filterStartDate == '') {
                    alert('請輸入日期範圍');
                    return;
                }
                if (window.comm.calDays(this.filterStartDate, this.filterEndDate) > 31) {
                    alert('日期範圍不可以超過31天');
                    return;
                }

                let action = '/EPCProgressManage/DownloadSDailyMulti?sd=' + this.filterStartDate + '&ed=' + this.filterEndDate + '&eId=' + this.tenderItem.Seq + '&eEM=' + this.dnDailyMode;
                // window.comm.dnFile(action, this.dnCallBack);
                let {data :res}  = await window.myAjax.get(action);
                alert(res.message);
                downloadTaskStore.isDownloading = res.downloadTaskTag;

            },
            //下載 監造報表
            downloadDaily() {
                let action = '/EPCProgressManage/DownloadSDaily?id=' + this.supDailyItem.Seq + '&eEM=' + this.dnDailyMode;
                window.comm.dnFile(action, this.dnCallBack);
            },
            //s20230831
            dnCallBack(result) {
                if (result) {
                    document.getElementById('closeExportModal').click();
                    this.dnDailyMode = "0";
                }
            },

            //至今日累計完成數量
            isTodayOver(item) {
                if (isNaN(item.TodayConfirm))
                    return item.TotalAccConfirm > item.Quantity;
                else {
                    return (item.TodayConfirm + item.TotalAccConfirm) > item.Quantity;
                }
            },
            calAccTotal(TodayConfirm, TotalAccConfirm) {
                if (isNaN(TodayConfirm))
                    return TotalAccConfirm;
                else
                    return Math.round((TodayConfirm + TotalAccConfirm) * 10000) / 10000;
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
                window.myAjax.post('/EPCProgressManage/MiscSave', {
                    supDailyItem: this.supDailyItem,
                    miscItem: this.miscItem,
                    planItems: this.planItems
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.$emit('reload');
                            if (this.supDailyItem.Seq == -1) {
                                this.getMiscItem(this.selectDay);
                                this.getMiscCalInfo();
                            }
                            if (this.selectTab == '') {//s20230908
                                alert('請繼續填寫 二、其它 頁面相關資料');
                                //s20230908
                                this.selectTab = 'menu02';
                                // document.getElementById('tabMenu01').classList.remove("active");
                                // document.getElementById('menu01').classList.remove("active");
                                // document.getElementById('tabMenu02').classList.add("active");
                                // document.getElementById('menu02').classList.add("active");
                                window.scrollTo(0, 0);
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
            //匯入(多日期)施工日誌(excel) 20230303
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
                window.myAjax.post('/EPCProgressManage/UploadMultiSDailyLog', uploadfiles,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            if (this.selectDay != null) this.getMiscItem(this.selectDay);
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
            
            getMiscItem(sDay) {
                this.selectDay = null;
                this.supDailyItem = null;
                this.miscItem = {};
                this.planItems = [];
                this.selectTab = '';
                if (sDay == null) return;
                window.myAjax.post('/EPCProgressManage/GetMiscItem', { id: this.tenderItem.Seq, tarDate: sDay })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.supDailyItem = resp.data.supDailyItem;
                            this.miscItem = resp.data.miscItem;
                            this.planItems = resp.data.planItems;
                            this.selectDay = sDay;
                            //s20230831
                            // document.getElementById('tabMenu02').classList.remove("active");
                            // document.getElementById('tabMenu01').classList.add("active");

                            // var that = this;
                            this.$refs.calendar.setSupDailyDateNote(this.supDailyItem);
                            // setTimeout(function greet() {
                            //     that.$refs.pagination.setPagination();
                            // }, 500);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        alert('網路異常,無法取得日誌');
                    });
            },
            //取得紀錄
            getMiscCalInfo() {
                this.isAdmin = false;
                window.myAjax.post('/EPCProgressManage/GetMiscInfo', { id: this.tenderItem.Seq, fromDate: this.fromDate })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isAdmin = resp.data.admin;
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
            onCalFromPage(item) {
                this.fromDate = item.year + '-' + item.month + '-01';
                this.getMiscCalInfo();
            },
            onCalDayClick(day) {
                //s20230412
                window.myAjax.post('/EPCProgressManage/CheckActiveDate', { id: this.tenderItem.Seq, tarDate: day.id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem.currentDate = {
                                Date : day.id,
                                ConstructionDateConfirmed : resp.data.ConstructionDateConfirmed,
                                MiscDateConfirmed : resp.data.MiscDateConfirmed    
                            };

                            this.getMiscItem(day.id);
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onCalDataChange() {
                this.getMiscCalInfo();
            },
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
        },
        mounted() {
            this.tenderItem.currentDate = null;
            console.log('mounted() 監造日誌');
        }
    }
</script>
<style>
    .vc-highlight {
        width: 95% !important;
    }
</style>
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