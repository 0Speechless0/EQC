<template>
    <div>
        <!-- form class="form-group">
            <div class="form-row">
                <div class="col-1">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option selected="selected" :value="-1"> 全部</option>
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="d-flex justify-content-between">
                    <button v-on:click.stop="getList" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
                </div>
                <div class="col-12">
                </div>
                <div class="d-flex justify-content-between">
                    <button class="btn btn-color11-1 btn-xs mx-1">
                        <i class="fas fa-download"></i>總表
                    </button>
                    <button class="btn btn-color11-1 btn-xs mx-1">
                        <i class="fas fa-download"></i>碳交易表
                    </button>
                </div>
            </div>
        </form -->
        <div class="d-flex bd-highlight">
            <div class="bd-highlight pr-1">
                <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                    <option selected="selected" :value="-1"> 全部</option>
                    <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight pr-1">
                <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                    <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                            v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight">
                <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                    <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                            v-bind:key="option.Value">
                        {{ option.Text }}
                    </option>
                </select>
            </div>
            <div class="bd-highlight d-flex justify-content-between">
                <button v-on:click.stop="getList" type="button" class="btn btn-outline-secondary btn-xs mx-1" data-dismiss="modal">查詢 <i class="fas fa-search"></i></button>
            </div>
            <div class="ml-auto bd-highlight d-flex justify-content-between">
                <!--
                <button class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-download"></i>總表</button>
                <button class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-download"></i>碳交易表</button>
                -->
            </div>
        </div>
        
        <div class="table-responsive">
            <table class="table table1 min910" border="0">
                <thead class="insearch">
                    <tr>
                        <th class="sort">排序</th>
                        <th class="number">工程編號</th>
                        <th>工程名稱</th>
                        <th>執行機關</th>
                        <th>執行單位</th>
                        <th>決標日期</th>
                        <th>核定碳排量</th>
                        <th>設計碳排量</th>
                        <th>交易碳排量</th>
                        <th class="text-center">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.ExecSubUnitName}}</td>
                        <td><span v-html="AwardDateCaption(item)"></span></td>
                        <td style="width: 10%;"><input v-model="item.ApprovedCarbonQuantity" type="text" class="form-control" disabled=""></td>
                        <td style="width: 10%;"><input v-model="item.CarbonDesignQuantity" type="text" class="form-control" disabled=""></td>
                        <td style="width: 10%;"><input v-model="item.CarbonTradeQuantity" type="text" class="form-control" disabled=""></td>
                        <td style="min-width: 105px;">
                            <div v-if="item.CarbonEmissionHeaderState==1 && item.CarbonTradeQuantity != null" class="row justify-content-center m-0">
                                <button @click="adjItemClick(item)" class="btn btn-color11-3 btn-xs mx-1" data-toggle="modal" data-target="#CarbonTrading" title="調整"> <i class="fas fa-pencil-alt"></i> 調整 </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- -->
        <div class="modal fade" id="CarbonTrading" data-backdrop="static" data-keyboard="false" style="display: none;" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered" style="max-width: fit-content;">
                <div class="modal-content">
                    <div class="modal-header bg-R ">
                        <h6 class="modal-title text-white">調整碳交易</h6>
                        <button @click="closeModal" id="btnCloseModal" type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="card whiteBG mb-4 pattern-F colorset_1">
                            <div class="tab-content">
                                <CarbonTrading ref="CarbonTradingModal" v-if="adjItem != null" v-bind:adjItem="adjItem" v-on:reload="reload"></CarbonTrading>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        components: {
            CarbonTrading: require('./CCT_CarbonTradingAdj.vue').default,
        },
        data: function () {
            return {
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',

                //選項
                selectYear: '',
                selectYearOptions: [],
                //機關
                selectUnit: '',
                selectUnitOptions: [],
                //機關單位
                selectSubUnit: -1,
                selectSubUnitOptions: [],
                items: [],
                adjItem: null,
            };
        },
        methods: {
            closeModal() {
                this.getList();
            },
            //調整
            adjItemClick(item) {
                this.adjItem = Object.assign({}, item);
            },
            //不同年度紅字
            AwardDateCaption(item) {
                if (item.DiffYear)
                    return '<font color="red">' + item.AwardDateStr+'</font>';
                else
                    return item.AwardDateStr;
            },
            reload() {
                document.getElementById('btnCloseModal').click();
            },
            //工程年分
            async getSelectYearOption() {
                const { data } = await window.myAjax.post('/TenderPlan/GetYearOptions');
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

                this.selectSubUnit = -1;
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetUnitOptions', { year: this.selectYear });
                this.selectUnitOptions = data;
                //if (this.userUnit == null) this.getUserUnit();

                sessionStorage.removeItem('selectYear');
                window.sessionStorage.setItem("selectYear", this.selectYear);
            },
            async onUnitChange(unitSeq) {
                if (this.selectUnitOptions.length == 0) return;

                this.items = [];
                this.selectSubUnit = -1
                this.selectSubUnitOptions = [];
                const { data } = await window.myAjax.post('/TenderPlan/GetSubUnitOptions', { year: this.selectYear, parentSeq: unitSeq });
                this.selectSubUnitOptions = data;
                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);

            },
            //
            onSubUnitChange(event) {
                //this.getList();
                sessionStorage.removeItem('selectSubUnit');
                window.sessionStorage.setItem("selectSubUnit", this.selectSubUnit);

            },
            async getList() {
                this.adjItem = null;
                if (this.selectYear == '' || this.selectUnit == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                window.myAjax.post('/EQMCarbonEmission/GetCarbonTradeEngList'
                    , {
                        year: this.selectYear,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            download(item) {
                window.myAjax.get('/TenderPlan/DownloadPccesXML?id=' + item.Seq, { responseType: 'blob' })
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
            console.log('mounted() 碳交易');
            this.getSelectYearOption();
        }
    }
</script>