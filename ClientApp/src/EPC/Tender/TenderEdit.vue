<template>
    <div>
        <h5 class="insearch mt-0">工程編號：{{engMain.TenderNo}} &nbsp;&nbsp; 工程名稱：{{engMain.TenderName}}</h5>
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item">
                <a v-on:click="selectTab='BaseData'" ref="BaseData" class="nav-link" data-toggle="tab" href="##">基本資料</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='ContractorQualityControl'" class="nav-link" data-toggle="tab" href="##">廠商聘用品管人員</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Supervisor'" class="nav-link" data-toggle="tab" href="##">監造人員</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='FullTimeEngineer'" class="nav-link" data-toggle="tab" href="##">專任工程人員</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='SiteRelate'" class="nav-link" data-toggle="tab" href="##">工地相關人員</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='Budgeting'" class="nav-link" data-toggle="tab" href="##">預算編列</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='ChangeDesignData'" class="nav-link" data-toggle="tab" href="##">變更設計資料</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='ProgressData'" class="nav-link" data-toggle="tab" href="##">進度資料</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='BackwardData'" class="nav-link" data-toggle="tab" href="##">落後資料</a>
            </li>
            <li class="nav-item">
                <a v-on:click="selectTab='PaymentRecord'" class="nav-link" data-toggle="tab" href="##">歷次付款資料</a>
            </li>
        </ul>
        <div>
            <BaseData v-if="selectTab=='BaseData'" v-bind:engMain="engMain"></BaseData>
            <ContractorQualityControl v-if="selectTab=='ContractorQualityControl'" v-bind:engMain="engMain"></ContractorQualityControl>
            <Supervisor v-if="selectTab=='Supervisor'" v-bind:engMain="engMain"></Supervisor>
            <FullTimeEngineer v-if="selectTab=='FullTimeEngineer'" v-bind:engMain="engMain"></FullTimeEngineer>
            <SiteRelate v-if="selectTab=='SiteRelate'" v-bind:engMain="engMain"></SiteRelate>
            <Budgeting v-if="selectTab=='Budgeting'" v-bind:engMain="engMain"></Budgeting>
            <ChangeDesignData v-if="selectTab=='ChangeDesignData'" v-bind:engMain="engMain"></ChangeDesignData>
            <ProgressData v-if="selectTab=='ProgressData'" v-bind:engMain="engMain"></ProgressData>
            <BackwardData v-if="selectTab=='BackwardData'" v-bind:engMain="engMain"></BackwardData>
            <PaymentRecord v-if="selectTab=='PaymentRecord'" v-bind:engMain="engMain"></PaymentRecord>
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
            BaseData: require('./TenderEdit_BaseData.vue').default,
            ContractorQualityControl: require('./TenderEdit_ContractorQualityControl.vue').default,
            Supervisor: require('./TenderEdit_Supervisor.vue').default,
            FullTimeEngineer: require('./TenderEdit_FullTimeEngineer.vue').default,
            SiteRelate: require('./TenderEdit_SiteRelate.vue').default,
            Budgeting: require('./TenderEdit_Budgeting.vue').default,
            ChangeDesignData: require('./TenderEdit_ChangeDesignData.vue').default,
            ProgressData: require('./TenderEdit_ProgressData.vue').default,
            BackwardData: require('./TenderEdit_BackwardData.vue').default,
            PaymentRecord: require('./TenderEdit_PaymentRecord.vue').default
        },
        methods: {
            getItem(isShowCreateMsg) {
                this.engMain = {};
                window.myAjax.post('/EPCTender/GetItem', { id: this.targetId })
                    .then(resp => {
                        if (resp.data.result == 0) {
                            this.engMain = resp.data.item;
                            this.selectTab = 'BaseData';
                            this.$refs.BaseData.classList.toggle('active');
                        } else {
                            alert(resp.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            console.log('mounted() 工程查詢-標管' + window.location.href);
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