<script setup>
import Modal from "./Modal.vue";
import { defineProps} from "vue";
const {store, viewImage} = defineProps(["store","viewImage"])


</script>

<template>
    <Modal title="下載">
        <template #body>

            <table class="table table1" border="0">
                <thead>
                    <tr>

                        <th scope="col-6" v-if="viewImage"></th>
                        <th scope="col-6" v-else>檔名</th>
                        <th scope="col-3">功能</th>
                        <th scope="col-3"> 刪除</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- 無圖結構 : string ，有圖結構 {fileName : string, fileLink : string } -->
                    <tr v-for="(downloaditem, index) in store.downloadFiliesUrl" v-bind:key="downloaditem">
                        <td v-if="viewImage">
                            <img :src="downloaditem.fileLink" with="600" heigh="400" />
                        </td>
                        <td v-else>{{ downloaditem.fileLink }}</td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="store.downloadURL(downloaditem.fileName ?? downloaditem)" href="#"
                                    class="btn-block mx-2 btn btn-color2" title="下載">下載</a>
                            </div>
                        </td>
                        <td>
                            <div class="row justify-content-center m-0">
                                <a v-on:click.stop="deleteDownloadFile(downloaditem.fileName ?? downloaditem)" href="#"
                                    class="btn-block mx-2 btn btn-color9-1 " title="刪除">刪除</a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="row justify-content-center m-0">
                <a v-on:click.stop="downloadAll()" href="#" class="btn-block mx-2 btn btn-color2" title="全部下載">全部下載</a>
            </div>
        </template>

    </Modal>
</template>