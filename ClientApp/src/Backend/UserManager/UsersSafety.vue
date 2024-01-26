<template>
    <div>
        <!--<ol class="breadcrumb">
            <li class="breadcrumb-item">-->
        <!--<a href="/Users/Update" title="後台管理">後台管理</a>-->
        <!--後台管理
            </li>
            <li class="breadcrumb-item active" aria-current="page" title="使用者管理">
                使用者管理
            </li>
        </ol>
        <h1>使用者管理</h1>-->

        <div class="table-responsive">
            <table border="0" class="table table1 min910">
                <thead>
                    <tr>
                        <th>項次</th>

                        <th >姓名</th>
                        <th >帳號</th>
                        <th >功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-bind:key="index" v-for="(item,index) in userList">
                        <td style="text-align: center;">{{index+1}}</td>
                        <td style="text-align: center;">{{item.DisplayName}}</td>
                        <td style="text-align: center;">{{item.UserNo}}</td>
                        <td class="d-flex justify-content-center">
                            <button class="btn btn-color11-3 btn-xs mx-1" @click="unLockConstCheckApp(item.ConstCheckAppLock, index)"><i class="fas fa-lock"></i>抽查APP解鎖</button>
                        </td>

                    </tr>
                    <tr v-if="userList==null||userList.length==0">
                        <td colspan="9" class="text-center">--查無資料--</td>
                    </tr>
                </tbody>
            </table>
            <div style="width:99%;" class="row justify-content-center">
                <!--v-on:change="onPageChange"-->
                <!-- <b-pagination id="pageControl" :total-rows="totalRows"
                              :per-page="perPage"
                              v-model="currentPage">
                </b-pagination> -->
                <!--<div>
                    目前的排序鍵值: <b>{{ sortBy }}</b>, 排序方式:
                    <b>{{ sortDesc ? 'Descending' : 'Ascending' }}</b>
                </div>-->
            </div>
        </div>

    </div>
</template>
<script>
    import moment from 'moment';
    import {userStore} from "./userStore";
    import {computed} from "vue";
    // Suppress the warnings
    moment.suppressDeprecationWarnings = true;

    export default {
        computed: {
            editableFields() {
                return this.fields.filter(field => {
                    return field.editable == false
                })
            }
        },
        props :["users"],
        data: function () {
            return {
                Role : 0,
                subUnitSeq : 0,
                nameSearch: null,
                isAdmin: false,
                isEQCAdmin: false,
                isLastLevel : false,
                isOutSource : false,
                userSeq : null,
                userInfo : {},
                subUnit : ["", "", ""],
                currentPage: 1,
                perPage: 10,
                totalRows: 0,
                posText : "",
                unitOptions : [],
                // 人員列表

                // 單位下拉-第一層
                units1: [],
                // 單位下拉-第二層
                units2: [],
                // 單位下拉-第三層
                units3: [],
                // 單位下拉-第一層(值)
                unitSeq1: '0',
                // 單位下拉-第二層(值)
                unitSeq2: '0',
                // 單位下拉-第三層(值)
                unitSeq3: '0',
                // 單位下拉-第一層(編輯用)
                unitsEdit1: [],
                // 單位下拉-第二層(編輯用)
                unitsEdit2: [],
                // 單位下拉-第三層(編輯用)
                unitsEdit3: [],
                // 編輯的人員資料
                userEdit: {},
                // 職稱下拉
                positions: [],
                // 角色下拉
                roles: [],
                //簽名檔
                file: null,
                files: new FormData(),
                dragging: false
            };
        },

        emits :[ "onUnLock"],
        methods: {
            async unLockConstCheckApp(seq, index)
            {
                let {data} = await window.myAjax.post("Users/unLockConstCheckApp", { constCheckAppLockSeq : seq});
                if(data)
                {
                    this.userList = 
                        this.userList.slice(0, index).concat(this.userList.slice(index+1, this.userList.length));
                    alert("解鎖成功");
                    this.$emit("onUnLock");
                }
            },


        },
        // watch :{
        //     users :{
        //         handler(value)
        //         {
        //             console.log("this.userList", value);
        //             this.userList_ = value;
        //         }
        //     }
        // },
        setup()
        {

            const  userList = computed(() => userStore.userList);
            console.log("fdf", userList);
            return {
                userList
            }
        }

    }
</script>
