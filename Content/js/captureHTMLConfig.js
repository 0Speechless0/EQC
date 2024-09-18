export default {

    //設定觸發擷取的元素，被點擊觸發 參數reset =true 的擷取API，
    //當屬性變更觸發參數reset =true 的 擷取API 

    //ex :"inputTags" : "input, select, textarea ..."
    inputTags: "input, select, textarea",

    //設定可能會觸發API呼叫造成資料變更的元素，這些元素會出發擷取API ，reset = false，被視為API擷取對象變更後的畫面

     //ex :"renderTags" : "button, a ..."
    renderTags: "button, a",

    //設定擷取API，實作API實必須提供參數 reset，當reset為true時代表發送變更前的html，反之為變更後的，
    //需額外判斷擷取對應的API

    //ex : "apiAPIURL": "/APIRecord/captureHTML"
    apiURL: "/APIRecord/captureHTML",

    //設定捕捉的最高層DOM邊界，只捕捉id 為 contentId以下的元素變更
    contentId: "content",

    //設定點擊圖案 Unicode
    clickMark: "&#128308;",

    // 設定差異標記忽略的HTML標籤
    DiffIgnoreTag : [ "OPTION" ],
    //設定等待頁面載入時間
    waitLoading : 1500

}