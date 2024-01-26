import moment from 'moment';
// Suppress the warnings
moment.suppressDeprecationWarnings = true;

export default {
    //s20230527
    getMoment: function (date) {
        return moment(date);
    },
    //工程狀態
    getEngStateCss: function (state) {
        if (state == '發包前')
            return "fa fa-circle text-danger mr-1";
        else if (state == '施工前')
            return "fa fa-circle text-warning mr-1";
        else if (state == '施工中')
            return "fa fa-circle text-success mr-1";
        else if (state == '結案')
            return "fa fa-circle text-info mr-1";
        else if (state == null || state == '' )
            return "";
        else
            return "fa fa-circle mr-1";
    },
    // 轉成民國年
    ToROCDate: function (date, hasTime) {
        var m = moment(date);
        if (m.isValid() == false) return '';
        var year = moment(date).format('YYYY') - 1911;
        var month = moment(date).format('M');
        var day = moment(date).format('D');
        var hour = moment(date).format('HH');
        var min = moment(date).format('mm');
        var sec = moment(date).format('ss');
        //var milsec = moment(date).format('mil');
        if (hasTime) {
            return year + '/' + month + '/' + day + ' ' + hour + ':' + min + ':' + sec;
        }
        else {
            return year + '/' + month + '/' + day;
        }
    },
    ToDate: function (date, hasTime) {
        var m = moment(date);
        if (m.isValid() == false) return '';
        var year = moment(date).format('YYYY');
        var month =padLeftZero(moment(date).format('M'));
        var day = padLeftZero(moment(date).format('D'));
        var hour = padLeftZero(moment(date).format('HH'));
        var min = padLeftZero(moment(date).format('mm'));
        var sec = moment(date).format('ss');
        //var milsec = moment(date).format('mil');
        if (hasTime) {
            return year + '-' + month + '-' + day + ' ' + hour + ':' + min + ':' + sec;
        }
        else {
            return year + '-' + month + '-' + day;
        }
    },
    formatDate: function (date, fmt) {
        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (date.getFullYear() + '').substr(4 - RegExp.$1.length));
        }

        let o = {
            'M+': date.getMonth() + 1,
            'd+': date.getDate(),
            'h+': date.getHours(),
            'm+': date.getMinutes(),
            's+': date.getSeconds()
        };
        for (let k in o) {
            if (new RegExp(`(${k})`).test(fmt)) {
                let str = o[k] + '';
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? str : padLeftZero(str));
            }
        }
        return fmt;
    },
    //計算兩日期間的天數
    calDays(startDate, endDate) {
        let dt1 = new Date(startDate);
        let dt2 = new Date(endDate);
        return ((dt2 - dt1) / (1000 * 60 * 60 * 24));
    },
    //create Dialog
    createDialog(obj, title, width, autoOpen, isShowCloseBtn) {
        $('body').addClass('stop-scrolling');
        $('html, body').scrollTop(0);
        let cTitle = '';
        if (typeof (title) != 'undefined') {
            cTitle = title;
        }
        let cWidth = 500;
        if (typeof (width) != 'undefined') {
            cWidth = width;
        }
        //RWD用
        if (cWidth > window.screen.width) {
            cWidth = '100%';
        }
        let cAutoOpen = false;
        if (typeof (autoOpen) != 'undefined') {
            cAutoOpen = autoOpen;
        }
        obj.dialog({
            autoOpen: cAutoOpen,
            draggable: true,
            resizable: false,
            open: function (event, ui) {                
                // 是否顯示標題旁的關閉按鈕和灰底關閉
                if (!isShowCloseBtn) {
                    $('.ui-dialog-titlebar-close', ui.dialog | ui).hide();
                }
                if (isShowCloseBtn) {
                    obj.scrollTop(0);
                    $('.ui-dialog-titlebar-close').on('click',
                        function () {
                            $('html, body').scrollTop(0);
                        });
                    $('.ui-widget-overlay').on('click',
                        function () {
                            obj.dialog('close');
                            $('html, body').scrollTop(0);
                        });
                }
            },
            close: function () {
                $('body').removeClass('stop-scrolling');
            },
            title: cTitle,
            width: cWidth,
            modal: true,
            maxHeight: 800
        });
    },
    //關閉DIALOG
    closeDialog(obj) {
        $(obj).dialog('close');
    },
    //顯示驗證結果
    showResultMessage(response, callback) {
        if (!response.IsSuccess) {
            var errorMessage = response.data.Message;
            var error = false;
            $.each(response.data.ValidationFrontEnd,
                function (index, value) {
                    //一拼回傳訊息
                    errorMessage += `\n${value.ErrorMessage}`;
                    error = true;
                });
            if (!error) {
                errorMessage = response.data.Message;
            }
            alert(errorMessage);
        } else {
            alert(response.data.Message);
            if (callback != null) {
                callback();
            }
        }
    },
    //空字串 or null shioulo 20210425
    stringEmpty(str) {
        try {
            return (!str || str.trim().length === 0);
        } catch {
            return (!str || str.length === 0);
        }
    },
    //0951225 國曆字串格式 095/12/25
    formatCDateStr(d) {
        if (d == null || d == '') return;

        return d.substring(0, 3) + '/' + d.substring(3, 5) + '/' + d.substring(5, 7);
    },
    //是否為數值 20230419
    isNumber(d) {
        const checkNumber = n => (typeof (n) === 'number' || n instanceof Number) || isFinite(n);
        return checkNumber(d);
    },
    //檔案下載 GET
    dnFile(action, funcCallback) {
        window.myAjax.get(action, { responseType: 'blob' })
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
                        if (funcCallback != null) funcCallback(false); //s20230830
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
                        //document.body.removeChild(link);
                        if (funcCallback != null) funcCallback(true);//s20230830
                    } else {
                        console.log('saveFilename is null');
                        if (funcCallback != null) funcCallback(false);//s20230830
                    }
                } else {
                    alert('格式錯誤下載失敗');
                    if (funcCallback != null) funcCallback(false);//s20230830
                }
            }).catch(error => {
                console.log(error);
            });
    },
    //中曆轉西元 s20230402
    toYearDate(dateStr) {
        if (dateStr == null || dateStr == '') return null;
        var dateObj = dateStr.split('/'); // yyy/mm/dd
        return new Date(parseInt(dateObj[0]) + 1911, parseInt(dateObj[1]) - 1, parseInt(dateObj[2]));
    },
    //日期檢查 s20230402
    isExistDate(dateStr) {
        var dateObj = dateStr.split('/'); // yyy/mm/dd
        if (dateObj.length != 3) return false;

        var limitInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

        var theYear = parseInt(dateObj[0]);
        if (theYear != dateObj[0]) return false;
        var theMonth = parseInt(dateObj[1]);
        if (theMonth != dateObj[1]) return false;
        var theDay = parseInt(dateObj[2]);
        if (theDay != dateObj[2]) return false;
        if (new Date(theYear + 1911, 1, 29).getDate() === 29) { // 是否為閏年?
            limitInMonth[1] = 29;
        }
        return theDay <= limitInMonth[theMonth - 1];
    },
    //加入千分位 s20231014
    numberComma(num){
       let comma = /\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g
        return num.toString().replace(comma, ',')
    }
}

function padLeftZero(str) {
    return ('00' + str).substr(str.length)
}