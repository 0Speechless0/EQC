//這支檔案用於建立gojs模塊並掛載到html元素上

import go, { Point } from "gojs";
import Common from "./Common/Common2.js";
export class Builder
{
    constructor(
        DiagramId, 
        init,
        DiagramOption
        )
    {
        this.DiagramId = DiagramId;
        const $ =  go.GraphObject.make;

        this.myDiagram =
          $(go.Diagram, this.DiagramId,
            DiagramOption
        );
        init($);
    }
    
    getDiagramModel()
    {
        return this.myDiagram.model;
    }

    load(json) {
        var model = go.Model.fromJson(json);
        this.myDiagram.model = model;

    }

    async importJson(file)
    {
        let reader = new FileReader();

        reader.onload = () => {
        this.load(reader.result);
        };


        reader.readAsText(file);

    }

    exportJson(fileName)
    {
        Common.download(JSON.stringify(this.myDiagram.model), fileName );
    }

    exportImg(filename="img")
    {
    let dataurl = this.myDiagram.makeImage({
        type: "image/png",
        size : go.Size(1600, 1000)
    }).src;

    let arr = dataurl.split(','),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = Buffer.from(arr[1], 'base64'); 
    
        Common.download(new File([bstr], filename+"."+mime.split("/")[1], {type:mime}) );
    }
}




// Define a custom tool that changes a drag operation on a Link to a relinking operation,
// but that operates like a normal DraggingTool otherwise.
// class DragLinkingTool extends go.DraggingTool {
//   constructor() {
//     super();
//     this.isGridSnapEnabled = true;
//     this.isGridSnapRealtime = false;
//     this.gridSnapCellSize = new go.Size(182, 1);
//     this.gridSnapOrigin = new go.Point(5.5, 0);
//   }

//   // Handle dragging a link specially -- by starting the RelinkingTool on that Link
//   doActivate() {
//     const diagram = this.diagram;
//     if (diagram === null) return;
//     this.standardMouseSelect();
//     const main = this.currentPart;  // this is set by the standardMouseSelect
//     if (main instanceof go.Link) { // maybe start relinking instead of dragging
//       const relinkingtool = diagram.toolManager.relinkingTool;
//       // tell the RelinkingTool to work on this Link, not what is under the mouse
//       relinkingtool.originalLink = main;
//       // start the RelinkingTool
//       diagram.currentTool = relinkingtool;
//       // can activate it right now, because it already has the originalLink to reconnect
//       relinkingtool.doActivate();
//       relinkingtool.doMouseMove();
//     } else {
//       super.doActivate();
//     }
//   }
// }
