

function renderPos(content, path) {

	var current = content;
	console.log("current path", path);
	for (var i = 0; i < path.length; i++) {
		if (!current || path[path.length - i - 1] > current.children.length) break;
		current = current.children[path[path.length - i - 1]] 
		if (!current) return;
	}
	return renderRedBall(current);
}

function renderRedBall(target) {
	var redBall = document.createElement("div");
	redBall.innerHTML = "&#128308;"
	target.append(redBall);
	target.tempDisplay = target.style["display"];
	target.style["display"] = "flex";
	target.pathTag = true;
	redBall.ball = true;
	return redBall;
}
var pos;
function renderBallSetted() {
	$.ajax({
		url: "../AlertSetting/GetCurrentSettingPos",
		type: "post",
		dataType: "html",
		data: {
			path: window.location.pathname
        },
		success: async (config) => {
			var resp = JSON.parse(config);
			console.log("resp", resp);
			if (resp != null)
				renderBallEle = renderPos(document.getElementById("content"), JSON.parse(resp));
			IndexPathGlobal = JSON.parse(resp);
			pos = resp;
		}

	})
}

function revertRedBall(target) {
	if (!target || !target.ball) return;
	console.log("revert");
	var current = target;
	if (target.parentNode)
		target.parentNode.style["display"] = target.parentNode.tempDisplay;
	while (current && current.id != "content" ) {

		current.pathTag = false;
		current = current.parentNode;
	}
	target.remove();

}

function confirmBall() {
	var arr = [];
	arr.concat(IndexPathGlobal);
	console.log("confirmBall", IndexPathGlobal);
	if (confirm("確定要設定此路徑?")) {
		$.ajax({
			url: "../AlertSetting/SetPos",
			type: "post",
			dataType: "html",
			data: {
				pos: JSON.stringify(IndexPathGlobal),
				menuPath: window.location.pathname
				//temp : reset == null 
			},
			success: async (config) => {
				if (config == "true") {
					window.location.href =
						window.location.origin +"/AlertSetting";
                }


			}

		});

	}
}

function add() {
	var list = document.getElementById("list");
	var div = document.createElement("div");
	div.innerHTML = "11111";
	list.appendChild(div);

}
var lastTarget;

function createPosPathHanlder(content ) {

	
	//var ele = Array.apply(null, content.querySelectorAll('button, a, input, textarea, select'))
	content.addEventListener("click", (e) => {
		e.preventDefault()
		if (e.target == lastTarget) {

			e.cancelBubble = true;
			if (e.stopPropagation) e.stopPropagation();

			var target;
			if (["A", "TEXTAREA", "INPUT", "SELECT"].includes(e.target.nodeName)) {
				var target = e.target.parentNode;
			}
			else {
				var target = e.target;
			}


			console.log("content", e.target);
			if (renderBallEle != target) {
				revertRedBall(renderBallEle);
			}

			if (target.ball) {
				confirmBall()
			}
			else if (!target.pathTag) {
				var IndexPath = [];
				current = target;
				while (current && current != content) {

					current.pathTag = true;
					var currentIndex = Array.prototype.indexOf.call(current.parentNode.children, current);
					IndexPath.push(currentIndex);
					current = current.parentNode;
				}

				var ball = renderRedBall(target)
				renderBallEle = ball;
				IndexPathGlobal = IndexPath;
			}

        }

		lastTarget = e.target;

	}, true)


}


document.addEventListener("click", (e) => {

	if (e.target.nodeName == "BUTTON" || e.target.nodeName == "A" || e.target.nodeName == "I") {
		console.log("alertHandler click")
		if (pos != null && !IndexPathGlobal.IsRedBallRender) {
			renderBallEle = renderPos(document.getElementById("content"), IndexPathGlobal);
			if (renderBallEle) IndexPathGlobal.IsRedBallRender = true;
        }


	}
});


document.addEventListener('DOMContentLoaded', async function () {

	var ele = Array.apply(null, document.getElementById("content").querySelectorAll('a, input, select, textarea'))
	ele.forEach(e => e.disabled = false);
	console.log("___content", document.getElementById("content"));
	createPosPathHanlder(document.getElementById("content"));
	resolveAfterMSeconds(3000);
	renderBallSetted()
})

var IndexPathGlobal = [];
var renderBallEle = null;
