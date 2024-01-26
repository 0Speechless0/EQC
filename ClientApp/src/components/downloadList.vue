<template>
    <div class="modal fade" id="downloadList" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="exampleModalLabel" aria-modal="true" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="projectUpload">下載</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    <table class="table table1" border="0">
                        <thead>
                            <tr>

                                <th scope="col-6" v-if="viewImage"></th>
                                <th scope="col-6" v-else>檔名</th>
                                <th scope="col-3" >功能</th>
                                <th scope="col-3"> 刪除</th>
                            </tr>
                        </thead>
                        <tbody>
                            <!-- 無圖結構 : string ，有圖結構 {fileName : string, fileLink : string } -->
                            <tr v-for="(downloaditem, index) in downloaditems" v-bind:key="downloaditem">
                                <td v-if="viewImage">
                                    <img :src="downloaditem.fileLink" with="600" heigh="400"/>
                                </td>
                                <td v-else>{{downloaditem}}</td>
                                <td>
                                    <div class="row justify-content-center m-0">
                                        <a v-on:click.stop="downloadone(downloaditem.fileName ??  downloaditem)" href="#" class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                                    </div>
                                </td>
                                <td>
                                    <div class="row justify-content-center m-0">
                                        <a v-on:click.stop="deleteDownloadFile(downloaditem.fileName ??  downloaditem)" href="#" class="btn-block mx-2 btn btn-color9-1 " title="刪除">刪除</a>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="row justify-content-center m-0">
                        <a v-on:click.stop="downloadAll()" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {

    props :["seq", "route", "viewImage"],
    data : function()
    {
        return {
            downloaditems : []
        }
    },
    methods:{
        deleteDownloadFile(fileName)
        {
            window.myAjax.post(`/${this.route}/deleteFile` , {fileName: fileName, seq : this.seq})
            .then(res => this.getdownloaditem());
        },
        downloadAll() {
            window.myAjax.get(`/${this.route}/DownloadAll?seq=${this.seq}` ,  { responseType: 'blob' })
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
        downloadone(fileName) {

            var num = this.downloaditems.length;
            window.myAjax.get(`/${this.route}/OneDownload?fileName=${fileName}&seq=${this.seq}`, { responseType: 'blob' })
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
        getdownloaditem(value = this.seq) {

                window.myAjax.post(`/${this.route}/GetDownloadList`, { seq: value })
                    .then(resp => {
                        this.downloaditems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
        }
    },
    watch:{
        seq : {
            async handler(value)
            {
                console.log("ff");
                await this.getdownloaditem(value)
            }
        }
    }
    ,
    mounted()
    {
        this.getdownloaditem();
    }

            

}
</script>