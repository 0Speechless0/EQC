//倒數計時控制
var countDown = {
    dfCountTime: 30,//預設倒數秒數
    blinkingTime: 10,//預設幾秒後開始閃爍
    countdownId: null,//setInterval ID
    countTime: -1,//計算用
    /**
     *
     * @param countId 顯示倒數數字的Element id
     * @param endFun 計時結束要做的function
     * @param dfCountTime 倒數秒數
     * @param blinkingTime 幾秒後開始閃爍
     */
    init: function (countId, endFun, dfCountTime, blinkingTime) {

        if (dfCountTime != undefined)
            this.dfCountTime = dfCountTime;

        if (blinkingTime != undefined)
            this.blinkingTime = blinkingTime;

        this.endFun = endFun;
        this.countId = countId;
        this.countTarget = document.getElementById(this.countId);
        this.countTarget.innerHTML = "";


    },

    /**
     * 到數計時
     */
    _countdown: function () {

        if (this.countTime == -1) {
            this.countTime = this.dfCountTime;
        }

        //小於10秒時會開始閃爍
        if (this.countTime <= this.blinkingTime) {
            this.countTarget.innerHTML = "" + secToHHMM(this.countTime--) + "";
            setTimeout(function () {
                countDown.countTarget.innerHTML = "";
            }, 500);
        } else {
            this.countTarget.innerHTML = "" + secToHHMM(this.countTime--) + "";
        }
        //TODO 目前報錯先註解掉
        //// 每一分鐘重置後端Session
        //if (this.countTime % 300) {
        //    var url = '@Url.Action("ResetSession", "Menu")';
        //    $.ajax({
        //        url: '/Menu/ResetSession',
        //        dataType: 'json',
        //        type: 'post',
        //        cache: false,
        //        async: true,
        //        success: function (data) {
        //        }
        //    })
        //}


        //時間到了
        if (this.countTime < 0) {
            this.endFun();//
            this.stop();
        }

    },
    /**
     * 停止到數
     */
    stop: function () {
        if (this.countdownId != null && this.countdownId != undefined) {
            clearInterval(this.countdownId);
        }
        this.countTime = -1;
        if (this.countTarget != undefined) {
            this.countTarget.innerHTML = "";
        }
    },
    /**
     * 開始
     */
    start: function () {
        //倒數計時
        this.countdownId = setInterval(function () {
            countDown._countdown();
        }, 1000);
    },

    restart: function () {
        this.countTime = this.dfCountTime;
        alert(this.countTime);
        clearInterval(this.countdownId);
        this.start();
    },

    
}
function paddingLeft(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return paddingLeft("0" + str, lenght);
}
function paddingRight(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return paddingRight(str + "0", lenght);
}
function secToHHMM(sec) {
    var mm, ss;
    mm = parseInt(sec / 60).toString();
    ss = (sec % 60).toString();
    return paddingLeft(mm, 2) + " : " + paddingLeft(ss, 2);
}
