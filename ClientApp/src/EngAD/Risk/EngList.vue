<template>
    <div>
        <h5>
            {{engData.title}}
            <button @click="onDownload" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-download"></i> 匯出Excel</button>
        </h5>
        <div class="row justify-content-between">
            <comm-pagination class="col-12" ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        </div>
        <div class="table-responsive">
            <table class="table table-responsive-md table-hover">
                <thead class="insearch">
                    <tr>
                        <th style="width: 42px;"><strong>項次</strong></th>
                        <th><strong>歸屬計畫</strong></th>
                        <th><strong>執行機關</strong></th>
                        <th><strong>標案名稱</strong></th>
                        <th><strong>決標金額</strong></th>
                        <th><strong>預定進度</strong></th>
                        <th><strong>實際進度</strong></th>
                        <th><strong>進度差異</strong></th>
                        <th><strong>落後原因分析</strong></th>
                        <th><strong>解決辦法</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" v-bind:key="item.Seq">
                        <td><strong>{{pageRecordCount*(pageIndex-1)+index+1}}</strong></td>
                        <td>{{item.BelongPrj}}</td>
                        <td>{{item.ExecUnitName}}</td>
                        <td>{{item.TenderName}}</td>
                        <td>{{item.BidAmount}}</td>
                        <td>{{item.PDAccuScheProgress}}</td>
                        <td>{{item.PDAccuActualProgress}}</td>
                        <td>{{item.DiffProgress}}</td>
                        <td>{{item.BDAnalysis}}</td>
                        <td>{{item.BDSolution}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engData', 'selChange', 'selUnits', 'selStartYear','selEndYear'],
        watch: {
            selChange: function (val) {
                this.$refs.pagination.pageIndex = 1;
                this.getList();
            },
        },
        data: function () {
            return {
                items: [],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        methods: {
            getList() {
                this.items = [];
                window.myAjax.post('/EADRisk/GetList', {
                    units: this.selUnits,
                    sYear: this.selStartYear,
                    eYear: this.selEndYear,
                    mode: this.engData.key,
                    pageRecordCount: this.pageRecordCount,
                    pageIndex: this.pageIndex
                })
                    .then(resp => {
                        this.recordTotal = resp.data.total;
                        this.items = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDownload() {
                window.myAjax({
                        method: 'post',
                        url: '/EADRisk/Download',
                        responseType: 'arraybuffer',
                        data:{
                            units: this.selUnits,
                            sYear: this.selStartYear,
                            eYear: this.selEndYear,
                            mode: this.engData.key
                        }
                    })
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
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },
        },
        async mounted() {
            console.log('mounted() 水利工程履約風險分析-工程');
            this.getList();
        }
    }
</script>
