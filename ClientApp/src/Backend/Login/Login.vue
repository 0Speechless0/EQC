<template>
    <!-- START CONTENT -->
    <div class="box">
        <div class=" row justify-content-end mb-2 pr-0 pr-md-5">
            <div class="login mx-2 col-12 col-sm-6 col-md-3">
                <img src="/Content/images1/web-name_1.png" class="w-100">
                <div v-if="checkLevel==0">
                    <div class="form-group">
                        <label for="inputAccount" class="sr-only">帳號</label>
                        <input v-model="userNo" type="text" id="emailAddress" aria-describedby="emailHelp" placeholder="請輸入帳號" required="required" class="form-control">
                    </div>
                    <div class="form-group">
                        <label for="inputPassword" class="sr-only">密碼</label>
                        <input v-model="passWd" type="password" id="password" placeholder="請輸入密碼" required="required" class="form-control">
                    </div>
                    <div class="form-group">
                        <label for="checkCode">驗證碼</label>
                        <input v-model="checkCode" id="checkCode" type="text" placeholder="請輸入" class="form-control">
                    </div>
                    <div class="form-group">
                        <div class="s-canvas"><s-identify :identifyCode="identifyCode"></s-identify></div>
                        <button @click="refreshCode()" type="button" class="btn btn-secondary">重新產生</button>
                    </div>
                    <div class="row">
                        <div class="col-12 mb-3">
                            <button @click="onLogin" type="button" class="btn btn-color1 btn-block btn-radius"> 登入 </button>
                        </div>
                    </div>
                </div>
                <div v-if="checkLevel==1">
                    <label style="color:red">{{emailMsg}}</label>
                    <div class="form-group">
                        <label for="checkMailCode">電子郵件驗證碼</label>
                        <input v-model="checkMailCode" id="checkMailCode" type="text" placeholder="請輸入" class="form-control">
                    </div>
                    <div class="row">
                        <div class="col-12 mb-3">
                            <button @click.stop="onLogin2" type="button" class="btn btn-color1 btn-block btn-radius"> 驗證 </button>
                        </div>
                    </div>
                    <div class="form-group">
                        <a href="##" @click.stop="onLogin1()">重新發送驗證碼</a>
                    </div>
                </div>
                <div class="mt-5"><small class="t-center">經濟部水利署 Copyright © 2022 All Rights Reserved </small></div>
            </div>
        </div>
        <!-- div class="login">
            <img class="w-100" src="/Content/images/web-name.png" alt="標題AutoMedia-線上教材自製平台">
            <form>
                <div class="form-group">
                    <label for="inputAccount" class="sr-only">帳號</label>
                    <input type="text" class="form-control" id="emailAddress" aria-describedby="emailHelp" placeholder="請輸入帳號" required />
                </div>
                <div class="form-group">
                    <label for="inputPassword" class="sr-only">密碼</label>
                    <input type="password" class="form-control" id="password" placeholder="請輸入密碼" required />
                </div>
                <div class="form-group">
                    <label for="checkCode">驗證碼</label>
                    <input id="checkCode" type="text" class="form-control" placeholder="請輸入" />
                </div>
                <div class="form-group">
                    <s-identify :identifyCode="identifyCode"></s-identify>
                    <b-button >重新產生</b-button>
                </div>
                <div class="row">
                    <div class="col-12 mb-3">
                        <button type="button" class="btn btn-color1 btn-block btn-radius" >
                            登入
                        </button>
                    </div>
                </div>
            </form>
            <div class="d-flex justify-content-end">
                <a href="/Download/eqc_app.apk" download="水利工程抽驗APP.apk" class="a-blue underl">Android版app下載</a>

            </div>
            <div class="d-flex justify-content-end">
                <a href="http://102kmsystem.wratb.gov.tw/appmobile/appindex.html" class="a-blue underl">IOS版app下載</a>
            </div>

            <div class="d-flex justify-content-end">
                <a href="http://automedia.sunyu-tech.com.tw/crm/index.php/homepage" class="a-blue underl">客服系統</a>
            </div>
            <div class="mt-5">
                <small class="t-center">經濟部水利署 Copyright © 2021 All Rights Reserved </small>
            </div>
        </div -->
    </div>
    <!-- END CONTENT -->
</template>

<script>
    import axios from 'axios';
    //import SIdentify from "@/components/SIdentify.vue";
    // Import stylesheet

    //import store from '@/store/index'

    export default {
        data: function () {
            return {
                users: [],
                userNo: '',
                passWd: '',
                identifyCode: '',
                identifyCodes: "0123456789abcdwerwshdjeJKDHRJHKOOPLMKQ",//隨便打的
                checkCode: '',
                //
                checkLevel: 0,
                checkMailCode: '',
                emailMsg: '',
            };
        },
        methods: {
            onLogin() {
                if (this.identifyCode != this.checkCode) {
                    alert('驗證碼錯誤');
                    this.refreshCode();
                    return;
                }
                this.onLogin1();
            },
            onLogin1() {
                this.checkLevel = 0;
                this.emailMsg = '';
                axios.post('/Login/CheckUserLevel1', { userNo: this.userNo, passWd: this.passWd })
                    .then(async(config) => {
                        console.log(config);
                        if (config.data.result == 1) {
                            window.useStaticStore().then(() =>   window.location.replace(config.data.homeUrl) );
                            // window.location.replace(config.data.homeUrl);

                        } else if (config.data.result == 0) {
                            this.checkLevel = 1;
                            this.emailMsg = config.data.msg;
                            this.checkTimeout();
                        } else {
                            this.refreshCode();
                            alert(config.data.msg);
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            //mail驗證碼有效5分鐘
            checkTimeout() {
                setInterval(function () {
                    location.reload();
                }, 300000);
            },
            //mail驗證碼
            onLogin2() {
                axios.post('/Login/CheckUserLevel2', { userNo: this.userNo, passWd: this.passWd, mailCode:this.checkMailCode })
                    .then(config => {
                        console.log(config);
                        if (config.data.result == 0) {
                            window.location.replace(config.data.homeUrl);
                        } else if (config.data.result == -2) {
                            location.reload();
                        }
                        else if (config.data.result == -3) {
                            alert(config.data.msg);
                        }
                        else
                            alert(config.data.msg);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            refreshCode() {
                this.identifyCode = '';
                this.makeCode(this.identifyCodes, 4);
            },
            randomNum(min, max) {
                max = max + 1;
                return Math.floor(Math.random() * (max - min) + min);
            },
            // 隨機生成驗證碼字符串
            makeCode(data, len) {
                for (let i = 0; i < len; i++) {
                    this.identifyCode += data[this.randomNum(0, data.length - 1)]
                }
            },
            download(type) {
                //window.open('/ChapterChart/ChapterDownload' + '?seq=' + item.Seq);
                window.myAjax.get('/Login/Download?type=' + type, { responseType: 'blob' })
                    .then(resp => {
                        const blob = new Blob([resp.data]);
                        const contentType = resp.headers['content-type'];
                        if (contentType.indexOf('application/json') >= 0) {
                            //alert(resp.data);
                            const reader = new FileReader();
                            reader.addEventListener('loadend', (e) => {
                                const text = e.srcElement.result;
                                const data = JSON.parse(text)
                                alert(data.message);
                            });
                            reader.readAsText(blob);
                        } else if (contentType.indexOf('application/blob') >= 0) {
                            var saveFilename = null;
                            const data = decodeURI(resp.headers['content-disposition']);
                            var array = data.split("filename*=UTF-8''");
                            if (array.length == 2) {
                                saveFilename = array[1];
                            } else {
                                array = data.split("filename=");
                                saveFilename = array[1];
                            }
                            if (saveFilename != null) {
                                const url = window.URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = url;
                                link.setAttribute('download', saveFilename);
                                document.body.appendChild(link);
                                link.click();
                            } else {
                                console.log('saveFilename is null');
                            }
                        } else {
                            alert('格式錯誤下載失敗');
                        }
                    }).catch(error => {
                        console.log(error);
                    });
            },
        },
        mounted() {
            this.refreshCode();
            //DO TO 測試用 開發完成要拿掉
            //this.userNo = 'guest';
            //this.passWd = '1234';
            //this.checkCode = this.identifyCode;
            //this.onLogin();
        }
    }
</script>

