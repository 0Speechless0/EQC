<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectEngName" @change="onEngNameChange($event)" class="form-control">
                        <option v-for="option in selectEngNameItems" v-bind:value="option.Value" v-bind:key="option.Value">
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
                        <th class="sort">排序</th>
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th class="number">執行機關</th>
                        <th>監造單位</th>
                        <th class="sort">材料數</th>
                        <th>審核進度</th>
                        <th class="number" colspan="2">材料設備總表</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.SupervisorUnitName}}</td>
                        <td class="text-center">{{item.EMDCount}}</td>
                        <td class="text-center">{{item.AuditProgress}}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="editEng(item)" class="tn-block mx-2 btn btn-color2" href="#" title="編輯">編輯</a>
                            </div>
                        </td>
                        <td class="text-center">
                            <a v-on:click.stop="onDownloadCheckSheet(item)" href="#" class="a-blue" title="下載">
                                <i class="fas fa-file-alt fa-2x"></i>
                            </a>
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
                selectYearOptions: [],
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
                items: [],
            };
        },
        components: {

        },
        methods: {
            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/SupervisionPlan/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;

                        if (sessionStorage.getItem('selectUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        } else {
                            this.selectUnit = sessionStorage.getItem('selectUnit');
                            this.initFlag = 2;
                            this.onUnitChange(this.selectUnit);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/EMDAudit/GetYearOptions');
                this.selectYearOptions = data;
                if (this.selectYearOptions.length > 0) {
                    if (sessionStorage.getItem('selectYear') == null) {
                        this.selectYear = this.selectYearOptions[0].Value;
                        this.onYearChange();
                    } else {
                        this.selectYear = sessionStorage.getItem('selectYear');
                        this.onYearChange();
                    }
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
                const { data } = await window.myAjax.post('/SupervisionPlan/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                if (this.userUnit == null) this.getUserUnit();
                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);
            },
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;
                this.items = [];
                this.selectSubUnit = ''
                this.selectSubUnitOptions = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                const { data } = await window.myAjax.post('/SupervisionPlan/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                if (this.initFlag == 0) {
                    this.initFlag = 1;
                    this.selectSubUnit = this.userUnitSub;
                    this.onSubUnitChange();
                }

                if (this.initFlag == 2) {
                    this.selectSubUnit = sessionStorage.getItem('selectSubUnit');
                    this.onSubUnitChange();
                }
                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);

            },
            //
            async onSubUnitChange(event) {
                this.items = [];
                this.selectEngName = -1;
                this.selectEngNameItems = [];
                this.pageIndex = 1;
                const { data } = await window.myAjax.post('/EMDAudit/GetEngNameOptions', { year: this.selectYear, unit: this.selectUnit, subUnit: this.selectSubUnit, engMain: this.selectEngName });
                this.selectEngNameItems = data;
                //有session
                if (this.initFlag == 2) {
                    this.initFlag = 1;
                    if (sessionStorage.getItem('selectEngName') != null) {
                        this.onEngNameChange(1);
                    } else {
                        this.initFlag = 1;
                        this.getList();
                    }
                } else {
                    this.initFlag = 1;
                    this.getList();
                }
                
                //
                sessionStorage.removeItem('selectSubUnit');
                window.sessionStorage.setItem("selectSubUnit", this.selectSubUnit);
            },
            onEngNameChange($event) {
                if ($event == 1) {
                    this.selectEngName = sessionStorage.getItem('selectEngName');
                }
                this.engNameFlag = true;
                this.pageIndex = 1;
                this.getList();

                //儲存到session
                sessionStorage.removeItem('selectEngName');
                window.sessionStorage.setItem("selectEngName", this.selectEngName);
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
            async getList() {
                if (this.selectYear == '' || this.selectUnit == '') return;

                window.myAjax.post('/EMDAudit/GetList'
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
                        //    this.selectEngNameItems = resp.data.engNameItems;
                        //}
                        this.recordTotal = resp.data.pTotal;
                        this.setPagination();
                        this.engNameFlag = false;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            editEng(item) {
                window.location = "/EMDAudit/EMDAuditEdit?id=" + item.Seq;
            },
            newItem() {
                window.location = "/EMDAudit/EMDAuditEdit?id=-1";
            },
            onDownloadCheckSheet(item) {
                this.downloadFile('/EMDAudit/DownloadReport?engMain=' + item.Seq + '&engNo=' + item.EngNo);
            },
            downloadFile(tarUrl) {
                console.log('tarUrl: ' + tarUrl);
                window.myAjax.get(tarUrl, { responseType: 'blob' })
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
            console.log('mounted() 材料送審管制');
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
        }
    }
</script>
