<script setup>
import {ref }  from "vue";
const selectTab = ref(null);
</script>

<template>
    <div>
        <div class="table">
                <thead>
                    <th></th>
                    <th> AA</th>
                    <th> BB</th>
                    <th> CC</th>
                </thead>
                <tbody>
                    <tr>
                        <td>  </td>
                        <td>
                            <button class="btn btn-primary" @click="selectTab =0">
                            AA
                            </button>
                        </td>
                        <td>
                            <button class="btn btn-primary" @click="selectTab =1">
                            BB
                            </button>
                        </td>
                        <td>
                            <button class="btn btn-primary" @click="selectTab =2">
                            CC
                            </button>
                        </td>
                    </tr>
                    <tr v-if="selectTab ==0">
                       <td colspan="3" class="text-center">
                            AA
                       </td>
                    </tr>
                    <tr  v-if="selectTab == 1">
                       <td colspan="3" class="text-center">
                            BB
                       </td>
                    </tr>
                    <tr v-if="selectTab == 2">
                       <td colspan="3" class="text-center">
                            CC
                       </td>
                    </tr>
                </tbody>

            </div>
    </div>
</template>