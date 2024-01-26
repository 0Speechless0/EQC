<template>
    <div>
        <span v-if="!this.file" class="file btn btn-lg  upload-div" style="background-color:gray;color:white">
            <span class="upload-text">{{ text }}</span>
                                <input type="file" class="upload form-control-file" name="file" @change="onChangeFile" />
        </span>

        <span v-else>
            <span style="padding:10px">
                {{ this.file.name }}
            </span>
            <span>

                <button type="button" class="btn btn-secondary" @click="cancelUpload()">取消</button>
            </span>
        </span>
    </div>



</template>

<script>


export default {
    props: ["text"],
    emits: ["fileChange"],
    data: () => {
        return {
            file: null
        }
    },
    methods: {

        onChangeFile(e) {
            this.file = e.target.files[e.target.files.length - 1];
            if (this.file) this.upload = true;
            this.$emit("fileChange");
            this.fule = null;
        },
        cancelUpload() {
            this.file = null;
            this.$emit("fileChange");
        },
    }
}


</script>

<style>

.upload {
    position: absolute;
    font-size: 15px;
    opacity: 0;
    left: 0;
    top:0;
    height:100%;
}
.upload-div {
    position: relative;
}


</style>