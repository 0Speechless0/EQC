<template>

    <div>
        <Narbar style="height:60px" @search="search" ref="Narbar" :tags="tags" />
        <div v-show="!addArtical" class="mb-3">
            <button class="btn btn-info " type="button" style="    background-color: #219086;"
                @click="addArtical = !addArtical" title="編輯">
                新增文章
            </button>
        </div>
        <div v-show="addArtical" class="mb-3">
            <button class="btn btn-secondary " type="button" 
                 title="收"
                 @click="addArtical = !addArtical">
                收
            </button>
        </div>
        <div class="card" style="margin-bottom: 20px; background-color:#E8E3E2;" v-show="addArtical">
            <div class="card-body ">
                <div class="mb-3">
                    <input type="text" class="form-control" placeholder="請輸入您的標題" style="width:35%" v-model="title" />
                </div>
                <div class="mb-3">
                    <el-select style="width:60%" v-model="selectTags" multiple filterable default-first-option
                        placeholder="請點選文章標籤" multiple-limit="3">
                        <el-option v-for="item in tags" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </div>
                <div class="d-flex flex-row bd-highlight mb-3" v-if="upload">


                    <Upload text="檔案上傳" ref="Upload" @fileChange="onChangeFile" />

                    <button type="button" class="btn btn-success" @click="upload = false" v-if="!this.file && upload">連結</button>
                </div>
                <div class="d-flex flex-row bd-highlight mb-3" v-else>
                    <button type="button" class="btn btn-secondary" @click="upload = true"> 取消連結 </button>
                </div>
                <div class="mb-3" v-if="!upload">
                    <input type="email" class="form-control" id="exampleFormControlInput1" placeholder="連結"
                        v-model="url">
                </div>
                <div class="mb-3">
                    <textarea class="form-control" id="exampleFormControlTextarea1" placeholder="寫些什麼...." rows="6"
                        v-model="text"></textarea>
                </div>
                <button type="button" class="btn btn-success ms-auto p-2" @click="Save()">發文</button>

            </div>
        </div>

            <Artical v-for="(artical, index) in ViewArticals" :artical="artical" :key="index" @afterEditArtical="() => getArticals()" :tags="tags"/>



      <!-- <div style="margin-top:50px" class="d-flex justify-content-center" >
          <b-pagination :total-rows="totalRows"
                        :per-page="perPage"
                        v-model="currentPage">
          </b-pagination>

      </div> -->
    </div>
</template>
<script>

import Artical from "./TechnicalArtical";

import Narbar from "./TechnicalNarbar";
import Upload from "./UploadComponent";
import axios from "axios";
import Common  from "../Common/Common";
import { relativeTimeThreshold } from "moment";
export default {
    watch: {

        currentPage:{
            handler: function(value){
                this.ViewPageArticals = this.ViewArticals.slice( (value-1)*this.perPage, value*this.perPage);
            }
        }
    },
    data: () => {
        return {
            addArtical :false,
            ViewArticals: [],
            searchStr: null,
            articals: [
            ],
            ViewPageArticals:[],
            file: null,
            upload: true,
            text: null,
            title: null,
            url: " ",
            totalRows :0,
            perPage:7,
            currentPage:1,
            tags:[
                {
                    label:"asdsdf",
                    value: "1"
                },
                {
                    label: "bbb",
                    value:"2"
                }
            ],
            selectTags:[]

        }
    },
    components: {
        Artical,
        Narbar,
        Upload
    },
    methods: {
        async getArticals() {
            
            let res = await axios.get("/Technical/GetArticals");
            if (res.data.status == "success") {
                this.articals = res.data.data;
                console.log("catchArticalData", res.data);
                this.articals.forEach( e => {
                    e.AuthorName = res.data.authors[e.Seq];
                });
                console.log("articals", this.articals);
                //預設搜尋
                    this.search(); 
                    // this.$refs.Narbar.days = 3;
                // this.ViewPageArticals = this.ViewArticals.slice( (this.currentPage-1)*this.perPage, this.currentPage*this.perPage);
                this.totalRows = this.articals.length;


                
            
            }
            else {
                alert("載入資料文章失敗");
            }

        },
        search( days = null, value ="", searchTags = []) {
                console.log(days, value, searchTags);
                if(days != null) this.ViewArticals = this.articals.filter(e => new Date(Common.ToDate(e.ModifyTime)) > new Date() - days*24*3600000 );
                else this.ViewArticals = this.articals;
                if(value != "") this.ViewArticals = this.articals.filter((element) => element.Title.includes(value));
                if(searchTags.length > 0 ) 
                    searchTags.forEach(findTagSeq =>{
                        this.ViewArticals = this.ViewArticals.filter(e => e.Tags.map(e => e.Seq).includes(findTagSeq))
                    })
                this.ViewArticals.sort((a, b) => a.Title.indexOf(value) - b.Title.indexOf(value));
        },
        onChangeFile() {
            this.file = this.$refs.Upload.file;
        },
        async Save() {
            let form = new FormData();
            if(!this.title || !this.text ) {
                alert("請輸入文章或標題");
                return;
            }
            form.append("Title", this.title);
            form.append("Text", this.text);
            form.append("Url", this.url );
            form.append("tags", JSON.stringify(this.selectTags));
            if (this.file) form.append("file", this.file, this.file.name);
            let res = await axios.post("Technical/Store", form, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });
            if (res.data.status == "success") {
                alert("發文成功");
                this.title = "";
                this.text = "";
                this.file = null;
                window.location.reload();

            }
            else {
                alert("發文失敗");
            }
        },
        async getAllTag() {
            let res = await axios.get("Technical/getAllTag");
            if(res.data.status == "success") {
                this.tags =res.data.data.map(element => {
                    return {
                        label : element.Text,
                        value : element.Seq
                    }
                });
            }
            else {
                alert("載入標籤失敗");
            }
        }

    },
    async mounted() {
        await this.getArticals();
        this.getAllTag();

        console.log("userSeq", localStorage.getItem('userSeq'));
    }
}

</script>

<style>



.demo {
    font-family: sans-serif;
    border: 1px solid #eee;
    border-radius: 2px;
    padding: 20px 30px;
    margin-top: 1em;
    margin-bottom: 40px;
    user-select: none;
    overflow-x: auto;
}

.card {

    width: 100%;
}

.detail {
    background: #D2F5F2;
    border-radius: 10px;
    border: 0px;
}

.comment {}

.btn {
    margin-left: 5px
}

.table-group-divider {
    border-top-color: gray;
}
</style>

