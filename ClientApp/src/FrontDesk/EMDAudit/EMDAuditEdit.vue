<template>
    <div>
        <!--
        <EngInfo v-bind:engMain="engMain"></EngInfo>
        -->
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a v-on:click="selectTab='EMDTab1'" class="nav-link active" data-toggle="tab" href="#">材料設備送審管制總表</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='EMDTab2'" class="nav-link" data-toggle="tab" href="">材料設備檢(試)驗管制總表</a>
            </li>
        </ul>
        <div>
            <EMDTab1 v-if="selectTab=='EMDTab1'" v-bind:engMain="engMain"></EMDTab1>
            <EMDTab2 v-if="selectTab=='EMDTab2'" v-bind:engMain="engMain"></EMDTab2>
        </div>
        <!-- <div class="row justify-content-center">
            <div class="col-12 col-sm-5 col-lg-4 col-xl-3 mt-3">
                <button v-on:click.stop="back()" role="button" class="btn btn-shadow btn-color1 btn-block">
                    回上頁
                </button>
            </div>
        </div> -->
    </div>
</template>
<script>
    export default {
        props: ['tenderItem'],
        data: function () {
            return {
                selectTab: '',
                //工程基本資料
                engMain: {},
                EMDSummary: [],
                EMDTestSummary: []
            };
        },
        components: {
            //EngInfo: require('./EngInfo.vue').default,
            EMDTab1: require('./EMDAuditEdit_Tab1.vue').default,
            EMDTab2: require('./EMDAuditEdit_Tab2.vue').default
        },
        methods: {
            //工程基本資料
            getItem() {
                this.step = 2;
                this.engMain = {};
                window.myAjax.post('/EMDAudit/GetEngItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.selectTab = 'EMDTab1';
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getEMDSummaryList() {
                window.myAjax.post('/EMDAudit/GetEMDSummaryList', { id: this.targetId })
                    .then(resp => {
                        this.EMDSummary = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getEMDTestSummaryList() {
                window.myAjax.post('/EMDAudit/GetEMDTestSummaryList', { id: this.targetId })
                    .then(resp => {
                        this.EMDTestSummary = resp.data.items;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            back() {
                //window.history.go(-1);
                window.location = "/EMDAudit/EMDAuditList";
            }
        },
        async mounted() {
            console.log('mounted() 材料送審管制' + window.location.href);
            if (this.tenderItem != null) {//shioulo 20220519
                this.targetId = this.tenderItem.Seq;
                console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    if (this.targetId <= 0) {
                        this.isAdd = true;
                        this.step = 1;
                    } else {
                        this.getItem();
                    }
                    return;
                }
            }
            /*let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId)) {
                    if (this.targetId <= 0) {
                        this.isAdd = true;
                        this.step = 1;
                    } else {
                        this.getItem();
                    }
                    return;
                }
            }*/
            
            window.location = "/FrontDesk";
        }
    }
</script>
