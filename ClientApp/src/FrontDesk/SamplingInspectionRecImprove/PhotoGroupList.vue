<template>
    <div class="">
        <!-- 改善前 -->
        <div class="my-3">
            <span class="before ">改善前：抽查不合格的照片會自動帶入</span>
            <div id="wrap">
                <span v-for="(item, index) in photoItems" v-bind:key="item.Seq">
                    <div v-if="item.ItemGroup==0" style="display: inline-block; margin:5px;">
                        <div class="my-3">
                            <div class="row align-items-start" style="width:600px;">
                                <div class="col">
                                    <img v-bind:src="getPhotoPath(item)" alt="改善前照片" class="rounded w-100" style="max-height:360px;">
                                    <p class="bg-gray px-3 py-2 my-2 rounded" style="word-break: break-all">{{item.Memo}}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </span>
            </div>
        </div>
        <!-- 改善中 -->
        <div class="my-3">
            <span class="ing">改善中</span>
            <div id="wrap">
                <span v-for="(item, index) in photoItems" v-bind:key="item.Seq">
                    <div v-if="item.ItemGroup==1" style="display: inline-block; margin:5px;">
                        <div class="my-3">
                            <div class="row align-items-start" style="width:600px;">
                                <div class="col">
                                    <!-- button v-on:click.stop="delPhoto(item, index)" class="btn icontopright"><i class="fas fa-times-circle"></i></button -->
                                    <img v-bind:src="getPhotoPath(item)" alt="改善前照片" class="rounded w-100" style="max-height:360px;">
                                    <p class="bg-gray px-3 py-2 my-2 rounded" style="word-break: break-all">{{item.Memo}}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </span>
            </div>
        </div>
        <!-- 改善後 -->
        <div class="my-3">
            <span class="after">改善後</span>
            <div id="wrap">
                <span v-for="(item, index) in photoItems" v-bind:key="item.Seq" >
                    <div v-if="item.ItemGroup==2" style="display: inline-block; margin:5px;">
                        <div class="my-3">
                            <div class="row align-items-start" style="width:600px;">
                                <div class="col">
                                    <!-- button v-on:click.stop="delPhoto(item, index)" class="btn icontopright"><i class="fas fa-times-circle"></i></button -->
                                    <img v-bind:src="getPhotoPath(item)" alt="改善前照片" class="rounded w-100" style="max-height:360px;">
                                    <p class="bg-gray px-3 py-2 my-2 rounded" style="word-break: break-all">{{item.Memo}}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </span>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['engMain', 'si', 'stdItem'],
        data: function () {
            return {
                photoItems: [],
            }
        },
        methods: {
            getPhotoPath(item) {
                return '/FileUploads/Eng/' + this.engMain + '/' + item.UniqueFileName;
            },
            getPhotoList() {
                this.photoItems = [];
                window.myAjax.post('/SamplingInspectionRecImprove/GetRecResultPhotos', { engMain: this.engMain, recSeq: this.si.Seq, ctlSeq: this.stdItem })
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
            }
        },
        async mounted() {
            console.log('mounted() 缺失改善相片');
            //this.getOption();
            this.getPhotoList();
        }
    }
</script>
