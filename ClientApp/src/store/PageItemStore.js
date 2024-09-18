
import { ref, computed } from "vue";

export default function usePageItem(route, _countPerPage, _pageSectionCount)
{
    const list = ref([]);
    const dataCount =  ref(0);
    const currentPage = ref(1);
    const countPerPage = ref(_countPerPage);
    const pageSectionCount = ref(_pageSectionCount)
    const pageCount =  computed(() => dataCount.value  > 0 ? Math.ceil(dataCount.value /countPerPage.value)  : 0  )
    var otherParams = {};
    function setParams(params)
    {
        otherParams = params;
        return this;
    }
    
    async function GetPageItem(page = currentPage.value, perPage = countPerPage.value)
    {

        currentPage.value = page;
        countPerPage.value = perPage;
        var finalParams = {page :page, perPage: perPage};
        Object.assign(finalParams, otherParams);
       let {data } = await window.myAjax.post(route, finalParams)

       if(data.count < dataCount.value &&  data.count % countPerPage.value == 0  )
       {
            currentPage.value -- 
       }
       dataCount.value = data.count;
       list.value = data.list;

       return list.value;
    }
    
    return {
        list,
        dataCount,
        currentPage,
        countPerPage,
        pageCount,
        GetPageItem,
        setParams
    }

}