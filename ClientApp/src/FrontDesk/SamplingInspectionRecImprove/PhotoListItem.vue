<template>
    <div class="">
        <div id="wrap">
            <div v-for="(item, index) in photoItems" v-bind:key="item.Seq" class="" style="display: inline-block; margin:2px;">
                <div class="row align-items-start" style="width:150px;">
                    <div class="col">
                        <button v-if="item.ItemGroup>0 && formConfirm==0" v-on:click.stop="delPhoto(item, index)" class="btn icontopright"><i class="fas fa-times-circle"></i></button>
                        <a class="a-blue underl mx-2" href="#" v-bind:title="getPhotoType(item)" data-toggle="modal" v-bind:data-target="'#pModal'+item.Seq">
                            <img v-bind:src="getPhotoPath(item)" v-bind:alt="getPhotoType(item)" class="rounded w-100" style="max-height:90px;">
                        </a>
                        <p v-if="item.RESTful==0" Align="Center">無蜂窩</p>
                        <p v-if="item.RESTful==1" Align="Center">有蜂窩</p>
                        <span class="textbottomleft">{{getPhotoType(item)}}</span>
                    </div>
                </div>
                <!-- 大圖 -->
                <div class="modal fade" v-bind:id="'pModal'+item.Seq" data-backdrop="static" data-keyboard="false" tabindex="-1" v-bind:aria-labelledby="'pModal'+item.Seq" aria-modal="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="projectUpload">{{getPhotoType(item)}}</h5>
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
        props: ['engMain', 'si', 'ctlSeq', 'formConfirm'],
        data: function () {
            return {
                items: [],
                photoItems: [],
            }
        },
        methods: {
            getPhotoType(item) {
                if (item.ItemGroup== 0) return '改善前';
                else if (item.ItemGroup == 1) return '改善中';
                else return '改善後';
            },
            getPhotoPath(item) {
                return '/FileUploads/Eng/' + this.engMain + '/' + item.UniqueFileName;
            },
            getPhotoList() {
                //if (this.selectStdItem == -1) return;
                this.photoItems = [];
                window.myAjax.post('/SamplingInspectionRecImprove/GetRecResultPhotos', { engMain: this.engMain, recSeq: this.si.Seq, ctlSeq: this.ctlSeq })
                    .then(resp => {
                        this.photoItems = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            delPhoto(item, index) {
                if (confirm('是否刪除確定?')) {
                    window.myAjax.post('/SamplingInspectionRecImprove/DelResultPhoto', { engMain: this.engMain, fileSeq: item.Seq })
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
            refresh() {
                this.getPhotoList();
            }
        },
        async mounted() {
            console.log('mounted() 抽檢相片');
            this.getPhotoList();
        }
    }
</script>
