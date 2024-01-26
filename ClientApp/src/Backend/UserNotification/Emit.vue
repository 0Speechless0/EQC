<script setup>

import { onMounted } from "vue";
import { useUserNotificationStore } from "../store/UserNotificationStore.js";
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';

const editorConfig = {

    }
const store = useUserNotificationStore();

</script>

<template>
    <div class="card-body">


            <div class="row">

                <label for="role" class=" col-6 col-sm-2 col-form-label">發送角色對象</label>
                <div class="col-6 col-sm-10">

                    <select class="form-control col-6 col-sm-6" id="role" v-model="store.emitContent.Role" name="roleSeq" form="form">
                        <option :value="null">所有人</option>
                        <option v-for="(role, index) in store.roleList.value" :value="role.value" :key="index">{{ role.Text
                        }}
                        </option>
                    </select>
                </div>
                <label for="unit" class="col-6 col-sm-2 col-form-label">發送單位對象</label>
                <div class="col-6 col-sm-10">

                    <select class="form-control col-6 col-sm-6" id="role" v-model="store.emitUnitType.value" name="unitSeq" form="form">
                        <option :value="null">所有單位</option>
                        <option :value="1">特定單位</option>
                    </select>
                </div>

            </div>
            <div class="row " v-if="store.emitUnitType.value == 1">
                <label for="specify_unit" class="col-6 col-sm-2 col-form-label">選擇單位</label>
                <div class="col-6 col-sm-10">

                    <select class="form-control col-4" v-model="store.emitContent.Unit" id="specify_unit">
                        <option v-for="(unit, index) in store.unitList.value" :value="unit.value" :key="index">{{ unit.Text
                        }}
                        </option>
                    </select>
                </div>

            </div>
            <br />
            <div class="row">


                <div class="form-group col-6 col-sm-10">
                    <label for="Tilte">系統公告內容</label>
                    <input type="text" class="form-control col-6 " id="Tilte" name="title"
                        v-model="store.emitContent.Title" />
                </div>
                <div class="form-group col-6 col-sm-10">
                    <label for="Tilte">到期日</label>
                    <input type="date" class="form-control col-6 " id="Tilte" name="title"
                        v-model="store.emitContent.ExpireTime" />
                </div>


            </div>
            <div class="row mt-2" style="margin-left: -5px;">
                <div class="form-group col-6 col-sm-10">
                    <label for="Html">內容</label>
                    <ckeditor :editor="ClassicEditor" class="form-control col-8" v-model="store.emitContent.EmitContent" :config="editorConfig"></ckeditor>
                </div>

            </div>

            <div class="row " style="margin-left: 1px;">
                <button type="submit" class="btn btn-primary" @click="store.submit()" >發送</button>
            </div>



</div></template>