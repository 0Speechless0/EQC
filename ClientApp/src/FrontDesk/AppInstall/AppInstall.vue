<template>
    <div>
        <form @submit.prevent>
            <div class="row justify-content-between" >
                <div class="form-inline col-12 col-md-12 my-2 ">
                    <label id="InspectionDate" class="my-1 mr-2">手機類型：</label>
                    <select class="form-control mb-2 mr-sm-4" v-model="phonetype">
                        <option value="0">Android</option>
                        <option value="1">IOS</option>
                    </select>
                    <label id="InspectionDate" class="my-1 mr-2">手機型號：</label>
                    <input v-model="phonemodel" type="text" class="form-control mb-2 mr-sm-2">
                    <button v-on:click="onclick()" class="btn btn-color3 mb-2 mr-sm-2" type="button">確定</button>
                    <a v-bind:href="downloadlink" download="水利工程抽驗APP.apk" class="my-1 mr-2" v-if="android">Android版app下載</a>
                    <label id="InspectionDate" class="my-1 mr-2" v-if="ios">IOS下載連結:{{downloadlink}}</label>

                </div>

            </div>
        </form>
        <div class="d-flex justify-content-center m-5">
            <div id="qrcode">   </div>
        </div>

    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                phonetype: null,
                downloadlink: null,
                ios: false,
                android: false,
                downloadcode: null,
                phonemodel:null
            };
        },
        methods: {
            onclick() {
                if (this.phonetype == null) {
                    alert("請選擇手機類型");
                    return;
                }
                window.myAjax.post('/AppInstall/Save', { PhoneType: this.phonetype, Phonemodel: this.phonemodel })
                    .then(resp => {
                        console.log(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });
                if (this.phonetype == 0) {
                    this.android = true;
                    this.ios = false;
                    this.downloadlink = "/Download/eqc_app.apk";
                    window.QRCodeImageGenerate("qrcode", window.location.origin + this.downloadlink);
                }
                else {
                    this.ios = true;
                    this.android = false;
                    window.myAjax.post('/AppInstall/GetLink')
                        .then(resp => {
                            if (resp.data.result == 0) {
                                this.downloadcode = resp.data.Link;
                                this.downloadlink = "https://apps.apple.com/redeem?code=" + resp.data.Link + "&ctx=apps";
                                window.QRCodeImageGenerate("qrcode", this.downloadlink);
                                this.ondownload();
                            } else {
                                this.ios = false;
                                alert(resp.data.message);
                            }
                        })
                        .catch(err => {
                            console.log(err);
                        });
                }
            },
            ondownload() {
                window.myAjax.post('/AppInstall/UpdateLink', { Downloadcode: this.downloadcode })
                    .then(resp => {
                        console.log(resp.data.message);
                    })
                    .catch(err => {
                        console.log(err);
                    });

            }
        },
        async mounted() {
            console.log('mounted() App下載');
        }
    }
</script>
