<template>
    <div class="">
        <div id="wrap">
            <div v-for="(item, index) in photoItems" v-bind:key="item.Seq" class="my-3" style="display: inline-block; margin:2px;">
                <div class="row align-items-start" style="width:300px;">
                    <div class="col">
                        <button v-on:click.stop="delPhoto(item, index)" class="btn icontopright"><i class="fas fa-times-circle"></i></button>
                        <a class="a-blue underl mx-2" title="改善前照片" data-toggle="modal" v-bind:data-target="'#pModal'+item.Seq">
                            <img v-bind:src="getPhotoPath(item)" alt="改善前照片" class="rounded w-100" style="max-height:250px;">
                        </a>
                        <textarea v-model="item.RESTful" class="form-control" @change="onChange(item)" v-bind:rows="item.rowSpan==1 ? 1 : 1" v-bind:disabled="si.FormConfirm>0"></textarea>
                        <textarea v-model="item.Memo" class="form-control" @change="onChange(item)" v-bind:rows="item.rowSpan==1 ? 1 : 1" v-bind:disabled="si.FormConfirm>0"></textarea>
                    </div>
                </div>
                <!-- 大圖 -->
                <div class="modal fade" v-bind:id="'pModal'+item.Seq" data-backdrop="static" data-keyboard="false" tabindex="-1" v-bind:aria-labelledby="'pModal'+item.Seq" aria-modal="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="projectUpload">改善前照片</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">×</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="table-responsive">
                                    <img v-bind:src="getPhotoPath(item)" class="rounded" style="max-height:600px;">
                                    <p class="bg-gray px-3 py-2 my-2 rounded" style="word-break: break-all">{{item.Memo}}</p>
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
        props: ['engMain', 'si', 'ctlSeq'],
        watch: {
            ctlSeq: function (val) {
                this.getPhotoList();
            },
        },
        data: function () {
            return {
                items: [],
                photoItems: [],
            }
        },
        methods: {
            getPhotoPath(item) {
                return '/FileUploads/Eng/' + this.engMain + '/' + item.UniqueFileName;
            },
            getPhotoList() {
                this.photoItems = [];
                if (this.ctlSeq == -1) return;
                this.getList(this.ctlSeq);
            },
            getList(seq) {
                this.photoItems = [];
                if (this.ctlSeq == -1) return;
                window.myAjax.post('/SamplingInspectionRec/GetRecResultPhotos', { engMain: this.engMain, recSeq: this.si.Seq, ctlSeq: seq })
                    .then(resp => {
                        this.photoItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delPhoto(item, index) {
                if (confirm('是否刪除確定?')) {
                    window.myAjax.post('/SamplingInspectionRec/DelResultPhoto', { engMain: this.engMain, fileSeq: item.Seq })
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.photoItems.splice(index, 1);
                            }
                            alert(resp.data.message);
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            onChange(item) {
                console.log(item.RESTful)
                window.myAjax.post('/SamplingInspectionRec/updataRESTful', { RESTful: item.RESTful, fileSeq: item.Seq, memo: item.Memo })
                    .then(resp => {
                        alert(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });

            },
            refresh(seq) {
                this.photoItems = [];
                if (seq == -1) return;
                this.getList(seq);
            }
        },
        async mounted() {
            console.log('mounted() 抽檢相片1');
        }
    }
</script>
