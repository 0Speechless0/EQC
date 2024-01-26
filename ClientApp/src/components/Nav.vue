<script setup>
import {defineProps , defineEmits, ref, watch} from "vue";
import {NavStore} from "../store/NavStore.js";

const props = defineProps(["tabs", "currentTab" ] );
const emits = defineEmits(["onTabChange"]);

NavStore.currentTab = props.currentTab;
const currentTab =  NavStore.currentTab;

import Emit from "../Backend/UserNotification/Emit.vue";
{/* <Nav :tabs="[
            [ Emit, '發佈'],
            [ List, '檢視']
        ]"  currentTab="Emit"  */}

const tabsNameMap = ref(
    props.tabs     
        .reduce(( a, c) => {
            const {__name : tabKey  }= c[0];
            var tabName = c[1];
            a[tabKey] = tabName;
            return a;
        }, {})
);

const tabs = 
    props.tabs.reduce((a, c) => {
        a[c[0].__name] = c[0];
        return a
    }, {})


console.log(tabs);

</script>

<template>

    <div>
        <div>
            <ul class="nav nav-tabs" role="tablist" >
                <li   class="nav-item"  v-for="(_ , tab, i ) in tabs" :key="i">
                    <a @click="NavStore.currentTab = tab" data-toggle="tab" href="" :class="'nav-link ' + (NavStore.currentTab== tab ? 'active' : '')">
                        {{ tabsNameMap[tab] }}
                    </a>
                </li>

            </ul>
        </div>
        <div class="tab-content">

                <component :is="tabs[NavStore.currentTab]" ></component>


        </div>
    </div>
</template>