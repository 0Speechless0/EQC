<template>
    <div>
        <form class="form-group">
            <div v-if="Role == 2 || Role == 1|| Role == 3" class="form-row">
                <unitFilter :maxUnitLevel="2" :startLevel="Role >2 ? 1: 0"
                            :initSubUnit="[ Role == 1 ? '' :UnitName1]"
                                    :newUnitLevelOptions="userStore.unitOptions" @afterUnitChange="afterUnitChange"  class="form-row col-10"></unitFilter>
                
                <div  class="col-10 col-sm-1 mt-3">
                    <input type="text" class="form-control" v-model="userStore.nameSearch"/>
                </div>
                <div class="col-2 col-sm-1 mt-3">
                    <button type="button" class="btn btn-success" @click="search()"> 搜尋</button>
                </div>
            </div>

        </form>
        <div>
            <ul class="nav nav-tabs" role="tablist">
                <li class="nav-item"><a v-on:click="selectUserList" data-toggle="tab" href="" class="nav-link active">清單</a></li>
                <li class="nav-item"  > <a v-on:click="selectUserSafety" data-toggle="tab" href="" class="nav-link">安全性</a></li>
                <li>
                    <a href="Users/DownloadUser" download class="btn btn-color11-1 btn-xs mx-1"><i class="fas fa-download"></i><font style="vertical-align: inherit;"><font style="vertical-align: inherit;">下載</font></font></a>
                </li>
            </ul>
            
        </div>

        <div class="tab-content">
            <UsersList v-if="selectTab=='UsersList'"  ref="userManager"></UsersList>
            <UsersSafety v-if="selectTab=='UsersSafty'"  ></UsersSafety>

        </div>
        <div style="width:99%;" class="row justify-content-center">
                <b-pagination id="pageControl"
                    :total-rows="userStore.totalRows"
                    :per-page="userStore.perPage"
                    v-model="userStore.currentPage">
                </b-pagination>
               
            </div>
    </div>
</template>
<script>
import { userStore as store } from './userStore';
import {ref, computed, watch } from "vue";
import unitFilter from "../../components/unitFilter.vue";
import UsersList from "./UsersList.vue";
import UsersSaftety from "./UsersSafety.vue";

    export default {
        components: {//s20230506
            UsersList: UsersList,
            UsersSafety: UsersSaftety,
            unitFilter : unitFilter
        },
        data: function () {
            return {
                selectTab: 'UsersList'
            };
        },
        methods :{
            setList(list)
            {
                this.userlist = list;
            },
            search(){
                this.userStore.currentPage = 1;
                this.userStore.getList();

            },
            afterUnitChange(newUnit)
            {
                console.log("newUnit", newUnit);
                this.userStore.subUnit = newUnit;
                this.userStore.getList();
            },
            selectUserSafety()
            {
                this.selectTab = "UsersSafty";
                this.userStore.hasConstCheckApp = true;
                this.userStore.currentPage = 1;
                this.userStore.getList();
            },
            selectUserList()
            {
                this.selectTab = "UsersList";
                this.userStore.hasConstCheckApp = false;
                this.userStore.currentPage = 1;
                this.userStore.getList();
            }
        },
        computed :{
            hasSafeList(){
                return this.userStore.userList.filter(e => e.ConstCheckAppLock > 0 ).length > 0;
            }

        },
        setup()
        {
            const userStore = store;
            console.log("store", store);
            const Role = computed(() => store.userInfo.RoleSeq) ;
            const UnitName1 = computed(() => store.userInfo.UnitName1) ;
            watch(() => userStore.currentPage, () => {
                userStore.getList();
                console.log(3);
            })
            return {
                userStore,
                Role,
                UnitName1
            }
        },

        async mounted()
        {


            // const self = this;
            // // 資料初始化
            // // self.Role = parseInt(localStorage.getItem('Role') );

            // let isAdmin = localStorage.getItem('isAdmin') == 'True' ? true : false;
            // let isEQCAdmin = localStorage.getItem('isEQCAdmin') == 'True' ? true : false;
            // let userSeq = localStorage.getItem("userSeq") ;
            await this.userStore.getUserInfo();
            console.log(this.userStore.userInfo);
            this.selectUserList()
            // if(this.Role == 3) this.userStore.unitOptions = [this.userStore.userInfo.UnitName1 ];

            // if( this.userStore.isLastLevel ||  (!this.userStore.isLastLevel && this.userStore.isOutSource) ) {
            //     this.userStore.getChildList();
            // }
            // else  if(this.Role != 3 )
                // this.userStore.getList();

        }

    }
</script>