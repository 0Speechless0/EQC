<template>


    <div class="card whiteBG mb-4 pattern-F ">
        <div class="card-header">
            <h3 class="card-title font-weight-bold">施工影像資料庫</h3>
        </div>
        <div class="card-body">

            <div class="justify-content-start form-inline">
                <label class="mr-1">年度</label>
                <select v-model="selectedTenderYearStart" class="form-control">
                    <option v-for="option in TenderYearList" v-bind:value="option" v-bind:key="option">
                        {{ option }}
                    </option>
                </select>
                ~
                <select v-model="selectedTenderYearEnd" class="form-control">
                    <option v-for="option in TenderYearListEnd" v-bind:value="option" v-bind:key="option">
                        {{ option }}
                    </option>
                </select>
                <div class="p-2">
                    <input type="text" class="form-control ml-1 " placeholder="關鍵字查詢" value="" v-model="searchStr"  />
                    <button type="button" class="btn btn-outline-secondary btn-sm " @click="search()"><i
                        class="fas fa-search"></i></button>
                </div>

            </div>

            <div v-for="(paginaion, yearIndex) in engYearPagination" :key="yearIndex">
                <h5>{{ paginaion.TenderYear + 1911 }} </h5>
                <div class="mb-3 d-flex justify-content-start" >
                        <el-pagination
                        background
                        layout="prev, pager, next"
                        @current-change="(val) => paginaion.view = paginaion.origin.slice(val, (val+1)*pageAmount )"
                        :page-size="pageAmount"
                        :current-page.sync="paginaion.pageIndex"
                        :total="paginaion.origin.length">
                        </el-pagination>
                        <span class="align-self-middle" style="color:red">
                            {{paginaion.origin.length }}
                        </span>
                        張
                    </div>
                <div class="row pics">
                    <div class="col-12 col-md-6 col-xl-3 mb-4" v-for="(eng, engIndex) in paginaion.view" :key="engIndex">
                        <div class="card" style="height:360px">
                            <ul class="list-group list-group-flush small">
                                <li class="list-group-item d-flex">
                                    <div>{{ eng.TenderNo }}</div>
                                                                    <a href="javascript:void(0)" class="card-link a-view ml-auto" data-toggle="modal"
                                    data-target="#modal" title="檢視" @click="showModal(eng)"><i
                                        class="fas fa-eye"></i></a>
                                </li>
                                <li class="list-group-item">{{ eng.TenderName }}</li>
                                
                            </ul>
                            <img :src="'FileUploads/Eng/' + eng.Seq + '/' + eng.UniqueFileName" style="height:180px" />
                            <div class="card-body d-flex justify-content-around">
                                


                                    <p class="align-self-center">{{ eng.Memo }}</p>
                                    <p v-if="eng.HasBee" class="ml-3 align-self-center">
                                        有蜂窩
                                    </p>



                            </div>



                        </div>
                    </div>
                </div>
            </div>

            <!-- 小視窗 看大圖 -->
            <div class="modal fade"  id="modal" >
                <div class="modal-dialog modal-xl modal-dialog-centered ">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h6 class="modal-title font-weight-bold">檢視資料 - {{ editItem.TenderName }}</h6>
                            <button type="button" id="close" class="close" data-dismiss="modal" aria-label="Close" @click="modalShow = false">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body text-center " style="height:500px;overflow: auto;">
                            <el-carousel height="400px" @change="carouselChange" :autoplay="false" trigger='click'
                                @mouseenter.native="isClick = true" @mouseleave.native="isClick = false">
                                <el-carousel-Item v-for="(image, index ) in editItem.Images" :key="index">


                                    <img :src="'FileUploads/Eng/' + editItem.Seq + '/' + image.UniqueFileName"
                                        style="width:60%;max-height: 400px;" />
                                </el-carousel-Item>
                            </el-carousel>
                            <div class="d-flex justify-content-center mt-4">

                                <input v-model="editItem.Memo" v-if="edit" />
                                <p class="card-text" v-else>{{ editItem.Memo }}</p>
                                <div class="ml-3" v-if="editItem.HasBee && !edit">
                                    有蜂窩
                                </div>
                                <select v-if="edit" v-model="editItem.HasBee">
                                    <option :value="editItem.HasBee" selected v-if="editItem.HasBee">{{editItem.HasBee}}</option>
                                    <option value="有蜂窩" v-else >有蜂窩</option>
                                    <option value="" v-if="!editItem.HasBee" selected>無蜂窩</option>
                                    <option value="" v-else >無蜂窩</option>
                                </select>
                                <a title="編輯" href="javascript:void(0)" class="ml-2"><i class="fas fa-pencil-alt"
                                        style="color:#000 ;" @click="editContent(editItem.Images[editImageIndex])"></i></a>
                                <a class="card-link a-delete ml-4" title="刪除" href="javascript:void(0)"><i
                                        class="fas fa-trash-alt "
                                        @click="Delete(editItem.Images[editImageIndex])"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>


</template>

<script>
import axios from "axios";

export default {

    watch: {
        selectedTenderYearStart: {
            handler(value) {
                this.TenderYearListEnd = this.TenderYearList.filter(e => e >= value);
                if (this.TenderYearListEnd.length == 1 && this.TenderYearListEnd[0] == value) {
                    this.selectedTenderYearEnd = this.TenderYearListEnd[0];
                }
            }
        }
    },
    data: () => {
        return {
            selectedTenderYearStart: 0,
            selectedTenderYearEnd: 0,
            TenderYearList: [],
            TenderYearListEnd: [],
            searchStr: "",
            staticEngList: [],
            isEdit: {},
            editImageIndex: 0,
            editItem: {},
            isClick: false,
            modalShow :false,
            edit: false,
            pageAmount : 10,
            engOriginList: [],
            engYearPagination: [
                // {
                //     TenderYear:"2022",
                //     TenderNo : "110-123123-23",
                //     TenderName : "HHHH",
                //     Image : "dfdf/df/sdf",
                //     HasBee: true
                // }

            ]
        }
    },
    methods: {
        showModal(item){
            this.modalShow = true;
            this.editItem = item;
            this.edit = false;
        },
        carouselChange(n) {
            if (this.isClick) {
                console.log("carouselChange", n);
                this.editImageIndex = n;
                let item  =this.editItem.Images[n];
                item.Images = this.editItem.Images;
                this.editItem = item;
            }

        },
        async editContent(item) {
            console.log("editContent");
            if (!this.edit) {
                this.edit= true;

                return;
            }

            let res = await axios.put("ConstructionImage/updateContent/" + item.Seq, {
                item: {
                    Memo: this.editItem.Memo,
                    RESTful: this.editItem.HasBee,
                    UniqueFileName : this.editItem.UniqueFileName
                }
            });
            if (res.data.status == "success") {
                alert("更新成功");
                this.getEngList();
                this.edit= false;
            }
        },
        async Delete(item) {
            let c = confirm("確定要刪除?");
            if (!c) return;
            let res = await axios.post("ConstructionImage/Delete/"+item.Seq,{
                UniqueFileName : item.UniqueFileName
            });
            if (res.data.status == "success") {
                alert("刪除成功");
               

                await this.getEngList();
                let eng = this.engOriginList.find( eng => eng.Seq == item.Seq && eng.Images);
            
                if(eng) {
                    this.editItem = eng
                }
                else{
                    this.editItem = {};
                    this.modalShow = false;
                    document.getElementById('close').click();
                   
                }
            }
        },
        async getEngList() {
            let res = await window.myAjax.get("ConstructionImage/getEngData", {
                params: {
                    startYear: this.selectedTenderYearStart,
                    endYear: this.selectedTenderYearEnd
                }
            });
            if (res.data.status == "success") {
                res.data.data.forEach( e => {
                    this.isEdit[e.Seq] = {};
                });
                this.engOriginList = res.data.data;
                this.TenderYearListEnd = this.TenderYearList;
                this.staticEngList = this.group(res.data.data, ["TenderYear", "Seq"], "Images")
                    .sort((a, b) => {
                        if (a.TenderYear > b.TenderYear) return -1;
                        if (a.TenderYear < b.TenderYear) return 1;
                        return 0;
                    });

                var engList = this.staticEngList.filter(e => e.TenderYear >= this.selectedTenderYearStart && e.TenderYear <= this.selectedTenderYearEnd);

                this.engYearPagination = engList.map(
                    yearGroup => {
                        return {
                            TenderYear : yearGroup.TenderYear,
                            pageIndex : 0,
                            view : yearGroup.list.slice(0, this.pageAmount),
                            origin : yearGroup.list
                        }

                    })
                console.log("group", this.staticEngList)
            }
        },
        async getEngYearList() {
            let res = await window.myAjax.get("ConstructionImage/getEngYearList");
            if (res.data.status == "success") {
                this.TenderYearList = res.data.data;
                this.TenderYearListEnd = this.TenderYearList;
            }

        },
        group(data, keys, lastKey) {

            let result = [];
            let temp = { _: result };

            data.forEach(function (a) {
                keys.reduce(function (r, k, index) {
                    if (index == keys.length - 1) {

                        if (!r[a[k]]) {
                            r[a[k]] = a;

                            r[a[k]][lastKey] = [];
                            r._.push(r[a[k]]);
                        }
                        return r[a[k]][lastKey];
                    }
                    else if (!r[a[k]]) {
                        r[a[k]] = { _: [] };
                        r._.push({ [k]: a[k], list: r[a[k]]._ });
                    }
                    return r[a[k]];
                }, temp).push(a);

            });

            return result;
        },
        search() {
            this.getEngList();

        }

    }
    ,
    mounted() {

        this.selectedTenderYearStart = new Date().getFullYear() - 1911;
        this.selectedTenderYearEnd = this.selectedTenderYearStart;
        this.getEngYearList();
        this.getEngList();
    }



}

</script>
<style>
</style>