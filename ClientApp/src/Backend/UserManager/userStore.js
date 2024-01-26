import {reactive } from "vue";
export const userStore = reactive({
    userList : [],
    currentPage: 1,
    perPage : 10,
    nameSearch : null,
    subUnit : ["", ""] ,
    userInfo : {},
    unitOptions : [],
    totalRows : 0,
    isLastLevel : false,
    isOutSource : false,
    hasConstCheckApp : false,
    async getList(){
        const self = this;
        let params = { page: this.currentPage,   per_page: this.perPage, subUnit: this.subUnit, nameSearch: this.nameSearch, hasConstCheckApp: this.hasConstCheckApp };
        window.myAjax.post('/Users/GetListV2', params)
            .then(resp => {
                resp.data.l.forEach((item, index) => {
                    item.UnitSeq2 = item.UnitSeq2 || '0';
                    item.UnitSeq3 = item.UnitSeq3 || '0';
                });
                this.userList = resp.data.l;
                this.unitOptions = resp.data.unitOptions == '' ? [] : resp.data.unitOptions; 
                this.totalRows = resp.data.t;
            })
            .then(err => {
                //console.log(err);
            });

        // if(this.nameSearch == null || this.nameSearch == "") {
        //     window.openModal("#pageControl");
        // }
        // else window.closeModal("#pageControl");
    },
    async getUserInfo() {
        let res = await window.myAjax.post("/Users/GetUserInfo");
        console.log(res.data);
        this.userInfo = res.data.userInfo;
        this.isLastLevel = res.data.isLastLevel;
        this.isOutSource = res.data.isOutsource;


    },
    getChildList : async() => {
        let res = await window.myAjax.post("/Users/GetChildList", {
            page: this.currentPage, per_page: this.perPage
        });
        this.userList = res.data.l;
        this.totalRows = res.data.t;
    }
});

