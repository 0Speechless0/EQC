import config from "./captureHTMLConfig.js";
console.log("config load", config)
document.addEventListener('DOMContentLoaded', async function () {

        console.log("contentLoad")
        window.content = document.getElementById(config.contentId);

        window.sessionStorage.setItem("lastLocation", window.location.pathname);

    window.content.addEventListener("mousedown", caputreOnClickCheck, true)
        await resolveAfterMSeconds(config.waitLoading);
        //await window.captureHTML(window.content, true);
        captureInputInit(window.content);
 

        //DomObserver.observe(window.content, {
        //    subtree: true,
        //    childList: true,
        //    attributes: false
        //    //...
        //});

}, false);

MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
var lastHTML;
var clickId;
var clickButtonId;
var DialogHtmlStack = [];
var openDialogId;
var lastOpenDialogId;
var lastCaptureInitHTMLDOM = document.createElement("div");
var captureInputIndex = 0;
var captureLock = false;
var OpeningDialogIds = new Set([]);
var InputClickedSet = new Set([]);
var InputCapturingSet = [];
var captureInputValue = [];
var clickedBtnId = [];
var changeLock = false;
var isCaptureReset = false;
var ModalOpenIndex = 0;
var renderTagNameArr = config.renderTags.split(",").map(e => e.toUpperCase().trim());
window.sessionStorage.setItem("lastLocation", "");

const lastHTMLDomArr = [];
const HTMLDomArr = []

function checkInputCapturingAllExisting(html) {
    console.log("InputCapturingSet", InputCapturingSet.length);
    if (InputCapturingSet.length == 0) return false;
    else {
        InputCapturingSet.forEach(e => {
            if (
                !html.includes(`id=${e.id}`)
            ) {
         
                return false;
            }
        })
    }
    return true;
}

async function caputreOnClickCheck(e) {

    clickId = e.target.id;

    var buttonParent = ancestorCheck(e.target, renderTagNameArr);
    var dialog = ancestorDialogCheck(e.target);
    console.log("click ID", clickId, e.target.nodeName, buttonParent)
    if (buttonParent!= null)
        clickButtonId = buttonParent.id;
    if (dialog) {

        if (dialog.id == "")
            dialog.id = "captureDialog" + ModalOpenIndex++
        /*                dialog.style["display"] = "block";*/
        if (!OpeningDialogIds.has(dialog.id) && (
            dialog.style["display"] == "block"
            || dialog.offsetParent != null
        )) {
            console.log("modal " + dialog.id + " open detect")
            if (dialog.offsetParent == null) {
                dialog.dialogType = 0;
            }
            else {
                dialog.dialogType = 1;
            }
            InputClickedSet.clear();

            OpeningDialogIds.add(dialog.id);

            lastOpenDialogId = openDialogId;
            openDialogId = dialog.id;
        }
    }
    else {
        console.log("restore clear DialogHtmlStack ")
        DialogHtmlStack = [];
    }
    //if (buttonParent != null) {
    //    clickId = buttonParent.id
    //    if (!isCaptureReset) {
    //        console.log("Capture Reset by click");
    //        await window.captureHTML(window.content, true);
    //    }

    //}
    console.log("e.target.id", clickId, e.target.nodeName);

    if (
        !lastCaptureInitHTMLDOM.innerHTML.includes(`id="${clickId}"`)
        && (clickId != "" )
    ) { 
        console.log("detect input not exist last")
        if (
            //InputClickedSet: 上次擷取後有變更的目標
            //InputClickedSet中皆不存在於  window.content.innerHTML 時進入
            (InputClickedSet.size == 0 ||
            InputClickedSet.reduce((a, c) => {
                if (!a.includes(`id="${c}"`)) return "";
                else return a;
            }, window.content.innerHTML) == "" )
        ) {
            console.log("detect last changed Input not exist or no changed input", InputClickedSet)
            InputClickedSet.clear();

            if (dialog) {
                await window.captureHTML(dialog, true);
            }
            else {
                await window.captureHTML(window.content, true);
            }
            
            //if (dialog) {
            //    lastCaptureInitHTMLDOM.innerHTML = fillHtmlInput(dialog, true)
            //    console.log("dialog.outerHTML", dialog.outerHTML)
            //}
            //console.log("reset lastCaptureInitHTML", window.content.innerHTML);
        }
  
    }
    else if (
        window.sessionStorage.getItem("lastLocation") != window.location.pathname
    ) {
        await window.captureHTML(window.content, true);

    }

    //所有可編輯內容的HTML元素
    if (config.inputTags.includes(e.target.nodeName.toLowerCase())
    ) {
    
        if (e.target.id != "")
            InputClickedSet.add(e.target.id);
        console.log("detect: add changeInputSet", InputClickedSet);
    }

    await resolveAfterMSeconds(1500);

    if(!captureLock) {
        /*   if (captureDone) await window.captureHTML(window.content.innerHTML, true);*/
        if (buttonParent != null) {
            console.log("click possiable trigger", buttonParent.id)

            var contentHasDone = false;
            if (!removeNoneDisplay(window.content.innerHTML).includes("id=" + buttonParent.id) ) {
                contentHasDone = true;
            }
            console.log("contentHasDone", contentHasDone, buttonParent);
            if (dialog) { 
                await window.captureHTML(dialog, false, contentHasDone);
                //await resolveAfterMSeconds(1500);
                //await window.captureHTML(dialog, true);
            }
            else {
                await window.captureHTML(window.content, false, contentHasDone);
                //await resolveAfterMSeconds(1500);
                //await window.captureHTML(window.content, true);
            }

            
        }
        if (dialog) {

            if (
                OpeningDialogIds.has(dialog.id) &&
                (
                    (dialog.dialogType == 0 && dialog.style["display"] == "none") ||
                    (dialog.dialogType == 1 && dialog.offsetParent == null)
                )
            ) {
                console.log("modal " + dialog.id + " close detect")
                InputClickedSet.clear();

                OpeningDialogIds.delete(dialog.id);
                if (OpeningDialogIds.size == 0) {
                    openDialogId = null;
                }
                else {
                    lastCaptureInitHTMLDOM.innerHTML = DialogHtmlStack[lastOpenDialogId];
                }

            }

        }


    }
    window.sessionStorage.setItem("lastLocation", window.location.pathname);
}
function createDomEleExistingMap(dom, DomEleExistingMap) {
    Array.apply(null, dom.children).forEach((child, i) => {
        if (child.children.length == 0) {
            DomEleExistingMap.set(child.id, child);
            return;
        }
        DomEleExistingMap.set(child.id, child);
        createDomEleExistingMap(child, DomEleExistingMap)
    })
}

function createMarkedDomElementDiffHTML(dom, lastDom) {
    var currentDomExistingMap = new Map([]);
    var currentDomTempExistingMap = new Map([]);
    var lastDomExistingMap = new Map([]);
    var resultDom = document.createElement("div");
    //resultDom.innerHTML = dom.innerHTML;
    resultDom = cloneDomWithClass(dom);
    createDomEleExistingMap(resultDom, currentDomExistingMap)

    createDomEleExistingMap(lastDom, lastDomExistingMap)
    console.log("lastDomExistingMap", lastDomExistingMap)
    currentDomExistingMap
        .forEach((value, key) => {
            if (config.DiffIgnoreTag.includes(value.nodeName)) return;
            if (!lastDomExistingMap.has(key)) {
                value.style["color"] = "blue";
                value.style["background"] = "lightblue";
            }
            else if (["SELECT", "INPUT", "TEXTAREA"].includes(value.nodeName)) {
                
                if (value.value != lastDomExistingMap.get(key).value ||
                    value.innerHTML != lastDomExistingMap.get(key).innerHTML
                ) {

                    value.style["color"] = "green";
                    value.style["background"] = "lightgreen";

                    var originValueDiv = document.createElement("div");
                    //value.parentNode.className += ' d-flex'
                    value.parentNode.insertBefore(originValueDiv, value.nextSibling)
                    lastDomExistingMap.get(key).outerHTML
                    originValueDiv.innerHTML = lastDomExistingMap.get(key).outerHTML;

                    originValueDiv.children[0].style["color"] = 'orange';
                    originValueDiv.style["color"] = 'orange';
                    originValueDiv.children[0].style["background"] = 'lightyellow';
                    originValueDiv.innerHTML = "&#8592;" + originValueDiv.innerHTML;

              
            


                }
                else if (lastDomExistingMap.get(key).value.checked != null &&  value.checked != lastDomExistingMap.get(key).value.checked) {
                    value.parentNode.style["color"] = "green";
                    value.parentNode.style["background"] = "lightgreen";
                }

            }


        })
    //lastDomExistingMap
    //    .forEach((value, key) => {
    //        if (config.DiffIgnoreTag.includes(value.nodeName)) return;
    //        if (!currentDomExistingMap.has(key)) {
    //            var parenteKey = value.parentNode.id;
                
    //            var nValue = document.createElement(value.nodeName);


    //            if (currentDomTempExistingMap.has(parenteKey)) {
    //                currentDomTempExistingMap.get(parenteKey).appendChild(nValue)
    //            }
    //            else if(currentDomExistingMap.has(parenteKey)) {
    //                currentDomExistingMap.get(parenteKey).appendChild(nValue);

    //            }
    //            if (value.children.length == 0) {
    //                nValue.innerHTML = value.innerHTML;
    //            }
    //            nValue.style["color"] = "red";
    //            nValue.style["background"] = "pink";
    //            currentDomTempExistingMap.set(key, nValue);

    //        }


    //    })
    return resultDom.innerHTML;

}


function RenderClickPos(html) {
    var dom = document.createElement("div");
    dom.innerHTML = html;
    console.log("draw circle on " + clickButtonId)
    if (clickButtonId && clickButtonId != "" ) {

        var pos = dom.querySelector(`#${clickButtonId}`);
        
        if (pos != null) {
            pos.className += " d-flex";
            var div = document.createElement(`div`);
            div.style["margin-left"] = "-20px";
            div.innerHTML = config.clickMark;
            pos.appendChild(div);
        }
            
    }

    return dom.innerHTML;
}

function RenderHtmlDiff(html) {
    console.log("render diff");
    var dom = document.createElement("div");
    dom.innerHTML = html;
    //所有ID需唯一
    var inputs = dom.querySelectorAll(config.inputTags);
    console.log(captureInputValue);
    for (var i = 0; i < inputs.length; i++) {
        if (captureInputValue[inputs[i].id] != inputs[i].value) {

            inputs[i].style["color"] = "red";
        }

    }

    return dom.innerHTML;
}


function DomStructCompare(htmlA, htmlB) {
    var domA = document.createElement("div");
    domA.innerHTML = htmlA;
    var domB = document.createElement("div");
    domB.innerHTML = htmlB;
   return  domA.querySelectorAll("*").length == domB.querySelectorAll("*").length
}

function ancestorDialogCheck(target) {
    var current = target;
    if (current == null)
        return null
    while (current != null && current.id != config.contentId) {
        if (typeof current.className == 'string') {
            if ( current.className.indexOf("modal ") != -1) return current;
        }
        current = current.parentNode;
    }
    return null;
}

function captureInputInit(htmlDom) {
    console.log("reset caputureId");
    var inputs = htmlDom.querySelectorAll(config.inputTags + "," + config.renderTags);
    console.log(inputs.length)

    InputCapturingSet = [];

    for (var i = 0; i < inputs.length; i++) {

        if (inputs[i].id == "")
            inputs[i].id = "capture" +  (++captureInputIndex);
        if (!inputs[i].changeListenerInit) {
            inputs[i].addEventListener("change", async (e) => {
                var dialog = ancestorDialogCheck(e.target);

                //await resolveAfterMSeconds(500);
                if (dialog) {
                    await window.captureHTML(dialog, false)
                }
                else {
                    await window.captureHTML(window.content, false)
                }

                console.log("change", e.target);
            })

        }

        InputCapturingSet.push(inputs[i]);
        inputs[i].changeListenerInit = true;


    }
    //inputs = htmlDom.querySelectorAll("*");
    //for (var i = 0; i < inputs.length; i++) {
    //    if (inputs[i].id == "")
    //        inputs[i].id = "normal" + (++captureInputIndex);
    //}
}
function DialogTempHTMLShow(html) {
    var tempDom = document.createElement("div");
    tempDom.innerHTML = html;

    // 彈跳視窗selector定義
    var dialogs = tempDom.querySelectorAll(".modal, div[role='dialog']")
    for (var i = 0; i < dialogs.length; i++) {
        if ( OpeningDialogIds.has(dialogs[i].id )) {
            dialogs[i].style["display"] = "block";
            dialogs[i].className = "";
            tempDom.prepend(dialogs[i]);
        }
        else {
            dialogs[i].style["display"] = "none";
        }

    }
    return tempDom.innerHTML
}

function removeNoneDisplay(html) {
    var dom = document.createElement("div");
    dom.innerHTML = html;
    var ele = dom.querySelectorAll('*')

    for (var i = 0; i < ele.length; i++) {

        if (ele[i].style["display"] == 'none' ) {
            ele[i].remove();
        }

    }
    // 彈跳視窗selector定義
    var dialogs = dom.querySelectorAll(".modal, div[role='dialog']")
    for (var i = 0; i < dialogs.length; i++) {
        if (!OpeningDialogIds.has(dialogs[i].id)) {
            dialogs[i].remove();
        }

    }


    return dom.innerHTML;
}    


function resolveAfterMSeconds(msec) {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve();
        }, msec);
    });
}

function ancestorCheckStyle(target, styleName, styleValue) {
    var current = target
    while (current != null && current.id != config.contentId) {

        if (current.style[styleName] == styleValue) return true;

        current = current.parentNode;

    }
}
function ancestorCheck(target, tagName)
{
    var current = target
    
    while (current != null) {
        var next = tagName.reduce((cur, tag) => {
            if (cur && cur.nodeName == tag) return false;
            else return cur;
        }, current);
        if (next)
            current = current.parentNode;
        else
            break;
    }
    console.log("trigger",current)
    return current ;
}

function fillHtmlInput(htmlDom, isOuter = false) {
    var inputs = htmlDom.querySelectorAll(config.inputTags );

    var tmpDom = document.createElement("div");
    tmpDom.innerHTML = isOuter ? htmlDom.outerHTML : htmlDom.innerHTML;
    var dist_inputs = tmpDom.querySelectorAll(config.inputTags);
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].nodeName == "SELECT") {

            try {
                var dist_input = tmpDom.querySelector(`#${inputs[i].id}`)
            }
            catch {
                continue;
            }

            var dist_options = dist_input.querySelectorAll("option")
            var optionsHtml = `<option selected="selected"> </option>`;
            for (var j = 0; j < dist_options.length; j++) {
                if (inputs[i].value == dist_options[j].value) {
                    optionsHtml  += dist_options[j].outerHTML.replace("<option", `<option selected="selected"`)
                    
                }
                else {
                    optionsHtml += dist_options[j].outerHTML.replace(`selected="selected"`, ``)
                }
            }
            dist_input.innerHTML = optionsHtml
            console.log("select", dist_input)

        }
        else if (inputs[i].nodeName == "TEXTAREA") {
            dist_inputs[i].innerHTML = inputs[i].value
        }
        else if (inputs[i].checked) {
     
            dist_inputs[i].outerHTML = inputs[i].outerHTML.replace("<input", `<input value="" checked`)
        }
        else {
            dist_inputs[i].outerHTML = inputs[i].outerHTML.replace("<input", `<input value="${inputs[i].value}" `)
        }

    }
    return tmpDom.innerHTML;
}

function cloneDomWithClass(htmlDom) {
    var inputs = htmlDom.querySelectorAll(config.inputTags);

    var tmpDom = document.createElement("div");
    tmpDom.innerHTML = htmlDom.innerHTML;

    for (var i = 0; i < inputs.length; i++) {
        var dist_inputs = tmpDom.querySelectorAll(config.inputTags);
        //if (inputs[i].nodeName == "SELECT") {

        //    var options = inputs[i].querySelectorAll("option")
        //    for (var j = 0; j < options.length; j++) {
        //        if (options[j].selected) {
        //            options[j].selected = true
        //        }
        //        else {
        //            options[j].selected =  false
        //        }
        //    }
        //    dist_inputs[i].innerHTML = inputs[i].innerHTML
        //}
        //if (inputs[i].nodeName == "TEXTAREA") {
        //    dist_inputs[i].innerHTML = inputs[i].value
        //}
        //else if (inputs[i].checked) {
        //    dist_inputs[i].checked = true;
        //}
        //else {
        //    dist_inputs[i].value = inputs[i].value;
        //}
/*        dist_inputs[i].outerHTML = inputs[i].className;*/
    }

    return tmpDom;
}


//var DomObserver = new MutationObserver(async function (mutations, observer) {
//    DomChangeBeforceTriggerLanuch = true;
//    wi
//});



//由於調整， reset =true 的時候 dom的內容已經不重要
window.captureHTML = async function captureHTML(dom, reset = false, contentHasDone = false) {

    captureInputInit(window.content);
    var html;

    if (DialogHtmlStack[openDialogId] && reset) {

        var html = DialogHtmlStack[openDialogId];
    }

    html = fillHtmlInput(dom);

    html = removeNoneDisplay(html);
    html = DialogTempHTMLShow(html)

    /*       var domSame = DomStructCompare(lastCaptureInitHTMLDOM.innerHTML, html);*/
    var usehtml = contentHasDone ? lastHTML : html;
    
    if (lastCaptureInitHTMLDOM.innerHTML != usehtml || reset || contentHasDone) {

        var resultHTML = usehtml;

        if (openDialogId && !DialogHtmlStack[openDialogId] && reset) {
            console.log("set dialogHTML", openDialogId);
            DialogHtmlStack[openDialogId] = html;
        }






        if (!reset) {
            var tempResultDom = document.createElement("div");
            tempResultDom.innerHTML = resultHTML;
            var resultHTML =
                createMarkedDomElementDiffHTML(tempResultDom, lastCaptureInitHTMLDOM)

        }




        resultHTML = RenderClickPos(resultHTML);

        console.log("resultHTML", resultHTML)
        var a = btoa(encodeURIComponent(resultHTML));

        //captureLock = true;
        if (reset == null)
            console.log("inint caputre by button click");

        if (!reset) {
            await $.ajax({
                url: config.apiURL,
                type: "post",
                dataType: "html",
                data: {
                    HTMLEncode: a,
                    reset: reset,
                    //temp : reset == null 
                },
                success: async (config) => {
                    captureLock = false;
                    console.log("action: capture", contentHasDone)
                }

            });
        }
        else {
            lastCaptureInitHTMLDOM.innerHTML = html;
            console.log("action : reset")
            lastCaptureInitHTMLDOM.innerHTML
                = DialogHtmlStack[openDialogId]
                    ? DialogHtmlStack[openDialogId] : lastCaptureInitHTMLDOM.innerHTML;
            if (!openDialogId)
                DialogHtmlStack[""] = lastCaptureInitHTMLDOM.innerHTML;
            if (!isCaptureReset)
                isCaptureReset = true;
        }
        lastHTML = html;
    }

}
