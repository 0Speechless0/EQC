export function download2(data, fileName, mime )
{
  var eleLink = document.createElement('a');

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
}