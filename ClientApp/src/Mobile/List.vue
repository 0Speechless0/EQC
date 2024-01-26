<template>
    <div>
        <div class="name">歡迎！ &nbsp;許小明 經理</div>

        <h1 class="mt-2">查看抽驗清單</h1>
        <h2>您的抽驗清單</h2>
        <div class="form-group">
            <label for="projectName"><span class="small-red">*</span>工程名稱</label>
            <select id="projectName" class="form-control" @change="getEngData(engMainkey)" v-model="engMainkey">
                <option v-bind:key="index" v-for="(data,index) in engMains" v-bind:value="data.Seq">{{data.EngName}}</option>
            </select>
        </div>
        <div class="mb_list1">
            <template v-for="(data, index) in engConstructions">
                <template v-if="data.checkDate2 != ''">
                    <a href="#" class="a-blue" v-bind:key="index">
                        <h3>{{engMainName}}</h3>
                        <h4>分項工程：{{ data.construction }}</h4>
                        <p>抽驗日期：{{ data.checkDate2 }}  {{ data.checkFlow }}</p>
                        <p>抽驗項目：{{ data.item }}</p>
                    </a>
                </template>
                <template v-if="data.checkDate2 == ''">
                    <a :href="url" class="a-blue" v-bind:key="index">
                        <h3>{{engMainName}}</h3>
                        <h4>分項工程：{{ data.construction }}</h4>
                        <p class="pr80">抽驗日期：{{ data.checkDate2 }}  {{ data.checkFlow }}</p>
                        <p class="pr80">抽驗項目：{{ data.item }}</p>
                        <span class="before tag">未上傳</span>
                    </a>
                </template>
            </template>
        </div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                account: document.querySelector("input[name=account]").value,
                mobile: document.querySelector("input[name=mobile]").value,
                engMainkey: null,
                engMainName: '',
                engMains: [],
                engConstructions: [],
                url: "MBEng?account=" + document.querySelector("input[name=account]").value + "&mobile=" + document.querySelector("input[name=mobile]").value
            };
        },
        components: {

        },
        methods: {
            async checkUser() {
                window.myAjax.post('/MBList/checkUser'
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
                window.myAjax.post('/MBList/getEngMain'
                    , {
                        mobile: this.mobile
                    })
                    .then(resp => {
                        this.engMains = resp.data;
                        this.engMainkey = resp.data[0].Seq;
                        this.engMainName = resp.data[0].EngName;
                        this.getEngData(this.engMainkey);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            async getEngData(engMainSeq) {

                window.myAjax.post('/MBList/getEngConstruction'
                    , {
                        engMainSeq: engMainSeq
                    })
                    .then(resp => {
                        this.engConstructions = resp.data;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            }
        },
        async mounted() {
            this.checkUser();
        }
    }
</script>
