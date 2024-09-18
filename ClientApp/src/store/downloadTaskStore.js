import {reactive } from "vue";

const downloadTaskStore  = reactive({
    isDownloading : false,
    async getDonloadTaskTag(){
        let { data : res } = await window.myAjax.post("DownloadTask/GetDownloadTaskTag");
        this.isDownloading = res.downloadTaskTag;
    }

});


export default downloadTaskStore;