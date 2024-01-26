<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-12 col-sm-1 mt-1">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-2 mt-2">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-2 mt-2">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-2">
                    <select v-model="selectEngName" @change="onEngNameChange($event)" class="form-control">
                        <option v-for="option in selectEngNameItems" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-2">
                    <select v-model="selectSubEngName" @change="onSubEngNameChange($event)" class="form-control">
                        <option v-for="option in selectSubEngNameItems" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
            </div>
        </form>
        <div class="row justify-content-between">
            <div class="form-inline col-12 col-md-8 mt-3">
                <label for="tableinfo" class="my-1 mr-2">
                    共
                    <span class="small-red">{{recordTotal}}</span>
                    筆，每頁顯示
                </label>
                <select v-model="pageRecordCount" @change="onPageRecordCountChange($event)" class="form-control sort">
                    <option value="30">30</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
                <label for="tableinfo" class="my-1 mx-2">筆，共<span class="small-red">{{pageTotal}}</span>頁，目前顯示第</label>
                <select v-model="pageIndex" @change="onPageChange($event)" class="form-control sort">
                    <option v-for="option in pageIndexOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
                <label for="tableinfo" class="my-1 mx-2">頁</label>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead>
                    <tr>
                        <th class="sort">項次</th>
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th class="number">執行機關</th>
                        <th>監造單位</th>
                        <th>分項工程名稱</th>
                        <th>缺失個數</th>
                        <th>編輯</th>
                        <th class="sort">不符合事項報告</th>
                        <th class="sort">NCR程序追蹤改善表</th>
                        <th class="sort">改善照片</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.subEngNameSeq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td><a v-on:click.stop="editEng(item)" href="#">{{item.EngName}}</a></td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.SupervisorUnitName}}</td>
                        <td>{{item.subEngName}}</td>
                        <td>{{item.missingCount}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editEng(item)" href="#" class="btn-block mx-2 btn btn-color2" title="編輯">編輯</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item, 44)" href="#" class="mx-2 btn btn-block btn-outline-secondary" title="下載">下載</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item, 45)" href="#" class="mx-2 btn btn-block btn-outline-secondary" title="下載">下載</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item, 46)" href="#" class="mx-2 btn btn-block btn-outline-secondary" title="下載">下載</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
     </div>
</template>
<script>
    export default {
        data: function () {
            return {
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',
                initFlag: 0,
                //分頁
                recordTotal:0,
                pageRecordCount: 30,
                pageTotal: 0,
                pageIndex: 0,
                pageIndexOptions:[],
                //選項
                selectYear: '',
                selectYearOptions:[],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //機關單位
                selectSubUnit: '',
                selectSubUnitOptions: [],
                //工程
                engNameFlag: false,
                selectEngName: -1,
                selectEngNameItems: [],
                //工程分項
                selectSubEngName: -1,
                selectSubEngNameItems: [],
                items: [],
                //
                editFlag: false,
            };
        },
        methods: {
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/SamplingInspectionRecImprove/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;
                        this.selectUnit = this.userUnit;
                        this.onUnitChange(this.selectUnit)
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/SamplingInspectionRecImprove/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    this.selectYear = this.selectYearOptions[0].Value;
                    this.onYearChange();
                }
            },
            async onYearChange(event) {
                this.items = [];
                //工程機關
                this.selectUnit = '';
                this.selectUnitOptions = [];
                this.selectSubUnit = '';
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                this.selectSubEngName = -1;
                this.selectSubEngNameItems = [];
                const { data } = await window.myAjax.post('/SamplingInspectionRecImprove/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                if (this.userUnit == null) this.getUserUnit();
            },
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;
                this.items = [];
                this.selectSubUnit = ''
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                this.selectSubEngName = -1;
                this.selectSubEngNameItems = [];
                const { data } = await window.myAjax.post('/SamplingInspectionRecImprove/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                if (this.initFlag == 0) {
                    this.initFlag == 1
                    this.selectSubUnit = this.userUnitSub;
                    this.onSubUnitChange();
                }
            },
            //
            onSubUnitChange(event) {
                this.pageIndex = 1;
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                this.selectSubEngName = -1;
                this.selectSubEngNameItems = [];
                this.getEngNameList();
                //this.getList();
            },
            onEngNameChange($event) {
                this.engNameFlag = true;
                this.selectSubEngName = -1;
                this.selectSubEngNameItems = [];
                this.getSubEngNameList();
                this.pageIndex = 1;
                this.getList();
            },
            onSubEngNameChange($event) {
                //this.engNameFlag = true;
                this.pageIndex = 1;
                this.getList();
            },
            onPageRecordCountChange(event) {
                this.setPagination();
                this.getList();
            },
            onPageChange(event) {
                this.getList();
            },
            //計算分頁
            setPagination() {
                this.pageTotal = Math.ceil(this.recordTotal / this.pageRecordCount);
                //
                this.pageIndexOptions = [];
                if (this.pageTotal == 0) {
                    this.pageIndex = 0;
                } else {
                    for (var i = 1; i <= this.pageTotal; i++) {
                        this.pageIndexOptions.push({ Text: i, Value: i });
                    }
                    if (this.pageIndex > this.pageIndexOptions.length) {
                        this.pageIndex = this.pageIndexOptions.length;
                    }
                }
            },
            //工程名稱清單
            getEngNameList() {
                if (this.selectYear == '' || this.selectUnit == ''|| this.selectSubUnit == '') return;
                window.myAjax.post('/SamplingInspectionRecImprove/GetEngNameList'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        engMain: this.selectEngName,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        //if (!this.engNameFlag) {
                            this.selectEngNameItems = resp.data.engNameItems;
                        //}
                        //this.recordTotal = resp.data.pTotal;
                        //this.setPagination();
                        //this.engNameFlag = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分項工程名稱清單
            getSubEngNameList() {
                if (this.selectEngName == '' || this.selectEngName == -1) return;
                window.myAjax.post('/SamplingInspectionRecImprove/GetSubEngNameList'
                    , {
                        engMain: this.selectEngName,
                    })
                    .then(resp => {
                            this.selectSubEngNameItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getList() {
                if (this.selectYear == '' || this.selectUnit == '' || this.selectSubUnit == '') return;
                window.myAjax.post('/SamplingInspectionRecImprove/GetList'
                    ,{
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        engMain: this.selectEngName,
                        subEngMain: this.selectSubEngName,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        //if (!this.engNameFlag) {
                        //    this.selectSubEngNameItems = resp.data.engNameItems;
                        //}
                        this.recordTotal = resp.data.pTotal;
                        this.setPagination();
                        //this.engNameFlag = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            download(item, mode) {
                window.myAjax.get('/SamplingInspectionRecImprove/SIRDownload?seq=' + item.subEngNameSeq+'&mode='+mode, { responseType: 'blob' })
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
            editEng(item) {
                window.myAjax.post('/SamplingInspectionRecImprove/EditEng', { seq: item.subEngNameSeq })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 抽驗缺失改善');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>
