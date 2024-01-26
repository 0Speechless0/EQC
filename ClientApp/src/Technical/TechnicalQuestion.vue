<template>
    <div class="row">
        <div class="col">
            
            <div class="card border-light mb-3" style="width:100%;">
                <div class=" card-body">

                    <div class="d-flex justify-content-between bd-highlight mb-3">
                        <h4 class="card-title ">問題</h4>
                        <div class="justify-content-end" v-show="canEditComment">
                            
                            <span>
                                <button class="btn btn-secondary " type="button" style="background-color: #904623;"
                                    @click="EditComment()" v-show="!editComment" title="編輯">
                                    編輯
                                </button>
                                <button class="btn btn-secondary" type="button" @click="EditComment()" v-show="editComment"
                                    style="background: #9E4A23">
                                    確認編輯
                                </button>
                                
                            </span>
                            <span >
                                <button class="btn btn-danger" @click="deleteComment()" title="刪除">刪除 </button>
                            </span>

                                








                        </div>

                    </div>
                    <div class="row">
                        <div class="col">
                            <p class="card-text" style="line-height:2;font-size:15px"> 來自{{ info.CommentAuthor }}</p>
                            <pre class="card-text" v-if="!editComment">{{ info.CommentText }}</pre>
                            <div class="row text-start" v-else>
                                <textarea class="form-control" placeholder="編輯文章" 
                                    style="height: 100px;" v-model="editComment"> </textarea>
                            </div>
                        </div>
                        <div class="col-md-auto  ">
                            <a :href="'FileUploads/Technical' + info.file.CommentPath" download v-if="info.file.CommentPath != '' && info.file.CommentFileType == 'image'" >
                            <img :src="'FileUploads/Technical' + info.file.CommentPath" class="card-img"
                                 />
                            </a>
                            <a :href="'FileUploads/Technical' + info.file.CommentPath" v-else-if="info.file.CommentPath != '' && info.file.CommentFileType == 'video'" download>
                            <video width="400" controls>
                                <source :src="'FileUploads/Technical' + info.file.CommentPath" type="video/mp4" />
                                <source :src="'FileUploads/Technical' + info.file.CommentPath" type="video/ogg" />
                                <source :src="'FileUploads/Technical' + info.file.CommentPath" type="video/webm" />
                                Your browser does not support HTML video.
                            </video>
                            </a>

                        </div>
                    </div>
                    <div class="card-text" style="padding:10px" v-if="editComment">
                        <Upload text="上傳圖片、影音" ref="Upload" @fileChange="onChangeFile" class="text-left"/>

                    </div>
                        <div class="align-self-end"  style="padding-top:10px">
                                                    
                            上次更新: {{ getDate(info.CommentModifyTime, true) }}

                        </div>
                    <hr />

                    <div class="d-flex justify-content-between bd-highlight mb-3 ">

                        <h4 class="card-title" style="padding-bottom: 10px;">
                            回覆
                        </h4>

                        <div class="justify-content-end" v-if="canEditReply">
                            <span v-if="showReply">
                                <button class="btn btn-secondary " type="button" style="background-color: #904623;"
                                    @click="EditReply()" v-show="!editReply" title="編輯">
                                    編輯
                                </button>
                                <button class="btn btn-secondary" type="button" @click="EditReply()" v-show="editReply"
                                    style="background: #9E4A23">
                                    確認編輯
                                </button>
                                
                            </span>
                            <span v-if="showReply">
                                <button class="btn btn-danger" @click="deleteReply()" title="刪除">刪除 </button>
                            </span>
                            <div>

                            </div>


                        </div>





                    </div>
                    <p v-if="info.ReplyText" class="card-text" style="line-height:2;font-size:15px"> 來自
                        {{ info.ReplyAuthor }}</p>
                    <div v-if="info.ReplyText && showReply">
                        <div class="row">
                            <div class="col">
                                <pre class="card-text" v-if="!editReply">{{ info.ReplyText }}
                                </pre>
                                <div class="row text-start" v-else>
                                    <textarea class="form-control" placeholder="編輯文章"
                                        style="height: 100px;" v-model="editReply"></textarea>
                                </div>
                            </div>
                            <div class="col-md-auto  ">
                                <a :href="'FileUploads/Technical' + info.file.ReplyPath" class="card-img" v-if="info.file.ReplyPath != '' && info.file.ReplyFileType == 'image'" download>
                                    <img :src="'FileUploads/Technical' + info.file.ReplyPath" class="card-img"
                                        />
                                </a>
                                <a :href="'FileUploads/Technical' + info.file.ReplyPath"  v-else-if="info.file.ReplyPath != '' && info.file.ReplyFileType == 'video'" download>
                                    <video width="400" controls>
                                        <source :src="'FileUploads/Technical' + info.file.ReplyPath" type="video/mp4" />
                                        <source :src="'FileUploads/Technical' + info.file.ReplyPath" type="video/ogg" />
                                        <source :src="'FileUploads/Technical' + info.file.ReplyPath" type="video/webm" />
                                        Your browser does not support HTML video.
                                    </video>
                                </a>
                            </div>
                        </div>


                    </div>
                    <div v-else-if="isAdmin && !info.ReplyText" class="card-text">
                        <FormWithFileTemplate @Save="Save(info.Seq)"  ref="FormWithFileTemplate" @show=" value => showReply = value">
                        </FormWithFileTemplate>
                    </div>
                        <div class="d-flex  justify-content-between card-text" >
                                                    <span class=" align-self-end" v-if="info.ReplyText">
                            上次更新: {{ getDate(info.CommentModifyTime, true) }}
                        </span>
                        <span>
                                <span v-if="info.ReplyText">

                                    <button type="button" class="btn btn-success" v-show="showReply && !isUserThumb"
                                        @click="giveThumb(info.Seq)"> {{ thumbCount }}讚</button>
                                    <button type="button" class="btn btn-success" v-show="showReply && isUserThumb"
                                        @click="recoveryThumb(info.Seq)" style="background-color: #0B8539;
    "> {{ thumbCount }}收讚</button>
                                </span>
                                <span v-if="info.ReplyText || isAdmin">
                                    <button class="btn btn-info" type="button" @click="checkReply()"
                                        v-show="!showReply && info.ReplyText" style="background-color: #41ADD3">
                                        查看回覆
                                    </button>
                                    <button class="btn btn-secondary" type="button" @click="checkReply()"
                                        v-show="showReply && info.ReplyText">
                                        收
                                    </button>
                                </span>
                        </span>

                        </div>
                </div>

            </div>

        </div>
    </div>


</template>

<script>
import axios from 'axios';
 import FormWithFileTemplate from './FormWithFileTemplate.vue';
 import Common from "../Common/Common.js";
 import Upload from "./UploadComponent.vue";
export default {
    props: ["info", "propIsAdmin", "canEdit"],
    emits: ["afterSave"],
    data: () => {
        return {
            isAdmin:    false,
            showReply: false,
            thumbCount: 0,
            isUserThumb: false,
            editReply: null,
            editComment: null,
            canEditComment :false,
            canEditReply : false,
            file : null
        };
    },
    methods: {
        getDate(time, hasTime = false) {

            return Common.ToROCDate(time, hasTime);
        },
        onchangeFile() {
            this.file = this.$refs.Upload.file;
        },
        async deleteComment() {
            if( !confirm("確定要刪除?") ) return ;
            let res = await axios.get("Technical/DeleteComment/" + this.info.Seq);
            if (res.data.status == "success") {
                this.$emit("afterSave");
            }
            else {
                alert("刪除失敗");
            }
        },
        async deleteReply() {
            if( !confirm("確定要刪除?") ) return ;
            let res = await axios.get("Technical/DeleteReply/" + this.info.Seq);
            if (res.data.status == "success") {
                this.$emit("afterSave");
            }
            else {
                alert("刪除失敗");
            }
        },
        async EditReply() {

            if (!this.editReply)
                this.editReply = this.info.ReplyText;
            else {

                let res = await axios.post("Technical/UpdateReply", 
                {   
                    id : this.info.Seq,
                    value: { Text: this.editReply } 
                });
                if (res.data.status == "success") {
                    alert("更新成功");
                }
                else {
                    alert("更新失敗");
                }



                this.$emit("afterSave");
                this.editReply = null;
            }
        },
        async EditComment() {

            if (!this.editComment)
                this.editComment = this.info.CommentText;
            else {

                let res = await   axios.post("Technical/UpdateComment", 
                {
                    id : this.info.Seq, 
                    value: { Text: this.editComment } 
                });
                if (res.data.status == "success") {
                    alert("更新成功");
                }
                else {
                    alert("更新失敗");
                }
                this.$emit("afterSave");
                this.editComment = null;
            }
            


        },
        checkReply() {
            this.showReply = !this.showReply;
        },
        async Save() {
            let form = new FormData();
            form.append("TechnicalCommentSeq", this.info.Seq);
            form.append("Text", this.$refs.FormWithFileTemplate.text);
            if (this.$refs.FormWithFileTemplate.file)
                form.append("file", this.$refs.FormWithFileTemplate.file, this.$refs.FormWithFileTemplate.file.name);
            let res = await axios.post("Technical/StoreReply", form);

            if (res.data.status == "success") {
                alert("回覆成功");
                this.$emit("afterSave");
            }
            else {
                alert("回覆失敗");
            }
        },
        async getReplyThumb(Seq) {
            let res = await axios.get("Technical/getReplyThumb/" + Seq);
            if (res.data.status == "success") {
                this.thumbCount = res.data.thumbCount;
                this.isUserThumb = res.data.isUserThumb;
            }
        },
        async giveThumb(Seq) {
            if (this.isUserThumb) return;
            let res = await axios.get("Technical/giveThumb/" + Seq);
            if (res.data.status == "success") {
                this.getReplyThumb(Seq);
            }
        },
        async recoveryThumb(Seq) {
            if (!this.isUserThumb) return;
            let res = await axios.get("Technical/recoveryThumb/" + Seq);
            if (res.data.status == "success") {
                this.getReplyThumb(Seq);
            }
        }
    },
    components: { FormWithFileTemplate, Upload },
    mounted() {
        this.isAdmin = this.propIsAdmin;
        this.getReplyThumb(this.info.Seq);

        console.log("mounted info", this.info);

    },
    updated() {
        this.canEditComment = localStorage.getItem("userSeq") == this.info.CommentAuthorSeq;
        this.canEditReply = localStorage.getItem("userSeq") == this.info.ReplyAuthorSeq;
    }
}
</script>

<style>
.card-title {
    font-weight: bold;
}
</style>