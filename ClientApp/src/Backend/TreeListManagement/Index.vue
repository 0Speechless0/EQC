<script setup>
    import Table from "../../components/FormalTable.vue";
    import useTreeMStore from "./treeManagementStore";
    import pageControl from "../../components/paginationV2.vue";
    const TreeMStore= useTreeMStore(10);
</script>

<template>
    <div>
        <div class="row d-flex" style="margin:1px" >
            <input type="text" class="form-control col-3" v-model="TreeMStore.searchStr.value">
            <button class="btn btn-primary" @click="TreeMStore.getTreeListByKeyWord()">搜尋樹名</button>
        </div>

        <Table 
           
            :column-info="[
                {
                    colViewName : '名稱',
                    colName :'Name',
                    editable : true
                },
                {
                    colViewName : '類別',
                    colName :'Type',
                    class : 'col-1',
                    editable : true
                },
                {
                    colViewName : '編號',
                    colName :'Code',
                    class : 'col-1',
                    editable : true
                },
                {
                    colViewName : '建立時間',
                    colName :'CreateTime'
                },
                {
                    colViewName : '編輯時間',
                    colName :'ModifyTime'
                }

            ]"
            :items="TreeMStore.list.value"
            :has-delete="true"
            :has-insert="true"
            :has-update="true"
            @onDeleteItem="(item) =>TreeMStore.deleteTree(item)"
            @onUpdateItem="(item) => TreeMStore.updateTree(item)"
            @onInsertItem="(item) => TreeMStore.insertTree(item.value)"
            >
        </Table>
        <pageControl
    
            :pageSectionCount="5"
            :countPerPage="TreeMStore.pageItemStore.countPerPage"
            :page="TreeMStore.pageItemStore.currentPage"
            :pageCount="TreeMStore.pageItemStore.pageCount"
            @pageChange="(page) => TreeMStore.getTreeList(page)"
        >
        </pageControl>
    </div>
</template>