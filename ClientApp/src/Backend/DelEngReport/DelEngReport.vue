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
                刪除工程提報所有資料
            </div>
        </div>
        <div class="row ">
            <div class="col-2 mt-3">
                <button v-on:click.stop="delEng(1)" role="button" class="btn btn-color9-1 btn-xs mx-1">
                    <i class="fas fa-trash-alt"></i>&nbsp;&nbsp;刪除
                </button>
            </div>
            <div class="col mt-3">
                刪除進度管理內所有資料:<br />預計進度, 監造/施工日誌, 估驗請款, 物價調整款
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                engNo:'',
            };
        },
        methods: {
            delEng(mode) {
                if (this.engNo.length == 0) {
                    alert('請輸入工程編號');
                    return;
                }
                if (confirm('是否確定刪除?')) {
                    window.myAjax.post('/DelEngReport/Del', { engNo: this.engNo, mode:mode }).then(resp => {
                        alert(resp.data.message)
                    }).catch(error => {
                        console.log(error);
                    });
                }
            },
        },
        async mounted() {
            console.log('mounted() 刪除工程提報');
        }
    }
</script>