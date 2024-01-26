<template>

    <div class="card text-dark bg-light mb-3">
        <div class="card-body">
            <h2 class="card-title " v-if="!articalEdit">{{ artical.Title }}
                <el-tag type="info" class="ml-2" v-for="(tag, index) in artical.Tags" :key="index">{{ tag.Text }}</el-tag>
            </h2>
            <div class="mb-3" v-else>
                <input type="text" class="form-control" placeholder="標題" style="width:35%" v-model="titleEdit" />

            </div>

            <div class="d-flex bd-highlight mb-3 flex-wrap">
                <div class="mr-auto align-self-end card-text" style="padding-left:20px">
                    發文者：{{ artical.AuthorName }}
                </div>

                <!-- <a  v-if="artical.Url || fileHref" :href="'https://www.facebook.com/sharer/sharer.php?u='+shareUrl" target="_blank">
                        <button class="btn btn-primary">
                            分享到FB
                        </button>
                    </a>  
                    <a  v-if="artical.Url || fileHref" :href="'https://social-plugins.line.me/lineit/share?url='+shareUrl" target="_blank">
                        <button class="btn btn-success">
                            分享到LINE
                        </button>
                    </a>  -->
                <div class="pr-2 align-self-end">
                    點及觀看次數:{{ artical.Click }}
                </div>
                <div class="pr-2 align-self-end">
                    已被回覆數:{{ artical.ArticalReplyCount }}
                </div>
                <div v-if="canEdit">
                    <button class="btn btn-secondary " type="button" style="background-color: #904623;"
                        @click="EditArtical()" v-show="!articalEdit && showDetail" title="編輯">
                        編輯
                    </button>
                    <button class="btn btn-secondary" type="button" @click="EditArtical()" v-show="articalEdit &&showDetail"
                        style="background: #9E4A23">
                        確認編輯
                    </button>
                    <button class="btn btn-danger" @click="deleteArtical()" v-show="showDetail" title="刪除">
                        刪除
                    </button>
                </div>

                <button class="btn btn-info" type="button" @click.stop="clickDetail()">
                    內文
                </button>
                <div class="align-self-end" style="margin-left:10px">
                    上次更新：{{ getDate(artical.ModifyTime, true) }}
                </div>

            </div>
            <div class="card-body" v-show="showDetail">
                <div>
                    <div class="row text-start" v-if="!articalEdit">

                        <div class="card-body detail " aria-labelledby="headingOne" v-if="artical.Url">
                            <pre><a :href="artical.Url" target="_blank" style="color:black">{{ artical.Text }} ...點擊查看附件</a>
                            </pre>

                        </div>
                        <div class="card-body detail " aria-labelledby="headingOne" v-else-if="fileHref">
                            <a :href="'FileUploads/Technical' + fileHref" target="_blank" style="color:black"> {{
                                    artical.Text
                            }}...點擊查看附件 </a>
                        </div>
                        <pre class="card-body detail " aria-labelledby="headingOne" v-else>{{ artical.Text }}</pre>
                    </div>
                    <div class="row text-start" v-else>
                        <el-select style="width:25%" v-model="selectTags
                        " multiple filterable default-first-option
                            placeholder="設定標籤...">
                            <el-option v-for="item in tags" :key="item.value" :label="item.label"
                                :value="item.value">
                            </el-option>
                        </el-select>
                        <textarea class="form-control" placeholder="編輯文章" id="floatingTextarea2" style="height: 100px;"
                            v-model="articalEdit"></textarea>
                    </div>
                    <FormWithFileTemplate @Save="Save(artical.Seq)" ref="FormWithFileTemplate" />

                    <h3 style="padding:20px" class="text-center"> 歷史問題 </h3>
                    <TechnicalQuestion v-for="(item, index) in list" :info="item" :key="index" :propIsAdmin="isAdmin"
                        @afterSave="getArticalData(artical.Seq)" />
                </div>
            </div>

        </div>

        <!-- <div class="modal fade" id="question" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="modal-body">
                        <Question />
                    </div>
                </div>
            </div>
        </div> -->
    </div>

</template>

<script>
import axios from "axios";
import Common from "../Common/Common.js";
import TechnicalQuestion from "./TechnicalQuestion.vue";
import FormWithFileTemplate from "./FormWithFileTemplate.vue";
export default {
    props: ["artical", "tags"],
    emits: ["afterEditArtical"],
    components: {
        TechnicalQuestion,
        FormWithFileTemplate
    },
    data: () => {
        return {
            list: [],
            fileHref: null,
            showDetail: false,
            isAdmin: false,
            editArtical: false,
            articalEdit: null,
            titleEdit: null,
            canEdit: false,
            selectTags: []
        }
    },
    watch: {
        selectTags:{
            handler(value) {
                console.log("selectTags", value);
            }
        }
    },
    computed: {
        shareUrl() {
            if (this.artical.Url) {
                return this.artical.Url;
            }
            else if (this.fileHref) {
                return window.location.origin + "/FileUploads/Technical" + this.fileHref.replaceAll('\\', '/');
            }
            return "";
        }
    },
    methods: {
        async deleteArtical() {
            if( !confirm("確定要刪除?") ) return ;
            let res = await axios.get("Technical/DeleteArtical/"+this.artical.Seq);

            if (res.data.status == "success") {
                this.$emit("afterEditArtical");
            }
            else {
                alert("刪除失敗");
            }
        },
        async EditArtical() {
            if (!this.articalEdit && !this.titleEdit) {
                this.articalEdit = this.artical.Text;
                this.titleEdit = this.artical.Title;
            }
            else {
                console.log("AAA")
                let res = await axios.post("Technical/UpdateArtical", { 
                    id :  this.artical.Seq ,
                    value: { 
                        Text: this.articalEdit, 
                        Title: this.titleEdit 
                    }, 
                    tags:this.selectTags 
                }, {
                    "Content-Type": "application/json"
                });
                if (res.data.status == "success") {
                    alert("更新成功");
                    this.$emit("afterEditArtical");
                }
                else {
                    alert("更新失敗");
                }
                this.articalEdit = null;
                this.titleEdit = null;
            }
        },
        getDate(time, hasTime = false) {

            return Common.ToROCDate(time, hasTime);
        },
        async getArticalData(seq) {
            let res = await axios.get("/Technical/getArticalData/" + seq);
            if (res.data.status == "success") {
                res.data.data.forEach(e => {
                    e.file = {
                        CommentPath: null,
                        ReplyPath: null,
                        CommentFileType: null,
                        ReplyFileType: null

                    };
                })
                this.list = res.data.data;
                this.isAdmin = res.data.isAdmin;
            }
            res = await axios.get("/Technical/getAllArticalFileName/" + seq);
            if (res.data.status == "success") {
                console.log("articalFile", res.data.data);
                this.fileHref = res.data.data.length > 0 ? res.data.data[0] : "";
            }
            else {
                alert("載入文章檔案失敗:" + seq);
            }
            this.getArticalDataFiles(seq);
        },
        async Save(articalSeq) {
            let form = new FormData();
            form.append("Text", this.$refs.FormWithFileTemplate.text);
            form.append("TechnicalArticalSeq", articalSeq);
            if (this.$refs.FormWithFileTemplate.file)
                form.append("file", this.$refs.FormWithFileTemplate.file, this.$refs.FormWithFileTemplate.file.name);
            let res = await axios.post("/Technical/StoreComment/", form);
            if (res.data.status == "success") {
                this.list = res.data.data;
                alert("發問成功");
                this.getArticalData(articalSeq);
            }
            else {
                alert("發問失敗");
            }
        },
        clickDetail() {
            this.showDetail = !this.showDetail;
            if (this.showDetail) this.getArticalData(this.artical.Seq);
        },
        async getArticalDataFiles(seq) {
            let res = await axios.get("/Technical/getArticalDataFiles/" + seq);
            if (res.data.status == "success") {
                console.log("catchfiles", res.data);
                res.data.data.forEach((e, index) => {
                    this.list[index].file.CommentPath = e.CommentPaths.length > 0 ? e.CommentPaths[0].filePath : "";
                    this.list[index].file.CommentFileType = e.CommentPaths.length > 0 ? e.CommentPaths[0].fileType : "a";
                    this.list[index].file.ReplyPath = e.ReplyPaths.length > 0 ? e.ReplyPaths[0].filePath : "";
                    this.list[index].file.ReplyFileType = e.ReplyPaths.length > 0 ? e.ReplyPaths[0].fileType : "";
                });
                console.log("files", this.list);
            }
            else {
                alert("載入檔案失敗");
            }
        },


    },
    mounted() {
        this.canEdit = this.artical.Author == localStorage.getItem("userSeq");
        this.selectTags =  this.artical.Tags.map(e =>  e.Seq );
    }

}

</script>

<style>
.problem-table {
    border-color: black;
}

.problem-card {
    border-bottom: 0px;
}

.btn-success {
    background-color: #79B600;
}
</style>
