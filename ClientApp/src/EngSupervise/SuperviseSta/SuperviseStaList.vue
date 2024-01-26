<template>
    <div>
        <table>
            <tr>
                <td style="width: 60px; background: #f2f2f2; text-align:center;width: 100px;">缺失類別</td>
                <td colspan="3" class="pl-2">
                    <div class="custom-control custom-radio custom-control-inline">
                        <input v-model="docNo" @change="selChange" value="1" type="radio" id="W1" class="custom-control-input">
                        <label for="W1" class="custom-control-label" style="width:120px">品質管制制度</label>
                    </div>
                    <div class="custom-control custom-radio custom-control-inline">
                        <input v-model="docNo" @change="selChange" value="2" type="radio" id="W2" class="custom-control-input">
                        <label for="W2" class="custom-control-label">施工品質</label>
                    </div>
                </td>
            </tr>
            <tr>
                <td rowspan="2" style="width: 60px; background: #f2f2f2; text-align: center;">期間</td>
                <td class="pl-2">
                    <select v-model="selSeason" @change="onSeasonChange" class="form-control">
                        <option value="1">第一季</option>
                        <option value="2">第二季</option>
                        <option value="3">第三季</option>
                        <option value="4">第四季</option>
                    </select>
                </td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td class="pl-2">
                    <input v-model.trim="filterStartDate" @change="selChange" type="date" class="form-control">
                </td>
                <td class="p-2">~</td>
                <td>
                    <input v-model.trim="filterEndDate" @change="selChange" type="date" class="form-control">
                </td>
            </tr>
        </table>
        <div class="form-row justify-content-center">
            <button v-on:click.stop="staList" type="button" class="btn btn-outline-secondary btn-xs mx-1"><i class="fas fa-search"></i> 查詢</button>
        </div>
        <p></p>
        <div class="table-responsive" style="background: #f2f2f2;">
            <table class="table table-responsive-md table-hover">
                <thead>
                    <tr>
                        <th style="width: 42px;"><strong>排序</strong></th>
                        <th><strong>缺失編號</strong></th>
                        <th><strong>缺失內容</strong></th>
                        <th style="width: 120px;"><strong>缺失件數</strong></th>
                        <th style="width: 120px;"><strong>缺失比率</strong></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in staItems" v-bind:key="item.Seq">
                        <td>{{index+1}}</td>
                        <td>{{item.MissingNo}}</td>
                        <td>{{item.Content}}</td>
                        <td>{{item.MissingCnt}}</td>
                        <td>{{item.MissingRate}}</td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr> 
                        <td></td>
                        <td></td>
                        <td></td>
                        <td>加總: {{staItems.reduce((a, c) => a+c.MissingCnt, 0)}}</td>
                        <td>平均 : {{ (staItems.reduce((a, c) => a+c.MissingRate, 0) /staItems.length).toFixed(2) }}</td>
                    </tr>
                </tfoot>
            </table>
        </div>
        <div v-if="staItems.length > 0" class="form-row justify-content-center">
            <div class="d-flex">
                <button v-on:click.stop="onDownloadClick(1)" class="btn btn-color11-3 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> docx</button>
                <button v-on:click.stop="onDownloadClick(2)" class="btn btn-color11-3 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> pdf</button>
                <button v-on:click.stop="onDownloadClick(3)" class="btn btn-color11-3 btn-xs mx-1" title="下載"><i class="fas fa-download"></i> odt</button>
                <a  :href="'/ESSuperviseSta/DownloadStaExcel?docNo=' + this.docNo 
                    + '&sDate=' + this.filterStartDate + '&eDate=' + this.filterEndDate + '&docType=0'" 
                    class="btn btn-color11-1 btn-xs mx-1" title="下載" download><i class="fas fa-download"></i> xlsx</a>
                <a  :href=" '/ESSuperviseSta/DownloadStaExcel?docNo=' + this.docNo 
                    + '&sDate=' + this.filterStartDate + '&eDate=' + this.filterEndDate + '&docType=1'" 
                    class="btn btn-color11-1 btn-xs mx-1" title="下載" download><i class="fas fa-download"></i> ods</a>
            </div>
        </div>
    </div>
</template>
<script>
import Common from '../../Common/Common2';
    export default {
        data: function () {
            return {
                staItems:[],
                selSeason: 0, //季
                filterStartDate: '',
                filterEndDate: '',
                selStartDate: '',
                selEndDate: '',
                docNo: 1,
            };
        },
        methods: {
            //季變更
            onSeasonChange() {
                this.selChange();
                if (this.selSeason == '0') {
                    this.filterStartDate = '';
                    this.filterEndDate = '';
                } else {
                    var now = new Date();
                    var d = new Date(now.getFullYear(), (this.selSeason - 1) * 3, 1);
                    this.filterStartDate = window.comm.formatDate(d, "yyyy-MM-dd");

                    d = new Date(d.setMonth(d.getMonth() + 3));
                    d = new Date(d.setDate(d.getDate() - 1));
                    this.filterEndDate = window.comm.formatDate(d, "yyyy-MM-dd");
                }
            },
            selChange() {
                this.staItems = [];
            },
            download(item, docType) {
                window.myAjax.get('/ESSuperviseSta/Download?id=' + item.Seq + '&docType=' + docType, { responseType: 'blob' })
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
            staList() {
                if (this.filterStartDate == '' || this.filterEndDate == '') {
                    alert("請輸入日期範圍");
                    return;
                }
                this.staItems = [];
                window.myAjax.post('/ESSuperviseSta/GetStaList'
                    , {
                        docNo: this.docNo,
                        sDate: this.filterStartDate,
                        eDate: this.filterEndDate
                    })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.staItems = resp.data.items;
                        } else
                            alert(resp.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            onDownloadClick(fileType) {
                window.myAjax.get('/ESSuperviseSta/DownloadSta?docNo=' + this.docNo + '&docType=' + fileType
                    + '&sDate=' + this.filterStartDate + '&eDate=' + this.filterEndDate, { responseType: 'blob' })
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
            getList() {
                this.getPhaseEngItems();
            }
        },
        mounted() {
            console.log('mounted() 督導統計');
        }
    }
</script>