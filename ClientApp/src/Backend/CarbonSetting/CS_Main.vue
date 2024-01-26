<template>
    <div>
        <div style="padding-top: 20px; padding-left: 20px; margin-bottom: -5px;">
            <ul role="tablist" class="nav nav-tabs">
                <li class="nav-item"><a v-on:click="selectTab='tabList'" data-toggle="tab" href="#menu01" class="nav-link active">碳排量設定</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabImport'" data-toggle="tab" href="#menu02" class="nav-link">匯入碳排量</a></li>
                <li class="nav-item"><a href="ExcelTemplate/核定碳排量匯入.xlsx" class="nav-link">下載範本</a></li>
            </ul>
        </div>
        <div class="tab-content">
            <TabList v-if="selectTab=='tabList'" ></TabList>
            <TabImport v-if="selectTab=='tabImport'" ></TabImport>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                selectTab: 'tabList',
            };
        },
        components: {
            TabList: require('./Tab_List.vue').default,
            TabImport: require('./Tab_Import.vue').default,
        },
        methods: {
        },
        mounted() {
            console.log('mounted 碳排量設定');
        }
    }
</script>