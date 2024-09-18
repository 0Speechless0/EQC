

var videoMap = {};

function removeFrame(id) {
	var modal = document.getElementById(id);
	var videoFrame = modal.querySelector(`iframe`);
	if (videoFrame)
		videoFrame.remove();
	openModalSet.forEach(e => {
		var originModal = document.getElementById(e);
		/*	$(originModal).modal("hide");*/
		$(originModal).modal("show");
	})
}

function handleFrame(id) {

	var modal = document.getElementById(id);
	var modal_body = modal.querySelector(`#${id}modalBody`);
	var modal_iframe = modal.querySelector("iframe");
	if (modal_iframe)
		modal_iframe.remove();

	var nIframe = document.createElement("iframe");
	nIframe.id = `${id}videoFrame`;
	modal_body.appendChild(nIframe)
	nIframe.outerHTML = videoMap[id];


}

function renderElement(target, item) {
	var redBall = document.createElement("div");
	var itemNo = item.No.replaceAll(".", "").trim();
	var clickModalId = "alert" + itemNo;

	videoIFrameStr = item.VideoUrl != null ? `
	<iframe  style="width:100%; height: 350px;" src="${item.VideoUrl}" 
	title="系統提示1_3" 
	frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen>
	</iframe>
	` : "";
	videoMap[clickModalId] = videoIFrameStr;
	redBall.innerHTML = `
		<button title="提示" type="button" class="btn btn-sm bg-white ml-4 mr-4" data-toggle="modal" data-target="#alert${itemNo}" onclick="handleFrame('${clickModalId}')" >
			<i style="color: red;" class="fas fa-lightbulb" ></i>
		</button>
        <div class="modal fade" id="alert${itemNo}" data-backdrop="static" data-keyboard="false" tabindex="-1"
            aria-labelledby="staticBackdropLabel" aria-hidden="true" style="margin-top:0px">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header pt-0" >
                        <h5 class="modal-title" id="staticBackdropLabel">提示</h5>
                    </div>
                    <div class="modal-body text-center" id="${clickModalId}modalBody">
                        <p class="text-left" style="color:black">
							${item.AlertMemo}
                        </p>
                    </div>
					<div class="modal-footer">
                        <button   title="我知道了" class="btn btn-color11-3 btn-xs mx-1 ml-auto" id='alertKnowBtn${itemNo}'>
                            <i class="fas fa-check"></i>
                            我知道了
                        </button>
                    </div>
                </div>
            </div>
        </div>
	`;

	target.append(redBall);
	target.tempDisplay = target.style["display"];
	target.style["display"] = "flex";
	document.getElementById(`alertKnowBtn${itemNo}`)
		.addEventListener("click", function (event) {
			$(document.getElementById(`alert${itemNo}`)).modal("hide");
			removeFrame(`${clickModalId}`);
			event.preventDefault();
		})

}



function renderPos(content, path, item) {

	var current = content;
	if (item.isRender) return;
	for (var i = 0; i < path.length; i++) {
		if (!current || path[path.length - i - 1] > current.children.length) break;
		current = current.children[path[path.length - i - 1]]
		if (!current) return;
	}
	console.log("current", current)

	renderElement(current, item);
	item.isRender = true;
}

$(window).on('show.bs.modal', function (e) {
	if (!e.target.id.startsWith("alert")) {
		openModalSet.add(e.target.id);
	}
	else {

		e.target.style["background"] = "#808080ab";
			
	}

	document.getElementsByTagName("body")[0].style["overflow"] = "hidden";


});

$(window).on('hide.bs.modal', function (e) {
	if (!e.target.id.startsWith("alert")) {
		openModalSet.delete(e.target.id);

	}

	if (openModalSet.size > 0) {
		document.getElementsByTagName("body")[0].style["overflow"] = "hidden";
		document.getElementById(`${Array.from(openModalSet).pop()}`).style["overflow"] = "auto";

	}
	else {
		document.getElementsByTagName("body")[0].style["overflow"] = "auto";
    }

});
//$(window).on('hide.bs.modal', function (e) {
//	if() {
//		document.getElementsByTagName("body")[0].style["overflow"] = "auto";
//	}

//});
var openModalSet = new Set([]);
var alertSettingPos = [];
async function createAlertRender(menuPath) {

	document.addEventListener("click", (e) => {

		if (e.target.nodeName == "BUTTON" || e.target.nodeName == "A" || e.target.nodeName == "I") {
			console.log("alertPosClick", alertSettingPos);
			alertSettingPos.forEach(ee => {
				renderPos(document.getElementById("content"), JSON.parse(ee.Position), ee);
			})
        }
	});

	document.addEventListener('DOMContentLoaded', async function () {
		await resolveAfterMSeconds(1000);
		$.ajax({
			url: "../AlertSetting/GetByMenuPath",
			type: "post",
			dataType: "html",
			data: {
				menuPath: window.location.pathname
			},
			success: async (config) => {
				var resp = JSON.parse(config);
				console.log("resp", resp);
				resp.forEach(e => {
					renderPos(document.getElementById("content"), JSON.parse(e.Position), e);
				})
				alertSettingPos = resp;

			}

		})


	})

}

