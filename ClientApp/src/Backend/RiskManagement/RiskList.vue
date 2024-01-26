<template>
        <div>
            <div class="row justify-content-between">
                <!-- <div class="col-12 col-md-8 col-xl-6 mt-3"><select class="form-control">
                        <option value="Chapter5">第五章 材料與設備抽驗程序及標準</option>
                        <option value="Chapter6">第六章 設備功能運轉測試抽驗程序及標準</option>
                        <option value="Chapter701">第七章 施工抽查程序及標準</option>
                        <option value="Chapter702">第七章 施工抽查程序及標準(環境保育)</option>
                        <option value="Chapter703">第七章 施工抽查程序及標準(職業安全衛生)</option>
                    </select></div> -->
                <div class="col-12 col-md-4 col-xl-4 mt-3">
                    <button role="button" class="btn btn-outline-secondary btn-xs mx-1" 
                        @click="page= pageCount" 
                        v-if="interPoint == 'destruct'">
                        <i class="fas fa-plus"></i>&nbsp;&nbsp;新增 
                    </button>
                </div>
            </div>
            <div>
                <div>
                    <div class="table-responsive">
                        <table border="0" class="table table1 min910" id="addnew05401">
                            <thead>
                                <tr>
                                    <th class="sort">排序</th>
                                    <th class="number">excel編號</th>
                                    <th>分項工程名稱</th>
                                    <th>建立日期</th>
                                    <th>更新日期</th>
                                    <th>功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, index) in subProjectData.list" :key="index">
                                    <td>
                                        <div>{{ index+1 }}</div>
                                    </td>
                                    <td>
                                        <div v-if="editIndex  != index ">{{ item.ExcelNo }}</div>
                                        <input v-else class="form-control" v-model="item.ExcelNo" /> 
                                    </td>
                                    <td>
                                        <div v-if="editIndex  != index ">{{ item.SubProjectName }}</div>
                                        <input v-else class="form-control" v-model="item.SubProjectName" /> 
                                    </td>
                                    <td> {{ computedDate(item, 'CreateTime') }}</td>
                                    <td> {{ computedDate(item, 'ModifyTime') }}</td>
                                    <td>
                                        <div class="row justify-content-center m-0">
                                            <a @click="editIndex = index"
                                                v-if="editIndex != index && interPoint == 'destruct'"
                                                href="javascript:void(0)" class="btn btn-color11-2 btn-xs m-1" title="編輯" id="edit-btn">
                                                <i class="fas fa-pencil-alt"></i> 編輯
                                            </a>
                                            <a 
                                                @click="updateEngRiskSubProject(item)"
                                                v-else-if="interPoint == 'destruct'"
                                                href="javascript:void(0)" class="btn btn-color11-1 btn-xs m-1" title="儲存" id="save-btn" >
                                                <i class="fas fa-save"></i> 儲存
                                            </a>
                                            <a 
                                            v-if="interPoint == 'destruct'"
                                                @click="deleteEngRiskSubProject(item)"
                                                href="#" title="刪除" class="btn btn-color9-1 btn-xs m-1">
                                                <i class="fas fa-trash-alt"></i> 刪除
                                            </a>
                                            <div class="row justify-content-center m-0">
                                                <a  v-if="interPoint == 'destruct'" title="工程拆解" class="btn btn-color11-3 btn-xs m-1" @click="edit(item)">
                                                    <i class="fas fa-plus"></i> 工程拆解
                                                </a>
                                                <a  v-if="interPoint == 'lookup'" title="風險評估" class="btn btn-color11-3 btn-xs m-1"  @click="edit(item)">
                                                    <i class="fas fa-eye"></i>風險評估
                                                </a>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr v-if="page == pageCount && interPoint == 'destruct'">
                                    <td>
                                        <div>#new </div>
                                    </td>
                                    <td>
                                        <input  class="form-control" v-model="insertSubProject.ExcelNo" /> 
                                    </td>
                                    <td>
                                        <input  class="form-control" v-model="insertSubProject.SubProjectName" /> 
                                    </td>
                                    <td> </td>
                                    <td> </td>
                                    <td>
                                        <a 
                                                @click="insertEngRiskSubProject()"

                                                href="javascript:void(0)" class="btn btn-color11-1 btn-xs m-1" title="儲存" id="save-btn" >
                                                <i class="fas fa-save"></i> 儲存
                                            </a>
                                    </td>
                                </tr>

                            </tbody>
                        </table>


                    </div>
                    <paginationV2
                        :page="page"
                        :pageCount="pageCount"
                        :pageSectionCount="5"
                        @pageChange="(p) => page = p"
                    >

                    </paginationV2>
                    <span class="d-inline-block" tabindex="0" v-b-tooltip title="Disabled tooltip">

  </span>
                    <!-- <div class="row justify-content-center" style="width: 99%;">
                        <ul role="menubar" aria-disabled="false" aria-label="Pagination"
                            class="pagination b-pagination">
                            <li role="presentation" aria-hidden="true" class="page-item disabled"><span
                                    role="menuitem" aria-label="Go to first page" aria-disabled="true"
                                    class="page-link"
                                    @click="pageSectionIndex = 0">«</span></li>
                            <li role="presentation" aria-hidden="true" class="page-item disabled"><span
                                    role="menuitem" aria-label="Go to previous page"
                                    aria-disabled="true" class="page-link"
                                    @click="pageSectionIndex > 0 ? pageSectionIndex-- : ">‹</span></li>
                            <li role="presentation" class="page-item active" 
                                :key="n"
                                v-for="n in (subProjectData.pageCount - page+1 < pageSectionCount) ? subProjectData.pageCount - page +1  : pageSectionCount">
                                <button
                                    role="menuitemradio" type="button" aria-label="Go to page 1"
                                    aria-checked="true" aria-posinset="1" aria-setsize="4" tabindex="0"
                                    class="page-link"
                                    @click="page = pageStart+ n">{{ pageStart+ n }}</button></li>
                            <li role="presentation" class="page-item"><button role="menuitem"
                                    type="button" tabindex="-1" aria-label="Go to next page"
                                    class="page-link"
                                    @click="pageSectionIndex < pageSectionEnd ? pageSectionIndex++ : ">›</button></li>
                            <li role="presentation" class="page-item"><button role="menuitem"
                                    type="button" tabindex="-1" aria-label="Go to last page"
                                    class="page-link"
                                    @click="pageSectionIndex = pageSectionEnd">»</button></li>

                        </ul>
                    </div> -->
                </div>
            </div>
        </div>


</template>

<script>

import paginationV2 from "../../components/paginationV2.vue";
import Common2 from "../../Common/Common2";
export default {
    props: ["interPoint", "editSubProject_"],
    emits:["renderTitle", "backToIndex", "changeSubProject"],
    watch:{
        editSubProject_:{
            handler(value)
            {
                this.editSubProject = value;
            }
        },
        page :{
            handler()
            {
                this.getEngRiskSubProject();
            },
            flush:'post'
        }
    },
    data: () => {
        return {
            page: 0,
            perPage : 10,
            subProjectData :{
                list : [],
                pageCount: 0
            },
            insertSubProject:{},
            editSubProject: null,
            editIndex : null
        };
    },
    computed: {
        pageCount(){
            return this.subProjectData.pageCount ==0  ? 1 : this.subProjectData.pageCount
        }
    },
    methods: {
        edit(item)
        {
            var title ;
                if(this.interPoint == "destruct") title = "新增分項工程項目"; 
                if(this.interPoint == "lookup") title = "編輯分項工程項目"; 
                this.$emit("renderTitle", title);
                this.$emit("changeSubProject", item);
        },
        async getEngRiskSubProject()
        {
            this.subProjectData  = (await window.myAjax.post("RiskManagement/getEngRiskSubProject", {page :this.page, perPage : this.perPage })).data;
        },
        computedDate(item, col)
        {
            var date = Common2.ToDate(item[col]);
            item[col] = date;
            return date;
        },

        async insertEngRiskSubProject()
        {
            let res = (await window.myAjax.post('RiskManagement/insertEngRiskSubProject', {m :this.insertSubProject })).data;    
            if(res == -1) alert("exceNo編碼重複!");
            else {
                await this.getEngRiskSubProject();
                this.insertSubProject = {};
                this.page = this.pageCount;
            }

        },
        async updateEngRiskSubProject(item)
        {
            await window.myAjax.post('RiskManagement/updateEngRiskSubProject',{ m : item} );
            this.getEngRiskSubProject();
            this.editIndex = null;
        },
        async deleteEngRiskSubProject(item)
        {
            await window.myAjax.post("/RiskManagement/deleteEngRiskSubProject", {subProjectSeq : item.Seq })
            this.getEngRiskSubProject();
        }
    },
    async mounted()
    {
        
        this.page = 1;

    },
    components:{
        paginationV2
    }
}

</script>