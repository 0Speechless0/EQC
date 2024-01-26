<template>
    <div class="modal fade show" id="MyDialog" ref="MyDialog" style="overflow:auto;background:rgb(0 0 0 / 50%)" v-bind:style="{display: modalShow ? 'block' : 'none'}" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document" style="max-width:none; width:80%">
            <div class="modal-content">
                <div class="modal-header bg-R text-white">
                    <h6 class="modal-title">線上編輯流程圖</h6>
                </div>
                <div class="modal-body">
                    <h3 class="text-center"> {{title }}</h3>
                    <div class="d-flex bd-highlight mb-3">
                      <div class=" p-2 bd-highlight" v-show="isEdit">
                          <button type="button " class="btn btn-color11-2 btn-xs m-1" @click="clearFlowChart()" >重置 </button>
                      </div>
                      <div class="p-2 bd-highlight" v-show="!isEdit">
                          <button type="button" class="btn btn-color11-3 btn-xs m-1" @click="download()" ><i class="fas fa-save"></i> 下載圖檔(jpg)</button>
                      </div>
                      <div class="p-2 bd-highlight" v-show="!isEdit">
                          <button type="button" class="btn btn-color11-3 btn-xs m-1" @click="exportTemplate()" ><i class="fas fa-save"></i> 下載範本(json)</button>
                      </div>
                      <div class="p-2 bd-highlight" v-show="!isEdit">
                        <span  class="file btn btn-lg  upload-div" style="background-color:gray;color:white">
                            <span class="upload-text">匯入範本(json) </span>
                                                <input type="file" id="file_upload_input" class="upload form-control-file" name="file" @change="importTemplate"   />
                        </span>
                      </div>

                      <div class="mr-auto p-2 bd-highlight" v-show="!isEdit">
                          <button type="button" class="btn btn-color11-2 btn-xs m-1" @click="onChangeEdit()"><i class="fas fa-pencil-alt"></i> 編輯</button>
                      </div>
                      <div class="mr-auto p-2 bd-highlight" v-show="isEdit">
                          <button type="button" class="btn btn-color11-3 btn-xs m-1" @click="storeFlowChartTpDiagram()"><i class="fas fa-save"></i> 儲存</button>
                      </div>

                      <div class="p-2 bd-highlight" >
                        <button type="button" class="btn btn-color9-1 btn-xs mx-1 m-1" data-dismiss="modal" v-on:click="handleModalShow()"><i class="fas fa-times"></i> 關閉</button>
                      </div>
                          
                    </div>
                <div style="width: 100%; display: flex; justify-content: space-between">
                    <div id="myPaletteDiv" v-show="isEdit"
                    style="width: 120px; background-color: white; position: relative; -webkit-tap-highlight-color: rgba(255, 255, 255, 0);border :solid ; border-color: lightgray;">
                    <canvas tabindex="0" width="100" height="750"
                        style="position: absolute; top: 0px; left: 0px; z-index: 2; user-select: none; touch-action: none; width: 100px; height: 750px;">This
                        text is displayed if your browser does not support the Canvas HTML element.</canvas>
                    <div style="position: absolute; overflow: auto; width: 100px; height: 750px; z-index: 1;">
                        <div style="position: absolute; width: 1px; height: 1px;"></div>
                    </div>
                    </div>
                    <div :id="'myDiagramDiv'+this.type" 
                    style="flex-grow: 1; height: 750px; background-color: white; position: relative; -webkit-tap-highlight-color: rgba(255, 255, 255, 0); border :solid ; border-color: lightgray;">
                    <canvas tabindex="0" width="746" height="750"
                        style="position: absolute; top: 0px; left: 0px; z-index: 2; user-select: none; touch-action: none; width: 746px; height: 750px;" >This
                        text is displayed if your browser does not support the Canvas HTML element.</canvas>
                    <div style="position: absolute; overflow: auto; width: 746px; height: 750px; z-index: 1;">
                        <div style="position: absolute; width: 1px; height: 1px;"></div>
                    </div>
                    </div>
                </div>
                </div>
                <textarea id="mySavedModel"  hidden></textarea>

            </div>
        </div>
    </div>

</template>

<script>
/* eslint-disable */
import go from "gojs";

let myDiagram ;
let myPalette ;
let type;
function init() {


    if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this

    // Since 2.2 you can also author concise templates with method chaining instead of GraphObject.make
    // For details, see https://gojs.net/latest/intro/buildingObjects.html
    const $ = go.GraphObject.make;  // for conciseness in defining templates
    myDiagram =
      $(go.Diagram, "myDiagramDiv"+type,  // must name or refer to the DIV HTML element
        {
          "LinkDrawn": showLinkLabel,  // this DiagramEvent listener is defined below
          "LinkRelinked": showLinkLabel,
          "undoManager.isEnabled": true  // enable undo & redo
        });


    // when the document is modified, add a "*" to the title and enable the "Save" button
    myDiagram.addDiagramListener("Modified", e => {
      var button = document.getElementById("SaveButton");
      if (button) button.disabled = !myDiagram.isModified;
      var idx = document.title.indexOf("*");
      if (myDiagram.isModified) {
        if (idx < 0) document.title += "*";
      } else {
        if (idx >= 0) document.title = document.title.slice(0, idx);
      }

      
    });
    //定義新的開始的圖的形狀
      go.Shape.defineFigureGenerator("CustomerRoundedRectangle", function(shape, w, h) {
        // this figure takes one parameter, the size of the corner
        var p1 = 250; // default corner size

        p1 = Math.min(p1, w / 2);
        p1 = Math.min(p1, h / 2); // limit by whole height or by half height?
        var geo = new go.Geometry();
        // a single figure consisting of straight lines and quarter-circle arcs
        geo.add(new go.PathFigure(0, p1)
            .add(new go.PathSegment(go.PathSegment.Arc, 180, 90, p1, p1, p1, p1))
            .add(new go.PathSegment(go.PathSegment.Line, w - p1, 0))
            .add(new go.PathSegment(go.PathSegment.Arc, 270, 90, w - p1, p1, p1, p1))
            .add(new go.PathSegment(go.PathSegment.Arc, 0, 90, w - p1, h - p1, p1, p1))
            .add(new go.PathSegment(go.PathSegment.Line, p1, h))
            .add(new go.PathSegment(go.PathSegment.Arc, 90, 90, p1, h - p1, p1, p1))
            // .add(new go.PathSegment(go.PathSegment.Line, w, h))
            // .add(new go.PathSegment(go.PathSegment.Line, 0, h).close())
        );
        // don't intersect with two top corners when used in an "Auto" Panel
        geo.spot1 = new go.Spot(0, 0, 0.3 * p1, 0.3 * p1);
        geo.spot2 = new go.Spot(1, 1, -0.3 * p1, 0);
        return geo;
    });
    myDiagram.isReadOnly = true;
    // notice whenever a transaction or undo/redo has occurred

    // helper definitions for node templates

    function nodeStyle() {
      return [
        // The Node.location comes from the "loc" property of the node data,
        // converted by the Point.parse static method.
        // If the Node.location is changed, it updates the "loc" property of the node data,
        // converting back using the Point.stringify static method.
        new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
        {
          // the Node.location is at the center of each node
          locationSpot: go.Spot.Center,
        },
        
      ];
    }

    // Define a function for creating a "port" that is normally transparent.
    // The "name" is used as the GraphObject.portId,
    // the "align" is used to determine where to position the port relative to the body of the node,
    // the "spot" is used to control how links connect with the port and whether the port
    // stretches along the side of the node,
    // and the boolean "output" and "input" arguments control whether the user can draw links from or to the port.
    function makePort(name, align, spot, output, input) {
      var horizontal = align.equals(go.Spot.Top) || align.equals(go.Spot.Bottom);
      // the port is basically just a transparent rectangle that stretches along the side of the node,
      // and becomes colored when the mouse passes over it
      return $(go.Shape,
        {
          fill: "transparent",  // changed to a color in the mouseEnter event handler
          strokeWidth: 0,  // no stroke
          width: horizontal ? NaN : 8,  // if not stretching horizontally, just 8 wide
          height: !horizontal ? NaN : 8,  // if not stretching vertically, just 8 tall
          alignment: align,  // align the port on the main Shape
          stretch: (horizontal ? go.GraphObject.Horizontal : go.GraphObject.Vertical),
          portId: name,  // declare this object to be a "port"
          fromSpot: spot,  // declare where links may connect at this port
          fromLinkable: output,  // declare whether the user may draw links from here
          toSpot: spot,  // declare where links may connect at this port
          toLinkable: input,  // declare whether the user may draw links to here
          cursor: "pointer",  // show a different cursor to indicate potential link point
          mouseEnter: (e, port) => {  // the PORT argument will be this Shape
            if (!e.diagram.isReadOnly) port.fill = "rgba(255,0,255,0.5)";
          },
          mouseLeave: (e, port) => port.fill = "transparent"
        });
    }

    function textStyle() {
      return {
        font: "bold 11pt Lato, Helvetica, Arial, sans-serif",
        stroke: "black"
      }
    }

    // define the Node templates for regular nodes

    myDiagram.nodeTemplateMap.add("",  // the default category
      $(go.Node, "Spot", nodeStyle(),
        // the main object is a Panel that surrounds a TextBlock with a rectangular Shape
        $(go.Panel, "Auto",
          $(go.Shape, "Rectangle",
            { fill: "white", stroke: "black", strokeWidth: 3.5},
            new go.Binding("figure", "figure")),
          $(go.TextBlock, textStyle(),
            {
              margin: 8,
              maxSize: new go.Size(160, NaN),
              wrap: go.TextBlock.WrapFit,
              editable: true
            },
            new go.Binding("text").makeTwoWay())
        ),
        // four named ports, one on each side:
        makePort("T", go.Spot.Top, go.Spot.TopSide, false, true),
        makePort("L", go.Spot.Left, go.Spot.LeftSide, true, true),
        makePort("R", go.Spot.Right, go.Spot.RightSide, true, true),
        makePort("B", go.Spot.Bottom, go.Spot.BottomSide, true, false)
      ));

    myDiagram.nodeTemplateMap.add("Conditional",
      $(go.Node, "Table", nodeStyle(),
        // the main object is a Panel that surrounds a TextBlock with a rectangular Shape
        $(go.Panel, "Auto",
          $(go.Shape, "Diamond",
            { fill: "white", stroke: "black", strokeWidth: 3.5 },
            new go.Binding("figure", "figure")),
          $(go.TextBlock, textStyle(),
            {
              margin: 8,
              maxSize: new go.Size(160, NaN),
              wrap: go.TextBlock.WrapFit,
              editable: true
            },
            new go.Binding("text").makeTwoWay())
        ),
        // four named ports, one on each side:
        makePort("T", go.Spot.Top, go.Spot.Top, false, true),
        makePort("L", go.Spot.Left, go.Spot.Left, true, true),
        makePort("R", go.Spot.Right, go.Spot.Right, true, true),
        makePort("B", go.Spot.Bottom, go.Spot.Bottom, true, false)
      ));

    myDiagram.nodeTemplateMap.add("Start",
      $(go.Node, "Table", nodeStyle(),
        $(go.Panel, "Spot",
          $(go.Shape, "CustomerRoundedRectangle",
            { desiredSize: new go.Size(100, 50), fill: "white", stroke: "black", strokeWidth: 3.5 }),
          $(go.TextBlock, "Start", textStyle(),
            new go.Binding("text"))
        ),
        // three named ports, one on each side except the top, all output only:
        makePort("L", go.Spot.Left, go.Spot.Left, true, false),
        makePort("R", go.Spot.Right, go.Spot.Right, true, true),
        makePort("B", go.Spot.Bottom, go.Spot.Bottom, true, false)
      ));

    myDiagram.nodeTemplateMap.add("End",
      $(go.Node, "Table", nodeStyle(),
        $(go.Panel, "Spot",
          $(go.Shape, "CustomerRoundedRectangle",
            { desiredSize: new go.Size(100, 50), fill: "white", stroke: "black", strokeWidth: 3.5 }),
          $(go.TextBlock, "End", textStyle(),
            new go.Binding("text"))
        ),
        // three named ports, one on each side except the bottom, all input only:
        makePort("T", go.Spot.Top, go.Spot.Top, false, true),
        makePort("L", go.Spot.Left, go.Spot.Left, false, true),
        makePort("R", go.Spot.Right, go.Spot.Right, false, true)
      ));
    myDiagram.nodeTemplateMap.add("Pos",
      $(go.Node, "Position") );
    // taken from ../extensions/Figures.js:
    go.Shape.defineFigureGenerator("File", (shape, w, h) => {
      var geo = new go.Geometry();
      var fig = new go.PathFigure(0, 0, true); // starting point
      geo.add(fig);
      fig.add(new go.PathSegment(go.PathSegment.Line, .75 * w, 0));
      fig.add(new go.PathSegment(go.PathSegment.Line, w, .25 * h));
      fig.add(new go.PathSegment(go.PathSegment.Line, w, h));
      fig.add(new go.PathSegment(go.PathSegment.Line, 0, h).close());
      var fig2 = new go.PathFigure(.75 * w, 0, false);
      geo.add(fig2);
      // The Fold
      fig2.add(new go.PathSegment(go.PathSegment.Line, .75 * w, .25 * h));
      fig2.add(new go.PathSegment(go.PathSegment.Line, w, .25 * h));
      geo.spot1 = new go.Spot(0, .25);
      geo.spot2 = go.Spot.BottomRight;
      return geo;
    });

    myDiagram.nodeTemplateMap.add("Comment",
      $(go.Node, "Auto", nodeStyle(),
        $(go.Shape, "Rectangle",
          { fill: "white", stroke: "black", strokeWidth: 0 }),
        $(go.TextBlock, textStyle(),
          {
            margin: 8,
            wrap: go.TextBlock.WrapFit,
            textAlign: "left",
            editable: true
          },
          new go.Binding("text").makeTwoWay())
        // no ports, because no links are allowed to connect with a comment
      ));

      myDiagram.nodeTemplateMap.add("Star",
      $(go.Node, "Auto", nodeStyle(),
        $(go.Shape, "File",
          { fill: null, stroke: "black", strokeWidth: 0 }),
        $(go.TextBlock, 
        
          {
            font: "bold 22pt Lato, Helvetica, Arial, sans-serif",
            stroke: "black"
          }
        ,
          {
            margin: 8,
            maxSize: new go.Size(200, NaN),
            wrap: go.TextBlock.WrapFit,
            textAlign: "left",

            editable: true
          },
          new go.Binding("text").makeTwoWay())
        // no ports, because no links are allowed to connect with a comment
      ));
    // replace the default Link template in the linkTemplateMap
    myDiagram.linkTemplate =
      $(go.Link,  // the whole link panel
        {
          routing: go.Link.AvoidsNodes,
          curve: go.Link.JumpOver,
          corner: 5, toShortLength: 4,
          relinkableFrom: true,
          relinkableTo: true,
          reshapable: true,
          resegmentable: true,
          // mouse-overs subtly highlight links:
          mouseEnter: (e, link) => link.findObject("HIGHLIGHT").stroke = "rgba(30,144,255,0.2)",
          mouseLeave: (e, link) => link.findObject("HIGHLIGHT").stroke = "transparent",
          selectionAdorned: false
        },
        new go.Binding("points").makeTwoWay(),
        $(go.Shape,  // the highlight shape, normally transparent
          { isPanelMain: true, strokeWidth: 8, stroke: "transparent", name: "HIGHLIGHT" }),
        $(go.Shape,  // the link path shape
          { isPanelMain: true, stroke: "gray", strokeWidth: 2 },
          new go.Binding("stroke", "isSelected", sel => sel ? "dodgerblue" : "gray").ofObject()),
        $(go.Shape,  // the arrowhead
          { toArrow: "standard", strokeWidth: 0, fill: "gray" }),
        $(go.Panel, "Auto",  // the link label, normally not visible
          { visible: false, name: "LABEL", segmentIndex: 2, segmentFraction: 0.5 },
          new go.Binding("visible", "visible").makeTwoWay(),
          $(go.Shape, "RoundedRectangle",  // the label shape
            { fill: "#F8F8F8", strokeWidth: 0 }),
          $(go.TextBlock, "Yes",  // the label
            {
              textAlign: "center",
              font: "10pt helvetica, arial, sans-serif",
              stroke: "#333333",
              editable: true
            },
            new go.Binding("text").makeTwoWay())
        )
      );

    // Make link labels visible if coming out of a "conditional" node.
    // This listener is called by the "LinkDrawn" and "LinkRelinked" DiagramEvents.
    function showLinkLabel(e) {
      var label = e.subject.findObject("LABEL");
      if (label !== null) label.visible = (e.subject.fromNode.data.category === "Conditional");
    }

    // temporary links used by LinkingTool and RelinkingTool are also orthogonal:
      myDiagram.model.linkFromPortIdProperty = "fromPort";
      myDiagram.model.linkToPortIdProperty = "toPort";
    myDiagram.toolManager.linkingTool.temporaryLink.routing = go.Link.Orthogonal;
    myDiagram.toolManager.relinkingTool.temporaryLink.routing = go.Link.Orthogonal;



    // initialize the Palette that is on the left side of the page
    myPalette =
      $(go.Palette, "myPaletteDiv",  // must name or refer to the DIV HTML element
        {
          // Instead of the default animation, use a custom fade-down
          "animationManager.initialAnimationStyle": go.AnimationManager.None,
          "InitialAnimationStarting": animateFadeDown, // Instead, animate with this function

          nodeTemplateMap: myDiagram.nodeTemplateMap,  // share the templates used by myDiagram
          model: new go.GraphLinksModel([  // specify the contents of the Palette
            { category: "Pos", loc:"0 -20" },
            { category: "Start", text: "開始",loc:"0, 100" },
            { text: "Step",loc:"0, 200"  },
            { category: "Conditional", text: "???" ,loc:"0, 300" },
            { category: "End", text: "結束" ,loc:"0, 400" },
            { category: "Comment", text: "請輸入內容...",loc:"0, 500"  },
            { category: "Star",text:"☆", loc:"0, 600"  }
          ])
          ,
          layout: $(go.Layout)
        });

    // This is a re-implementation of the default animation, except it fades in from downwards, instead of upwards.
    function animateFadeDown(e) {
      var diagram = e.diagram;
      var animation = new go.Animation();
      animation.isViewportUnconstrained = true; // So Diagram positioning rules let the animation start off-screen
      animation.easing = go.Animation.EaseOutExpo;
      animation.duration = 900;
      // Fade "down", in other words, fade in from above
      animation.add(diagram, 'position', diagram.position.copy().offset(0, 200), diagram.position);
      animation.add(diagram, 'opacity', 0, 1);
      animation.start();
    }
    myDiagram.addModelChangedListener(function(evt) {
      myDiagram.model.linkFromPortIdProperty = "fromPort";
      myDiagram.model.linkToPortIdProperty = "toPort";
      if (evt.isTransactionFinished) save(evt.model);
    });
  } // end init


  // Show the diagram's model in JSON format that the user may edit
  function save(model) {
    console.log("saveDiagram",model.toJson() );
    document.getElementById("mySavedModel").value = model.toJson();
    myDiagram.isModified = false;
  }
  function load(jsonStr)
  {
    return new Promise(resolve => {
      try{
        myDiagram.model = go.Model.fromJson(JSON.parse(jsonStr));
      }
      catch(e){
        alert("請使用json格式");
      }
      resolve(true);

    })

  }
  function clear()
  {
    myDiagram.model = go.Model.fromJson({ "class": "go.GraphLinksModel","nodeDataArray":[ { "category": "Pos", "loc":"0 0" }]});
  }

  // print the diagram by opening a new window holding SVG images of the diagram contents for each page
  function printDiagram() {
    var svgWindow = window.open();
    if (!svgWindow) return;  // failure to open a new Window
    var printSize = new go.Size(700, 960);
    var bnds = myDiagram.documentBounds;
    var x = bnds.x;
    var y = bnds.y;
    while (y < bnds.bottom) {
      while (x < bnds.right) {
        var svg = myDiagram.makeSvg({ scale: 1.0, position: new go.Point(x, y), size: printSize });
        svgWindow.document.body.appendChild(svg);
        x += printSize.width;
      }
      x = bnds.x;
      y += printSize.height;
    }
    setTimeout(() => svgWindow.print(), 1);
  }

  function setDiagramReadOnly(value){
    console.log(value);
    myDiagram.isReadOnly = !value;

  }
  function makeImage() {
    return myDiagram.makeImage({
      type: "image/png",
      size : new go.Size(947,1250)
    });
  }
  function myDiagramToFile(filename) {
    let dataurl = makeImage().src;
    let arr = dataurl.split(','),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = Buffer.from(arr[1], 'base64'); 
    
    return new File([bstr], filename+"."+mime.split("/")[1], {type:mime});
  }
  // window.addEventListener('DOMContentLoaded', init);
  function readFileAsync(file) {
      return new Promise((resolve, reject) => {
        let reader = new FileReader();

        reader.onload = () => {
          resolve(reader.result);
        };

        reader.onerror = reject;

        reader.readAsText(file);
      })
  }



export default{
    props: [ "modalShow", "editFlowChartTp", "type" , "title", "hasDownload", "route", "engSeq"],
    emits:["handleModalShow", "upload", "download" ],
    data: () => {
      return {
        count :0,
        isEdit: false,
        saveDiagramJsonStr :null,
        originHref : ""

      }
    },
    watch: {
      saveDiagramJsonStr:{
        handler(value){
          console.log("DiagramJsonStr", value);
        }
      }
    },
    methods:{
        onChangeEdit() {
          this.isEdit = !this.isEdit;
          setDiagramReadOnly(this.isEdit);
          
        },
        handleModalShow(){
            this.isEdit =false;
            setDiagramReadOnly(this.isEdit);
            this.$emit("handleModalShow");
        },
        clearFlowChart(){
          let res = confirm("你確定要重置嗎?");
          if(!res) return;

          clear();
        },
        async getFlowChartTpDiagram() {
          let res = await window.myAjax.post(this.route+"/getFlowChartTpDiagramJson/" ,  { type :this.type, id: this.editFlowChartTp  });
          if(res.data.status == "success") {
            if(JSON.parse(res.data.jsonStr) != null) {
              load(res.data.jsonStr);
            }
            else {
              clear();
            }
          } 
        },
        async storeFlowChartTpDiagram(saveDiagramJsonStr= null) {
          let form = new FormData();
          this.saveDiagramJsonStr = saveDiagramJsonStr ?? document.getElementById("mySavedModel").value;
          form.append("ItemId", this.editFlowChartTp);
          form.append("DiagramJson", btoa(this.saveDiagramJsonStr) );
          form.append("Type", this.type);

          let res = await window.myAjax.post( this.route+"/storeFlowChartTpDiagramJson", form);
          if( res.data.status == "success") {
            alert("資料上傳成功");
            this.onChangeEdit();
          } 
          this.count++;
          const file = myDiagramToFile(this.type+"_"+ this.editFlowChartTp+"."+this.count);
          // this.$emit("uploadImage", this.editFlowChartTp, file);
          this.uploadImage(file);
        },
        changeDiagramJsonStr(e) {
          console.log("changeDiagramJsonStr" , e.target.value);
          this.saveDiagramJsonStr = e.target.value;

        },
        download(){
          this.$emit("download", this.editFlowChartTp);
        
        },
        async uploadImage(file) {
            let form = new FormData();
            // const files = this.files;
            form.append("file", file, file.name);
            form.append("seq", this.editFlowChartTp);
            form.append("engMain", this.engSeq ?? null);
            this.$emit("upload", this.editFlowChartTp, file);
            let {data: res} = await window.myAjax.post(this.route+"/"+this.type+"Upload", form, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            })
            if(res.message)
            {
              alert(res.message);
            }
        },
        exportTemplate() {
          let exportData =  document.getElementById("mySavedModel").value;
          var eleLink = document.createElement('a');
          eleLink.download = `匯出範本${ new Date().getTime()}.json`;
          eleLink.style.display = 'none';
          // 字元內容轉變成blob地址
          var blob = new Blob([exportData]);
          eleLink.href = URL.createObjectURL(blob);
          // 觸發點選
          document.body.appendChild(eleLink);
          eleLink.click();
          // 然後移除
          document.body.removeChild(eleLink);
        },
        async importTemplate(evt) {
          let files = evt.target.files; // FileList object
          // use the 1st file from the list
          let f = files[0];
          let content = await readFileAsync(f);
          document.getElementById('file_upload_input').value= null;
          var loadResult = await load(content);
          if(loadResult) this.storeFlowChartTpDiagram(content);
        }
    },

    watch: {
      editFlowChartTp:{
        handler : function(value) {
          clear();
          this.getFlowChartTpDiagram();
        
      
        }
      }
    },
    mounted(){
      type = this.type;
      init();
      

    }
    




    
}

</script>
