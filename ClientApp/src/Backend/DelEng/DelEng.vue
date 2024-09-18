<template>
    <div>
        <div class="form-row">
            <div class="col-6">
                <label class="my-2 mx-2">工程編號</label>
                <input v-model.trim="engNo" type="text" placeholder="輸入工程編號" class="col-12 col-md-10 form-control">
            </div>
        </div>
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="delEng(0)" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-trash-alt"></i>&nbsp;&nbsp;刪除
                </button>
            </div>
            <div class="col mt-3">
                刪除工程所有資料
            </div>
        </div>
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="delEng(1)" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-trash-alt"></i>&nbsp;&nbsp;刪除
                </button>
            </div>
            <div class="col mt-3">
                刪除進度管理內所有資料:<br />預計進度, 監造/施工日誌, 估驗請款, 物價調整款, 工程變更
            </div>
        </div>
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="delEng(2)" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-trash-alt"></i>&nbsp;&nbsp;刪除
                </button>
            </div>
            <div class="col mt-3">
                刪除進度管理內工程變更資料
            </div>
        </div>
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="engStateChange" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-pencil-alt"></i>&nbsp;&nbsp;狀態變更
                </button>
            </div>
            <div class="col mt-3">
                工程狀態變更:&nbsp;編輯中 > 編輯
            </div>
        </div>
        <hr style="border-top: 1px #aaa solid;" />
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="delEngReport()" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-trash-alt"></i>&nbsp;&nbsp;刪除
                </button>
            </div>

            <div class="col mt-3">
                刪除工程提報 <br /> <div class="form-inline">
                    <label for="SeqInputLable" class="mr-2">識別碼</label> <input v-model="engReportId" class="form-control" id="SeqInputLable" type="number"/>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                engReportId : null, 
                engNo:'',
            };
        },
        methods: {
            delEngReport()
            {
                if(!this.engReportId) 
                    alert("請輸入工程提報識別碼");
                window.myAjax.post("DelEng/DelEngReport", {id :this.engReportId})
                .then(resp => {
                    if(resp.data.result == 0)
                    {
                        alert(resp.data.message);
                        this.engReportId = null;
                    }
                })
            },
            delEng(mode) {
                if (this.engNo.length == 0) {
                    alert('請輸入工程編號');
                    return;
                }
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/DelEng/Del', { engNo: this.engNo, mode:mode }).then(resp => {
                        alert(resp.data.message)
                    }).catch(error => {
                        console.log(error);
                    });
                }
            },
            engStateChange() {
                if (this.engNo.length == 0) {
                    alert('請輸入工程編號');
                    return;
                }
                if (confirm('是否確定變更工程狀態?')) {
                    window.myAjax.post('/DelEng/EngStateChange', { engNo: this.engNo}).then(resp => {
                        alert(resp.data.message)
                    }).catch(error => {
                        console.log(error);
                    });
                }
            },
        },
        async mounted() {
            console.log('mounted() 刪除工程');
        }
    }
</script>