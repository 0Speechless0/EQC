<template>
    <div>
        <comm-pagination ref="pagination" :recordTotal="recordTotal" v-on:onPaginationChange="onPaginationChange"></comm-pagination>
        <div v-for="(item, index) in items" v-bind:key="item.index"
             class="row" style="width: 100%; padding-right: 15px; padding-left: 15px; margin-left: auto; border-bottom: 1px #bbb solid;" >
            <div class="col-4">
                <div class="card">
                    <div class="card-body">
                        <p class="card-text text-center">施工前</p>
                    </div>
                    <img v-if="item.p1 != null" v-bind:src="getPhotoPath(item.p1)" style="max-height:300px">
                </div>
            </div>
            <div class="col-4">
                <div class="card">
                    <div class="card-body">
                        <p class="card-text text-center">施工中</p>
                    </div>
                    <img v-if="item.p2 != null" v-bind:src="getPhotoPath(item.p2)" style="max-height:300px">
                </div>
            </div>
            <div class="col-4">
                <div class="card">
                    <div class="card-body">
                        <p class="card-text text-center">施工後</p>
                    </div>
                    <img v-if="item.p3 != null" v-bind:src="getPhotoPath(item.p3)" style="max-height:300px">
                </div>
            </div>
        </div>
        <br />
    </div>
</template>
<script>
    export default {
        props: ['engMainSeq'],
        watch: {
            engMainSeq: function (val) {
                if (this.engMainSeq > -1) this.getList();
            }
        },
        data: function () {
            return {
                items: [],
                //分頁
                recordTotal: 0,
                pageRecordCount: 30,
                pageIndex: 1,
            };
        },
        methods: {
            getPhotoPath(name) {
                return '/FileUploads/Eng/' + this.engMainSeq + '/' + name;
            },
            getList() {
                this.items = [];
                window.myAjax.post('/EngHistoryInfo/GetPhotoList', {
                    id: this.engMainSeq,
                    pageIndex: this.pageIndex,
                    perPage: this.pageRecordCount,
                })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.items = resp.data.items;
                            this.recordTotal = resp.data.total;
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //分頁
            onPaginationChange(pInx, pCount) {
                this.pageRecordCount = pCount;
                this.pageIndex = pInx;
                this.getList();
            },           
        },
        async mounted() {
            console.log('mounted 抽查照片清單');
            this.getList();
        }
    }
</script>