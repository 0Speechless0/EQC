<template>
    <div>
        <div style="padding-top: 20px; padding-left: 20px; margin-bottom: -5px;">
            <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item"><a v-on:click="selectTab='tabMaterial'" data-toggle="tab" href="##" class="nav-link active">材料</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabMachine'" data-toggle="tab" href="##" class="nav-link">機具</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabFuel'" data-toggle="tab" href="##" class="nav-link">燃料</a></li>
                <li class="nav-item"><a v-on:click="selectTab='tabCustomize'" data-toggle="tab" href="##" class="nav-link">自定義</a></li>
            </ul>
        </div>
        <div class="tab-content">
            <TabMaterial v-if="selectTab=='tabMaterial'" v-on:addSubItem="addSubItem"></TabMaterial>
            <TabMachine v-if="selectTab=='tabMachine'" v-on:addSubItem="addSubItem"></TabMachine>
            <TabFuel v-if="selectTab=='tabFuel'" v-on:addSubItem="addSubItem"></TabFuel>
            <TabCustomize v-if="selectTab=='tabCustomize'" v-on:addSubItem="addSubItem"></TabCustomize>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                modelShow:false,
                selectTab: 'tabMaterial',
            };
        },
        components: {
            TabMaterial: require('./Tab_Material.vue').default,
            TabMachine: require('./Tab_Machine.vue').default,
            TabFuel: require('./Tab_Fuel.vue').default,
            TabCustomize: require('./Tab_Customize.vue').default,
        },
        methods: {
            addSubItem(item, amount, memo) {
                this.$emit('addSubItem', item, amount, memo);
            },
            strEmpty(str) {
                return window.comm.stringEmpty(str);
            }
        },
        mounted() {
            console.log('mounted() 材料、機具、燃料維護功能');
        }
    }
</script>