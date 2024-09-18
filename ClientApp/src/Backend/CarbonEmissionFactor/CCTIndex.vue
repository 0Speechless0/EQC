<template>
    <div>
        <div>
            <ul class="nav nav-tabs" role="tablist">
                <!-- <li class="nav-item"><a v-on:click="selectTab='tabCtlTable'" data-toggle="tab" href="##" class="nav-link active">管制填報表(原報表)</a></li> -->
                <li class="nav-item"><a v-on:click="selectTab='tabCarbonTrade'" data-toggle="tab" href="##" class="nav-link">碳交易</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabCarbonReport'" data-toggle="tab" href="##" class="nav-link">碳報表</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabCarbonEmissionReport'" data-toggle="tab" href="##" class="nav-link">管制填報表(詳細)</a></li>
            </ul>
        </div>
        <div class="tab-content">
            <TabCtlTable v-if="selectTab=='tabCtlTable'"></TabCtlTable>
            <TabCarbonTrade v-if="selectTab=='tabCarbonTrade'"></TabCarbonTrade>
            <TabCarbonReport v-if="selectTab=='tabCarbonReport'"></TabCarbonReport>
            <TabCarbonEmissionReport v-if="selectTab=='tabCarbonEmissionReport'"></TabCarbonEmissionReport>
        </div>
    </div>
</template>
<script>
    export default {
        components: {//s20230506
            TabCtlTable: require('./CCT_TabCtlTable.vue').default,
            TabCarbonTrade: require('./CCT_TabCarbonTrade.vue').default,
            TabCarbonReport: require('./CCT_TabCarbonReport.vue').default,
            TabCarbonEmissionReport: require('./CCT_TabCarbonEmissionReport.vue').default,
        },
        data: function () {
            return {
                selectTab: 'tabCtlTable',
            };
        },
        methods: {
        },
        mounted() {
            console.log('mounted() 統計報表');
        }
    }
</script>