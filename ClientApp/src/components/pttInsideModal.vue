<template>
    <el-dialog :title="viewNotification.Title" :visible.sync="_detailModalShow" width="80%" :close-on-click-modal="false">
      <span class="d-flex row mb-2 " v-if="isEdit">
        <div class="col ">
                    發送單位 : {{ viewNotification.GetUnitTypeName(viewNotification.Unit) }}
        </div>
        <div class="col">
            發送角色 : {{ viewNotification.GetRoleTypeName(viewNotification.Role) }}
        </div>

      </span>
      <span v-if="!isEdit" class="card pl-1" v-html="viewNotification.EmitContent">

      </span>

      <ckeditor v-else :editor="ClassicEditor" class="form-control col-8" v-model="viewNotification.EmitContent" :config="editorConfig"></ckeditor>
      
      <span  slot="footer" class="dialog-footer">
          <slot name="footer"></slot>
      </span>
    </el-dialog>
  </template>

<script>

    export default{
        props:["viewNotification", "detailModalShow", "isEdit", "ClassicEditor", "store"],
        emits:["onCancel"],
        data: () => {
            return {
              editorConfig : {}
            }
        },
        computed:
        {
          _detailModalShow: 
          {
            get()
            {
              return this.detailModalShow;
            },
            set(value)
            {
              if(!value)
                this.$emit("onCancel");
            }

          }
        },
        methods:{
            close(){
            },
    
        },
        mounted()
        {
          
        }

    
    }
    
</script>
<style>
  .el-dialog__title
  {
    font-size:  24px !important;
  }
</style>
<style scoped>
  td {
    vertical-align: middle !important;
    font-size: 18px !important;
  }
  th {
    vertical-align: middle !important;
    font-size: 18px !important;
  }

</style>