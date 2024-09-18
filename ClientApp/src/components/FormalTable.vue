<!-- const columnInfo = [
{ 
    colViewName : "操作者:帳號",
    colName : "UserName"
},
 ...
] -->

<script setup>
    import {defineProps, defineEmits, ref,reactive } from "vue";
    const {items, columnInfo, minWidth, hasUpdate, hasDelete, hasInsert} = defineProps(["items", "columnInfo", 'minWidth', 'hasUpdate', 'hasDelete', 'hasInsert']);
    console.log("ADSDS", items);
    const  emit = defineEmits("onUpdateItem, onDeleteItem", "onInsertItem");
    function getTdStyle(value)
    {
        if(!isNaN(parseInt(value)) )
        {
            return "text-right";
        }
        else{
            return "text-left";
        }
    }
    function Insert()
    {
        emit('onInsertItem', insertItem);
        insertItem.value = {};
    }
    const insertItem = ref({ 
     });
    const editIndex = ref(null);
</script>

<template>
    <table class="table table1 min910" :style="{ 'min-width': minWidth +'px' }">
        <thead>
            <tr>
                <th>序</th>
                <th v-for="(col,index) in columnInfo" :key="index" :style="{'min-width' : (col.colWidth ?? 0 ) +'px' }" :class="`${col.class}`">
                    {{ col.colViewName }}
                </th>
                <slot name="otherTh" >
                </slot>
                <th v-if="hasUpdate || hasDelete">

                </th>

            </tr>
        </thead>
        <tbody>
            <tr v-if="hasInsert">
                <td class="text-center">#</td>
                <td  v-for="(col, index2) in columnInfo" :class="getTdStyle(insertItem[col.colName])" :key="index2"> 
                    <input class="form-control" v-model="insertItem[col.colName] "  v-if="col.editable"/>
                </td>
                <td class="text-center">
                    <button  title="新增" class="btn btn-color11-3 btn-xs sharp mx-1" @click="Insert"><i class="fas fa-plus"></i></button>
                </td>
            </tr>
            <tr v-for="(item, index) in items" :key="index">
                <td class="text-center">{{ index+1 }}</td>
                <slot name="body" :item="item">
                       
                        <td  v-for="(col, index2) in columnInfo" :key="'1'+index2" :class="getTdStyle(item[col.colName])"> 
         
                            <input  v-if="index == editIndex && col.editable" class="form-control" v-model="item[col.colName] " />
                            <span v-else>         {{ item[col.colName] }}</span>
                        </td>

                </slot>
                <td  class="d-flex justify-content-center" v-if="hasUpdate || hasDelete">
                    <div v-if="hasUpdate">
                        <button v-if="editIndex != index" title="編輯" class="btn btn-color11-2 btn-xs m-1" @click="editIndex = index">
                        <i class="fas fa-pencil-alt"></i> 編輯
                        </button>
                        <button v-else title="儲存" class="btn btn-color11-3 btn-xs m-1" @click="() => {editIndex = null; emit('onUpdateItem', item);}">
                            <i class="fas fa-save"></i> 儲存
                        </button>
                    </div>
                    <div v-if="hasDelete" >
                        <button title="刪除" class="btn btn-color9-1 btn-xs m-1"  @click="() => {editIndex = null; emit('onDeleteItem', item);}">
                            <i class="fas fa-trash-alt"></i> 刪除
                        </button>
                    </div>

                    
                </td>
                <slot name="otherTd" :item="item">
                </slot>
            </tr>
        </tbody>
    </table>
</template>

<style scoped>
    th {
        border-color: #ddd !important;

    }
</style>