<template>
    <div>
        <div class="insearch mb-3">
            <div class="form-row">
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <div class="form-group mb-0">
                        <label>督導日期</label>
                        <select v-model="selectSuperviseDate" @change="onSuperviseDateChange($event)" class="form-control">
                            <option v-for="option in selectSuperviseDateOption" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <div class="form-group mb-0">
                        <label>單位</label>
                        <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                            <option v-for="option in selectUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-auto mb-3 mb-sm-0">
                    <div class="form-group mb-0">
                        <label>工程名稱</label>
                        <select v-model="selectTender" @change="onEngNameChange" class="form-control">
                            <option v-for="option in selectTenderNameOption" v-bind:value="option.Value" v-bind:key="option.Value">
                                {{ option.Text }}
                            </option>
                        </select>
                    </div>
                </div>
            </div>
        </div>
        <div v-if="tenderItem != null">
            <h5 class="insearch mt-0 py-2">
                工程：{{tenderItem.EngName}}({{tenderItem.EngNo}})<br>標案：{{tenderItem.TenderName}}({{tenderItem.TenderNo}})<br />
                {{tenderItem.DurationCategory}}：{{tenderItem.EngPeriod}}天、{{tenderItem.StartDateStr}} ~ {{tenderItem.SchCompDateStr}}
            </h5>
            <div class="btn-toolbar my-2" role="toolbar">
                <div class="btn-group mr-2" role="group">
                    <button @click="downloadPlan" type="button" class="btn btn-outline-secondary btn-sm" title="瀏覽監造計畫書">瀏覽監造計畫書</button>
                    <button @click="downloadQuality" type="button" class="btn btn-outline-secondary btn-sm" title="瀏覽品質計畫書">瀏覽品質計畫書</button>
                    <button @click="viewChapterPhoto" type="button" class="btn btn-outline-secondary btn-sm" title="瀏覽工程設計圖說" href="inside02_b_committee design diagram.html">瀏覽工程設計圖說</button>
                </div>
                <div class="btn-group mr-2" role="group">
                    <button @click="downloadCDaily" type="button" class="btn btn-outline-secondary btn-sm" title="施工日誌">施工日誌</button>
                    <button @click="downloadSDaily" type="button" class="btn btn-outline-secondary btn-sm" title="監造報表">監造報表</button>
                </div>
            </div>
            <h5>施工抽查成果統計</h5>
            <div class="table-responsive">
                <table class="table table-responsive-md table-hover">
                    <thead>
                        <tr class="insearch">
                            <th rowspan="2" style="width:60px"><strong>序號</strong></th>
                            <th rowspan="2"><strong>抽查項目</strong></th>
                            <th rowspan="2" style="width:80px"><strong>抽驗次數</strong></th>
                            <th colspan="2" class="text-center"><strong>抽驗結果</strong></th>
                            <th rowspan="2" style="width:80px"><strong>合格率</strong></th>
                        </tr>
                        <tr class="insearch">
                            <th class="text-center" style="width:80px"><strong>合格</strong></th>
                            <th class="text-center" style="width:80px"><strong>不合格</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in constructionItems" v-bind:key="index">
                            <td>{{index+1}}</td>
                            <td>{{item.checkName}}</td>
                            <td class="text-center"><a @click="openCheck" href="##" class="a-3 my-1">{{item.totalRec}}</a></td>
                            <td class="text-center">{{item.okCount}}</td>
                            <td class="text-center">{{item.totalRec - item.okCount}}</td>
                            <td class="text-center">{{Math.round(item.okCount*10000/item.totalRec)/100}}%</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <h5>其他統計表</h5>
            <div class="table-responsive">
                <table class="table table-responsive-md table-hover">
                    <thead>
                        <tr class="insearch">
                            <th rowspan="2" style="width:60px"><strong>序號</strong></th>
                            <th rowspan="2"><strong>抽查項目</strong></th>
                            <th rowspan="2" style="width:80px"><strong>抽驗次數</strong></th>
                            <th colspan="2" class="text-center"><strong>抽驗結果</strong></th>
                            <th rowspan="2" style="width:80px"><strong>合格率</strong></th>
                        </tr>
                        <tr class="insearch">
                            <th class="text-center" style="width:80px"><strong>合格</strong></th>
                            <th class="text-center" style="width:80px"><strong>不合格</strong></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in otherItems" v-bind:key="index">
                            <td>{{index+1}}</td>
                            <td>{{item.checkName}}</td>
                            <td class="text-center"><a @click="openCheck" href="##" class="a-3 my-1">{{item.totalRec}}</a></td>
                            <td class="text-center">{{item.okCount}}</td>
                            <td class="text-center">{{item.totalRec - item.okCount}}</td>
                            <td class="text-center">{{Math.round(item.okCount*10000/item.totalRec)/100}}%</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <h5>工程：{{tenderItem.EngName}}({{tenderItem.EngNo}})<br>標案：{{tenderItem.TenderName}}({{tenderItem.TenderNo}})</h5>
            <div class="row pics">
                <div v-for="(item, index) in photoItems" v-bind:key="index" class="col-12 col-md-6 col-xl-3 mb-4">
                    <div class="card">
                        <img v-bind:src="getPhotoPath(item)" style="width:198px; height:150px;">
                        <div class="card-body">
                            <p class="card-text">{{item.Memo}}</p>
                            <input v-model="item.RESTful" type="text" class="form-control">
                            <a @click="viewPhoto(item)" href="javascript:void(0)" class="card-link a-view" data-toggle="modal" data-target="#BIGpic_01" title="檢視"><i class="fas fa-eye"></i></a>
                            <a @click="savePhoto(item)" href="javascript:void(0)" class="card-link a-edit" title="儲存"><i class="fas fa-save"></i></a>
                            <!--

    <a href="javascript:void(0)" class="card-link a-delete" title="刪除"><i class="fas fa-trash-alt"></i></a>
    -->

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 小視窗 看大圖 -->
        <div class="modal fade" id="BIGpic_01">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div v-if="photoItem != null" class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title font-weight-bold">{{photoItem.Memo}}[{{photoItem.state}}]</h6>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <img v-bind:src="getPhotoPath(photoItem)" style="max-height:600px;" class="w-100">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color3" data-dismiss="modal">關閉</button>
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
                //督導日期
                selectSuperviseDate: '',
                selectSuperviseDateOption: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //工程
                selectTender: '',
                selectTenderNameOption: [],
                tenderItem: null,
                //抽查成果統計
                sysDate: '',
                constructionItems: [],
                otherItems: [],
                photoItems: [],
                photoItem: null,
                rootPath:'',
            };
        },
        methods: {

            //開啟抽驗單
            openCheck() {
                window.myAjax.post('/ESCommittee/SamplingInspectionRecEdit', { id: this.selectTender })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //瀏覽工程設計圖說
            viewChapterPhoto() {
                window.myAjax.post('/ESCommittee/ViewChapterPhoto', { id: this.selectTender })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載 品質計畫書
            downloadQuality() {
                let action = '/QualityPlan/DownloadQualityPlanExt?seq=' + this.selectTender+"&ext=pdf";
                this.downloadDoc(action);
            },
            //下載 監造計畫書
            downloadPlan() {
                let action = '/SupervisionPlan/PlanDownloadExt?seq=' + this.selectTender + "&ext=pdf";
                this.downloadDoc(action);
            },
            //下載 監造日報
            downloadSDaily() {
                let action = '/ESCommittee/DownloadSDaily?id=' + this.selectTender + '&tarDate=' + this.sysDate;
                this.downloadDoc(action);
            },
            //下載 施工日報
            downloadCDaily() {
                let action = '/ESCommittee/DownloadCDaily?id=' + this.selectTender + '&tarDate=' + this.sysDate;
                this.downloadDoc(action);
            },
            downloadDoc(action) {
                window.myAjax.get(action, { responseType: 'blob' })
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
            
            //照片
            savePhoto(item) {
                window.myAjax.post('/ESCommittee/SavePhoto', { p: item })
                    .then(resp => {
                        alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            viewPhoto(item) {
                this.photoItem = item;
            },
            getPhotoPath(item) {
                return this.rootPath + '/' + item.fName;
            },
            //督導日期
            async getSuperviseDateOption() {
                this.tenderItem = null;
                const { data } = await window.myAjax.post('/ESCommittee/GetSuperviseDateOptions');
                this.selectSuperviseDateOption = data;
            },
            async onSuperviseDateChange() {
                this.tenderItem = null;
                this.selectUnit = '';
                this.selectUnitOptions = [];
                this.selectTender = '';
                this.selectTenderNameOption = [];
                this.constructionItems = [];
                this.otherItems = [];
                this.photoItems = [];
                const { data } = await window.myAjax.post('/ESCommittee/GetUnitOptions', { date: this.selectSuperviseDate });
                this.selectUnitOptions = data;
            },
            async onUnitChange() {
                if (this.selectUnitOptions.length == 0) return;
                this.tenderItem = null;
                this.selectTender = '';
                this.selectTenderNameOption = [];
                this.constructionItems = [];
                this.otherItems = [];
                this.photoItems = [];
                const { data } = await window.myAjax.post('/ESCommittee/GetTenderOptions', { date: this.selectSuperviseDate, unit:this.selectUnit });
                this.selectTenderNameOption = data;
            },
            onEngNameChange() {
                this.tenderItem = null;
                this.constructionItems = [];
                this.otherItems = [];
                this.photoItems = [];
                window.myAjax.post('/ESCommittee/GetTender', { id: this.selectTender })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.constructionItems = resp.data.cItems;
                            this.otherItems = resp.data.oItems;
                            this.photoItems = resp.data.pItems;
                            this.sysDate = resp.data.date;
                            this.rootPath = resp.data.rPath;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 委員督導');
            this.getSuperviseDateOption();
        }
    }
</script>