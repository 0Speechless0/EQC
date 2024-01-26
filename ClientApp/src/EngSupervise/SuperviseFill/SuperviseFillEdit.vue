<template>
    <div>
        <h5 class="insearch mt-0 py-2">工程編號：{{engItem.EngNo}} &nbsp; 工程名稱：{{engItem.EngName}} &nbsp; 督導期別：{{engItem.PhaseCode}}</h5>
        <h5>評分</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover table2">
                <tbody>
                    <tr>
                        <th colspan="2">
                            <div class="row">
                                <template >
                                    <div class="col-4" v-for="(item, index) in committeeScoreList" v-bind:key="index">
                                        <span class="row">
                                            <div class="col-5" >
                                                {{item.CName}}
                                            </div>
                                            <div class="col-7" ><input v-model.number="item.Score" type="number" class="form-control" value="84"></div>
                                        </span>
                                     </div>
                                </template>
                            </div>
                        </th>
                    </tr>
                    <tr>
                        <td style="width: 60px; text-align: center;">
                            <button @click="onSaveCommitteeScore" class="btn btn-color11-2 btn-xs m-1" title="儲存評分">儲存評分</button>
                        </td>
                        <td>總分:{{engItem.CommitteeAverageScore}}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="table-responsive">
            <table class="table table-responsive-md table-hover table2">
                <!--<tr>
                <th colspan="2">評分方式</th>
                <td colspan="6">總平分(Total) = 品質管理(Q) + 施工品質(W) + 施工進度(P) = <span class="text-R">{{ totalScore==-1 ? '' : totalScore+'分(乙)'}}</span></td>
            </tr>-->
                <tr>
                    <th colspan="8" class="bg-3-30">紀錄彙整區</th>
                </tr>
                <tr>
                    <th style="width: 80px;">項次</th>
                    <th>缺失編號</th>
                    <th>缺失區位</th>
                    <th>扣點數</th>
                    <th>督導紀錄</th>
                    <th>領隊、督導委員</th>
                    <th>管理</th>
                </tr>
                <tr v-if="items.length==0">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td style="min-width: unset;">
                        ty
                        <div class="d-flex justify-content-center">
                            <button @click="onNewRecord(-1)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                        </div>
                    </td>
                </tr>
                <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                    <td style="min-width: unset;">{{index+1}}</td>
                    <template v-if="item.Seq != editSeq">
                        <td>{{item.MissingNo}}</td>
                        <td>{{item.MissingLoc}}</td>
                        <td>{{item.DeductPointStr}}</td>
                        <td>{{item.SuperviseMemo}}</td>
                        <td>
                            <div v-for="(committee, inx) in engItem.committees" v-bind:key="committee.Value" class="custom-control custom-checkbox custom-control-inline">
                                <input disabled v-model="item.committeeList" v-bind:value="committee.Value" type="checkbox" name="ExecutiveAgency" v-bind:id="'ck'+index+'-'+inx" class="custom-control-input">
                                <label v-bind:for="'ck'+index+'-'+inx" class="custom-control-label">{{committee.Text}}</label>
                            </div>
                        </td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                                <button @click="onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                                <button @click="onNewRecord(index)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                            </div>
                        </td>
                    </template>
                    <template v-if="item.Seq == editSeq">
                        <td><input v-model.trim="editRecord.MissingNo" @change="onMissingNoChange" maxlength="30" type="text" class="form-control"></td>
                        <td>
                            <input v-model.trim="editRecord.MissingLoc" maxlength="10" type="text" class="form-control">
                            {{editRecord.missingContent}}
                        </td>
                        <td><input v-model.trim="editRecord.DeductPointStr" maxlength="6" type="text" class="form-control"></td>
                        <td><textarea v-model="editRecord.SuperviseMemo" maxlength="500" rows="5" class="form-control"></textarea></td>
                        <td>
                            <div v-for="(committee, inx) in engItem.committees" v-bind:key="committee.Value" class="custom-control custom-checkbox custom-control-inline">
                                <input v-model="editRecord.committeeList" v-bind:value="committee.Value" type="checkbox" name="ExecutiveAgency" v-bind:id="'ck'+index+'-'+inx" class="custom-control-input">
                                <label v-bind:for="'ck'+index+'-'+inx" class="custom-control-label">{{committee.Text}}</label>
                            </div>
                        </td>
                        <td style="min-width: unset;">
                            <div class="d-flex justify-content-center">
                                <button @click="onSaveRecord" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                <button @click="onEditCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-mil">X</i></button>
                            </div>
                        </td>
                    </template>
                </tr>
            </table>
        </div>
        <p class="text-R">* 表單依照缺失編號排序</p>
        <h5>抽驗項目</h5>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover table2">
                <tbody>
                    <tr>
                        <th style="width:60px">項目</th>
                        <th>
                            <div class="row" style="display: flex;">
                                <select v-model="selSamplingName" class="col-12 form-control" >
                                    <option>混凝土鑽心試驗</option>
                                    <option>鋼筋</option>
                                    <option>工地密度試驗</option>
                                    <option>其他</option>
                                </select>
                                <input v-if="selSamplingName=='其他'" v-model.trim="otherSamplingName" maxlength="20" type="text" class="col-12 form-control" placeholder="選擇其他的話請自行輸入">
                            </div>
                        </th>
                        <th style="width:60px">位置</th>
                        <th><input v-model.trim="newSamplingRecord.Location" maxlength="100" type="text" class="form-control"></th>
                        <th>數量(單位)</th>
                        <th><input v-model.trim="newSamplingRecord.Quantity" maxlength="10" type="text" class="form-control"></th>
                        <th style="text-align: center;">
                            <button @click="addSamplingRecord" class="btn btn-outline-secondary btn-xs sharp m-1">
                                <i class="fas fa-plus"></i>
                            </button>
                        </th>
                    </tr>
                    <tr v-for="(item, index) in samplingItems" v-bind:key="item.Seq">
                        <template v-if="item.Seq != editSamplingSeq">
                            <td>項目</td>
                            <td>{{item.SamplingName}}</td>
                            <td>位置</td>
                            <td>{{item.Location}}</td>
                            <td>數量(單位)</td>
                            <td>{{item.Quantity}}</td>
                            <td style="text-align: center; ">
                                <button @click="onEditSamplingRecord(item)" role="button" class="btn btn-color11-1 btn-xs sharp m-1"> <i class="fas fa-pencil-alt"></i></button>
                                <button @click="onDelSamplingRecord(item)" role="button" class="btn btn-color9-1 btn-xs sharp m-1"><i class="fas fa-trash-alt"></i></button>
                            </td>
                        </template>
                        <template v-if="item.Seq == editSamplingSeq">
                            <td>項目</td>
                            <td><input v-model.trim="editSamplingRecord.SamplingName" maxlength="20" type="text" class="form-control"></td>
                            <td>位置</td>
                            <td><input v-model.trim="editSamplingRecord.Location" maxlength="100" type="text" class="form-control"></td>
                            <td>數量(單位)</td>
                            <td><input v-model.trim="editSamplingRecord.Quantity" maxlength="10" type="text" class="form-control"></td>
                            <td style="text-align: center;">
                                <div class="d-flex justify-content-center">
                                    <button @click="updateSamplingRecord" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                                    <button @click="onEditSamplingCancel" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-mil">X</i></button>
                                </div>
                            </td>
                        </template>
                    </tr>
                </tbody>
            </table>
            <p class="text-R">註：本預設項目請自行輸入，另數量(單位)欄位請輸入範例為1組、1支或空白</p>
            <div class="table-responsive">
                <table class="table table-responsive-md table-hover table2">
                    <tbody>
                        <tr>
                            <th style="width: 100px;">檢驗、拆驗</th>
                            <td><textarea v-model.trim="engItem.Inspect" cols="2" maxlength="200" type="text" class="form-control"></textarea></td>
                        </tr>
                        <tr>
                            <td style="text-align: center;">
                                <button @click="onSaveInspect" class="btn btn-color11-2 btn-xs m-1" title="儲存檢驗">儲存檢驗</button>
                            </td>
                            <td><button @click="onSetSamplingItems" class="btn btn-color11-1 btn-xs m-1" title="插入檢驗項目">插入檢驗項目</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <p class="text-R">註：未輸入者預設為未實施檢驗(拆驗)</p>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return { 
                targetId: null,
                engItem: {},
                items: [],
                totalScore: -1,
                editSeq: -99,
                editRecord: {},
                //20230215
                committeeScoreList: [], //委員評分
                samplingItems: [], //抽驗項目
                selSamplingName:'',
                otherSamplingName: '',
                editSamplingSeq: -99,
                editSamplingRecord: {},
                newSamplingRecord: {},
            };
        },
        methods: {
            onNewRecord(index) {
                if (this.editSeq > -99) return;
                window.myAjax.post('/ESSuperviseFill/NewRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editRecord = resp.data.item;
                            this.editRecord.SuperviseEngSeq = this.engItem.Seq;
                            this.items.splice(index+1, 0, this.editRecord);
                            this.editSeq = this.editRecord.Seq;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //刪除紀錄
            onDelRecord(item) {
                if (this.editSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/ESSuperviseFill/DelRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getResords();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //儲存
            onSaveRecord() {
                var str = this.editRecord.DeductPointStr;
                if (window.comm.stringEmpty(str)) this.editRecord.DeductPoint = 0;
                else {
                    var dp = str;
                    var d = 1;
                    if (str.indexOf('*') == 0) {
                        d = 2;
                        dp = str.substring(1, str.length);
                    }
                    if (isNaN(dp)) {
                        alert("扣點數必須是數值");
                        return;
                    } else if (dp < 0) {
                        alert("扣點數必須是正數");
                        return;
                    } else {
                        this.editRecord.DeductPoint = d*dp;
                    }
                }
                window.myAjax.post('/ESSuperviseFill/UpdateRecords', { m: this.editRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.editSeq = -99;
                            this.getResords();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //缺失編號異動
            onMissingNoChange() {
                if (!window.comm.stringEmpty(this.editRecord.MissingLoc)) return;

                var str = this.editRecord.MissingNo;
                var loc = "";
                if (str.indexOf('4.01') == 0)
                    loc = "主辦機關";
                else if (str.indexOf('4.02') == 0)
                    loc = "監造單位";
                else if (str.indexOf('4.03') == 0)
                    loc = "承攬廠商";
                else if (str.indexOf('5.') == 0)
                    loc = "施工品質";
                else if (str.indexOf('6.01') == 0)
                    loc = "施工進度";
                else if (str.indexOf('7.') == 0)
                    loc = "規劃設計";
                else if (str.indexOf('8.01') == 0)
                    loc = "其他建議";
                this.editRecord.MissingLoc = loc;
            },
            //取消編輯
            onEditCancel() {
                this.editSeq = -99;
                this.getResords();
            },
            //編輯紀錄
            onEditRecord(item) {
                if (this.editSeq > -99) return;
                this.editRecord = Object.assign({}, item);
                this.editSeq = this.editRecord.Seq;
            },
            //紀錄清單
            getResords() {
                this.items = [];
                this.totalScore = -1;
                window.myAjax.post('/ESSuperviseFill/GetRecords', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.totalScore = resp.data.totalScore;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程
            getEngItem() {
                this.engItem = {};
                window.myAjax.post('/ESSuperviseFill/GetEng', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getCommitteeScoreList();
                            this.getResords();
                            this.engItem = resp.data.item;
                            this.getNewSamplingRecord();
                            this.getSamplingRecords();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //委員評分 20230215
            getCommitteeScoreList() {
                this.engItem = {};
                window.myAjax.post('/ESSuperviseFill/GetCommitteeScoreList', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.committeeScoreList = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //委員評分 20230215
            onSaveCommitteeScore() {
                window.myAjax.post('/ESSuperviseFill/SaveCommitteeScore', { id: this.targetId, items:this.committeeScoreList })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engItem.CommitteeAverageScore = resp.data.avgScore;
                        }
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //插入檢驗項目。
            onSetSamplingItems() {
                var data = "本次督導實施「";
                var sp = '';
                var i;
                for (i = 0; i < this.samplingItems.length; i++) {
                    data = data + sp + this.samplingItems[i].SamplingName + this.samplingItems[i].Location + this.samplingItems[i].Quantity;
                    sp = "；";
                }
                data = data + "」，請送經TAF認證之實驗室進行契約規定之相關試驗，試驗報告及判讀結果請併同缺失改善報告併復。";
                console.log(data);
                if (this.engItem.Inspect)
                    this.engItem.Inspect = data + this.engItem.Inspect;
                else
                    this.engItem.Inspect = data;
            },
            //儲存檢驗、拆驗。
            onSaveInspect() {
                window.myAjax.post('/ESSuperviseFill/SaveInspect', { id: this.targetId, inspect: this.engItem.Inspect })
                    .then(resp => {
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getNewSamplingRecord() {
                window.myAjax.post('/ESSuperviseFill/GetNewSamplingRecord')
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.newSamplingRecord = resp.data.item;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
                
            },
            //編輯檢驗項目
            onEditSamplingRecord(item) {
                if (this.editSamplingSeq > -99) return;
                this.editSamplingRecord = Object.assign({}, item);
                this.editSamplingSeq = this.editSamplingRecord.Seq;
            },
            //檢驗項目清單
            getSamplingRecords() {
                this.samplingItems = [];
                this.editSamplingSeq = -99;
                window.myAjax.post('/ESSuperviseFill/GetSamplingRecords', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.samplingItems = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //更新檢驗項目
            updateSamplingRecord() {
                if (this.strEmpty(this.editSamplingRecord.SamplingName) || this.strEmpty(this.editSamplingRecord.Location) || this.strEmpty(this.editSamplingRecord.Quantity)) {
                    alert('項目,位置,數量(單位) 必須輸入');
                    return;
                }
                window.myAjax.post('/ESSuperviseFill/UpdateSamplingRecord', { m: this.editSamplingRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.getSamplingRecords();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });

            },
            //新增檢驗項目
            addSamplingRecord() {
                if (this.strEmpty(this.selSamplingName) || this.strEmpty(this.newSamplingRecord.Location) || this.strEmpty(this.newSamplingRecord.Quantity)) {
                    alert('項目,位置,數量(單位) 必須輸入');
                    return;
                }
                if (this.selSamplingName == '其他' && this.otherSamplingName == '') {
                    alert('項目 選擇其他, 請自行輸入');
                    return;
                }
                if (this.selSamplingName == '其他') {
                    this.newSamplingRecord.SamplingName = this.otherSamplingName;
                } else {
                    this.newSamplingRecord.SamplingName = this.selSamplingName;
                }
                this.newSamplingRecord.SuperviseEngSeq = this.targetId;
                window.myAjax.post('/ESSuperviseFill/UpdateSamplingRecord', { m: this.newSamplingRecord })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.otherSamplingName = '';
                            this.selSamplingName = '';
                            this.getNewSamplingRecord();
                            this.getSamplingRecords();
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });

            },
            //刪除檢驗項目
            onDelSamplingRecord(item) {
                if (this.editSamplingSeq > -99) return;
                if (confirm('是否確定刪除資料？')) {
                    window.myAjax.post('/ESSuperviseFill/DelSamplingRecord', { id: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.getSamplingRecords();
                            }
                            alert(resp.data.msg);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            //取消編輯
            onEditSamplingCancel() {
                this.getSamplingRecords();
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            },
        },
        mounted() {
            console.log('mounted() 督導填報-編輯');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                if (Number.isInteger(this.targetId)) {
                    this.getEngItem();
                    return;
                }

            }
            window.location = "/ESSuperviseFill";
        }
    }
</script>