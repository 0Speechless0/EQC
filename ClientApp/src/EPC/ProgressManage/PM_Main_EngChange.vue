 <template>
     <div>
         <h5 class="insearch mt-0 py-2">
             <div class="form-row mb-1">
                 <div class="col-12 my-1">
                    編號：{{tenderItem.EngNo}} <br>
                     名稱：{{tenderItem.EngName}}<br>
                     預定進度:{{tenderItem.SchProgress}}
                     &nbsp;實際進度:{{tenderItem.AcualProgress}}<br>
                {{tenderItem.DurationCategory}}：{{tenderItem.EngPeriod}}天、{{tenderItem.StartDateStr}}
                    <span v-if="tenderItem.prjSchStartDateStr  && tenderItem.prjSchStartDateStr != Com.ToROCDate(tenderItem.schOrgStartDate)" style="color:purple"> ( {{tenderItem.prjSchStartDateStr }})</span>
                     ~ {{tenderItem.SchCompDateStr}}
                     <span v-if="tenderItem.prjSchCompDateStr && tenderItem.prjSchCompDateStr != Com.ToROCDate(tenderItem.schOrgEndDate)" style="color:purple"> ( {{tenderItem.prjSchCompDateStr   }})</span>
                     <button v-if="spHeader.PccesXMLDateStr != null && spHeader.PccesXMLDateStr != ''" @click="downloadPcces(spHeader)" class="btn btn-color11-1 btn-xs mx-1">
                         <i class="fas fa-download"></i> Pcces下載({{spHeader.PccesXMLDateStr}})
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='ScheEngChange'" v-bind:disabled="targetId == null" v-bind:class="{'btn-color3':!isActiveState('ScheEngChange'), 'btn-color11-4':isActiveState('ScheEngChange')}" class="btn btn-block font-weight-bold">
                         工程變更
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click="selectTab='Construction'" v-bind:disabled="isNotWork" v-bind:class="{'btn-color3':!isActiveState('Construction'), 'btn-color11-2':isActiveState('Construction')}" class="btn btn-block font-weight-bold">
                         施工日誌
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click="selectTab='Supervision'" v-bind:disabled="isNotWork" v-bind:class="{'btn-color3':!isActiveState('Supervision'), 'btn-color11-3':isActiveState('Supervision')}" class="btn btn-block font-weight-bold">
                         監造報表
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click="selectTab='AskPayment'" v-bind:disabled="isNotWork" v-bind:class="{'btn-color3':!isActiveState('AskPayment'), 'btn-color11-3-1':isActiveState('AskPayment')}" class="btn btn-block font-weight-bold">
                         估驗請款
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='PriceAdjustment'" v-bind:disabled="isNotWork" v-bind:class="{'btn-color3':!isActiveState('PriceAdjustment'), 'btn-color11-1':isActiveState('PriceAdjustment')}" class="btn btn-block font-weight-bold">
                         物價調整款
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-6 mb-1">
                 </div>
             </div>
         </h5>
         <div>
             <EngChange v-if="selectTab=='ScheEngChange'" v-bind:tenderItem="tenderItem" v-bind:sepHeader="sepHeader" v-on:reload="reload"></EngChange>
             <Construction v-if="selectTab=='Construction'" v-bind:tenderItem="tenderItem" v-bind:canEditUser="canEditConstruction"></Construction>
             <Supervision v-if="selectTab=='Supervision'" v-bind:tenderItem="tenderItem" v-bind:canEditUser="canEditSupervision" v-on:reload="reload"></Supervision>
             <AskPayment v-if="selectTab=='AskPayment'" v-bind:tenderItem="tenderItem"></AskPayment>
             <PriceAdjustment v-if="selectTab=='PriceAdjustment'" v-bind:tenderItem="tenderItem" v-bind:spHeader="spHeader"></PriceAdjustment>
         </div>
         <template v-if="selectTab==''" >
             <div style="font-size:1.4em">
                 <font color="blue" style="font-size:1.6em">變更前注意事項</font><br />
                 辦理變更設計或修正施工預算案前，應先查詢<font color="red">原訂約廠商有無被停權</font>，並將資料列印存檔。<br />
                 倘仍在停權期間，因特殊需要，需洽原訂約廠商施作時，應先報上級機關核准。
             </div>
             <div v-if="spHeader.SPState == 0" style="color: red; font-size: 1.6em;">
                 <br />
                 注意:預定進度作業未完成作業
             </div>
             <div v-if="rejectCompanys.length > 0">
                 <div style="color: red; font-size: 1.6em;">
                     <br />
                     停權廠商
                 </div>

                 <table class="table table-responsive-md">
                     <thead class="insearch">
                         <tr>
                             <th><strong>廠商代號</strong></th>
                             <th><strong>廠商名稱</strong></th>
                             <th><strong>拒絕往來生效日</strong></th>
                             <th><strong>拒絕往來截止日</strong></th>
                         </tr>
                     </thead>
                     <tbody>
                         <tr v-for="(item, index) in rejectCompanys" v-bind:key="item.Seq">
                             <td>{{item.Corporation_Number}}</td>
                             <td>{{item.Corporation_Name}}</td>
                             <td>{{item.Effective_DateStr}}</td>
                             <td>{{item.Expire_DateStr}}</td>
                         </tr>
                     </tbody>
                 </table>
             </div>
         </template>
     </div>
</template>
<script>
import Com from "../../Common/Common2";
    export default {
        data: function () {
            return {
                targetId: null,
                tenderItem: {},
                selectTab: '',
                spHeader: {},
                sepHeader: {}, //s20230329
                canEditConstruction: false,
                canEditSupervision: false,
                rejectCompanys:[], //s20230720
                isInit : false,
                Com : Com
            };
        },
        components: {
            EngChange: require('./PM_EngChange_Progress.vue').default,
            Construction: require('./PM_EngChange_Construction.vue').default,
            Supervision: require('./PM_EngChange_Supervision.vue').default,
            AskPayment: require('./PM_EngChange_AskPayment.vue').default,
            PriceAdjustment: require('./PM_EngChange_PriceAdjustment.vue').default,
        },
        computed : {
            isNotWork() {
                if (this.targetId == null || this.sepHeader.SPState < 1 || this.spHeader.SPState != 1 || !this.isInit)
                    return true;
                return false;
            },
        },
        methods: {
            checkInit()
            {
                window.myAjax.post('/EPCProgressEngChange/CheckInit', { engSeq: this.targetId })
                    .then(resp => {
                        this.isInit = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //停權廠商 s20230720
            getRejectCompany() {
                this.rejectCompanys = [];
                window.myAjax.post('/EPCProgressEngChange/GetRejectCompany', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.rejectCompanys = resp.data.items;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },

            isActiveState(key) {
                if (this.targetId == null) return false;
                return (this.selectTab == '' || this.selectTab==key)
            },
            formatDateStr(d) {
                return window.comm.formatCDateStr(d);
            },
            //前置作業主檔 s20230329
            getSEPHeader() {
                this.ceHeader = {};
                window.myAjax.post('/EPCSchEngProgress/GetSEPHeader', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.sepHeader = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //預定進度主檔
            getCEHeader() {
                this.spHeader = { };
                window.myAjax.post('/EPCSchProgress/GetSPHeader', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.spHeader = resp.data.item;
                        } else if(this.selectTab=='ScheProgress')
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //下載
            downloadPcces(item) {
                window.myAjax.get('/EPCSchProgress/DownloadPccesXML?id=' + item.EngMainSeq, { responseType: 'blob' })
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
            reload() {
                this.getItem();
            },
            //取標案
            getItem() {
                this.canEditConstruction = false;
                this.canEditSupervision = false;
                
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EPCProgressManage/GetTrender',{ id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                            this.tenderItem.schOrgStartDate  = resp.data.schOrgStartDate;
                            this.tenderItem.schOrgEndDate = resp.data.schOrgEndDate;
                            this.canEditConstruction = resp.data.editConstruction;
                            this.canEditSupervision = resp.data.editSupervision;
                            this.tenderItem.isContractBreaked = resp.data.isContractBreaked;
                            this.getSEPHeader();//s20230329
                            this.getCEHeader();
                            this.getRejectCompany();

                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted 進度管理-工程變更');
            this.targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);
            this.checkInit();
            this.getItem();
            //sessionStorage.getItem('selectYear') == null
            //sessionStorage.removeItem('selectYear');
                //window.sessionStorage.setItem("selectYear", this.selectYear);
        }
    }
</script>
<style scoped>
    button:disabled {
        cursor: not-allowed;
        pointer-events: all !important;
    }
</style>