# 子系統功能擴充

以目前系統的路由機制，需要搭配資料庫的兩隻表 :
- Menu : 內容為子系統功能的相關資訊，包括所有不同等級層的子系統功能，以欄位parentSeq為0的為最大的子系統功能，除了parentSeq 為0之外，都會顯示於網頁的左清單顯示
- SystemType : 內容為子系統的別名，一對多關連到Menu，作用為網頁上方Header的路由名稱顯示

點選子系統清單，左側清單會顯示該子系統的功能清單，點擊後右側顯示功能頁面，以下將說說明如何新增一個子系統到子系統清單和新的子系統功能到左側清單

![Screenshot_2](https://github.com/sunyu-tech/EQC_NEW/assets/36149504/d84bd024-df52-4e0e-804b-8a229a6caa00)


#### 1.新增子系統資料到SystemType
理論上可以一直新增，但要配合畫面顯示，子系統清單有太多的子系統可能造成跑版

`如果不需要新增，跳過此步驟`

``` sql
declare @orderNo as nvarchar(50);
declare @Seq as int;
set @orderNo = (select top 1 OrderNo from SystemType order by OrderNo desc)+1
set @Seq = (select top 1 Seq from SystemType order by Seq desc)+1
INSERT INTO [dbo].[SystemType]
           (
		   [Seq]
           ,[Name]
           ,[OrderNo]
           ,[IsEnabled]
		   )
     VALUES
           (
		   @Seq
           ,'子系統名稱'
           ,@orderNo
           ,1)
 
```

#### 2.註冊子系統到Session

系統透過登入後儲存的Session確認可進入的所有功能並顯示於左側清單，
因此當新的功能是在新的子系統中，需在`Common/SessionManager`的`CheckSession`中載入節點進入權依據，反之，可以跳過此步驟。

我們使用 menuService.LoadMenu方法註冊到menuData中
1. 參數1 :  子系統代碼 (請自行定義不重複即可)
2. 參數2 :  使用者序列號，這裡用userData.Seq就好

Common/SessionManager :

``` c       
public void CheckSession(System.Web.SessionState.HttpSessionState _session)
{
    SessionManager sm = new SessionManager();
    string account = _session["Account"].ToString();
    // 傳進來Session的值與存在SessionManager不一樣時, 將SessionManager重新取值
    if (sm.GetUser().UserNo != account)
    {
        UserInfo userData = userService.GetUserByAccount(account).FirstOrDefault();
        List<Role> roleData = null; 
        List<VMenu> menuData = null;
        if (userData != null)
        {
            roleData = userService.GetRoleByAccount(account);//shioulo 20210707 
            menuData = menuService.LoadMenu(1, userData.Seq);
            menuData.AddRange(menuService.LoadMenu(2, userData.Seq));
            menuData.AddRange(menuService.LoadMenu(3, userData.Seq));//shioulo 20220426 
            menuData.AddRange(menuService.LoadMenu(20, userData.Seq));//alex20230327vm 
            
            //在此新增節點進入權依據 menuData.AddRange(menuService.LoadMenu(20, userData.Seq))
            
            SetSession(userData, roleData, menuData);
        }
    }
}
```

#### 3.新增資料到Menu

以下是新增Menu的Sql語法
``` sql
/****** Script for SelectTopNRows command from SSMS  ******/

-- 代碼對應子系統
--1	前台
--2	後台維護
--3	工程履約
--4	查詢專區
--8	工程督導
--9	儀表板
--10	建立標案
--11	交流平台
--12	工程提報
--20	植樹專區

declare @insertSeq as int;
declare @order as int;
declare @SystemTypeSeq as int = '子系統代碼';
declare @ParentSeq as int = (select top 1 Seq from Menu where SystemTypeSeq = @SystemTypeSeq and ParentSeq = 0);
declare @Path as nvarchar(50) = '功能路由';
declare @Name as nvarchar(50) = '功能名稱';
set @insertSeq = (select top 1 Seq from Menu where Seq < 1000 order by Seq desc )+1;
set @order = (select top 1 OrderNo from Menu where Menu.SystemTypeSeq = @SystemTypeSeq order by OrderNo desc)+1;
insert Menu (Seq, SystemTypeSeq, Name, ParentSeq, PathName, OrderNo, IsEnabled, CreateTime) 
values (@insertSeq, @SystemTypeSeq,@Name, @ParentSeq, @Path, @order, 1 ,GETDATE() );

insert MenuRole(MenuSeq, RoleSeq) values(@insertSeq, 1)
```
##### SQL Menu更新提醒 

我們偉大的修維決定他每次新增menu的seq從1000開始，所以我們新增的menu必需小於1000，
不需要考量如果新增到超過1000的問題，因為我們偉大的修維不可能會錯

#### 4.建立Controller

我們使用visual studio 2019 建立 controller ，在右邊找到方案總管找到Controller資料夾按 : 
* 右鍵 > 新增控制器 > 命名controller

這裡採駝峰式命名， 如 GoodController， 名稱為Good，如果這樣命名，前面Sql新增Menu語法中`功能路由`就是 `Good/Index`

`以下用Good示範，請自行將Good取代`

以下程式繼承MyController，MyController繼承原生的Controller，使用原生的Json方法可能會發生序列化的問題，可以使用MyController自定的`ResponseJson`方法，以`Newtonsoft`為基礎解決序列化問題

`SessionFilter` 主要解決權限判段問題，讓此頁面功能依角色權限開放，如果角色沒有權限，會登出並自動跳轉到登入頁
```c
using System.Web.Mvc;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using EQC.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class GoodController : MyController
    {
        //頁面跳轉走這function，如果回傳是ActionResult 就是頁面跳轉
        public ActionResult Index() 
        {
            Utils.setUserClass(this);
            return View();
        }
        
        //如果是JsonResult 就是回傳API
        public JsonResult GetList(int page, int per_page, string sort_by)
        {
            List<City> list = cityService.GetList(page-1, per_page, sort_by);
            return Json(new
            {
                l = list
            });
        }
        
        //如果City模型是visual studio 生成的edmx模型，且有關連的情形，使用原生的Json方法無法正確序列化
        //此時可以使用MyController的ResponseJson方法
        public void GetList(int page, int per_page)
        {
            using(var context  = new DBEntities() )
            {
                //這裡回傳 List<City>
                var list  = context.City.ToList() 
                    .GetRange(page, per_page * page);

                ResponseJson(new
                {
                    l = list
                });
            }
        }
    }
}
```



#### 5.建立View
在Index 方法上點擊右鍵新增檢視頁，版面配置套用 `Views/Shared/_MainLayout.cshtml`，其他不用改
```c
namespace EQC.Controllers
{
    [SessionFilter]
    public class GoodController : Controller
    {
        public ActionResult Index() //在這裡點右鍵 > 新增檢視
        {
            return View();
        }
    }
}
```
之後visual studio 方案總管找到 `Views/Good/Index.cshtml`，將內容修改如下
``` html
@{
    ViewBag.Title = "myGood";
    ViewBag.BarTitle = ViewBag.Title;
    ViewBag.hasCard = fasle;
    Layout = "~/Views/Shared/_MainLayout.cshtml";
}
<my-good></my-good>
```
`<my-good>` 是 從vue編譯後的componet標籤名稱， 包含該功能所有前端畫面及程式， 這裡請自行取代`my-good`，不與其他功能重複即可

以下幾點注意 :
1.  需將`myGood` 取代成`步驟2`中定義的功能名稱
2.  `<my-good>`將自動套用版面`Views/Shared/_MainLayout.cshtml`，預設如圖會有上方標頭 :

![Screenshot_3](https://github.com/sunyu-tech/EQC_NEW/assets/36149504/826365d6-2a35-4625-ae78-43d16711b8d9)

如果不需要，ViewBag.hasCard 設為 `false`



#### 6.建立View Component

前端已經不是傳統MVC框架，比較像是前後端分離，前端使用Vue框架取代部分View的工作，程式編譯成`app.js`後在原來的View中引入js檔
另外，在vue中使用的css 、套件的css也需要引入編譯後的 `app.css`

`Views/Shared/_MainLayout.cshtml` 需要引入 :

1. 編譯js目標位置 : `Content/dist/js/app.js`
2. 編譯css目標位置 : `Content/dist/css/app.css`

Views/Shared/_MainLayout.cshtml:
```html
<body class="@bodyClass">

    //vue 渲染起始
    <div id="WRAPPER" class="container-fluid" style="@mainContentStyle" >
    </div>
    ...
</body>
 <footer>
    ...
 </footre>
 ...
    <link href="@Url.Content("~/Content/dist/css/app.css")" rel="stylesheet">
    <script src=@Url.Content("~/Content/dist/js/app.js")></script>
    <script> 
        //啟動Vue渲染
        var app = new Vue({}).$mount('#WRAPPER');
        
        //設定localStorage
        localStorage.setItem('isAdmin', '@userInfo.IsAdmin');
        localStorage.setItem('isEQCAdmin', '@userInfo.IsEQCAdmin');
        ...
    </script>
```
從渲染起始往下的子結構中，只要內容標籤包括像`<my-good></my-good>` 這樣的標籤，會被vue渲染成 component
component包括該功能的顯示前端頁面和程式，接著，我們需要在vue專案中建立component 的標籤如`<my-good></my-good>`

vue專案根目錄 : `ClientApp\`

開啟 `ClientApp\src\sub2.js` :
``` js
import Vue from 'vue';
Vue.component("tree-list", require("./Tree/TreeList.vue").default);
Vue.component("tree-new", require("./Tree/TreeNew.vue").default);

//新增<my-good> component
Vue.component("my-good", require("./Good/Index.vue").default);
```
這裡的相對位置從 `ClientApp/src/` 開始，抓到  `Index.vue`，即 Good 基於Vue專案下的前端程式碼檔案

記得改完儲存，然後在vue專案根目錄執行指令 :
``` sh
npm run build
```
# vue component 測試
`npm run build`執行過程需要一段時間，在某些需要調整版面而頻繁查看前端畫面的情況下或測試前端API，`npm run build`過於浪費時間
我們使用`npm run serve`來運行本地伺服器查看網頁改善開發的便利

這裡需要設定 `url path`導向的程式
`./src/test.js` :
``` js
//測試使用者帳號
var testAccount = "guest";

//測試使用者密碼
var testPassword = "a12345b";

var TestComponent;
//API設定;這裡https://localhost:44300是API目的位址，即
window.myAjax = axios.create({ baseURL: process.env.NODE_ENV != 'production' ? 'https://localhost:44300': window.location.origin });

const pageGenerate = () => {

    switch(window.location.pathname)
    {
        //在此case path 設定TestComponent
        case "/Good" :
            TestComponent = require('./Good/Index.vue').default;break;
                ...
    }

}
...

```
一樣在vue專案根目錄執行cmd 
``` sh
npm run serve
```

造訪 `http://localhost:{port}/Good` 查看測試Component畫面

# 文件產製模組
* git : https://github.com/sunyu-tech/EQC.Proposal
* dll : EQC.ProposalV2.dll
#### 1.dll 加入專案參考

先 clone git專案，運行專案產生dll至編譯目的位置，編譯模式選擇 `Release`，

找到 `EQC.ProposalV2.dll` 複製到 Web專案中 

接著用visual studio方案總管加入參考的功能加入參考`EQC.ProposalV2.dll`

#### 2.執行產製工作

每個產製工作實體都應該繼承ExportInstance介面，並實作 Export方法 
(詳情閱讀EQC.ProposalV2專案的說明文件)

我們會運行Export，成功回傳 true，失敗回傳false，

``` c

namespace EQC.ProposalV2.Interface
{
    public interface ExportInstance
    {
         bool Export(
            string outputFileName,  //文件輸出fullName
            string templateFileName, //樣板輸入fullName
            string attachmentDir,   //圖片資料夾 path
            string logDir,          // 紀錄資料夾 path
            string attachmentDirAlt = null //備用圖檔資料夾 path
        );
    }
}

```
如果圖檔資料夾找不到特定名稱的圖檔，則嘗試去找備用圖檔資料夾，預設null則不尋找備用資料夾

`備用圖檔資料夾可以當作是共用圖檔資料夾`，有些共用的圖檔不需要每個資料夾都存，我們只需要放在備用圖檔資料夾即可



