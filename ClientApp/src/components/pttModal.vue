
<script >
import Common from "../Common/Common2";
const base64 = require('js-base64');  
export default {
  data: () => {

    return {
      notifications: [
      {
        Title :"AD",
        EmitContent : "SDfsdf" 

      },
      {
        Title :"AD",
        EmitContent : "SDfsdf" 

      },
      {
        Title :"AD",
        EmitContent : "SDfsdf" 

      },

      ],
      viewNotification : {
        Title :"AD",
        EmitContent : "SDfsdf" 

      },
      mainModalShow : true,
      detailModalShow :false
    }
  },
  methods: {
    showContent(notification, index)
    {
      this.viewNotification.Index = index;
      this.viewNotification = notification;
      this.mainModalShow = false;
      this.detailModalShow = true;
    },
    showMainModal() {
      this.mainModalShow = true;
      this.detailModalShow = false;

    },
    async knowNotification() {
      let {data} = await window.myAjax.post("UserNotification/Know");
      if(data == true)
      {
        this.mainModalShow = false;
        
      }
    }
  },
  mounted()
  {
    this.notifications = JSON.parse(  
      base64.decode((Common.getCookie('SerialNotifications'))

      )      
    )
      
    ;
    console.log("this.notifications", this.notifications);

  }
}


</script>

<template>
  <div>


    <el-dialog title="系統公告" :visible.sync="mainModalShow" width="80%">
      <div class="card mb-3" v-for="(notification, index) in notifications" v-bind:key="index">
        <div class="card-header">
          {{ notification.Title }}
        </div>
        <div class="card-body" v-html="notification.EmitContent" >

        </div>
      </div>


      <!-- <table class="table table1" border="0">
        <thead>
          <tr>
            <th class="col-7">公告訊息內容</th>
            <th class="col-3">公告日期</th>

            <th scope="col-2"> 查看</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(notification, index) in notifications" v-bind:key="index">
            <td>{{ notification.Title }}</td>
            <td class="text-center">{{ notification.CreateTime.split("T")[0] }}</td>

            <td class="text-center">
              <el-button icon="el-icon-search" type="primary" @click="showContent(notification)" circle></el-button>
            </td>
          </tr>
        </tbody>
      </table> -->
      <span slot="footer" class="dialog-footer">
        <el-button type="primary" @click="knowNotification()">知道了</el-button>
      </span>
    </el-dialog>
    <!-- <el-dialog :title="viewNotification.Title" :visible.sync="detailModalShow" width="80%">
      <span class="card pl-1" v-html="viewNotification.EmitContent">

      </span>
      <span slot="footer" class="dialog-footer">
        <el-button @click="showMainModal()">返回</el-button>
      </span>
    </el-dialog> -->
  </div>
</template>
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
  .card-header {
    font-size: 24px;
    color: black !important;
    background-color: lightblue !important;
  }
  .card-body {
    border: 2px solid !important;
    border-color: lightblue !important;
    border-top: 0px;

  }
</style>