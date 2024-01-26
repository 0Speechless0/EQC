import moment from 'moment';
// Suppress the warnings
moment.suppressDeprecationWarnings = true;


export default {

    yearDiff: function(d1, d2) {
        d1 = new Date(d1);
        d2 = new Date(d2);
        return d2.getFullYear() - d1.getFullYear();
    },
    monthDiff : function (d1, d2) {
        d1 = new Date(d1);
        d2 = new Date(d2);
        var months;
        months = (d2.getFullYear() - d1.getFullYear()) * 12;
        months -= d1.getMonth();
        months += d2.getMonth();
        return months <= 0 ? 0 : months;
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
    download(data, fileName, mime)
    {
      var eleLink = document.createElement('a');

      eleLink.download = data.name ?? ( fileName ?? "" ) ;
      eleLink.style.display = 'none';
      // 字元內容轉變成blob地址
      var blob = new Blob([data],
        {
          type : data.type?? ( mime ?? "text/plain" ) 
        }  
        
      );
      eleLink.href = URL.createObjectURL(blob);
      // 觸發點選
      document.body.appendChild(eleLink);
      eleLink.setAttribute('download', fileName);
      eleLink.click();
      // 然後移除
      document.body.removeChild(eleLink);
    },
    download2(data, fileName, mime )
    {
      var eleLink = document.createElement('a');
        console.log("fileData", data);
      eleLink.download = data.name ?? ( fileName ?? "" ) ;
      eleLink.style.display = 'none';
      // 字元內容轉變成blob地址

      var blob = new Blob([data], {type:mime ?? "text/plain" });
      eleLink.href = URL.createObjectURL(blob);
      // 觸發點選
      document.body.appendChild(eleLink);
      eleLink.setAttribute('download', fileName);
      eleLink.click();
      // 然後移除
      document.body.removeChild(eleLink);
    },
    dnFile(action) {
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
                      link.addEventListener("click", function(event){
                        event.preventDefault()
                        });
                    //   link.click();
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
    async readFiles(inputEvent, handler)
    {
        var files = inputEvent.target.files;
        console.log(files);
        var reader = new FileReader();
        // reader.onload = handler;
        
        for(var i =0 ; i< files.length; i ++)
        {            
            let text = await files[i].text();
            handler(text, files[i].name);

        }

    },
    async readGroupFiles(inputEvent, fileGroup, handler)
    {
        var files = inputEvent.target.files;
        console.log(files);
        var reader = new FileReader();
        // reader.onload = handler;
        
        for(var i =0 ; i< files.length; i ++)
        {            
            let text = await files[i].text();
            handler(text, files[i].name, fileGroup);

        }

    },
    htmlDecode(str)
    {
        var div = document.createElement('div');
        div.innerHTML = str;
        return div.innerText|| div.textContent;
    },
    getCookie(cookieName) {
        var cookieValue = document.cookie;
        var cookieStart = cookieValue.indexOf(" " + cookieName + "=");
        if (cookieStart == -1) {
            cookieStart = cookieValue.indexOf("=");
        }
        if (cookieStart == -1) {
            cookieValue = null;
        }
        else {
            cookieStart = cookieValue.indexOf("=", cookieStart) + 1;
            var cookieEnd = cookieValue.indexOf(";", cookieStart);
            if (cookieEnd == -1) {
                cookieEnd = cookieValue.length;
            }
            cookieValue = unescape(cookieValue.substring(cookieStart, cookieEnd));
        }
        return cookieValue;
    }
}


function padLeftZero(str) {
    return ('00' + str).substr(str.length)
}
