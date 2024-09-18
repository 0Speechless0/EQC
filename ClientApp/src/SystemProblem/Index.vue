<script setup>

import Edit from "./Edit";
import List from "./List.vue";
import {ref} from "vue";
const tab = ref("");

</script>
<template>
    <div>
        <div style="padding-top: 10px;">
            <ul role="tablist" class="nav nav-tabs">
                <li class="nav-item"><a data-toggle="tab" href="#menu01" class="nav-link active" @click="tab ='menu01'">系統問題回報</a></li>
                <li class="nav-item"><a data-toggle="tab" href="#menu02" class="nav-link" @click="tab ='menu02'">處理進度</a>
                </li>
            </ul>
        </div>


        <div class="tab-content">
            <div id="menu01" class="tab-pane active">
                <Edit @onsave="tab = `menu02`"></Edit>
            </div>
            <div id="menu02" class="tab-pane">
                <List  v-if="tab == 'menu02'"></List>
            </div>

        </div>
    </div>

</template>