<template>
    <div>
        <form class="form-group">
            <div class="form-row">
                <div class="col-1 mt-3">
                    <select v-model="selectYear" @change="onYearChange($event)" class="form-control">
                        <option selected="selected" :value="-1"> 全部</option>
                        <option v-for="option in selectYearOptions" v-bind:value="option.Value" v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectUnit" @change="onUnitChange(selectUnit)" class="form-control">
                        <option v-for="option in selectUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-12 col-sm-3 mt-3">
                    <select v-model="selectSubUnit" @change="onSubUnitChange($event)" class="form-control">
                        <option v-for="option in selectSubUnitOptions" v-bind:value="option.Value"
                                v-bind:key="option.Value">
                            {{ option.Text }}
                        </option>
                    </select>
                </div>
                <div class="col-2 col-sm-2">
                    <label for="council" style="
                    float: right;
                    padding-top: 3px;
                " class="m-2">勾稽工程會</label>
                </div>
                <div class="col-2 col-sm-2 mt-3">
                    <select id="council" class="form-control" @change="onCouncilOptionChange($event)" v-model="hasCouncilOption">
                        <option :value="-1"> 全部</option>
                        <option :value="0"> 有 </option>
                        <option :value="1"> 無 </option>
                    </select>
                </div>
            </div>
        </form>
        <div class="row justify-content-between">
            <comm-pagination class="col-10" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
            <div class="col-auto mr-3">
                <div class="row justify-content-between">
                    <button @click="copyEngDlg()" data-toggle="modal"
                            data-target="#refCopyEngModal" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                        <i class="fas fa-plus"></i>&nbsp;複製工程
                    </button>
                    <button v-on:click.stop="newItem()" role="button" class="btn btn-outline-secondary btn-xs mx-1">
                        <i class="fas fa-plus"></i>&nbsp;新增工程
                    </button>
                </div>
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
                        <th>勾稽工程會</th>
                        <th>生態檢核</th>
                        <th>Pcces檔案</th>
                        <th class="text-center">明細</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td>{{pageRecordCount*(pageIndex-1)+index+1}}</td>
                        <td>{{item.EngNo}}</td>
                        <td>{{item.EngName}}</td>
                        <td>{{item.ExecUnit}}</td>
                        <td>{{item.ExecSubUnit}}</td>
                        <td><span v-html="hasCouncil(item)"></span></td>
                        <td>{{ item.IsEcPrepare ? '是' : '否' }}</td>
                        <td>
                            <button v-if="item.PccesXMLDateStr != ''" @click="download(item)"
                                    class="btn btn-color11-1 btn-xs mx-1">
                                <i class="fas fa-download"></i> 下載<br />{{item.PccesXMLDateStr}}
                            </button>
                        </td>
                        <td style="min-width: 105px;">
                            <div class="row justify-content-center m-0">
                                <!-- <button v-if="!(item.DocState==null || item.DocState==-1)"
                                v-on:click.stop="editEng(item)" class="btn btn-outline-secondary btn-xs mx-1"
                                title="查看">
                                <i class="fas fa-eye"></i> 查看
                            </button> -->
                                <button v-on:click.stop="editEng(item)" class="btn btn-color11-3 btn-xs mx-1 mt-1" title="編輯">
                                    <i class="fas fa-pencil-alt"></i> 編輯
                                </button>
                                <button v-on:click.stop="editTenderPlan(item)" class="btn btn-color11-3 btn-xs mr-1 mt-1"><i class="fas fa-pencil-alt"> 管考</i></button>
                                <PackageDownload  btnclass="btn btn-color11-3 btn-xs mr-1 mt-1" :systemType="10" :seq="item.Seq"></PackageDownload>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 複製工程 s20230623 -->
        <div class="modal fade" id="refCopyEngModal" data-backdrop="static" data-keyboard="false" tabindex="-1"
             aria-labelledby="refCopyEngModal" aria-modal="true">
            <div class="modal-dialog modal-lx modal-dialog-centered ">
                <div class="modal-content">
                    <div class="modal-header bg-0 text-white">
                        <h6 class="modal-title" id="projectUpload">工程複製</h6>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-4">複製工程編號</div>
                            <div class="col-8"><input v-model.trim="srcEngNo" maxlength="30" @change="onSrcEngNoChange" class="form-control" type="text" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-4">新的工程編號</div>
                            <div class="col-8"><input v-model.trim="newEngNo" maxlength="30" class="form-control" type="text" /></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button @click="copyEng" type="button" class="btn btn-primary">
                            複製
                        </button>
                        <button type="button" id="closeCopyEngModal" class="btn btn-secondary" data-dismiss="modal" aria-label="Close">
                            取消
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <EngApprovalList ref="engApprovalList"></EngApprovalList>
    </div>
</template>
<script>

    export default {
        components: {
            PackageDownload : require("../../FilePackage/Modal.vue").default,
            EngApprovalList: require('./EngApprovalList.vue').default, //s20231006
        },
        data: function () {
            return {
                tarEdit : "Edit3",
                //使用者單位資訊
                userUnit: null,
                userUnitSub: '',
                initFlag: 0,
                hasCouncilOption: -1,
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
                //pageIndexOptions:[],
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
                //s20230623
                srcEngNo:'',
                newEngNo: '',
            };
        },
        methods: {
                        //編輯工程
            editTenderPlan(item) {
                window.sessionStorage.setItem(window.epcSelectTrenderSeq, item.Seq);
                window.myAjax.post('/EPCTender/EditTenderPlan', { seq: item.Seq, tarEdit: this.tarEdit })
                    .then(resp => {
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //s20230623 複製工程
            openEngDlg() {
                this.srcEngNo = '';
                this.newEngNo = '';
            },
            onSrcEngNoChange() {
                if (this.newEngNo == '' && this.srcEngNo != '') {
                    this.newEngNo = this.srcEngNo+'-99';
                }
            },
            copyEng() {
                if (this.newEngNo == '' || this.srcEngNo == '') {
                    alert('工程編號必須輸入');
                    return;
                }
                window.myAjax.post('/TenderPlan/CopyEng', { sNo: this.srcEngNo, eNo: this.newEngNo })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            document.getElementById('closeCopyEngModal').click();
                            this.getList();
                        }
                        alert(resp.data.msg);

                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            //取得使用者單位資訊
            getUserUnit() {
                window.myAjax.post('/TenderPlan/GetUserUnit')
                    .then(resp => {
                        this.userUnit = resp.data.unit;
                        this.userUnitSub = resp.data.unitSub;

                        if (sessionStorage.getItem('selectUnit') == null) {
                            this.selectUnit = this.userUnit;
                            this.onUnitChange(this.selectUnit);
                        } else {
                            this.GetSession();
                        }

                    })
                    .catch(err => {
                        console.log(err);
                    });
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
                if (this.userUnit == null) this.getUserUnit();

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
                if (this.initFlag == 0) {
                    this.initFlag == 1
                    this.selectSubUnit = this.userUnitSub;
                    this.onSubUnitChange();
                }

                if (this.initFlag == 2) {
                    this.initFlag == 1
                    this.selectSubUnit = sessionStorage.getItem('selectSubUnit');
                    this.onSubUnitChange();
                }
                //儲存到session
                sessionStorage.removeItem('selectUnit');
                window.sessionStorage.setItem("selectUnit", this.selectUnit);

            },
            //
            onSubUnitChange(event) {
                this.pageIndex = 1;
                this.getList();
                sessionStorage.removeItem('selectSubUnit');
                window.sessionStorage.setItem("selectSubUnit", this.selectSubUnit);

            },
            onPaginationChange(pInx, pCount) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
            onCouncilOptionChange(event) {
                //console.log("pInx:" + this.$refs['pagination'].pageIndex + " pCount:" + pCount);
                this.getList();
            },
            async getList() {
                if (this.selectYear == '' || this.selectUnit == '') return;
                if (this.selectSubUnit == null || this.selectUnit == '') this.selectSubUnit = -1;
                this.$refs.engApprovalList.getList(this.selectUnit);//s20231106
                window.myAjax.post('/TenderPlan/GetList'
                    , {
                        year : this.selectYear,
                        hasCouncil: this.hasCouncilOption,
                        unit: this.selectUnit,
                        subUnit: this.selectSubUnit,
                        pageRecordCount: this.pageRecordCount,
                        pageIndex: this.pageIndex
                    })
                    .then(resp => {
                        this.items = resp.data.items;
                        this.recordTotal = resp.data.pTotal;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            editEng(item) {
                //window.location = "/TenderPlan/TenderPlanEdit?id=" + item.Seq;
                window.myAjax.post('/TenderPlan/EditEng', { seq: item.Seq })
                    .then(resp => {
                        window.sessionStorage.setItem(window.eqSelTrenderPlanSeq, item.Seq);
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            newItem() {
                window.myAjax.post('/TenderPlan/EditEng', { seq: -1 })
                    .then(resp => {
                        console.log(resp.data.Url);
                        window.location.href = resp.data.Url;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            GetSession() {
                //
                this.selectUnit = sessionStorage.getItem('selectUnit');
                this.onUnitChange(this.selectUnit);
                //
                this.initFlag = 2;
                this.selectSubUnit = sessionStorage.getItem('selectSubUnit');
                this.onSubUnitChange(this.selectSubUnit);

            },
            //下載
            download(item) {
                window.comm.dnFile('/TenderPlan/DownloadPccesXML?id=' + item.Seq);
                /*window.myAjax.get('/TenderPlan/DownloadPccesXML?id=' + item.Seq, { responseType: 'blob' })
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
                    });*/
            },
            hasCouncil(item) {
                return item.PrjXMLSeq != null ? "有" : "<font color='red'>無</font>"
            }
        },
        async mounted() {
            console.log('mounted 建立標案' + this.userUnit + ' ' + this.userUnitSub);
            if (this.selectYearOptions.length == 0) {
                this.getSelectYearOption();
            }
            console.log('mounted() 工程標案清單 ' + this.tarEdit);
            //s20230517
            if (document.getElementById('leftMenuBody').offsetLeft >= 0) {
                setTimeout(function () {
                    document.getElementById('leftMenuTab').click();
                }, 150);
            }
            document.getElementById('leftMenuBody').style.display = "none"; //s20230524
        }
    }
</script>
