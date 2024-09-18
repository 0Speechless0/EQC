 <template>
     <div>
         <h5 class="insearch mt-0 py-2">
             <div class="form-row justify-content-between mb-1">
                 <div class="col-12 my-1">
                    編號：{{tenderItem.EngNo}} <br>
                    名稱：{{tenderItem.EngName}}<br>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='EMDAudit'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('EMDAudit'), 'btn-color11-1':isActiveState('EMDAudit')}" class="btn btn-block font-weight-bold">
                         材料設備
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="onSelectSIR" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('SamplingInspectionRec'), 'btn-color11-2':isActiveState('SamplingInspectionRec')}" class="btn btn-block font-weight-bold">
                         施工抽查
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="onSIRImprove" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('SIRImprove'), 'btn-color11-3':isActiveState('SIRImprove')}" class="btn btn-block font-weight-bold">
                        缺失改善
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-2 mb-1">
                     <button v-on:click.stop="selectTab='Supervise'" v-bind:disabled="isNotWork()" v-bind:class="{'btn-color3':!isActiveState('Supervise'), 'btn-color11-4':isActiveState('Supervise')}" class="btn btn-block font-weight-bold">
                         督導
                     </button>
                 </div>
                 <div class="col-12 col-sm-4 col-md-4 mb-1">
                     <button @click="dnDoc1" class="btn btn-color11-1 btn-xs mx-1">
                         <i class="fas fa-download"></i> 施工抽查成果
                     </button>
                     <!-- button @click="dnDoc2" class="btn btn-color11-1 btn-xs mx-1">
                         <i class="fas fa-download"></i> 工程資料表
                     </button -->
                     <button @click="dnDoc3" class="btn btn-color11-1 btn-xs mx-1">
                         <i class="fas fa-download"></i> 材料設備表單
                     </button>
                 </div>
             </div>
         </h5>
         <div>
             <emdaudit-edit v-if="selectTab=='EMDAudit'" v-bind:tenderItem="tenderItem"></emdaudit-edit>
             <template v-if="selectTab=='SamplingInspectionRec'">
                 <sampling-inspection-rec-list v-if="activeModel==0" v-bind:tenderItem="tenderItem" v-on:editSIR="onEditSIR"></sampling-inspection-rec-list>
                 <sampling-inspection-rec-edit v-if="activeModel==1" :constCheckMode="constCheckMode" :constCheckItem="constCheckItem"
                                               :tenderItem="tenderItem" ></sampling-inspection-rec-edit>
             </template>
             <template v-if="selectTab=='SIRImprove'">
                 <sampling-inspection-recimprove-list v-if="activeModel==0" v-bind:tenderItem="tenderItem" v-on:editSIRImprove="onEditSIRImprove"></sampling-inspection-recimprove-list>
                 <sampling-inspection-recimprove-edit v-if="activeModel==1" :constCheckMode="constCheckMode" :constCheckItem="constCheckItem"
                                               :tenderItem="tenderItem" ></sampling-inspection-recimprove-edit>
             </template>
             <Supervise v-if="selectTab=='Supervise'" v-bind:tenderItem="tenderItem"></Supervise>
         </div>
     </div>
</template>
<script>
    export default {
        data: function () {
            return {
                targetId: null,
                tenderItem: {},
                selectTab: '',
                activeModel: 0,
                subEngNameSeq: -1,
                //s20230520
                constCheckMode: -1,
                constCheckSeq: -1,
            };
        },
        components: {
            Supervise: require('./QV_Supervise.vue').default,
        },
        methods: {
            dnDoc3() {
                this.download('/EMDAudit/DownloadReport?engMain=' + this.tenderItem.Seq + '&engNo=' + this.tenderItem.EngNo);
            },
            dnDoc1() {
                this.download('/EPCQualityVerify/DnDoc1?id=' + this.tenderItem.Seq);
            },
            /* s20230818 mark
            dnDoc2() {
                this.download('/EPCQualityVerify/DnDoc2?id=' + this.tenderItem.PrjXMLSeq);
            },*/
            //下載
            download(action) {
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
            //抽查記錄填報
            onSelectSIR() {
                this.activeModel = 0;
                this.subEngNameSeq = -1;
                this.selectTab = 'SamplingInspectionRec';
            },
            onEditSIR(mode, item) {//s20230520
                //this.subEngNameSeq = mode;
                //if (item != null) this.subEngNameSeq = 478; //test
                this.constCheckMode = mode;
                this.constCheckItem = item;
                this.activeModel = 1;
            },
            //抽查缺失改善
            onSIRImprove() {
                this.activeModel = 0;
                this.subEngNameSeq = -1;
                this.selectTab = 'SIRImprove';
            },
            onEditSIRImprove(mode, item) {//20230522
                this.subEngNameSeq = mode;
                if (item != null) this.subEngNameSeq = 478; //test
                this.constCheckMode = mode;
                this.constCheckItem = item;
                this.activeModel = 1;
            },
            isNotWork() {
                return this.targetId == null;
            },
            isActiveState(key) {
                if (this.targetId == null) return false;
                return (this.selectTab == '' || this.selectTab==key)
            },
            formatDateStr(d) {
                return window.comm.formatCDateStr(d);
            },
            //取標案
            getItem() {
                if (this.targetId == null) {
                    alert('請先選取標案');
                    return;
                }
                window.myAjax.post('/EPCQualityVerify/GetEngMain',{ id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.tenderItem = resp.data.item;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        async mounted() {
            console.log('mounted() 品質查證');
            this.targetId = window.sessionStorage.getItem(window.epcSelectTrenderSeq);
            this.getItem();
            //sessionStorage.getItem('selectYear') == null
            //sessionStorage.removeItem('selectYear');
                //window.sessionStorage.setItem("selectYear", this.selectYear);
        }
    }
</script>