<template>
    <div class="">
        <div id="wrap">
            <div v-for="(item, index) in downloaditems" v-bind:key="index" class="my-3" style="display: inline-block; margin:2px;">
                <div class="row align-items-start" style="width:300px;">
                    <div class="col">
                        <button v-on:click.stop="deleteDownloadFile(item.fileName)" class="btn icontopright"><i class="fas fa-times-circle"></i></button>
                        <a class="a-blue underl mx-2"  data-toggle="modal" v-bind:data-target="'#pModal'+index+seq.split('.')[1]">
                            <img v-bind:src="item.fileLink"  class="rounded w-100" width="300" height="200">
                        </a>
                    </div>
                </div>
                <!-- 大圖 -->
                <div class="modal fade" v-bind:id="'pModal'+index+seq.split('.')[1]" data-backdrop="static" data-keyboard="false" tabindex="-1" v-bind:aria-labelledby="'pModal'+index+seq" aria-modal="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="projectUpload">查看</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">×</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="table-responsive mb-0">
                                    <img v-bind:src="item.fileLink" class="rounded" width="600" height="400">
                                    <div class="justify-content-center d-flex mt-3">

                                    <button title="下載" class="btn btn-color11-3 btn-xs mx-1" @click="downloadone(item.fileName)"><i class="fas fa-download"></i>下載</button>
                                    </div>
                                </div>
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

    props :["seq", "route", "uploadTrigger"],
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
        uploadTrigger : {
            async handler(value)
            {
                if(Object.keys(value).length == 0 )await this.getdownloaditem(this.seq)
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