

import {ref} from "vue";

import {usePageItem } from "./PageItemStore";

export function userWebBaseStore(controllerName, countPerPage = null)
{

    const items = ref([]);
    const keyWord = ref(null);
    if(countPerPage != null)
    {
        const pageItem = usePageItem(`${controllerName}\GetPagination`, countPerPage);
    }

    async  function Get(pageIndex = null, _countPerPage = null)
    {
        if(countPerPage == null)
        {
            let {data}  = await window.myAjax.post(`${controllerName}\Get`) ;
            items.value =  data;

        }
        else if (pageIndex != null){
           await pageItem.GetPageItem(pageIndex, _countPerPage ?? countPerPage);
           items.value = pageItem.list.value;
        }

        
    }

    async  function GetByKeyWord()
    {
        let {data}  = await window.myAjax.post(`${controllerName}\Get`, {keyWord :keyWord.value}) ;
        items.value =  data;
     
    }

    async  function Insert(model)
    {
       let {data :res }  =  await window.post(`${controllerName}\Insert`, { model : model});
       if(res ==  true)
       {
        alert("新增成功")
       }
       else{
        alert("新增失敗")
       }
    }

    async  function Update(model)
    {
        let {data :res }  =  await window.post(`${controllerName}\Update`, { model : model});
        if(res ==  true)
        {
         alert("修改成功")
        }
        else{
         alert("修改失敗")
        }
    }

    async  function Delete(id)
    {
        let {data :res }  =  await window.post(`${controllerName}\Delete`, { id : id});
        if(res ==  true)
        {
         alert("刪除成功")
        }
        else{
         alert("刪除失敗")
        }
    }

    return {
        items,
        Get, 
        Insert,
        Update,
        Delete,
        GetByKeyWord
    }

}