<template>

<div>
    <div id="sample">

        <div id="myDiagramDiv" style="border: 1px solid black; width: 100%; height: 800px; position: relative; -webkit-tap-highlight-color: rgba(255, 255, 255, 0); cursor: auto; font: 12px sans-serif;">
            <canvas tabindex="0" width="1589" height="498" style="position: absolute; top: 0px; left: 0px; z-index: 2; user-select: none; touch-action: none; width: 1589px; height: 498px; cursor: auto;">This text is displayed if your browser does not support the Canvas HTML element.</canvas>
            <div style="position: absolute; overflow: auto; width: 1589px; height: 498px; z-index: 1;font-size: larger;">
                <div style="position: absolute; width: 1px; height: 1px;"></div>
            </div>
        </div>
        <div id="myImageDiv" style="border: 1px solid black; width: 100%; height: 800px; position: relative; -webkit-tap-highlight-color: rgba(255, 255, 255, 0); cursor: auto; font: 12px sans-serif;" hidden>
            <canvas tabindex="0" width="1589" height="498" style="position: absolute; top: 0px; left: 0px; z-index: 2; user-select: none; touch-action: none; width: 1589px; height: 498px; cursor: auto;">This text is displayed if your browser does not support the Canvas HTML element.</canvas>
            <div style="position: absolute; overflow: auto; width: 1589px; height: 498px; z-index: 1;font-size: larger;">
                <div style="position: absolute; width: 1px; height: 1px;"></div>
            </div>
        </div>
        <div style="display: flex;padding: 10px;" class="row">
            <button id="SaveButton"  class="btn btn-color11-2 btn-sm mx-1"  @click="importDestruction" >儲存</button>
            <button id="SaveButton"  class="btn btn-color11-1 btn-sm mx-1"  @click="exportDestructionJson" >匯出範本(JSON)</button>
            <input type="file" class="btn btn-color11-2 btn-sm mx-1"  @change="importDestructionJson" id="importJson" hidden/>
            <label  class="btn btn-color11-1 btn-sm mx-1"  style="margin-bottom: 0px;" for="importJson">匯入範本(JSON)</label>
            <button id="SaveButton"  class="btn btn-color11-3 btn-sm mx-1"  @click="exportDestructionImg" >匯出圖檔(PNG)</button>
            <button role="button" class="btn btn-color9-1 btn-sm mx-1" @click="goBack"> 回上頁 </button>
        </div>
    </div>

</div>
</template>

<script>
import { Builder  } from './gojs/Builder';
import { ImageBuilder  } from './gojs/ImageBuilder';
var builder = new Builder("myDiagramDiv");
var imgBuilder = new ImageBuilder("myImageDiv");
export default {
    props:["subProject"],
    emits:["goBack"],
    data : () => {
        return {
            subProjectJson: ""

        }
    },
    mounted()
    {        
        this.subProjectJson = this.subProject.SubProjectJson ?? 

        `
            
            { 
                "class": "GraphLinksModel",
                    "nodeDataArray": [
                        {"key":-2,"category":"Background","loc":"0 0"},
                        {"key":1,"text":"${this.subProject.SubProjectName}","category":"Loading","loc":"60 186", "level":1},
                        {"key":2,"text":"第二層","no":1,"loc":"360 186", "level":2},
                        {"key":3,"text":"第三層","loc":"660 186", "no":1, "level":3},
                        {"key":4,"text":"項目","loc":"1000 186", "no":1, "level":4, "category":"End"}

                    ],
                    "linkDataArray": [
                    {"from":1,"to":2},
                    {"from":2,"to":3},
                    {"from":3,"to":4}
                ]
            }
        
        `
        
        ;
         builder.init();
         imgBuilder.init();
         builder.load(this.subProjectJson, this.subProject.SubProjectName);
         imgBuilder.load(this.subProjectJson, this.subProject.SubProjectName);
    },
    methods : {
        goBack()
        {
            this.$emit('goBack', this.subProjectJson);
        },
        exportDestructionJson()
        {   
            builder.exportJson(`${this.subProject.SubProjectName}工程拆解json匯出檔-${new Date().toDateString() }.json`);
        },
        exportDestructionImg()
        {         
            imgBuilder.load(JSON.stringify(builder.getDiagramModel()) );
            imgBuilder.exportImg(`${this.subProject.SubProjectName}工程拆解json匯出圖檔-${new Date().toDateString() }`);
        },
        importDestructionJson(evt)
        {
            let files = evt.target.files; 
            if(files.length > 0 ) 
            builder.importJson(files[0], this.subProject.SubProjectName)
            imgBuilder.importJson(files[0], this.subProject.SubProjectName);
            
        },
        async importDestruction(normal)
        {
            var DiagramModel = builder.getDiagramModel();
            console.log("DiagramModel", DiagramModel.nodeDataArray, DiagramModel.linkDataArray);
            var jsonStr = (await window.myAjax.post("RiskManagement/importDestruction", { 
                subProjectSeq:this.subProject.Seq,  
                nodeData : DiagramModel.nodeDataArray,
                linkData : DiagramModel.linkDataArray,
                normal : normal
            })).data.diagramStr

            if(jsonStr) {
                builder.load(jsonStr, this.subProject.SubProjectName);
                
                alert("儲存成功");
                this.subProjectJson = jsonStr;
            }
            else {
                alert("儲存失敗!");
            }
            if(jsonStr)
            {
                var formData =new FormData();
                // formData.append("subProjectSeq", this.subProject.Seq);
                imgBuilder.load(JSON.stringify(builder.getDiagramModel()) );
                var file = imgBuilder.createImg(`${this.subProject.SubProjectName}`);
                console.log(file);
                formData.append("file", file );
                var res = (
                    await window.myAjax.post("RiskManagement/UploadGeneratedImg",
                       formData,
                        {
                            headers: {
                             "Content-Type": "multipart/form-data"
                            }
                        }
                    )
                ).data;
            }
        }
    }
}

</script>
<style >

</style>