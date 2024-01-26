
import { ref, computed } from "vue";

export default function usePageItem(route, _countPerPage)
{
    const list = ref([]);
    const dataCount =  ref(0);
    const currentPage = ref(1);
    const countPerPage = ref(_countPerPage);
    const pageCount =  computed(() => dataCount.value  > 0 ? Math.ceil(dataCount.value /countPerPage.value)  : 0  )
    async function GetPageItem(page = currentPage.value, perPage = countPerPage.value)
    {

        currentPage.value = page;
        countPerPage.value = perPage;
       let {data } = await window.myAjax.post(route, {page :page, perPage: perPage})

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
        GetPageItem
    }

}