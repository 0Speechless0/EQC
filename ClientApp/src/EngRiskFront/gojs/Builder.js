import go, { Point } from "gojs";

export class Builder {
    constructor(DiagramId, DataSlotId) {
        this.DiagramId = DiagramId;
        this.DataSlot = DataSlotId;
    }

    init() {

        // Since 2.2 you can also author concise templates with method chaining instead of GraphObject.make
        // For details, see https://gojs.net/latest/intro/buildingObjects.html
        const $ = go.GraphObject.make;  // for conciseness in defining templates
        console.log(this.DiagramId);

        var myDiagram =
            $(go.Diagram, this.DiagramId,
                {
                    allowCopy: false,
                    layout:
                        $(go.LayeredDigraphLayout,
                            {
                                setsPortSpots: false,  // Links already know their fromSpot and toSpot
                                columnSpacing: 5,
                                isInitial: false,
                                isOngoing: false,
                                alignOption: go.LayeredDigraphLayout.AlignAll
                            }),
                    validCycle: go.Diagram.CycleNotDirected,
                    "undoManager.isEnabled": true,
                    SelectionDeleted: function (e) {
                        var deleteNode = e.subject.first();
                        e
                            .diagram.model.nodeDataArray.sort((a, b) => a.loc.split(" ")[1] < b.loc.split(" ")[1]);
                        e
                            .diagram.model.nodeDataArray
                            .filter(node => node.level == deleteNode.data.level)
                            .forEach((node, index) => {
                                node.no = index + 1;
                                e.diagram.model.raiseDataChanged(node, 'no');
                            })

                    }
                });
        // myDiagram.allowZoom = false;
        myDiagram.scale = 0.8;

        // when the document is modified, add a "*" to the title and enable the "Save" button
        myDiagram.addDiagramListener("TextEdited", e => {
            if (e.subject.text.length > 10) alert("字串長度不能大於10");
            e.subject.part.data.text = e.subject.text.slice(0, 10);
            e.diagram.model.raiseDataChanged(e.subject.part.data, "text");
        });

        const graygrad = $(go.Brush, "Linear",
            { 0: "white", 0.1: "whitesmoke", 0.9: "whitesmoke", 1: "lightgray" });

        myDiagram.nodeTemplate =  // the default node template
            $(go.Node, "Table",
                { selectionAdorned: false, textEditable: true, locationObjectName: "BODY" },
                new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                $(go.Panel, "Auto",
                    { name: "BODY", row: 0, column: 0 },
                    $(go.Shape, "Rectangle",
                        { fill: graygrad, stroke: "gray", minSize: new go.Size(46, 31), maxSize: new go.Size(46, Infinity) },
                        new go.Binding("fill", "isSelected", s => s ? "dodgerblue" : graygrad).ofObject()),
                    $(go.TextBlock, "center",
                        {
                            stroke: "black", font: "16px sans-serif", editable: true,
                            isMultiline: false,
                            margin: new go.Margin(3, 3 + 11, 3, 11)
                        },
                        new go.Binding("text", "no").makeTwoWay(),
                        new go.Binding("stroke", "isSelected", s => s ? "white" : "black").ofObject()
                    )
                ),
                // the main body consists of a Rectangle surrounding the text
                $(go.Panel, "Auto",
                    { name: "BODY", row: 0, column: 1 },
                    $(go.Shape, "Rectangle",
                        { fill: graygrad, stroke: "gray", minSize: new go.Size(180, 31), maxSize: new go.Size(240, Infinity) },
                        new go.Binding("fill", "isSelected", s => s ? "dodgerblue" : graygrad).ofObject()),
                    $(go.TextBlock,
                        {
                            stroke: "black", font: "16px sans-serif", editable: true,
                            overflow: go.TextBlock.OverflowEllipsis,
                            isMultiline: false,
                            margin: new go.Margin(3, 3 + 11, 3, 3 + 4), alignment: go.Spot.Left
                        },
                        new go.Binding("text").makeTwoWay(),
                        new go.Binding("stroke", "isSelected", s => s ? "white" : "black").ofObject()
                    )
                ),
                // output port
                $(go.Panel, "Auto",
                    { row: 0, column: 2, portId: "from", fromLinkable: true, cursor: "pointer", click: addNodeAndLink },
                    $(go.Shape, "Circle",
                        { width: 22, height: 22, fill: "white", stroke: "dodgerblue", strokeWidth: 3 }),
                    $(go.Shape, "PlusLine",
                        { width: 11, height: 11, fill: null, stroke: "dodgerblue", strokeWidth: 3 })
                ),
                // input port
                $(go.Panel, "Auto",
                    { alignment: go.Spot.Left, portId: "to", toLinkable: true },
                    $(go.Shape, "Circle",
                        { width: 8, height: 8, fill: "white", stroke: "gray" }),
                    $(go.Shape, "Circle",
                        { width: 4, height: 4, fill: "dodgerblue", stroke: null })
                )
            );

        myDiagram.nodeTemplate.contextMenu =
            $("ContextMenu",
                $("ContextMenuButton",
                    $(go.TextBlock, "Rename"),
                    { click: (e, obj) => e.diagram.commandHandler.editTextBlock() },
                    new go.Binding("visible", "", o => o.diagram && o.diagram.commandHandler.canEditTextBlock()).ofObject()),
                // add one for Editing...
                $("ContextMenuButton",
                    $(go.TextBlock, "Delete"),
                    { click: (e, obj) => e.diagram.commandHandler.deleteSelection() },
                    new go.Binding("visible", "", o => o.diagram && o.diagram.commandHandler.canDeleteSelection()).ofObject())
            );

        myDiagram.nodeTemplateMap.add("Loading",
            $(go.Node, "Table",
                { selectionAdorned: false, textEditable: false, locationObjectName: "BODY", deletable: false },
                new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                // the main body consists of a Rectangle surrounding the text
                $(go.Panel, "Auto",
                    { name: "BODY", row: 0, column: 0 },
                    $(go.Shape, "Rectangle",
                        { fill: graygrad, stroke: "gray", minSize: new go.Size(180, 31) },
                        new go.Binding("fill", "isSelected", s => s ? "dodgerblue" : graygrad).ofObject()),
                    $(go.TextBlock,
                        {
                            stroke: "black", font: "16px sans-serif", editable: true,
                            margin: new go.Margin(3, 3 + 11, 3, 3 + 4), alignment: go.Spot.Left
                        },
                        new go.Binding("text", "text"),
                        new go.Binding("stroke", "isSelected", s => s ? "white" : "black").ofObject()
                    )

                    // output port

                ),
                $(go.Panel, "Auto",
                    { row: 0, column: 1, alignment: go.Spot.Right, portId: "from", fromLinkable: true, click: addNodeAndLink },
                    $(go.Shape, "Circle",
                        { width: 22, height: 22, fill: "white", stroke: "dodgerblue", strokeWidth: 3 }),
                    $(go.Shape, "PlusLine",
                        { width: 11, height: 11, fill: null, stroke: "dodgerblue", strokeWidth: 3 })
                )
            ));

        myDiagram.nodeTemplateMap.add("End",
            $(go.Node, "Table",
                { selectionAdorned: false, textEditable: true, locationObjectName: "BODY" },
                new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                $(go.Panel, "Auto",
                    { name: "BODY", row: 0, column: 0 },
                    $(go.Shape, "Rectangle",
                        { fill: graygrad, stroke: "gray", minSize: new go.Size(46, 31), maxSize: new go.Size(46, Infinity) },
                        new go.Binding("fill", "isSelected", s => s ? "dodgerblue" : graygrad).ofObject()
                    ),
                    $(go.TextBlock, "center",
                        {
                            stroke: "black", font: "16px sans-serif", editable: true,
                            isMultiline: false,
                            margin: new go.Margin(3, 3 + 11, 3, 11)
                        },
                        new go.Binding("text", "no"),
                        new go.Binding("stroke", "isSelected", s => s ? "white" : "black").ofObject()
                    )
                ),
                // the main body consists of a Rectangle surrounding the text
                $(go.Panel, "Auto",
                    { name: "BODY", row: 0, column: 1 },
                    $(go.Shape, "Rectangle",
                        { fill: graygrad, stroke: "gray", minSize: new go.Size(180, 31) },
                        new go.Binding("fill", "isSelected", s => s ? "dodgerblue" : graygrad).ofObject()),
                    $(go.TextBlock,
                        {
                            stroke: "black", font: "16px sans-serif", editable: true,
                            margin: new go.Margin(3, 3 + 11, 3, 3 + 4), alignment: go.Spot.Left
                        },
                        new go.Binding("text").makeTwoWay(),
                        new go.Binding("stroke", "isSelected", s => s ? "white" : "black").ofObject()
                    )
                ),
                // input port
                $(go.Panel, "Auto",
                    { alignment: go.Spot.Left, portId: "to", toLinkable: true },
                    $(go.Shape, "Circle",
                        { width: 8, height: 8, fill: "white", stroke: "gray" }),
                    $(go.Shape, "Circle",
                        { width: 4, height: 4, fill: "dodgerblue", stroke: null })
                )
            ));
        myDiagram.nodeTemplateMap.add("Background",
            $(go.Node, "Part",
                { deletable: false, width: 1600, movable: false },
                new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                $(go.Panel, "Table",
                    {},


                    $(go.Shape, "Rectangle",
                        { fill: "white", column: 0, row: 0, height: 1000, width: 300, stroke: "lightgray", strokeWidth: 1 }
                    ),
                    $(go.TextBlock,
                        { background: "#F7BC9C", font: "22px sans-serif", width: 300, height: 35, text: "分項工程", column: 0, row: 0, alignment: go.Spot.TopCenter, textAlign: "center", verticalAlignment: go.Spot.Center }),

                    $(go.Shape, "Rectangle",
                        { fill: "white", column: 1, row: 0, height: 1000, width: 300, stroke: "lightgray", strokeWidth: 1 }
                    ),
                    $(go.TextBlock,
                        { background: "#F7E29C", font: "22px sans-serif", width: 300, height: 35, text: "第一階段內容", column: 1, row: 0, alignment: go.Spot.TopCenter, textAlign: "center", verticalAlignment: go.Spot.Center }),
                    $(go.Shape, "Rectangle",
                        { fill: "white", column: 2, row: 0, height: 1000, width: 300, stroke: "lightgray", strokeWidth: 1 }
                    ),
                    $(go.TextBlock,
                        { background: "#DBF79C", font: "22px sans-serif", width: 300, height: 35, text: "第二階段內容", column: 2, row: 0, alignment: go.Spot.TopCenter, textAlign: "center", verticalAlignment: go.Spot.Center }),
                    $(go.Shape, "Rectangle",
                        { fill: "white", column: 3, row: 0, height: 1000, width: 600, stroke: "lightgray", strokeWidth: 1 }
                    ),
                    $(go.TextBlock,
                        { background: "#9CF7CA", font: "22px sans-serif", width: 600, height: 35, text: "作業內容", column: 3, row: 0, alignment: go.Spot.TopCenter, textAlign: "center", verticalAlignment: go.Spot.Center }),
                )
            )


        );

        // this is a click event handler that adds a node and a link to the diagram,
        // connecting with the node on which the click occurred


        // Highlight ports when they are targets for linking or relinking.
        let OldTarget = null;  // remember the last highlit port
        function highlight(port) {
            if (OldTarget !== port) {
                lowlight();  // remove highlight from any old port
                OldTarget = port;
                port.scale = 1.3;  // highlight by enlarging
            }
        }
        function lowlight() {  // remove any highlight
            if (OldTarget) {
                OldTarget.scale = 1.0;
                OldTarget = null;
            }
        }

        // Connecting a link with the Recycle node removes the link
        myDiagram.addDiagramListener("LinkDrawn", e => {
            const link = e.subject;
            if (link.toNode.category === "Recycle") myDiagram.remove(link);
            lowlight();
        });
        myDiagram.addDiagramListener("LinkRelinked", e => {
            const link = e.subject;
            if (link.toNode.category === "Recycle") myDiagram.remove(link);
            lowlight();
        });

        myDiagram.linkTemplate =
            $(go.Link,
                { selectionAdorned: false, fromPortId: "from", toPortId: "to", relinkableTo: true },
                $(go.Shape,
                    { stroke: "gray", strokeWidth: 2 },
                    {
                        mouseEnter: (e, obj) => { obj.strokeWidth = 5; obj.stroke = "dodgerblue"; },
                        mouseLeave: (e, obj) => { obj.strokeWidth = 2; obj.stroke = "gray"; }
                    })
            );
        function addNodeAndLink(e, obj) {
            const fromNode = obj.part;
            const diagram = fromNode.diagram;
            diagram.startTransaction("Add State");
            // get the node data for which the user clicked the button
            const fromData = fromNode.data;

            // create a new "State" data object, positioned off to the right of the fromNode

            const linkedNodes = fromNode.findNodesOutOf();

            for (var i = 0; i < linkedNodes.count; i++) {
                linkedNodes.next();

                linkedNodes.value.data.no = i + 1;
                diagram.model.raiseDataChanged(linkedNodes.value.data, 'no');
            }
            const lastestLinkedNode = linkedNodes.value;
            let p;
            if (linkedNodes.count > 0) {
                p = lastestLinkedNode.location.copy();
            }
            else {
                p = diagram.model.levelLastestNodePos[fromData.level + 1].copy();
                fromNode.location.y = diagram.model.levelLastestNodePos[fromData.level + 1].y + 50;
                diagram.model.raiseDataChanged(fromNode.location, 'y');
            }

            p.y += 50;
            diagram.model.levelLastestNodePos[fromData.level + 1] = p;
            diagram.model.levelLastestNodeNo[fromData.level + 1] = fromData.no + 1;
            const toData = {
                category: fromData.level == 3 ? "End" : "",
                text: `new${lastestLinkedNode ? lastestLinkedNode.data.no + 1 : 1}`,
                loc: go.Point.stringify(p),
                no: lastestLinkedNode ? lastestLinkedNode.data.no + 1 : 1,
                level: fromData.level + 1
            };
            // add the new node data to the model
            const model = diagram.model;
            model.addNodeData(toData);
            console.log(model.nodeDataArray);
            // create a link data from the old node data to the new node data
            const linkdata = {
                from: fromData.key,
                to: model.getKeyForNodeData(toData)
            };
            // and add the link data to the model
            model.addLinkData(linkdata);
            // select the new Node
            const newnode = diagram.findNodeForData(toData);
            diagram.select(newnode);
            // snap the new node to a valid location
            newnode.location = p;

            console.log(newnode.location);
            // then account for any overlap
            // shiftNodesToEmptySpaces();
            newnode.minLocation = new go.Point(newnode.location.x, -Infinity);
            newnode.maxLocation = new go.Point(newnode.location.x, Infinity);
            diagram.commitTransaction("Add State");

        }
        function commonLinkingToolInit(tool) {
            // the temporary link drawn during a link drawing operation (LinkingTool) is thick and blue
            tool.temporaryLink =
                $(go.Link, { layerName: "Tool" },
                    $(go.Shape, { stroke: "dodgerblue", strokeWidth: 5 }));

            // change the standard proposed ports feedback from blue rectangles to transparent circles
            tool.temporaryFromPort.figure = "Circle";
            tool.temporaryFromPort.stroke = null;
            tool.temporaryFromPort.strokeWidth = 0;
            tool.temporaryToPort.figure = "Circle";
            tool.temporaryToPort.stroke = null;
            tool.temporaryToPort.strokeWidth = 0;

            // provide customized visual feedback as ports are targeted or not
            tool.portTargeted = (realnode, realport, tempnode, tempport, toend) => {
                if (realport === null) {  // no valid port nearby
                    lowlight();
                } else if (toend) {
                    highlight(realport);
                }
            };
        }

        const ltool = myDiagram.toolManager.linkingTool;
        commonLinkingToolInit(ltool);
        // do not allow links to be drawn starting at the "to" port
        ltool.direction = go.LinkingTool.ForwardsOnly;

        const rtool = myDiagram.toolManager.relinkingTool;
        commonLinkingToolInit(rtool);
        // change the standard relink handle to be a shape that takes the shape of the link
        rtool.toHandleArchetype =
            $(go.Shape,
                { isPanelMain: true, fill: null, stroke: "dodgerblue", strokeWidth: 5 });


        //prevent nodes from being dragged to the left of where the layout placed them
        myDiagram.addDiagramListener("LayoutCompleted", e => {
            myDiagram.nodes.each(node => {
                if (node.category === "Background") return;
                node.minLocation = new go.Point(node.location.x, -Infinity);
                node.maxLocation = new go.Point(node.location.x, Infinity);
            });
        });

        this.myDiagram = myDiagram;
    }
    getDiagramModel() {
        return this.myDiagram.model;
    }

    load(json, subProjectName = this.subProjectName) {
        var model = go.Model.fromJson(JSON.parse(json));
        model.nodeDataArray.sort((a, b) => a.no - b.no);
        model.levelLastestNodePos = [];
        model.levelLastestNodeNo = [];
        model
            .nodeDataArray
            .forEach(e => {
                if(e.level == 1){
                    e.text = subProjectName;
                }
                var locArr = e.loc.split(" ").map(str => parseFloat(str));
                e.category = !e.category ? "" : e.category
                model.levelLastestNodePos[e.level] = new Point(locArr[0], locArr[1]);
                model.levelLastestNodeNo[e.level] = e.no;
            });


        this.myDiagram.model = model;

    }
    download(data, fileName, mime) {
        var eleLink = document.createElement('a');

        eleLink.download = data.name ?? (fileName ?? "");
        eleLink.style.display = 'none';
        // 字元內容轉變成blob地址
        var blob = new Blob([data],
            {
                type: data.type ?? (mime ?? "text/plain")
            }

        );
        eleLink.href = URL.createObjectURL(blob);
        // 觸發點選
        document.body.appendChild(eleLink);
        eleLink.click();
        // 然後移除
        document.body.removeChild(eleLink);
    }

    async importJson(file, subProjectName) {
        this.subProjectName = subProjectName;
        return new Promise((resolve) => {
            let reader = new FileReader();
            reader.onload = () => {
                this.load(reader.result, subProjectName);
                console.log("1");
                resolve();
            };
    
    
            reader.readAsText(file);


        }, subProjectName)


    }
    exportJson(fileName) {
        this.download(JSON.stringify(this.myDiagram.model), fileName);
    }
    exportImg(filename = "img") {

        this.download(this.createImg(filename));
    }
    createImg(filename = "img") {
        let dataurl = this.myDiagram.makeImage({
            type: "image/png",
            position: new go.Point(0,0),
            maxSize: new go.Size(1600, 1000)
        }).src;
        let arr = dataurl.split(','),
            mime = arr[0].match(/:(.*?);/)[1],
            bstr = Buffer.from(arr[1], 'base64');

        return new File([bstr], filename + "." + mime.split("/")[1], { type: mime });
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
