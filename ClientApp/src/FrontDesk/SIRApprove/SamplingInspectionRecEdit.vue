<template>
    <div>
        <EngInfo v-bind:engMain="engMain"></EngInfo>
        <form @submit.prevent>
            <h2>抽查填報</h2>
            <div class="form-inline col-12 col-md-8 my-2">
                <label id="InspectionDate" class="my-1 mr-2">抽查日期：</label>
                <select v-model="searchCheckType" @change="onSearchCheckTypeChange" class="form-control mb-2 mr-sm-2">
                    <option v-for="option in recCheckTypeOption" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Value==1 ? '施工抽查' : (option.Value==2 ? '設備運轉測試' : (option.Value==3 ? '職業安全' : '生態保育'))  }}
                    </option>
                </select>
                <select v-model="selectRecItem" @change="onSelectRecItemChange" class="form-control sort">
                    <option v-for="option in selectRecItemOption" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
                <button v-on:click="getRecHeader(selectRecItem)" class="btn btn-color3 mb-2 mr-sm-2" type="button"><i class="fas fa-search"></i></button>
            </div>
            <label class="m-1 small-green">* 關鍵字搜尋可尋找抽查管理標準名稱</label>
            <div class="">
                <table class="table table2 min720 my-0" border="0">
                    <tbody>
                        <tr>
                            <th>檢查日期</th>
                            <td>
                                <div class="form-inline">
                                    <b-input-group class="mydatewidth">
                                        <input v-bind:value="chsRecDate" @change="onDateChange(chsRecDate, $event)" type="text" class="form-control" placeholder="yyy/mm/dd">
                                        <b-form-datepicker v-model="si.CCRCheckDate" :hide-header="1==1"
                                                           button-only right size="sm" @context="onDatePicketChange($event)">
                                        </b-form-datepicker>
                                    </b-input-group>
                                    <!-- input type="text" class="form-control mb-2 mr-sm-2" id="checkDate" value="110/03/02" -->
                                    <select v-model="si.CCRCheckType1" @change="onCheckTypeChange" class="form-control mb-2 mr-sm-2">
                                        <option value="1">施工抽查</option>
                                        <option value="2">設備運轉測試</option>
                                        <option value="3">職業安全</option>
                                        <option value="4">生態保育</option>
                                    </select>
                                    <select v-model="si.CCRCheckFlow" @change="onCheckFlowChange" class="form-control mb-2 mr-sm-2">
                                        <option value="1">施工前</option>
                                        <option value="2">施工中</option>
                                        <option value="3">施工後</option>
                                    </select>
                                    <select v-model="si.ItemSeq" @change="onStdChange" class="form-control mb-2 mr-sm-2">
                                        <option v-for="option in ItemSeqOption" v-bind:value="option.Value" v-bind:key="option.Value">
                                            {{ option.Text }}
                                        </option>
                                    </select>

                                    <input v-model.trim="keyWord" type="text" class="form-control mb-2 mr-sm-2" placeholder="請輸入關鍵字" aria-label="關鍵字搜尋">
                                    <button v-on:click="onSearchCheckType(keyWord)" class="btn btn-color3 mb-2 mr-sm-2" type="button"><i class="fas fa-search"></i></button>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <th>檢查位置</th>
                            <td>
                                <div class="form-inline">
                                    <!--
                                    <label id="InspectionDate" class="my-1 mr-2">經度：</label>
                                    <input v-model="si.CCRPosLati" @change="onResultChange" type="text" class="form-control mb-2 mr-sm-2">
                                    <label id="InspectionDate" class="my-1 mr-2">緯度：</label>
                                    <input v-model="si.CCRPosLong" @change="onResultChange" type="text" class="form-control mb-2 mr-sm-2">
                                    <button class="btn btn-color3 mb-2 mr-sm-2" type="button">重新抓取所在位置</button>
                                                -->
                                    <input v-model="si.CCRPosDesc" @change="onResultChange" type="text" class="form-control w-100 mb-2 mr-sm-2" placeholder="輸入位置描述">
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="isNew" class="row justify-content-center">
                <div class="col-6 col-lg-6 mt-3">
                    <button v-on:click="onNewRec" role="button" class="btn btn-shadow btn-color1 btn-block">
                        <i class="fas fa-plus"></i>&nbsp;&nbsp;新增抽查
                    </button>
                </div>
            </div>
            <label class="m-1 small-green">* 下方為自動帶出抽查管理標準的項目</label>
            <div class="table-responsive mb-4">
                <table class="table table1 min910 my-0 tableLayoutFixed" border="1">
                    <thead>
                        <tr>
                            <th style="width:150px">管理項目</th>
                            <th style="width:200px">抽查標準（定量定性）</th>
                            <th style="width:200px" v-if="!isNew">實際抽查情形</th>
                            <th v-if="!isNew" style="width:120px">抽查結果</th>
                            <th v-if="!isNew">照片</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in items" v-bind:key="item.ControllStSeq">
                            <td v-if="item.rowSpan>0" v-bind:rowspan="item.rowSpan">{{item.CheckItem1}}</td>
                            <td v-if="item.CheckFields>=0 && item.rowSpanStd1>0" v-bind:rowspan="item.rowSpanStd1" >{{item.Stand1}}</td>
                            <td v-if="!isNew && item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                                <div contenteditable="true" aria-multiline="true">
                                    <textarea v-model="item.CCRRealCheckCond" @change="onResultChange(item)" class="form-control" v-bind:rows="item.rowSpan==1 ? 3 : 6"></textarea>
                                </div>
                            </td>
                            <td v-if="!isNew && item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                                <div>
                                    <select v-model="item.CCRCheckResult" @change="onResultChange(item)" class="form-control">
                                        <option v-for="option in getCheckResultOption(item)" v-bind:value="option.Value" v-bind:key="option.Value">
                                            {{ option.Text }}
                                        </option>
                                    </select>
                                    <div class="custom-control custom-checkbox">
                                        <input v-model="item.CCRIsNCR" @change="onResultChange(item)" v-bind:id="'check_NCR_'+index" type="checkbox" class="custom-control-input">
                                        <label class="custom-control-label" v-bind:for="'check_NCR_'+index">NCR</label>
                                    </div>
                                    <button v-on:click="uploadPhotoModal(item)" role="button" class="btn btn-shadow btn-color1 btn-block">
                                        上傳照片
                                    </button>
                                </div>
                            </td>
                            <td v-if="!isNew && item.rowSpan>0" v-bind:rowspan="item.rowSpan">
                                <PhotoList v-bind:engMain="engMain.Seq" v-bind:si="si" v-bind:ctlSeq="item.ControllStSeq" v-bind:ref="'photoList'+item.Seq"></PhotoList>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </form>

        <div v-if="!isNew" class="row justify-content-center">
            <div class="col-6 col-lg-6 mt-3">
                <button v-on:click="onUpdateRec" role="button" class="btn btn-shadow btn-color1 btn-block">
                    儲存
                </button>
            </div>
            <div class="col-3 col-lg-2 mt-3">
                <button v-on:click="onFormConfirm" v-bind:disabled="si.FormConfirm==1" v-bind:class="{'btn-color1':!(si.FormConfirm==1), 'btn-color3':(si.FormConfirm==1)}" role="button" class="btn btn-shadow btn-block">
                    確認
                </button>
            </div>
            <div class="col-3 col-lg-2 mt-3">
                <button v-on:click="onDelRec" role="button" class="btn btn-shadow btn-danger btn-block">
                    刪除
                </button>
            </div>
        </div>
        <div v-if="isNew" class="row justify-content-center">
            <div class="col-6 col-lg-6 mt-3">
                <button v-on:click="onNewRec" role="button" class="btn btn-shadow btn-color1 btn-block">
                    <i class="fas fa-plus"></i>&nbsp;&nbsp;新增抽查
                </button>
            </div>
        </div>

        <!-- PhotoList v-if="!isNew && items.length>0 && si.Seq>-1" v-bind:engMain="engMain.Seq" v-bind:si="si" ref="photoList"></PhotoList -->

        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
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
                        <button type="button" class="btn btn-color3" data-dismiss="modal" v-on:click="closeModal()">Close</button>
                        <button type="button" class="btn btn-color1" v-on:click.stop="upload()">上傳</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                fResultChange: false,    
                targetItem: { Name: '' },

                isNew: true,
                keyWord:'',
                //選項
                chsRecDate: '',
                recCheckTypeOption:[],
                ItemSeqOption: [], //檢驗標準項目清單
                
                //表頭資料
                si: {
                    Seq:-1,
                    EngConstructionSeq:-1,
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
                searchCheckType: -1,
                searchCheckTypeOld: -1,
                selectRecItem: -1,
                selectRecItemOption: [],
                fGetRecMode:false,
                //檔案上傳
                modalShow: false,
                photoItem: null,
                targetInput: {},
                files: new FormData(),
                photoDesc:'',
                //
                targetId: -1,
                engMain: {},
                items:[]
            };
        },
        components: {
            EngInfo: require('./EngInfo.vue').default,
            PhotoList: require('./PhotoList.vue').default,
        },
        methods: {
            //抽查結果選項
            getCheckResultOption(item) {
                var option = [
                    { Value: 1, Text: '檢查合格' }, { Value: 2, Text: '有缺失' }
                ];
                if (item.CheckItem1.indexOf('☆檢驗停留點')==-1)
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
                        } else {
                            alert(resp.data.message);
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
                window.myAjax.post('/SamplingInspectionRec/GetStdTypeOption', { engMain: this.engMain.Seq, checkType: this.si.CCRCheckType1 })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.ItemSeqOption = resp.data.item;
                            if(this.fGetRecMode) this.getRecResult();
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
                    , { engMain: this.engMain.Seq, checkType: this.si.CCRCheckType1, stdType: this.si.ItemSeq, checkFlow: this.si.CCRCheckFlow })
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
                    , { engMain: this.engMain.Seq, keyWord: keyWord })
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
                    if(item) item.ResultItem = 1;
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
                if (!this.isExistDate(event.target.value)) {
                    event.target.value = srcDate;
                    alert("日期錯誤");
                } else {
                    this.si.CCRCheckDate = this.toYearDate(event.target.value);
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
            onNewRec() {
                if (this.items.length == 0) {
                    alert('無檢驗項目無法新增抽查');
                    return;
                }
                if (this.si.CCRPosDesc == null || this.si.CCRPosDesc.length==0) {
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
                window.myAjax.post('/SamplingInspectionRec/UpdateRec', { recItem: this.si, items:this.items })
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
                if (confirm('是否 確認此抽查單?')) {
                    window.myAjax.post('/SamplingInspectionRec/FormConfirm', { id: this.si.Seq })
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

                window.myAjax.post('/SamplingInspectionRec/GetRec', { recSeq: id })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.si = resp.data.item;
                            this.chsRecDate = this.si.chsCheckDate;
                            this.si.CCRCheckDate = this.toYearDate(this.chsRecDate);
                            this.srcSi = JSON.parse(JSON.stringify(this.si));
                            this.fGetRecMode = true;
                            this.onCheckTypeChange();
                            //this.getRecResult();
                            //this.isNew = false;
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
                window.myAjax.post('/SamplingInspectionRec/GetRecResult', { rec: this.si })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.isNew = false;
                            this.items = resp.data.item;
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
                this.fGetRecMode = false;
            },
            //檢驗日期變更
            onSearchCheckTypeChange() {
                if (!this.noSaveAlert()) {
                    this.searchCheckType = this.searchCheckTypeOld;
                    return;
                }
                this.searchCheckTypeOld = this.searchCheckType;
                if (this.isNew) this.selectRecItem = -1;
                this.selectRecItemOption = [];
                window.myAjax.post('/SamplingInspectionRec/GetRecOptionByCheckType', { constructionSeq: this.targetId, checkTypeSeq: this.searchCheckType })
                    .then(resp => {
                        this.selectRecItemOption = resp.data;
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
                    window.myAjax.post('/SamplingInspectionRec/DelRec', { id:this.si.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.isNew = true;
                                this.items = [];
                                this.getRecCheckTypeOption();
                                this.onSearchCheckTypeChange();
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
                this.photoItem = item;
                this.modalShow = true;
                //v-bind:class="{show:modalShow, sm:modalShow}"
                //this.$refs['MyDialog'].classList.add('show');
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
                files.append("engMain", this.engMain.Seq);
                files.append("recSeq", this.photoItem.ConstCheckRecSeq);
                files.append("ctlSeq", this.photoItem.ControllStSeq);
                files.append("photoDesc", this.photoDesc);
                window.myAjax.post('/SamplingInspectionRec/PhotoUpload1', files,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    }).then(resp => {
                        if (resp.data.result == 0) {
                            this.closeModal();
                            this.$refs['photoList' + this.photoItem.Seq][0].refresh();
                        }
                        alert(resp.data.message);
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 抽查紀錄填報-編輯' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                this.si.EngConstructionSeq = this.targetId;
                console.log(this.targetId);
                if (Number.isInteger(this.targetId) && this.targetId > 0) {
                    var Today = new Date();
                    this.si.CCRCheckDate = Today;
                    this.chsRecDate = Today.getFullYear()-1911 + "/" + (Today.getMonth() + 1) + "/" + Today.getDate();
                    this.getEngItem();
                    return;
                }
            }
            window.location = "/FrontDesk";
        }
    }
</script>