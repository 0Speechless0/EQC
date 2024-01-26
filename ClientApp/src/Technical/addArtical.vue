<template>

    <div class="container">
        <div class="mb-3">
            <label for="exampleFormControlInput1" class="form-label" >標題 </label>
            <input type="email" class="form-control" id="exampleFormControlInput1" v-model="title">
        </div>
        <div class="mb-3 text-center d-flex justify-content-evenly">
            <div class="file btn btn-lg btn-primary upload-div" v-if="!this.file && upload">
							上傳檔案
							<input type="file" class="upload" name="file" @change="onChangeFile" />
			</div>
            <div v-else>
                <span>
                    {{this.file.name}}
                </span>
                <span >
                                
                                <button type="button" class="btn btn-secondary" @click="cancelUpload()" >取消</button>
                </span>
            </div>
            <button type="button" class="btn btn-info" @click="upload= !upload" v-if="!this.file">連結</button>
        </div>
        <div class="mb-3" v-if="!upload">
            <label for="exampleFormControlInput1" class="form-label" >連結 </label>
            <input type="email" class="form-control" id="exampleFormControlInput1" v-model="href">
        </div>
        <div class="mb-3" >
            <label for="exampleFormControlTextarea1" class="form-label ">說明</label>
            <textarea class="form-control" id="exampleFormControlTextarea1" rows="12" v-model="text"></textarea>
        </div>
        <div class="mb-3 text-center">
            <button type="button" class="btn btn-success" data-bs-dismiss="modal" @click="Save()"> 送出</button>
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" > 不送</button>
            <div>
            </div>
        </div>

    </div>


</template>


<script>
import axios from "axios";

export default {
    emits: ["afterPost"],
    data: () => {
        return {
            title: "",
            text: "",
            file: null,
            upload: true,
    
        };
    },
    methods: {
        cancelUpload(){
            this.file = null;
            console.log("sdf");
        },
        onChangeFile(e) {
            this.file = e.target.files[e.target.files.length - 1];
            if(this.file) this.upload = true;
        },
        async Save() {
            let form = new FormData();
            form.append("Title", this.title);
            form.append("Text", this.text);
            form.append("file", this.file, this.file.name);
            let res = await axios.post("Technical/Store", form, { headers: {
                    "Content-Type": "multipart/form-data"
                } });
            if (res.data.status == "success") {
                alert("發文成功");
                this.title = "";
                this.text = "";
                this.file = null;
                this.$emit("afterPost");
            }
            else {
                alert("發文失敗");
            }
        }
    },
    components: {}
}
</script>

