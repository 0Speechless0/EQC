<template>
<div :class="'modal fade '+ (show ?'show': '') "  style="overflow:auto;background:rgb(0 0 0 / 50%)" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true"
  :style="'display:'+(show ? 'block': 'none')" 
>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header bg-R text-white">
        <h6 class="modal-title fs-5" id="exampleModalLabel">表單新增</h6>
        
      </div>
      <div class="modal-body">
        <form>
          <div class="mb-3">
            <label for="recipient-name" class="col-form-label">表單代碼</label>
            <input type="text" class="form-control" id="recipient-name" v-model="form.FormCode">
          </div>
          <div class="mb-3">
            <label for="recipient-name" class="col-form-label">表單名稱</label>
            <input type="text" class="form-control" id="recipient-name" v-model="form.FormName">
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-color9-1 btn-xs mx-1" data-bs-dismiss="modal" @click="close">關閉</button>
        <button type="button" class="btn btn-color11-3 btn-xs  mr-1" @click="insertForm()">新增</button>
      </div>
    </div>
  </div>
</div>

</template>

<script>

export default {
    props: ["show"],
    emits :["afterInsert", "close"],
    data : () => {
        return {
            form : {

            }
        }
    },
    methods:{

        async insertForm()
        {
           if( !this.form.FormCode  )
           {
              alert("編碼不可為空");
              return;
           }
            var res = (await window.myAjax.post("SignManagement/insertForm", { m : this.form})).data;
            if(!res)
            {
              alert("此表單已經新增");
              return ;
            }
            this.$emit("afterInsert", this.form);
        },
        close(){

          this.$emit("close");
        }
    },
    mounted()
    {
        console.log("dff");
    }

}
</script>