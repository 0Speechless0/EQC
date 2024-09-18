<template>
<div>
   <div :class="['m-2 p-2', file ? '' : 'btn-color3']">
       <div v-if="!file" :class="['dropZone', dragging ? 'dropZone-over' : '']"
            @dragestart="dragging = true"
            @dragenter="dragging = true"
            @dragleave="dragging = false">
           <div class="dropZone-info" @drag="onChange">
               <span class="fa fa-cloud-upload dropZone-title"></span>
               <span class="dropZone-title">拖拉檔案至此區塊 或 點擊此處</span>
               <div class="dropZone-upload-limit-info">
                   許可附屬檔名: xlsx
               </div>
           </div>
           <input type="file" @change="onChange" />
       </div>
       <div  v-if="file" class="form-row justify-content-center">
           <div class="dropZone-uploaded">
               <div class="dropZone-uploaded-info">
                   <span class="dropZone-title">選取的檔案:{{file.name}}</span>
                   <div class="uploadedFile-info">
                       <button @click="removeFile" type="button" class="col-2 btn btn-shadow btn-color1">
                           取消選取
                       </button>
                   </div>
               </div>
           </div>
       </div>
      </div>
     <div v-if="file" class="row justify-content-center my-3">
         <div class="col-12 col-sm-4">
             <button @click="upload" type="button" class="btn btn-shadow btn-color1 btn-block">
                 確認
             </button>
         </div>
      </div>
</div>
</template>
<script>

import axios from "axios";
export default {
    emits:["afterSuccess"],
    props: ["route"],
    data : () => {
        return {
            file : null,
            files: null,
            dragging :false,
            route_ : null
        }
    },
    methods:{
      removeFile(){
          this.file = null;
      },
      onChange(e) {
          console.log("d", this.file);
          var files = e.target.files || e.dataTransfer.files;

          // 預防檔案為空檔
          if (!files.length) {
              this.dragging = false;
              return;
          }

          this.createFile(files[0]);
      },
      createFile(file) {
          // 附檔名判斷

          console.log(file);
          if (!file.name.endsWith(".xlsx")) {
              alert('請選擇 excel 檔案');
              this.dragging = false;
              return;
          }
          this.file = file;
          this.dragging = false;
          this.files = new FormData();
          this.files.append("file", this.file, this.file.name);
      },
      async upload() {
        let res = await window.myAjax.post(this.route_, this.files, {
            headers: {
              "Content-Type": "multipart/form-data"
            }
        });

        if(res.data.status == "success") {
          alert("匯入成功");
          this.$emit("afterSuccess");
        }
        else {
          alert("匯入失敗");
        }
      }
    },
    mounted() {
        let prefix =  this.route.split("/")[1];
        if( prefix == undefined ) this.route_ = this.route + "/excelUpload" ;

    }

}    

</script>
