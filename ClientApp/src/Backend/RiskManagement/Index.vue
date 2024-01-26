<template>
    <div>
        <div>

            <div class="card-header">
                    <h3 class="card-title font-weight-bold" v-if="interPoint == 'destruct'">{{ title ?? "施工風險分項工程拆解" }}</h3>
                    <h3 class="card-title font-weight-bold" v-if="interPoint == 'lookup'">{{ title ?? "施工風險評估" }}</h3>
                </div>
                
                <div id="app" class="card-body" >
                    <RiskList v-show="title==null" 
                        :interPoint="interPoint" 
                        :editSubProject_ ="editSubProject"
                        @backToIndex="() => title = null"
                        @renderTitle="(t) => title = t"
                        @changeSubProject="(item) => editSubProject = item"> 

                    </RiskList>
                    <ConstructionTree v-if="title && interPoint == 'destruct'" 
                        @goBack="(jsonStr) => {title= null; editSubProject.SubProjectJson = jsonStr;}"
                        :subProject="editSubProject" ></ConstructionTree>
                    <Lookup v-if="title && interPoint == 'lookup'" 
                        @goBack="title= null"
                        :subProject="editSubProject" >
                    </Lookup>


                </div>
        </div>


    </div>

</template>

<script>
import ConstructionTree from './ConstructionTree.vue';
import RiskList from './RiskList.vue';
import Lookup from "./lookup.vue";

export default {
    components: { ConstructionTree, RiskList, Lookup },
    data : () => {
       return {
        interPoint : null,
        title : null,
        editSubProject : {}

       }
    },

    mounted() {
        this.interPoint =  window.location.pathname.split('/')[2]; 
        console.log(this.interPoint);
    },
    methods :{
        async getEngRiskSubProject()
        {
            this.subProjectData  = (await window.myAjax.post("RiskManagement/getEngRiskSubProject", {page :this.page, perPage : this.perPage })).data;
            console.log(this.subProjectData);
        },
    }
}

</script>