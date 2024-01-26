<template>

    <div>
        <div v-show="!add" class="mb-3 pt-3">
            <button class="btn btn-info " type="button" style="    background-color: #219086;"
                @click="add = !add; $emit('show', add)" title="=新增" >
                新增
            </button>
        </div>
        <div v-show="add" class="mb-3 pt-3">
            <button class="btn btn-secondary " type="button" 
                 title="收"
                 @click="add = !add">
                收
            </button>
        </div>
        <div class="card text-center" style="margin-top:10px ;background-color:#E8E3E2"  v-show="add" >

            <div class="card-text" style="padding:10px">
                <Upload text="上傳圖片、影音" ref="Upload" @fileChange="onChangeFile" class="text-left"/>

            </div>
            <div class="mb-3 " style="margin:10px">



                <textarea class="form-control" placeholder="發問" id="floatingTextarea2"
                    style="height: 100px;" v-model="text"></textarea>
                <div class="text-center">
                    <button type="button" class="btn btn-success" style="margin:10px" @click="Save()">回覆</button>
                </div>
            </div>
        </div>
    </div>


</template>

<script>

import Upload from "./UploadComponent.vue";
export default{
    emits: ["Save", "show"],
    data:() => {
        return {
            file:null,
            text:null ,
            add: false
        }
    },
    methods:{
        onChangeFile() {
            console.log(this.$refs.Upload.file);
            this.file = this.$refs.Upload.file;
        }, 
        Save(){
            if(!this.text){
                alert("請輸入內容");
                return ;
            }
            this.$emit("Save");

            this.text = null;
            this.$refs.Upload.file = null;
        }
    },
    components:{
        Upload
    }

}


</script>