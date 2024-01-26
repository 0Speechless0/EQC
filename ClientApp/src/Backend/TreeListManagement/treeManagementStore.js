
import {ref, computed }  from "vue";
import usePageItem  from "../../store/PageItemStore"
export default function useTreeManagement(countPerPage) {
    const pageItemStore =  usePageItem("TreeManagement/GetPagination", countPerPage); 
    const list = ref([]);
    // const treeTypes = computed(()=> {
    //     return Object.keys(list.value.reduce((a, c) => {
    //         a[c.Type] = null;
    //         return a;
    //     }, []));
    // })
    const editItems = ref({});
    const searchStr = ref("");
    async function getTreeList(page)
    {
        searchStr.value = null;
        list.value = await pageItemStore.GetPageItem(page, countPerPage);
    }
    async function getTreeListByKeyWord(str)
    {   

        list.value = (await window.myAjax.post("TreeManagement/GetByKeyWord", { keyWord : searchStr.value })).data.list;
    }
    async function updateTree(item)
    {
        let {data : res} = await window.myAjax.post("TreeManagement/Update", { model : item});
        if(res == true) {
            alert("更新成功");
        }
    }
    async function deleteTree(item, index)
    {
        let {data : res} = await window.myAjax.post("TreeManagement/Delete", { id : item.Seq});
        if(res == true) {
            alert("刪除成功");
            list.value = await pageItemStore.GetPageItem();
        }
    }
    async function insertTree(model)
    {
        let {data : res} = await window.myAjax.post("TreeManagement/Insert", { model : model});
        if(res == true) {
            alert("新增成功");
            getTreeList(1);
        }

    }
    getTreeList(1);
    return {
        getTreeList,
        updateTree,
        deleteTree,
        insertTree,
        getTreeListByKeyWord,
        searchStr,
        pageItemStore,
        editItems,
        list,
        // treeTypes
    }

}