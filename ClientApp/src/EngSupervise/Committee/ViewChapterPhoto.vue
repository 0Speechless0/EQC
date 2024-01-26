<template>
    <div>
        <h5 class="insearch mt-0 py-2">工程編號：{{engMain.EngNo}} &nbsp;&nbsp; 工程名稱：{{engMain.EngName}}</h5>
        <div class="row pics">
            <div v-for="(item, index) in photoItems" v-bind:key="index" class="col-12 col-md-6 col-xl-3 mb-4">
                <div class="card">
                    <img v-bind:src="getPhotoPath(item)" class="" style="height:20vh ;">
                    <div class="card-body">
                        <p class="card-text">{{item.Memo}}</p>
                        <a @click="viewPhoto(item)" href="javascript:void(0)" class="card-link a-view" data-toggle="modal" data-target="#BIGpic_01" title="檢視"><i class="fas fa-eye"></i></a>

                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="BIGpic_01">
            <div class="modal-dialog modal-xl modal-dialog-centered ">
                <div v-if="photoItem != null" class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title font-weight-bold">{{photoItem.Memo}}</h6>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <img v-bind:src="getPhotoPath(photoItem)" class="w-100">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color3" data-dismiss="modal">關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                engMain: {},
                targetId:'-1',
                photoItems: [],
                photoItem: null,
                rootPath:'',
            };
        },
        methods: {
            //照片
            viewPhoto(item) {
                this.photoItem = item;
            },
            getPhotoPath(item) {
                return this.rootPath + '/' + item.fName;
            },
            getChapterPhoto() {
                window.myAjax.post('/ESCommittee/GetChapterPhoto', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.photoItems = resp.data.items;
                            this.rootPath = resp.data.rPath;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //工程資訊
            getEngItem() {
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/ESCommittee/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.getChapterPhoto();
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
        },
        mounted() {
            console.log('mounted() 委員督導-工程設計圖');
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                if (Number.isInteger(this.targetId)) {
                    this.getEngItem();
                    return;
                }

            }
            window.location = "/ESCommittee";
        }
    }
</script>