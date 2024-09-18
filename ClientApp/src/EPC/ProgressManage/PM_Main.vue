 <template>
     <div>
         <h5 class="insearch mt-0 py-2">
             <div class="form-row mb-1" v-if="tenderItem">
                 <div class="col-10 my-1" >
                     編號：{{tenderItem.EngNo}} <br>
                     名稱：{{tenderItem.EngName}}<br>
                     預定進度:{{tenderItem.SchProgress}}
                     &nbsp;實際進度:{{tenderItem.AcualProgress}}<br>
                     {{tenderItem.DurationCategory}}：{{tenderItem.EngPeriod}}天、{{ Com.ToROCDate(tenderItem.schOrgStartDate) ?? tenderItem.StartDateStr}}
                     <span style="color:purple" v-if="$refs.SchProgress && $refs.SchProgress.SchDateIsChange "> ({{ tenderItem.prjSchStartDateStr }} )</span> ~ {{Com.ToROCDate(tenderItem.schOrgEndDate) ?? tenderItem.SchCompDateStr}}
                     <span style="color:purple" v-if="$refs.SchProgress && $refs.SchProgress.SchDateIsChange "> ({{ tenderItem.prjSchCompDateStr }} )</span>
                     <div   v-if="tenderItem.ProgressDoneEarly">
                        <span style="color:purple">已提前完工(<span style="color:orange"> {{ Com.ToROCDate(tenderItem.ProgressDoneEarly) }} </span>)</span>
                    </div>
                     <button v-if="sepHeader.PccesXMLDateStr != null && sepHeader.PccesXMLDateStr != ''" @click="downloadPcces(sepHeader)" class="btn btn-color11-1 btn-xs mx-1">
                         <i class="fas fa-download"></i> Pcces下載({{sepHeader.PccesXMLDateStr}})
                     </button>

                 </div>



                 <div class="col-2 my-1 text-center p-4" style="
                    "> 

                    <a  href="./CarbonReductionCal/Index"  class="btn btn-color11-2 btn-lg mx-1">
                                            <i  class="fas fa-wrench"></i>&nbsp;&nbsp;施工減碳
                                        </a>    
                </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='ScheEngProgress'" v-bind:disabled="targetId == null" v-bind:class="{'btn-color3':!isActiveState('ScheEngProgress'), 'btn-color11-4':isActiveState('ScheEngProgress')}" class="btn btn-block font-weight-bold">
                         前置作業
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1" >
                     <button v-on:click.stop="selectTab='ScheProgress'" v-bind:disabled="this.targetId == null || this.sepHeader.SPState<1" v-bind:class="{'btn-color3':!isActiveState('ScheProgress'), 'btn-color11-1':isActiveState('ScheProgress')}" class="btn btn-block font-weight-bold">
                         預定進度
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1" v-if="!tenderItem.IsSupervision">
                     <button v-on:click="selectTab='Construction'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('Construction'), 'btn-color11-2':isActiveState('Construction')}" class="btn btn-block font-weight-bold">
                         施工日誌
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click="selectTab='Supervision'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('Supervision'), 'btn-color11-3':isActiveState('Supervision')}" class="btn btn-block font-weight-bold">
                         監造報表
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click="selectTab='AskPayment'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('AskPayment'), 'btn-color11-3-1':isActiveState('AskPayment')}" class="btn btn-block font-weight-bold">
                         估驗請款
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='PriceAdjustment'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('PriceAdjustment'), 'btn-color11-1':isActiveState('PriceAdjustment')}" class="btn btn-block font-weight-bold">
                         物價調整款
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-6 mb-1">
                 </div>
             </div>
         </h5>
         <div>
             <ScheEng v-if="selectTab=='ScheEngProgress'" v-bind:tenderItem="tenderItem" v-bind:sepHeader="sepHeader" v-on:reload="reload"></ScheEng>
             <ScheProgress v-if="tenderItem && spHeader" v-show="selectTab=='ScheProgress'" ref="SchProgress"  v-bind:tenderItem="tenderItem" v-bind:spHeader="spHeader" v-on:reload="reload">


             </ScheProgress>
             <Construction v-if="selectTab=='Construction'" v-bind:tenderItem="tenderItem" v-bind:canEditUser="canEditConstruction">
                <template #constructionDaysSetting>
                    <button v-if="  tenderItem.currentDate "  @click="constructionDoneEarly"  type="button" class="btn btn-color11-3 btn-block btn-sm mb-1"  >
                        提前完工 ({{ Com.ToROCDate(tenderItem.currentDate.Date)}})</button>
                </template>

             </Construction>
             <Supervision v-if="selectTab=='Supervision'" v-bind:tenderItem="tenderItem" v-bind:canEditUser="canEditSupervision" v-on:reload="reload">
                <template #constructionBtn>
                    <button v-if="tenderItem.IsSupervision" v-on:click="selectTab='Construction'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('Construction'), 'btn-color11-2':isActiveState('Construction')}" class="btn btn-color11-2 btn-sm mb-2">
                         查看施工日誌
                     </button>
                </template>
             </Supervision>
             <AskPayment v-if="selectTab=='AskPayment'" v-bind:tenderItem="tenderItem"></AskPayment>
             <PriceAdjustment v-if="selectTab=='PriceAdjustment'" v-bind:tenderItem="tenderItem" v-bind:spHeader="spHeader"></PriceAdjustment>
         </div>
         <label class="mb-1 mx-2 small-red">* 第一次，請先點選前置作業確認標案價格及契約數量，⑤</label>
     </div>
</template>
<script>
import Com from "../../Common/Common2";
    export default {
        data: function () {
            return {
                targetId: null,
                tenderItem: null,
                selectTab: '',
                spHeader: null,
                sepHeader: {}, //s20230329
                canEditConstruction: false,
                canEditSupervision: false,
                Com :Com
            };
        },

        components: {
            ScheEng: require('./PM_SchEng.vue').default,
            ScheProgress: require('./PM_ScheProgress.vue').default,
            Construction: require('./PM_Construction.vue').default,
            Supervision: require('./PM_Supervision.vue').default,
            AskPayment: require('./PM_AskPayment.vue').default,
            PriceAdjustment: require('./PM_PriceAdjustment.vue').default,
        },
        methods: {
            constructionDoneEarly()
            {
                window.myAjax.post("EPCProgressManage/constructionDoneEarly", { id: this.targetId, tarDate: this.tenderItem.currentDate.Date })
                .then(resp => {
                    if(resp.data == true)
                    {
                        alert("完成");
                        this.getItem();
                        this.selectTab ='';
                    }
                    else{
                        alert(resp.data);
                    }
                      
                })
            },
            isNotWork() {
                if(!this.spHeader) return true;
                if (this.targetId == null || this.sepHeader.SPState < 1 || this.spHeader.SPState != 1)
                return true;
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
                window.myAjax.get('/EPCSchEngProgress/DownloadPccesXML?id=' + item.EngMainSeq, { responseType: 'blob' })
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
                            this.tenderItem.schOrgStartDate =  resp.data.schOrgStartDate;
                            this.tenderItem.schOrgEndDate = resp.data.item.ProgressDoneEarly ?? resp.data.schOrgEndDate;
                            this.canEditConstruction = resp.data.editConstruction;
                            this.canEditSupervision = resp.data.editSupervision;
                            this.tenderItem.ProgressDiagramAceessCode = resp.data.ProgressDiagramAceessCode
                            this.tenderItem.IsSupervision = resp.data.IsSupervision;
                            this.getSEPHeader();//s20230329
                            this.getCEHeader();
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 進度管理');
            this.targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq)
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
