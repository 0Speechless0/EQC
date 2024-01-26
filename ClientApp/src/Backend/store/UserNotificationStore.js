import {reactive, ref, watch } from "vue";

import { useSelectionStore } from "../../store/SelectionStore.js";
import usePageItem from "../../store/PageItemStore.js";
import Common from  "../../Common/Common2.js";

export function useUserNotificationStore()
{

    const selectionStore =  useSelectionStore();
    const emitUnitType = ref(null);
    let dateNow = new Date();
    const emitContent = reactive({
        Role : null,
        Unit : null,
        Title :null,
        EmitContent : null,
        ExpireTime : Common.ToDate(new Date(dateNow.getFullYear(), dateNow.getMonth() + 1, 0), false)
    });
    const specifyUnit = ref(null);
    const unitList  = ref([]);
    const roleList  = ref([]);

    const isEditJsonImport =ref(false);

    const detailModalShow = ref(false);
    const pageItem = usePageItem("UserNotification/GetList", 10);


    const viewNotification =ref({
        GetRoleTypeName :  
        (roleSeq) =>
        {

            return selectionStore.Map["Role" + roleSeq] ?? "所有";
        },
        GetUnitTypeName : 
        (unitSeq) =>
        {
            console.log("dsfdf");
            return selectionStore.Map["Unit" + unitSeq] ?? "所有";
        },
        detailModalShow : detailModalShow

    });


    watch(() => detailModalShow , (v) => {

        if(v == false)
        {
            isEditJsonImport.value = false;
        }
    })

    console.log("pageItem",  pageItem
    );
    selectionStore.GetSelection("Unit/GetUnitList", "Unit")
        .then( (result) => unitList.value = result );

    selectionStore.GetSelection("Role/GetRoleListv2", "Role")
        .then( (result) => roleList.value = result );

    async function submit(notification)
    {

        if(notification)
        {

           Object.assign(emitContent, notification);
        }
        console.log("emitContent", emitContent);

        let {data} = await window.myAjax.post("UserNotification/Submit", {
            roleSeq : emitContent.Role,
            unitSeq : emitContent.Unit ,
            content : emitContent.EmitContent ?? "",
            title : emitContent.Title,
            expire_time : emitContent.ExpireTime
        });
        if(data == true) {
            alert("發送成功");
            await pageItem.GetPageItem(); 
            emitContent.value = {};
        }


    }
    function lookForNotification(notification)
    {
        detailModalShow.value = false;
        detailModalShow.value = true;
        viewNotification.value = Object.assign(viewNotification.value, notification);
        console.log("viewNotification", viewNotification.value);
    }

    async function deleteNotification(seq)
    {
        let {data} = await window.myAjax.post("UserNotification/Delete", { 
            seq :seq
        } );
        if(data == true) {
            alert("刪除成功");
            await pageItem.GetPageItem(); 
        }
    }

    return {
        unitList,
        roleList,
        emitUnitType,
        emitContent,
        specifyUnit,
        viewNotification,
        detailModalShow,
        pageItem ,
        isEditJsonImport ,
        lookForNotification,
        deleteNotification,
        submit,

    }
}