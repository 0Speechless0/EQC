import { ref } from "vue";

function compare(a, b) {
    if ( parseInt(a.Value) <parseInt(b.Value) ) {
      return -1;
    }
    if (parseInt(a.Value) > parseInt(b.Value) ) {
      return 1;
    }
    // a 必須等於 b
    return 0;
  }



const Map = ref({});


//選項功能模組 :
  
export function useSelectionStore(_route) {

    //for each item in List 
    //  Value : 選項值
    //  Text  : 選項顯示文字

    const List = ref([]);
    const param = ref({});
    const route = ref(_route);
    function setParameter(_param)
    {
      param.value = _param;
      return this;
    }

    async function Get(prefix= "")
    {

        List.value = (await window.myAjax.post(route.value, param.value)).data;
        Map.value =
        List.value.reduce((a, c)=>{ 
            a[prefix+ c.Value] = c.Text
            return a ;   
        }, {});

        List.value.sort(compare);
        console.log(List.value);
        return List
    }

    async function GetSelection(route, prefix= "")
    {

        List.value = (await window.myAjax.post(route, param.value)).data;
        Map.value =
        List.value.reduce((a, c)=>{ 
            a[prefix+ c.Value] = c.Text
            return a ;   
        }, {});

        List.value.sort(compare);
        console.log(List.value);
        return List
    }
    

    return {

        //選項清單
        GetSelection,
        setParameter,
        Get,
        //選項文字字典
        Map,
    }

}