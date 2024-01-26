
import {ref} from "vue";
import Common from "../Common/Common2";
export function useFileStore(controllerName, dir)
{
    const fileData = ref({});
    const downloadFiliesUrl = ref([{

        fileName : "",
        fileLink : ""

    }]);
    const fileObject = ref({});


    getDownloadFiliesUrl();
    
    function onFileChange(event, fileGroupKey)
    {
        let i = 0;
        fileObject.value[fileGroupKey] = [];
        while( i < event.target.files.length)
        {

            fileObject.value[fileGroupKey].push(event.target.files[i]);
            i++;

        }

        readFiles(event, fileGroupKey);
        console.log(fileData.value, fileObject.value);

    }

    function getTempFilies(fileGroupKey)
    {
        return fileData.value[fileGroupKey];
    }
    async function getDownloadFiliesUrl(group = "")
    {
        let {data} = await window.myAjax.post(`${controllerName}/GetDownloadFilesUrl`, {dir :`${dir}${group}`});
        downloadFiliesUrl.value  =data;
    }
    async function uploadFilies(fileGroupKey ="")
    {
        let form = new FormData();
        Object.entries(fileObject.value)
            .filter(fileGroupEntry => fileGroupEntry[0] == fileGroupKey || !fileGroupKey )
            .forEach(fileGroupEntry => {
            fileGroupEntry[1].forEach(file=> {

                form.append(fileGroupEntry[0], file);
            })

        })
        form.append("dir", `${dir}${fileGroupKey}`)
        let {data :res } =  await window.myAjax.post(`${controllerName}/UploadFile`, 
            form,
            {
                headers:{ 
                    'Content-Type': 'multipart/form-data' 
                
                 }
            }   

        );
        if(res == true)
        {
            await getDownloadFiliesUrl();
            fileObject.value = {};
            alert("上傳成功");
        }
        else
        {
            alert("上傳失敗")
        }
    }
    async function downloadData(fileName, mine)
    {
        Object.entries(fileData.value)
        .filter(content => content[0] == fileName)
        .forEach(content => {
            Common.download2(content[1], content[0], mine);
        })


    }
    async function downloadURL(fileName, group="")
    {
        let fileData = await window.myAjax.post(`${controllerName}/DownloadFile`, {fileName : fileName, dir :`${dir}${group}` },{responseType: 'blob'});  
        Common.download2(fileData.data, fileName, fileData.data.type)

    }
    async function deleteFile(fileName, group="")
    {
        let {data :res } =  await window.myAjax.post(`${controllerName}/DeleteFile`, {fileName : fileName , dir :`${dir}${group}` });  
        if(res == true)
        {
            alert("刪除成功");
        }

    }
    async function downloadAll(group="")
    {
        let {data :res } =  await window.myAjax.post(`${controllerName}/DownloadAll`, { dir :`${dir}${group}` });  
        if(res == true)
        {
            alert("刪除成功");
        }

    }


    function readFiles(event, fileGroup, handler = (content, file) => {} )
    {
        fileData.value = [];

        function myHandlerV2(content, fileName, fileGroup)
        {   
            handler(content);

            fileData.value[fileGroup].push({name : fileName, content : content});
    
        }

        function myHandlerV1(content, fileName)
        {   
            handler(content);
            fileData.value[fileName]=  content;
    
        }

        if(fileGroup == null)
        {


            Common.readGroupFiles(event, myHandlerV1);
        }
        else
        {

            fileData.value[fileGroup] = [];
            Common.readGroupFiles(event, fileGroup, myHandlerV2);
        }

    }

    return {
        readFiles,
        onFileChange,
        downloadURL,
        downloadData,
        uploadFilies,
        getDownloadFiliesUrl,
        getTempFilies,
        deleteFile,
        downloadAll,
        downloadFiliesUrl
    }

}
