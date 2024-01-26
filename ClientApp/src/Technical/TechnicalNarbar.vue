<template>

    <nav class="navbar fixed-bottom d-flex flex-row-reverse" style="background:#FCF5F5;width:100%">

        <div class="d-flex" style="width:100%">

            <div class="mr-2">
                <button type="button" class="btn btn-outline-success" @click="modalShow = true" v-show="isAdmin">建立標籤</button>
            </div>
            <div class="mr-auto">
                <button type="button" class="btn btn-outline-danger" @click="deleteModalShow = true" v-show="isAdmin">刪除標籤</button>
            </div>
            <el-select style="width:25%" v-model="selectTags" multiple filterable  default-first-option
                placeholder="請點選文章標籤" multiple-limit="3">
                <el-option v-for="item in tags" :key="item.value" :label="item.label" :value="item.value">
                </el-option>
            </el-select>
            <input class="form-control " type="search" style="width:25%" v-model="searchStr" />

            <button type="button" class="btn btn-outline-success" @click="search()" sty>搜尋</button>


            <div class="input-group-prepend">
                <span class="input-group-text">最近</span>
            </div>
            <input type="number" class="form-control" aria-label="Amount (to the nearest dollar)" style="width:80px"
                v-model="days">
            <div class="input-group-append">
                <span class="input-group-text">天的文章</span>
            </div>
        </div>

        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)"
            v-bind:style="{ display: modalShow ? 'block' : 'none' }" tabindex="-1" role="dialog"
            aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white" >
                        <h6 class="modal-title">建立標籤</h6>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex">
                            <input class="form-control " type="search" style="width:50%" v-model="newTag" />

                            <button type="button" class="btn btn-outline-success" @click="storeTag()">建立</button>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal"
                            v-on:click="modalShow = false"><i class="fas fa-times"></i> 關閉</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade show" id="MyDialog" ref="MyDialog" style="background:rgb(0 0 0 / 50%)"
            v-bind:style="{ display: deleteModalShow ? 'block' : 'none' }" tabindex="-1" role="dialog"
            aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-R text-white" >
                        <h6 class="modal-title">刪除標籤</h6>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex">
                        
                            <el-select style="width:75%" v-model="deleteTags" multiple filterable  default-first-option
                                placeholder="請選擇要刪除的標籤" >
                                <el-option v-for="item in tags" :key="item.value" :label="item.label" :value="item.value">
                                </el-option>
                            </el-select>
                            <button type="button" class="btn btn-outline-danger" @click="deleteTag()">刪除</button>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-color9-1 btn-xs mx-1" data-dismiss="modal"
                            v-on:click="deleteModalShow = false"><i class="fas fa-times"></i> 關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </nav>


</template>

<script>
import axios from "axios";
export default {
    props :["tags"],
    emits: ["search"],
    data: () => {
        return {
            isAdmin: false,
            modalShow: false,
            searchStr: null,
            days: 1,
            newTag: null,
            tags: [],
            selectTags: [],
            deleteTags: [],
            deleteModalShow: false
        }
    },
    watch: {
        value: {
            handler(value) {
                console.log(value);
            }
        }
    },
    methods: {
        search() {
            this.$emit('search',this.days, this.searchStr ?? "", this.selectTags);
        },
        async storeTag(){
            let form = new FormData();
            form.append("Text", this.newTag);
            let res = await axios.post("Technical/storeTag", form);
            if(res.data.status == "success") {
                alert("建立成功");
                this.modalShow = false
                window.location.reload();

            }
        },
        async deleteTag(){
            
            
            let res = await axios.post("Technical/deleteTags", {
                tags: this.deleteTags
            });
            if(res.data.status == "success") {
                alert("刪除成功");
                this.deleteModalShow = false
                window.location.reload();
            }
        }
    }
    ,
    mounted() {
        this.isAdmin = localStorage.getItem("isAdmin") == "True";

    }

}

</script>


