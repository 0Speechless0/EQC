<template>
    <div>
        <div class="name">歡迎！ &nbsp;許小明 經理</div>
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                工程拍照或AR量測
            </li>
            <li class="breadcrumb-item active" aria-current="page" title="抽驗記錄填報">
                抽驗記錄填報
            </li>
        </ol>
        <h1>抽驗記錄填報</h1>
        <h2>請填寫抽驗內容</h2>
        <form>
            <div class="form-group">
                <label for="projectName"><span class="small-red">*</span>工程名稱</label>
                <select id="projectName" class="form-control" @change="getEngData(engMainkey)" v-model="engMainkey">
                    <option v-bind:key="index" v-for="(item,index) in engMains" v-bind:value="item.Seq">{{item.EngName}}</option>
                </select>
            </div>
            <div class="form-row">
                <div class="form-group col-sm-8 col-md-8">
                    <label for="Sub-project"><span class="small-red">*</span>分項工程</label>
                    <select id="Sub-project" class="form-control">
                        <option v-bind:key="index" v-for="(item,index) in engConstructions" v-bind:value="item.Seq">{{item.ItemName}}</option>
                    </select>

                </div>
                <div class="form-group col-sm-4 col-md-4">
                    <label for="keywordSearch">關鍵字搜尋</label>
                    <div class="input-group mb-3">
                        <input type="text" class="form-control" placeholder="請輸入關鍵字" aria-label="關鍵字搜尋">
                        <button class="btn btn-color3" type="button"><i class="fas fa-search"></i></button>
                    </div>
                </div>
            </div>
            <hr>
            <div class="form-row">
                <div class="form-group col">
                    <label for="checkDate"><span class="small-red">*</span>檢查日期</label>
                    <div class="form-row">
                        <div class="form-group col-sm-4 col-md-3">
                            <input type="text" class="form-control" id="checkDate" v-model="checkDate">
                        </div>
                        <div class="form-group col-sm-4 col-md-3">
                            <select class="form-control" @change="getCheckData()" v-model="ccrCheckTypeKey">
                                <option v-bind:key="index" v-for="(data,index) in ccrCheckTypes" v-bind:value="data.key">{{data.item}}</option>
                            </select>
                        </div>
                        <div class="form-group col-sm-4 col-md-3">
                            <select class="form-control" @change="getCCManageItem()" v-model="checkListKey">
                                <option v-bind:key="index" v-for="(data,index) in checkLists" v-bind:value="data.seq">{{data.item}}</option>
                            </select>
                        </div>
                        <div class="form-group col-sm-4 col-md-3">
                            <select class="form-control"  @change="getCCManageItem()" v-model="ccFlowKey1">
                                <option v-bind:key="index" v-for="(data,index) in ccFlows1" v-bind:value="data.key">{{data.item}}</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col">
                    <label for="checkPositionX"><span class="small-red">*</span>檢查位置</label>
                    <div class="form-row">
                        <div class="form-group col-6 col-sm-4 col-md-3">
                            <input type="text" class="form-control" id="checkPositionX" placeholder="120.685415">
                        </div>
                        <div class="form-group col-6 col-sm-4 col-md-3">
                            <input type="text" class="form-control" id="checkPositionY" placeholder="24.137198 ">
                        </div>
                        <div class="form-group col-sm-4 col-md-3">
                            <button class="btn btn-color3 w-100" type="button">重新抓取所在位置</button>
                        </div>
                        <div class="form-group col-md-9">
                            <input type="text" class="form-control" id="PositionDescription" placeholder="輸入位置描述">
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-sm-8 col-md-8">
                    <label for="Sub-project"><span class="small-red">*</span>管理項目 (自動帶出施工抽查標準的項目)</label>
                    <select id="Sub-project" class="form-control" @change="drawCCManageItem($event)" v-model="ccManageItemKey">
                        <option v-bind:key="index" v-for="(data,index) in ccManageItems" v-bind:value="data.seq">{{data.item}}{{(data.item2!='' ? '；' + data.item2 : '')}}</option>
                    </select>
                </div>
                <div class="form-group col-sm-4 col-md-4">
                    <label for="keywordSearch">關鍵字搜尋</label>
                    <div class="input-group mb-3">
                        <input type="text" class="form-control" placeholder="請輸入關鍵字" aria-label="關鍵字搜尋">
                        <button class="btn btn-color3" type="button"><i class="fas fa-search"></i></button>
                    </div>
                </div>
            </div>
            
            <label v-show="drawCCManageItems.length > 0">抽查標準(定量定性)</label>
            <table v-show="drawCCManageItems.length > 0" border="0" class="table table1 col-12 col-md-6">
                    <tbody>
                        <template v-for="(item, index) in drawCCManageItems">
                            <tr v-bind:key="index">
                                <td>{{ item.Stand1 }}</td>
                                <td v-if="item.Stand2 !=''">{{ item.Stand2 }}</td>
                                <td v-if="item.Stand3 !=''">{{ item.Stand3 }}</td>
                                <td v-if="item.Stand4 !=''">{{ item.Stand4 }}</td>
                                <td v-if="item.Stand5 !=''">{{ item.Stand5 }}</td>
                            </tr>
                        </template>
                    </tbody>
            </table>
            
            <div class="form-row">
                <div class="form-group col-12 col-md-6">
                    <h2>抽查結果</h2>
                    <select id="Resault" class="form-control">
                        <option selected="">請選擇</option>
                        <option>檢查合格</option>
                        <option>有缺失</option>
                        <option>無此項目</option>
                    </select>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-12 col-md-6">
                    <label for="situation">實境抽查情形</label>
                    <textarea class="form-control" id="situation" rows="3"></textarea>
                </div>
            </div>
            <button class="btn btn-color3 mb-3" type="button">點擊此處並選擇工程照片</button>
            <button class="btn btn-color3 mb-3" type="button">尚未簽名</button>
        </form>
        <hr>
        <div class="row justify-content-center mt-3">
            <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
                <a href="#" role="button" class="btn btn-shadow btn-color1 btn-block">
                    暫存&nbsp;&nbsp;<i class="fas fa-save"></i>
                </a>
            </div>
            <div class="col-12 col-sm-4 col-md-4 col-lg-4 col-xl-2 mt-3">
                <a href="#" role="button" class="btn btn-shadow btn-color2 btn-block">
                    儲存並上傳&nbsp;&nbsp;<i class="fas fa-upload"></i>
                </a>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                account: document.querySelector("input[name=account]").value,
                mobile: document.querySelector("input[name=mobile]").value,
                checkDate: this.today(),
                engMainkey: null,
                engMains: [],
                engConstructions: [],
                ccrCheckTypeKey: 1,
                ccrCheckTypes: [{ "key": 1, "item": "施工抽查" }, { "key": 2, "item": "設備運轉測試" }, { "key": 3, "item": "職業安全" }, { "key": 4, "item": "生態保育" }],
                ccFlowKey1: 1,
                ccFlows1: [{ "key": 1, "item": "施工前" }, { "key": 2, "item": "施工中" }, { "key": 3, "item": "施工後" }],
                checkListKey: 0,
                checkLists: [],
                ccManageItemKey: 0,
                ccManageItems: [{ seq: '0', item: '請選擇', item2: '' }],
                tmpItemSeq: 0,
                tmpCCFlow: '',
                tmpCCManageItem: '',
                drawCCManageItems: []
            };
        },
        methods: {
            async checkUser() {
                window.myAjax.post('/MBEng/checkUser'
                    , {
                        account: this.account,
                        mobile: this.mobile
                    })
                    .then(resp => {
                        if (resp.data.length == 0) {
                            alert('參數錯誤');
                            window.location.replace('/');
                        } else {
                            this.getEngMain()
                        }
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getEngMain() {
                window.myAjax.post('/MBEng/getEngMain'
                    , {
                        mobile: this.mobile
                    })
                    .then(resp => {
                        this.engMains = resp.data;
                        this.engMainkey = resp.data[0].Seq;
                        this.getEngData(this.engMainkey);
                        this.getCheckData();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getEngData(engMainSeq) {               

                // 分項工程
                window.myAjax.post('/MBEng/getEngConstruction'
                    , {
                        engMainSeq: engMainSeq
                    })
                    .then(resp => {
                        this.engConstructions = resp.data;
                        this.getCheckData();
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getCheckData() {

                this.ccManageItemKey = 0;
                this.ccManageItems = [{ seq: '0', item: '請選擇', item2: '' }];
                this.ccManageItemKey = 0;
                this.drawCCManageItems = [];

                switch (this.ccrCheckTypeKey) {

                    case 1: // 施工抽查
                        window.myAjax.post('/MBEng/getConstCheckList'
                        , {
                            engMainSeq: this.engMainkey
                        })
                        .then(resp => {
                            this.checkListKey = 0;
                            this.checkLists = resp.data;
                            this.checkLists.splice(0, 0, { item: '請選擇', seq: '0' });
                        })
                        .catch(err => {
                            console.log(err);
                        });
                        break;

                    case 2: // 設備運轉測試
                        window.myAjax.post('/MBEng/getEquOperTestList'
                            , {
                                engMainSeq: this.engMainkey
                            })
                            .then(resp => {
                                this.checkListKey = 0;
                                this.checkLists = resp.data;
                                this.checkLists.splice(0, 0, { item: '請選擇', seq: '0' });
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 3: // 職業安全
                        window.myAjax.post('/MBEng/getOccuSafeHealthList'
                            , {
                                engMainSeq: this.engMainkey
                            })
                            .then(resp => {
                                this.checkListKey = 0;
                                this.checkLists = resp.data;
                                this.checkLists.splice(0, 0, { item: '請選擇', seq: '0' });
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 4: // 生態保育
                        window.myAjax.post('/MBEng/getEnvirConsList'
                            , {
                                engMainSeq: this.engMainkey
                            })
                            .then(resp => {
                                this.checkListKey = 0;
                                this.checkLists = resp.data;
                                this.checkLists.splice(0, 0, { item: '請選擇', seq: '0' });
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                }
            },
            async getCCManageItem() {
                this.tmpItemSeq = this.checkListKey
                this.tmpCCFlow = this.ccFlowKey1

                // 管理項目
                switch (this.ccrCheckTypeKey) {

                    case 1: // 施工抽查
                        window.myAjax.post('/MBEng/getCCManageItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccFlow: this.ccFlowKey1
                            })
                            .then(resp => {
                                this.ccManageItemKey = 0;
                                this.ccManageItems = resp.data;
                                this.ccManageItems.splice(0, 0, { seq: '0', item: '請選擇', item2: '' });
                                this.ccManageItemKey = 0;
                                this.drawCCManageItems = [];
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;

                    case 2: // 設備運轉測試
                        window.myAjax.post('/MBEng/getEPCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                            })
                            .then(resp => {
                                this.ccManageItemKey = 0;
                                this.ccManageItems = resp.data;
                                this.ccManageItems.splice(0, 0, { seq: '0', item: '請選擇', item2: '' });
                                this.ccManageItemKey = 0;
                                this.drawCCManageItems = [];
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 3: // 職業安全
                        window.myAjax.post('/MBEng/getOSCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                            })
                            .then(resp => {
                                this.ccManageItemKey = 0;
                                this.ccManageItems = resp.data;
                                this.ccManageItems.splice(0, 0, { seq: '0', item: '請選擇', item2: '' });
                                this.ccManageItemKey = 0;
                                this.drawCCManageItems = [];
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 4: // 生態保育
                        window.myAjax.post('/MBEng/getECCCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccFlow: this.ccFlowKey1
                            })
                            .then(resp => {
                                this.ccManageItemKey = 0;
                                this.ccManageItems = resp.data;
                                this.ccManageItems.splice(0, 0, { seq: '0', item: '請選擇', item2: '' });
                                this.ccManageItemKey = 0;
                                this.drawCCManageItems = [];
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                }
            },
            async drawCCManageItem(event) {
                this.tmpCCManageItem = event.target.options[event.target.options.selectedIndex].text
                var tmpManageItemArray = this.tmpCCManageItem.split('；')
                var manageItem1 = tmpManageItemArray[0].replace(' ', '%')
                var manageItem2 = tmpManageItemArray[1] != undefined ? tmpManageItemArray[1].replace(' ', '%') : ''

                // 抽查標準(定量定性)
                switch (this.ccrCheckTypeKey) {

                    case 1: // 施工抽查
                        window.myAjax.post('/MBEng/drawCCManageItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccFlow: this.ccFlowKey1,
                                ccManageItem1: manageItem1,
                                ccManageItem2: manageItem2
                            })
                            .then(resp => {
                                this.drawCCManageItems = resp.data;
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;

                    case 2: // 設備運轉測試
                        window.myAjax.post('/MBEng/drawEPCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccManageItem1: manageItem1,
                                ccManageItem2: manageItem2
                            })
                            .then(resp => {
                                this.drawCCManageItems = resp.data;
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 3: // 職業安全
                        window.myAjax.post('/MBEng/drawOSCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccManageItem1: manageItem1,
                                ccManageItem2: manageItem2
                            })
                            .then(resp => {
                                this.drawCCManageItems = resp.data;
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                    case 4: // 生態保育
                        window.myAjax.post('/MBEng/drawECCCheckItem'
                            , {
                                constCheckListSeq: this.checkListKey,
                                ccFlow: this.ccFlowKey1,
                                ccManageItem1: manageItem1,
                                ccManageItem2: manageItem2
                            })
                            .then(resp => {
                                this.drawCCManageItems = resp.data;
                            })
                            .catch(err => {
                                console.log(err);
                            });
                        break;
                }
            },
            today() {
                let nowDate = new Date();
                let year = nowDate.getFullYear() - 1911;
                let month = (nowDate.getMonth() + 1) > 9 ? nowDate.getMonth() + 1 : '0' + (nowDate.getMonth() + 1);
                let day = nowDate.getDate() > 9 ? nowDate.getDate() : '0' + (nowDate.getDate());
                let time = `${year}/${month}/${day}`;
                return time;
            }
        },
        async mounted() {
            this.checkUser();
        }
    }
</script>
