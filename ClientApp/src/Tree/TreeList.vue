<template>
    <div class="card whiteBG mb-4 colorset_B" v-if="route == null">
        <div class="card-header d-flex ">
            <h3 class="card-title font-weight-bold">產製列表</h3>
            <div class="mt-auto">
                <a style="color: white;" href="https://www.youtube.com/watch?v=EGinhApSUJo&ab_channel=%E6%95%A6%E9%99%BD%E7%A7%91%E6%8A%80" target="_blank">
                                        操作影片
                                    </a>
            </div>

        </div>
        <div class="card-body">
            <div id="app">
                <div>
                    <form class="form-group">
                        <div class="form-row">
                            <div class="col-1 mt-3"><select class="form-control" v-model="year">
                                    <option v-for="(option, index) in data.yearOptions" :value="option" :key="index">{{
                                        option }}</option>
                                </select></div>
                            <unitFilter :newUnitLevelOptions="data.unitOptions" :maxUnitLevel="1"
                                @afterUnitChange="afterUnitChange" class="form-row col-6"></unitFilter>

                            <div class="col-12 col-sm-1 mt-3">
                                <button type="button"
                                    style="color: #fff;background-color: #6c757d;border-color: #6c757d; border-radius: 5px;"
                                    @click="getEngEditList()">查詢</button>
                            </div>

                            <div class="col-12 col-sm-4 mt-3 d-flex justify-content-end">
                                <span class="m-1">樹種年度</span>
                                <input class="form-control  col-6 col-sm-6 col-lg-3" type="text" v-model="exportYear" />
                            </div>
                        </div>
                    </form>

                    <div class="table" v-if="data.list && data.list.length > 0">
                        <div class="row justify-content-between">
                            <div class="form-inline col-12 col-md-8 small"><label class="my-1 mr-2"> 共 <span
                                        class="text-danger">{{ data.count }}</span> 筆，每頁顯示 </label><select
                                    class="form-control sort form-control-sm" v-model="perPage">
                                    <option value="10">10</option>
                                    <option value="20">20</option>
                                    <option value="30">30</option>
                                </select><label class="my-1 mx-2">筆，共<span class="text-danger">{{ pageCount
                                }}</span>頁，目前顯示第</label><select class="form-control sort form-control-sm"
                                    v-model="page">
                                    <option v-for="n in pageCount" :value="n" :key="n"> {{ n }} </option>
                                </select><label class="my-1 mx-2">頁</label></div>

                        </div>

                        <table border="0" class="table table1 min910">
                            <thead class="insearch">
                                <tr>
                                    <th class="sort">排序</th>
                                    <th class="number">工程編號</th>
                                    <th>類別</th>
                                    <th>工程名稱</th>
                                    <th>執行機關</th>
                                    <th>執行單位</th>
                                    <!-- <th>勾稽工程會</th>
                            <th>Pcces檔案</th> -->
                                    <th style="width: 200px;" class="text-center">
                                        <div class="row justify-content-center m-0">
                                            <a class="p-1"
                                                :href="`./getTreeCollection?Year=${exportYear}&ExecUnitName=${exportUnit}`"
                                                download>
                                                <button title="下載" class="btn btn-color11-3 btn-xs mx-1">
                                                    <i class="fas fa-download"></i> 下載總表excel
                                                </button>
                                            </a>
                                            <a class="p-1"
                                                :href="`./getTreeCollectionArea?Year=${exportYear}&ExecUnitName=${exportUnit}`"
                                                download>
                                                <button title="下載" class="btn btn-color11-3 btn-xs mx-1">
                                                    <i class="fas fa-download"></i> 下載面積表excel
                                                </button>
                                            </a>
                                        </div>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for=" (item, index) in data.list" :key="index">
                                    <td>{{ index + 1 }}</td>
                                    <td>{{ item.EngNo }}</td>
                                    <td>{{ item.TPEngTypeName }}</td>
                                    <td>{{ item.EngName }}</td>
                                    <td>{{ item.execUnitName }}</td>
                                    <td>{{ item.execSubUnitName }}</td>
                                    <!-- <td>無</td>
                            <td><button class="btn btn-color11-1 btn-xs mx-1"><i
                                        class="fas fa-download"></i> 下載<br>111/11/21 </button></td> -->
                                    <td>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="form-group" style="color:red" v-if="data.list && data.list.length == 0">
                        無資料
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import unitFilter from "../components/unitFilter.vue";
export default {

    data: () => {
        return {
            route: null,
            editTreeMainPrjSeq: null,
            editTreeMainSeq: null,
            lastSubUnitIndex: 0,
            subUnit: ["", ""],
            data: {},
            year: new Date().getFullYear() - 1911,
            page: 1,
            perPage: 10,
            exportYear: null,
            exportUnit: null

        }
    },
    components: {
        unitFilter
    },
    // watch :{
    //     perPage: {
    //         async handler()
    //         {
    //             await this.getEngEditList()

    //         },
    //         flush: 'post'
    //     },
    //     page: {
    //         async handler()
    //         {
    //             await this.getEngEditList()

    //         },
    //         flush: 'post'
    //     },
    //     year :{
    //         async handler()
    //         {
    //             await this.getEngEditList()
    //         },
    //         flush: 'post'
    //     },
    //     subUnit:{
    //         async handler()
    //         {
    //             await this.getEngEditList()
    //         },
    //         flush: 'post'
    //     }
    // },
    watch: {
        year: {
            handler(value) {
                this.page = 1;
            }
        }
    },
    methods: {
        afterUnitChange(subUnit) {
            this.page = 1;
            this.subUnit = subUnit;
        },
        async getEngEditList(subUnit) {
            this.exportUnit = this.subUnit[0];
            this.data = (await window.myAjax.post("Tree/GetEngEditList",
                {
                    page: this.page,
                    perPage: this.perPage,
                    year: this.year,
                    subUnit: this.subUnit
                })).data
        },
        async download() {
            // var resp =  await window.myAjax.get("Tree/getTreeCollection");
            // const blob = new Blob([resp.data]);
            // var a = document.createElement('a');
            // var objectUrl = URL.createObjectURL(blob);
            // a.filename = `${new Date().year - 1911}年植樹彙整總表.xlsx`;
            // a.href = objectUrl;
            // a.click();
            // window.URL.revokeObjectURL(objectUrl);
        }

    },

    computed: {
        pageCount() {
            return this.data.count != undefined ? Math.ceil(this.data.count / this.perPage) : 0;
        }
    },
    async mounted() {
        this.data = (await window.myAjax.post("Tree/GetEngEditList",
            {
                page: this.page,
                perPage: this.perPage,
                year: this.year,
                subUnit: this.subUnit
            })).data;
        this.data.list = null;
        this.exportYear = new Date().getFullYear() - 1911;

    }

}

</script>