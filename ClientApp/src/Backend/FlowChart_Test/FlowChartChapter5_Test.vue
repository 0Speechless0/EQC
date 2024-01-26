<template>
    <div>
        <div v-if="loading">
            Loading...
        </div>
        <div class="table">
            <table class="table table3 min910" border="0">
                <thead>
                    <tr>

                        <th class="number">編號</th>
                        <th>名稱</th>
                        <th>管理標準</th>
                        <th>流程圖</th>
                        <th>紀錄表</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.id">
                        <td>
                            <div v-if="!item.edit">{{item.ExcelNo}}</div>
                            <input maxlength="20" v-if="item.edit" type="text" v-model.trim="item.ExcelNo" class="form-control" />
                        </td>
                        <td>
                            <div v-if="!item.edit">{{item.MDName}}</div>
                            <input maxlength="50" v-if="item.edit" type="text" v-model.trim="item.MDName" class="form-control" />
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-if="item.detailCount>0" v-on:click.stop="QCStd(item,$event)" href="#" class="mx-2 btn btn-block btn-outline-primary" title="管理標準">管理標準</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-if="item.FlowCharOriginFileName && item.FlowCharOriginFileName.length>0" v-on:click="showPicture(item)" href="#" class="mx-2 btn btn-block btn-link text-decoration-none col" title="查看流程圖" data-toggle="modal" data-target="#exampleModal"><i class="fas fa-project-diagram"></i> 查看流程圖</a>
                                <a v-if="item.FlowCharOriginFileName && item.FlowCharOriginFileName.length>0" v-on:click="downloadDiagramJson(item)" href="#" class="mx-2 btn  btn-link text-decoration-none col"  title="下載JSON"><i class="fas fa-download"></i> 下載JSON</a>
                            </div>
                            <!-- 大圖 -->
                            <div class="modal fade" id="exampleModal" data-backdrop="static" data-keyboard="false" tabindex="-1" v-bind:aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
                                <div class="modal-dialog modal-lg">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="projectUpload">流程圖</h5>
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">×</span>
                                            </button>
                                        </div>
                                        <div class="modal-body">
                                            <div class="table-responsive">
                                                <!-- <img v-bind:src="getPtmpRowhotoPath()" class="rounded" style="max-width:750px;"> -->
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>

                        <td>
                            <div v-if="item.detailCount>0" class="row justify-content-center m-0">
                                <a v-on:click.stop="download(item)" href="#" class="mx-2 btn btn-color5" title="查看" ><i class="far fa-eye"></i> 查看</a>
                            </div>
                        </td>
                    </tr>
                    <tr name="tmpRow" style="display: none;">
                        <td colspan="5" style="background: #f4f5f6;">
                            <qcBase5 v-bind:op1="QCSseq" :op2="QCSopenclose" ></qcBase5>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script type="JavaScript" src="https://cdnjs.cloudflare.com/ajax/libs/vue/1.0.18/vue.min.js"></script>
<script>
    import Common from "../../Common/Common2.js";
    export default {
        props: ['addItem'],
        watch: {
            'addItem': function (nval, oval) {
                this.newItem();
            },
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            },
        },
        data: function () {
            return {
                editFlag: false,
                loading: false,
                items: [],
                modalShow: false,
                targetItem: null,
                targetInput: {},
                files: new FormData(),    
                //分頁
                pageIndex: 1,
                perPage: 10,
                totalRows: 0,
                //管理標準
                QCSseq: '',
                QCSitem: '',
                QCSvaule: false,
                QCSopenclose: 0,//點擊管理標準第二次關閉
                //圖片預覽url
                photo: '',
                Wordphoto: '',
            };
        },
        components: {
            qcBase5: require('./QCStdBase5StdMaterialAndEquipment.vue').default
        },
        methods: {
            async downloadDiagramJson(item)
            {
                let {data} = await window.myAjax.post("FlowChartTp/getFlowChartTpDiagramJson", {id: item.Seq, type:"Chapter5"});

                Common.download2(data.jsonStr, item.MDName+"流程圖.json", "application/json");

            },
            openModal(item) {
                //this.$refs['MyDialog'].classList.toggle('modal');
                //this.$refs['MyDialog'].classList.toggle('show');
                this.files = new FormData();
                this.targetItem = item;
                this.modalShow = true;
            },
            closeModal() {
                //this.$refs['MyDialog'].classList.toggle('modal');
                //this.$refs['MyDialog'].classList.toggle('show');
                this.targetInput.value = '';
                this.modalShow = false;
            },
            fileChange(event) {
                this.targetInput = event.target;
                this.files.append("file", this.targetInput.files[0], this.targetInput.files[0].name);
                this.files.append("seq", this.targetItem.Seq);
            },
            
            download(item) {
                window.myAjax.get('/FlowChartTp_TestWeb/Chapter5Download?seq='+item.Seq, { responseType: 'blob' })
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
            Cekdownload(item) {
                this.Wordphoto = '';
                window.myAjax.post('/FlowChartTp_TestWeb/Chapter5CekDownload', { seq: item.Seq })
                    .then(resp => {
                        if (resp.data.url != null) {
                            console.log(resp.data.url);
                            this.Wordphoto = resp.data.url;
                        } else {
                            console.log(resp.data.message);
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
            getList() {
                this.editFlag = false;
                this.loading = true;
                this.items = [];
                let params = { pageIndex: this.pageIndex, perPage: this.perPage};
                window.myAjax.post('/FlowChartTp_TestWeb/Chapter5', params)
                    .then(resp => {
                        this.items = resp.data.items;
                        this.totalRows = resp.data.pTotal;
                        this.loading = false;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            editItem(item) {
                if (this.editFlag) return;

                this.editFlag = true;
                item.edit = this.editFlag;
            },
            
            sortItems() {
                this.items.sort(function (a, b) {
                    var nameA = a.OrderNo;
                    var nameB = b.OrderNo;
                    if (nameA < nameB) {
                        return -1;
                    }
                    if (nameA > nameB) {
                        return 1;
                    }
                    return 0;
                });
            },
            QCStd(item, e) {
                this.QCSvaule = true;
                this.QCSseq = item.Seq;
                this.QCSitem = item.MDName;

                if ((this.QCSopenclose % 2) == 0) {
                    this.QCSopenclose = 1;
                    $(e.currentTarget).closest("tbody").find("tr[name='tmpRow']").show();
                    var clickRow = $(e.currentTarget).closest("tr");
                    var newRow = $(e.currentTarget).closest("tbody").find("tr[name='tmpRow']");
                    $(clickRow).after(newRow);
                } else if ((this.QCSopenclose % 2) != 0) {
                    $(e.currentTarget).closest("tbody").find("tr[name='tmpRow']").hide();
                    this.QCSopenclose = 0;
                } 
            },
            deltable(e) {
            },
            showPicture(item) {
                window.myAjax.post('/FlowChartTp_TestWeb/Chapter5Show', { seq: item.Seq })
                    .then(resp => {
                        if (resp.data.url != null) {
                            console.log(resp.data.url);
                            this.photo = resp.data.url;
                        } else {
                            console.log(resp.data.message);
                        }


                        //return '/FileUploads/Tp/' + resp.url;
                    }).catch(error => {
                        console.log(error);
                    });
            },
            getPhotoPath(item) {

                return this.photo;
            },
            getWordPhotoPath() {
                return this.Wordphoto;
            }
        },
        
        /*watch: {
            // 顯示目前頁數的資料
            pageIndex: {
                handler: function (value) {
                    this.getList();
                }
            }
        },*/
        async mounted() {
            console.log('mounted() 第五章 材料設備清冊範本1');
            this.getList();
        }
    }
</script>