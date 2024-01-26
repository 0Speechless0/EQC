<template>
    <div>
        <h5 class="insearch mt-0">
            工程：{{engMain.EngName}}({{engMain.EngNo}})<br>標案：{{engMain.TenderName}}({{engMain.TenderNo}})
        </h5>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a v-on:click="selectTab='BaseData'" ref="BaseData" class="nav-link" data-toggle="tab" href="##">決標資料</a>
            </li>
            <!-- li class="nav-item">
                <a v-on:click="selectTab='MaterialAdj'" class="nav-link" data-toggle="tab" href="##">物料調整</a>
            </li -->
            <li class="nav-item">
                <a v-on:click="selectTab='Committee'" class="nav-link" data-toggle="tab" href="##">委員資料</a>
            </li>
        </ul>
        <div>
            <BaseData v-if="selectTab=='BaseData'" v-bind:engMain="engMain"></BaseData>
            <MaterialAdj v-if="selectTab=='MaterialAdj'" v-bind:engMain="engMain"></MaterialAdj>
            <Committee v-if="selectTab=='Committee'" v-bind:engMain="engMain"></Committee>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectTab: '',
                engMain: {},                
            };
        },
        components: {
            BaseData: require('./TenderEdit2_PlanEdit.vue').default,
            MaterialAdj: require('./TenderEdit2_MaterialAdj.vue').default,
            Committee: require('./TenderEdit2_Committee.vue').default,
        },
        methods: {
            getItem(isShowCreateMsg) {
                this.engMain = {};
                window.myAjax.post('/EPCTender/GetTrender', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.selectTab = 'BaseData';
                            this.$refs.BaseData.classList.toggle('active');
                        } else {
                            alert(resp.data.message);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 工程查詢-管考' + window.location.href);
            let urlParams = new URLSearchParams(window.location.search);
            if (urlParams.has('id')) {
                this.targetId = parseInt(urlParams.get('id'), 10);
                console.log(this.targetId);
                if (Number.isInteger(this.targetId) && this.targetId > 0) {
                    this.getItem();
                    return;
                }
            }
            window.history.back();
        }
    }
</script>