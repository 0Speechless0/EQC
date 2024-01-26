import {computed, reactive, ref } from "vue";


function ImageGroup(item, groupKey, groupValueKey)
{
    const imageGroup = ref({
        groupKey: item[groupKey], groupValueKey : item[groupValueKey].map(e => Image(e, item[groupKey]) ) ,
    });
    
    return imageGroup;
}

function Image(item, group)
{
    const itemUrl = computed(() => group != "" ?  group +  "/"+ item : item  )
    return reactive({
        relativeUrl : itemUrl ,
        fileName : item 
        
    });

}

export function useImageLocalStore(route, groupKey, groupValueKey)
{

    const imageData = ref({});
    const lastDirName = ref("");
    const searchStr = ref("");
    const selectImg = ref({});
    const dirSearchLock =ref(false);
    // const groupList = ref([])
    const groupList =  computed(() => 
        Object.values(imageData.value)
            .filter(e => 
                e.value.groupKey.split("/").length  - 1 == dirStack.value.length - 1 && 
                e.value.groupKey != ""  )
            .map(e => e.value.groupKey.replace("/", ":") ) );
    const dirStack = ref([]);
    

    const selectedFileUrl =  computed(() =>  
     selectImg.value.path !='' ?    selectImg.value.path +'/' + selectImg.value.name :  selectImg.value.name
   );


    function GetParentDirImage()
    {
        dirStack.value.pop();
        var dirName = dirStack.value[dirStack.value.length -1];
        GetImage(dirName);
    }
    async function  GetImage(dirName = "", keyWord)
    {

        dirName = dirName .replace(":", "/"); 
        let { data } = await window.myAjax.post(route, {dirName : dirName, keyWord : keyWord});


        imageData.value = data
            .map(e => ImageGroup(e, groupKey, groupValueKey) )
            
            .reduce( (imageValue, element) => {
                imageValue[element.value.groupKey] = element;
                return imageValue;
            }, []);
        // data
        //     .map(e => ImageGroup(e, groupKey, groupValueKey) )
            
        //     .foreach( ( element) => {
        //         imageData.value[element.value.groupKey] = element;
        //     });
        // groupList.value = 
            //     Object.values(imageData.value)
            // .filter(e => e.value.groupValueKey.length > 0 )
            // .map(e => e.value.groupKey.replace("/", ":") )
            imageData.value.forEach(element => {
                console.log("imageData", element);
            });


        if(dirStack.value[dirStack.value.length -1] != dirName)
            dirStack.value.push(dirName)
    }
    function  GetImageByKeyWord()
    {
        var dirName = dirStack.value[dirStack.value.length -1];
        GetImage(dirName, searchStr.value)

    }

    function lookUpImg(item)
    {   
        selectImg.value = item;
        console.log("selectImg", selectImg.value);
    }
    return {
        imageData,
        searchStr,
        selectImg,
        selectedFileUrl  ,
        groupList,
        dirStack,
        GetImage,
        GetImageByKeyWord,
        lookUpImg,
        GetParentDirImage
    }

}