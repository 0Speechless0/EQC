<script setup>
    import { reactive, onMounted,watch, ref} from "vue";
    import formalTable from "../../components/FormalTable.vue";
    import pagination from "../../components/paginationV2.vue";
    import Modal from "../../components/Modal.vue";

    const modal = ref(null); 
    const modalChangeText = ref(null);
    const filter = reactive({
        controllerName : null,
        systemTypeSeq : null,
        error : 0,
        api : 0,
        currentPage : 1,
        perPage : 10,
        userKeyWord : null,
        hasChange : null
    });

    const temp = reactive({
        totalCount : 0,
        items : [],
        userKeyWord : null
    });
    const option  = reactive({
        systemTypeList : [],
        controllerNameList : [],
        apiOptionsList : [],
        extendedOptions : []
    });

    const columnInfo = [
        { 
            colViewName : "操作者:帳號",
            colName : "UserName"
        },
        { 
            colViewName : "動作",
            colName : "ActionName"
        },
        { 
            colViewName : "來源",
            colName : "Origin"
        },
        { 
            colViewName : "使用表格",
            colName : "ActionTable"
        },
        { 
            colViewName : "開始時間",
            colName : "CreateTime"
        },
        { 
            colViewName : "結束時間",
            colName : "EndingTime"
        },
        { 
            colViewName : "執行時間(毫秒)",
            colName : "execuateTime"
        },


    ];
    
    async function getRecordFunctionOption(optionOnly, userKeyWord)
    {
        filter.optionOnly = optionOnly;
        filter.userKeyWord = userKeyWord;
        let {data : res} = await window.myAjax.post("APIRecord/GetAPIRecordAndOption", filter);
        option.controllerNameList = res.controllerOption;
        option.apiOptionsList = res.apiOptions;
        if(option.extendedOptions.length == 0 )
            option.extendedOptions = res.extendedOptions;
        temp.totalCount = res.count;
        res.list.forEach(e => {
            e.execuateTime = new Date(e.EndingTime) - new Date(e.CreateTime) 
            e.ActionName =  e.ControllerName + e.ActionName;
        });
        temp.items = res.list;
        
        filter.userKeyWord = null;
    }

    watch(() => filter.api, (v) => {
        filter.controllerName = null
        getRecordFunctionOption(true)
    });

    watch(() => filter.systemTypeSeq, (v) => {
        getRecordFunctionOption(true)
    });
    watch(() => filter.currentPage, (v) => {
        getRecordFunctionOption(false, temp.userKeyWord)
    });

    onMounted(async () => {
        option.systemTypeList  = (await window.myAjax.get("Menu/GetSystemTypeList") ).data;

    })

    function openModal(item)
    {
        if(item.ErrorCode)
        {
            modalChangeText.value = item.ErrorCode;
        }
        else
        {
            modalChangeText.value = item.ChangeText;
        }
        modal.value.show = true;
    }
</script>

<template>
    <div>

            <div class="form-group form-inline ">
                <div class="mr-4">動作結果狀態 :</div>
                <span    class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`a0`" name="a" v-model="filter.error" :value="0">
                    <label class="form-check-label" for="a0">正常</label>
                </span>
                <span   class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`a1`" name="a" v-model="filter.error" :value="1">
                    <label class="form-check-label" for="a1">錯誤</label>
                </span>
            </div>
            <div class="form-group form-inline ">
                <div class="mr-4">變更資料 :</div>
                <span    class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`f0`" name="f" v-model="filter.hasChange" :value="1">
                    <label class="form-check-label" for="f0">有</label>
                </span>
                <span   class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`f1`" name="f" v-model="filter.hasChange" :value="0">
                    <label class="form-check-label" for="f1">無</label>
                </span>
                <span    class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`f2`" name="f" v-model="filter.hasChange" :value="null">
                    <label class="form-check-label" for="f2">全部</label>
                </span>
            </div>
            <div class="form-group form-inline">
                <div class="mr-4">來源類型:</div>
                <span    class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`b0`" name="b" v-model="filter.api" :value="0">
                    <label class="form-check-label" for="b0">網頁端</label>
                </span>
                <span   class="form-check mr-4">
                    <input type="radio" class="form-check-input" :id="`b1`" name="b" v-model="filter.api" :value="1">
                    <label class="form-check-label" for="b1">非網頁端</label>
                </span>

            </div>
            <hr>
            <div v-if="!filter.api">
                <div class="form-group  form-inline ">
                <div class="col-1">子系統:</div>
                <div class="form-group  form-inline col-11">
                    <span v-for="(item,index) in option.systemTypeList"  :key="index"  class="form-check m-2">
                        <input type="radio" class="form-check-input" name="c" :id="`c${index}`" :value="item.Value" v-model="filter.systemTypeSeq">
                        <label class="form-check-label" :for="`c${index}`">{{ item.Text }}</label>
                        </span>

                    </div>
                </div>

                <div class="form-group form-inline" >
                    <div class="col-1">子系統功能:</div>
                    <p v-if="option.controllerNameList.length == 0" style="color:red" class="text-center col-1" > 無</p>
                    <div class="form-group form-inline col-11" >
                    <span v-for="(item,index) in option.controllerNameList"  :key="index"  class="form-check m-2">
                        <input type="radio" class="form-check-input" name="d"  :id="`d${index}`" :value="item.Value" v-model="filter.controllerName">
                        <label class="form-check-label" :for="`d${index}`">{{ item.Text }}</label>
                    </span>
                    </div>
                </div>

                <hr />
                <div class="form-group form-inline" >
                    <div class="col-1">其他子功能:</div>
                    <p v-if="option.extendedOptions.length == 0" style="color:red" class="text-center col-1" > 無</p>
                    <div class="form-group form-inline col-11" >
                    <span v-for="(item,index) in option.extendedOptions"  :key="index"  class="form-check m-2">
                        <input type="radio" class="form-check-input" name="d"  :id="`e${index}`" :value="item.Value" v-model="filter.controllerName">
                        <label class="form-check-label" :for="`e${index}`">{{ item.Text }}</label>
                    </span>
                    </div>
                </div>
            </div>
            <div v-else>
                <div class="form-group form-inline" >
                    <div class="col-2">API:</div>
                    <p v-if="option.apiOptionsList.length == 0" style="color:red" class="text-center col-1" > 無</p>
                    <span v-for="(item,index) in option.apiOptionsList"  :key="index"  class="form-check m-2">
                        <input type="radio" class="form-check-input" name="g"  :id="`g${index}`" :value="item.Value" v-model="filter.controllerName">
                        <label class="form-check-label" :for="`g${index}`">{{ item.Text }}</label>
                    </span>

                </div>
            </div>

            <hr>
            <div class="form-group row ">

                <div class="col-sm-2">
                <input type="text"  class="form-control" id="staticEmail" v-model="temp.userKeyWord">
                </div>
                <div class="col-sm-2">
                    <button type="button" class="btn btn-primary mb-2" @click="getRecordFunctionOption(false, temp.userKeyWord)">帳號查詢</button>
                </div>
                <div class="col-sm-2">
                <button type="button" class="btn btn-success mb-2" @click="getRecordFunctionOption">查詢全部</button>
                </div>

            </div>
            <hr>
            <div class="form-group">
                <formalTable :columnInfo="columnInfo" :items="temp.items">
                    <template #otherTh >
                            <th>
                                變更/錯誤
                            </th>
                    </template>
                    <template #otherTd="{item}" >
                        <td v-if="item.ChangeText">
                            <button @click.prevent="openModal(item)" title="編輯" class="btn btn-color11-2 btn-xs m-1"><i class="fas fa-eye"></i> 查看</button>
                        </td>
                        <td v-else>
                            <p style="color:red">無</p>
                        </td>
                    </template>
                </formalTable>
            </div>
            <div class="form-group d-flex justity-content-center" v-if="temp.totalCount > 0">
                <pagination 
                    :page="filter.currentPage" 
                    :pageCount="Math.ceil(temp.totalCount/filter.perPage) " 
                    :pageSectionCount="5"
                    @pageChange="(p) => filter.currentPage = p"
                    > 

                </pagination>
            </div>
        <Modal ref="modal" title="變更紀錄">
            <template #body>
                <pre>{{ modalChangeText }}</pre>
       
            </template>
        </Modal>
    </div>
</template>
<style>

</style>