<template>
    <div class="table-responsive">
    <table class="table table-responsive-md table-hover table2">
        <thead class="insearch">
            <tr>
                <th class="col-1"><strong>項次</strong></th>
                <th class="col-4"><strong>編碼</strong></th>
                <th class="col-12"><strong>工作項目</strong></th>
                <th><strong>類型1</strong></th>
                <th><strong>類型2</strong></th>
                <th><strong>15T</strong></th>
                <th><strong>21T</strong></th>
                <th><strong>35T</strong></th>
                <th><strong>單位</strong></th>
                <th class="text-center"><strong>管理</strong></th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(item, index) in store.items" v-bind:key="item.Seq">
                <td style="min-width: unset;">{{store.pageRecordCount*(store.pageIndex-1)+index+1}}</td>
                <template v-if="item.Seq != store.editSeq">
                    <td>{{item.Code}}</td>
                    <td>{{item.Description}}</td>
                    <td>{{item.Type1}}</td>
                    <td>{{item.Type2}}</td>
                    <td>{{item[`C15T`]}}</td>
                    <td>{{item[`C21T`]}}</td>
                    <td>{{item[`C35T`]}}</td>
                    <td>{{item.Unit}}</td>
                    <td style="min-width: unset;">
                        <div class="d-flex justify-content-center">
                            <!-- <button @click="tableControl.editDetail(item)" class="btn btn-color11-2 btn-xs sharp m-1" title="碳分析"><i class="fas fa-newspaper"></i></button> -->
                            <button @click="tableControl.onEditRecord(item)" class="btn btn-color11-1 btn-xs sharp m-1" title="編輯"><i class="fas fa-pencil-alt"></i></button>
                            <button @click="tableControl.onDelRecord(item)" class="btn btn-color9-1 btn-xs sharp m-1" title="刪除"><i class="fas fa-trash-alt"></i></button>
                        </div>
                    </td>
                </template>
                <template v-if="item.Seq == store.editSeq">
                    <td><input v-model.trim="store.editRecord.Code" maxlength="20" type="text" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord.Description" maxlength="50" type="text" class="form-control"></td>
                    <td><input v-model="store.editRecord.Type1" type="text" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord.Type2"  type="text" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord[`C15T`]" maxlength="20" type="number" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord[`C21T`]" maxlength="20" type="number" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord[`C35T`]" maxlength="20" type="number" class="form-control"></td>
                    <td><input v-model.trim="store.editRecord.Unit" maxlength="20" type="text" class="form-control"></td>
                    <td style="min-width: unset;">
                        <div class="d-flex justify-content-center">
                            <button @click="tableControl.onSaveRecord(store.editRecord)" class="btn btn-color11-2 btn-xs sharp m-1" title="儲存"><i class="fas fa-save"></i></button>
                            <button @click="tableControl.onEditCancel()" class="btn btn-color9-1 btn-xs sharp m-1" title="取消"><i class="fas fa-times"></i></button>
                        </div>
                    </td>
                </template>
            </tr>
            <tr>
                <td></td>
                <td><input v-model.trim="store.newItem.Code" maxlength="20" type="text" class="form-control"></td>
                <td><input v-model.trim="store.newItem.Description" maxlength="50" type="text" class="form-control"></td>
                <td><input v-model="store.newItem.Type1" type="text" class="form-control"></td>
                <td><input v-model.trim="store.newItem.Type2"  type="text" class="form-control"></td>
                <td><input v-model.trim="store.newItem[`C15T`]" maxlength="20" type="number" class="form-control"></td>
                <td><input v-model.trim="store.newItem[`C21T`]" maxlength="20" type="number" class="form-control"></td>
                <td><input v-model.trim="store.newItem[`C35T`]" maxlength="20" type="number" class="form-control"></td>
                <td><input v-model.trim="store.newItem.Unit" maxlength="20" type="text" class="form-control"></td>
                <td style="min-width: unset;">
                    <div class="d-flex justify-content-center">
                        <button @click="tableControl.onNewRecord(store.newItem)" class="btn btn-outline-secondary btn-xs sharp m-1" title="新增"><i class="fas fa-plus"></i></button>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</div>
</template>
<script setup>

import { onMounted,reactive, defineProps } from 'vue';
const props = defineProps(["TableControl"]);
const store = reactive(props.TableControl);
const tableControl = props.TableControl;
onMounted( () => {
tableControl.getResords();

})


// export default {
//     data : () =>  TablelControl.data,
//     methods : {
//         ...TablelControl  
//     }

// }

</script>