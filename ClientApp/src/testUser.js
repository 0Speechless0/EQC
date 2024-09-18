export const useAccount = (account) =>
{
    return [

        {
            //監造單位
            role : 5,
            testAccount : "S97586963",
            testPassword : "!97586963!"
            
        },
        {
            //系統管理者 工務科
            role : 1,
            testAccount : "kbb370381",
            testPassword : "a3146"
            
        },
        {
            role : 20,
            // 建立者
            testAccount : "wra05025",
            testPassword : "83442586"
            
        },
        {
            role : 3,
            //管理科長
            testAccount : "wca03051",
            testPassword : "83442586"
   
        },
        {
            role : 3,
            //工務科長
            testAccount : "wca03108",
            testPassword : "83442586"
        },
        {
            role : 3,
            testAccount : "wca03018",
            testPassword : "83442586"
        },
             //分署長室
        {
            role : 3,
            testAccount : "wca03012",
            testPassword : "83442586"
        },
        {
            //局長 工務科
            testAccount : "wu530303",
            testPassword : "83442586"
            
        },
        {
            //執行者 工務科 科長
            testAccount : "wra05046",
            testPassword : "83442586"
            
        },
        {
            //執行者 規劃科
            testAccount : "wra05017",
            testPassword : "83442586"
            
        },
        {
            //執行者 管理
            testAccount : "wra05086",
            testPassword : "83442586"
            
        },
        {
            //管理者
            testAccount : "wra05070",
            testPassword : "83442586"
            
        },
        {
            //施工廠商
            testAccount : "C90310884",
            testPassword : "!90310884!"
            
        },
        {
            //施工廠商
            testAccount : "C90310884JJJ",
            testPassword : "83442586"
            
        },
        {
            testAccount : "anita",
            testPassword : "83442586"
        }
    
    
    ].find( e => e.testAccount == account);
}

