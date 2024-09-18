export default class Canvas {

        constructor(_canvas, _img) {
            this.canvas = _canvas;
            this.ctx = this.canvas.getContext("2d");
            this.img = _img;
              // ref: https://www.zhihu.com/question/37698502
            let width = 400;//this.canvas.width,
            let height = 200;//this.canvas.height;
            if (window.devicePixelRatio) {
                this.canvas.style.width = width + "px";
                this.canvas.style.height = height + "px";
                this.canvas.height = height * window.devicePixelRatio;
                this.canvas.width = width * window.devicePixelRatio;
                this.ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
            }
            this.canvas.getMousePos = this.getMousePos;
            this.canvas.mouseMove = this.mouseMove;
            this.canvas.touchMove = this.touchMove;
            this.canvas.ctx = this.ctx;
            this.canvas.addEventListener('mousedown', function (evt) {
                var mousePos = evt.currentTarget.getMousePos(evt.currentTarget.mouseMove, evt);
                evt.currentTarget.ctx.beginPath();
                evt.currentTarget.ctx.moveTo(mousePos.x, mousePos.y);
                evt.preventDefault();
                evt.currentTarget.addEventListener('mousemove', evt.currentTarget.mouseMove, false);
            });

            this.canvas.addEventListener('mouseup', function (evt) {
                evt.currentTarget.removeEventListener('mousemove', evt.currentTarget.mouseMove, false);
            }, false);
            this.canvas.addEventListener('touchstart', function (evt) {
                // console.log('touchstart')
                // console.log(evt)
                var touchPos = this.getTouchPos(this.canvas, evt);
                evt.currentTarget.ctx.beginPath(touchPos.x, touchPos.y);
                evt.currentTarget.ctx.moveTo(touchPos.x, touchPos.y);
                evt.preventDefault();
                evt.currentTarget.addEventListener('touchmove', evt.currentTarget.touchMove, false);
            });

            this.canvas.addEventListener('touchend', function (evt) {
                // console.log("touchend")
                evt.currentTarget.removeEventListener('touchmove', evt.currentTarget.touchMove, false);
            }, false);



            this.Signature = "";
            return this
        }
        // mouse
        getMousePos(canvas, evt) {
            var rect = evt.currentTarget.getBoundingClientRect();
            return {
                x: evt.clientX - rect.left,
                y: evt.clientY - rect.top
            };
        }
    
        mouseMove(evt) {
            var mousePos = evt.currentTarget.getMousePos(evt.currentTarget, evt);
            evt.currentTarget.ctx.lineCap = "round";
            evt.currentTarget.ctx.lineWidth = 2;
            evt.currentTarget.ctx.lineJoin = "round";
            evt.currentTarget.ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
            evt.currentTarget.ctx.shadowColor = 'black';// 邊緣顏色
            evt.currentTarget.ctx.lineTo(mousePos.x, mousePos.y);
            evt.currentTarget.ctx.stroke();
        }
        // touch
        getTouchPos(canvas, evt) {
            var rect = this.canvas.getBoundingClientRect();
            return {
                x: evt.touches[0].clientX - rect.left,
                y: evt.touches[0].clientY - rect.top
            };
        }
    
        touchMove(evt) {
            // console.log("touchmove")
            var touchPos = this.getTouchPos(this.canvas, evt);
            // console.log(touchPos.x, touchPos.y)
    
            this.this.ctx.lineWidth = 2;
            this.this.ctx.lineCap = "round"; // 繪制圓形的結束線帽
            this.this.ctx.lineJoin = "round"; // 兩條線條交匯時，建立圓形邊角
            this.this.ctx.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒 
            this.this.ctx.shadowColor = 'black'; // 邊緣顏色
            this.this.ctx.lineTo(touchPos.x, touchPos.y);
            this.this.ctx.stroke();
        }
        clear()
        {
            this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        }
        onConvertToImageClick(id) {
            var image = this.canvas.toDataURL("image/png");
            this.img.innerHTML = "<img src='" + image + "' style='width: 400px; height: 200px;' />";
            this.Signature = image;

        }
        setImage(base64) {
            this.img.innerHTML = "<img src='" + base64 + "' style='width: 400px; height: 200px;' />";
            this.Signature = base64 ?? "";

        }
        
}

