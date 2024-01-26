import Common from "../../Common/Common2";
export default class TableControl  {

    tabStatus = {
        NavvyTab : this.selectTab == "NavvyTab" ? "active" : "",
        TruckTab : this.selectTab == "TruckTab" ? "active" : "",
        Tab : this.selectTab == "Tab" ? "active" : "",

    };
    selectTab ='';
    modelMap ={
        "NavvyTab" : "typeM1",
        "TruckTab" : "typeM2",
        "Tab" : "typeM3"
    }
    typeMap = {
        "NavvyTab" : 0,
        "TruckTab" : 1,
        "Tab" : 2
    }
    listMode= true;
    targetId= null;
    items= [];
    totalScore= -1;
    editSeq= -99;
    editRecord= {};
    newItem= {};
    masterItem= {}
    //分頁
    recordTotal= 0
    pageIndex= 1
    pageRecordCount= 30
    lastUpdate= null
    searchStr=null

    constructor(tab)
    {

        this.selectTab =tab;
    }
    editDetail(item){
        this.masterItem = item;
        this.listMode = false
    }
    closeSubList() {
        this.listMode = true;
        this.getResords();
    }
    search() {
        this.getResords();
    }
    getlastUpdateDate(hasTime = false) {

        return Common.ToROCDate(this.lastUpdate, hasTime);
    }
    checkCode(item) {
        if (item.KeyCode2 == '-1')
            return item.Code + '<br /><font color="red">(待修正)</font>'
        else
            return item.Code;
    }
    onNewRecord(uItem) {
        var saveRecordItem = {};
        saveRecordItem[this.modelMap[this.selectTab] ] = uItem; 
        var type = this.typeMap[this.selectTab];

        if (uItem.Code == null ||uItem.Code.length < 10) {
            alert('編碼必須輸入至少10碼');
            return;
        }
        window.myAjax.post('/CarbonReduction/Insert', {type:type, ...saveRecordItem })
            .then(resp => {
                if (resp.data.result == 0) {
                    uItem.Seq = resp.data.insertSeq;
                    alert("新增成功!");
                    this.newItem = {};
                    this.items.push(uItem);
                }
            })
            .catch(err => {
                console.log(err);
            });
    }
    //刪除紀錄
    onDelRecord(item) {
        if (this.editSeq > -99) return;
        if (confirm('是否確定刪除資料？')) {
            window.myAjax.post('/CarbonReduction/Delete', { id: item.Seq })
                .then(resp => {
                    if (resp.data.result == 0) {
                        this.getResords();
                    }
                    alert("刪除成功");
                })
                .catch(err => {
                    console.log(err);
                });
        }
    }
    strEmpty(str) {
        return window.comm.stringEmpty(str);
    }
    //儲存
    onSaveRecord(uItem) {
        //console.log(uItem);
        // if (this.strEmpty(uItem.Code) || this.strEmpty(uItem.Item) || uItem.KgCo2e == null ) {
        //     alert('編碼,工作項目,碳排係數 必須輸入!');
        //     return;
        // }
        if (uItem.Code.length < 10) {
            alert('編碼必須輸入至少10碼');
            return;
        }
        var saveRecordItem = {  };
        
        saveRecordItem[this.modelMap[this.selectTab] ] = uItem; 
        var type = this.typeMap[this.selectTab];
        window.myAjax.post('/CarbonReduction/Update', { type :type, ...saveRecordItem  })
            .then(resp => {
                if (resp.data.result == 0) {
                    this.editSeq = -99;
                    this.getResords();
                    // if (uItem.Seq == -1) this.onNewRecord();
                } else
                    alert(resp.data.msg);
            })
            .catch(err => {
                console.log(err);
            });
    }
    //取消編輯
    onEditCancel() {
        this.editSeq = -99;
        this.getResords();
    }
    //編輯紀錄
    onEditRecord(item) {
        if (this.editSeq > -99) return;
        this.editRecord = Object.assign({}, item);
        this.editSeq = this.editRecord.Seq;
    }
    //紀錄清單
    getResords() {
        this.items = [];
        
        window.myAjax.post('CarbonReduction/GetList', {
                pageRecordCount: this.pageRecordCount,
                pageIndex: this.pageIndex,
                keyWord : this.searchStr ?? "",
                type : this.typeMap[this.selectTab]
            })
            .then(resp => {
                if (resp.data.result == 0) {
                    this.items = resp.data.items;
                    this.recordTotal = resp.data.pTotal;
                    this.lastUpdate = resp.data.lastUpdate;
                }
            })
            .catch(err => {
                console.log(err);
            });
    }
    //分頁
    onPaginationChange(pInx, pCount) {
        this.pageRecordCount = pCount;
        this.pageIndex = pInx;
        this.getResords();
    }
    //匯入 excel
    fileChange(event) {
        var files = event.target.files || event.dataTransfer.files;
        // 預防檔案為空檔
        if (!files.length) return;

        //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
        //application/vnd.ms-excel
        if (!files[0].type.match('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {// && !files[0].type.match('application/vnd.ms-excel') ) {
            alert('請選擇 .xlsx Excel檔案');
            return;
        }
        var uploadfiles = new FormData();
        uploadfiles.append("file", files[0], files[0].name);
        this.upload(uploadfiles);
    }
    upload(uploadfiles) {
        window.myAjax.post('/CarbonReduction/Upload', uploadfiles,
            {
                headers: { 'Content-Type': 'multipart/form-data' }
            }).then(resp => {
                if (resp.data.result == 0) {
                    this.getResords();
                }
                alert("更新成功!");
            }).catch(error => {
                console.log(error);
            });
    }
    download()
    {
        Common.dnFile("/CarbonReduction/Download");
    }


}

