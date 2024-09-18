<template>
<div>

    <div class="table-responsive">
    <table class="table table1"  id="addnew258">
        <thead class="insearch">
            <tr >
                <th style="width: 200px;"><strong>樹種年度</strong></th>
                <th style="width: 200px;"><strong>樹種</strong></th>
                <th style="width: 200px;"><strong>樹名</strong></th>
                <th style="width: 200px;"><strong>預定種植株數</strong></th>
                <th style="width: 200px;"><strong>實際種植株數</strong></th>
                <th style="text-align: center; width: 00px;">
                    <a href="javascript:void(0)" role="button" class="btn btn-color11-3 btn-xs sharp mr-1" @click="addTreeNumList()">
                        <i class="fas fa-plus"></i>
                    </a>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(item,index) in  treeNumList" :key="index">
                <td>
                    {{ TreeYear }}
                </td>
                <td>
                    <select class="form-control" v-model="item.TreeType">
                        <option value="喬木">喬木</option>
                        <option value="灌木">灌木</option>
                    </select>
                </td>
                <td>
                    <!-- <select class="form-control " v-model="item.TreeTypeSeq">
                        <option v-for="(option, index) in treeList" :key="index" :value="option.Seq">{{option.Name}}</option>

                    </select> -->
                    <!-- <div v-if="item.TreeTypeName == null">
                        <el-select
                        style="width: 80%;font-weight: 400;"
                        v-model="item.TreeTypeSeq"
                     
 
                        placeholder="請輸入樹種關鍵字"
                        filterable
                        allow-create
                        default-first-option

                        no-match-text="無資料"
                    >
                        <el-option
                        v-for="(option,index2) in treeList"
                        :key="`${index}${index2}`"
                        :label="option.Name"
                        :value="option.Seq"

                        />
                    </el-select>
                    </div> -->
                    <input class="form-control"     style="width: 80%;font-weight: 400;" v-model="item.TreeTypeName" placeholder="請輸入內容" />
                    
                </td>
                
                <td><input type="number" class="form-control" v-model="item.ScheduledPlantNum" ></td>
                <td><input type="number" class="form-control" v-model="item.ActualPlantNum" ></td>
                <td>
                    <a  href="javascript:void(0)" role="button" class="btn btn-color11-4 btn-xs sharp mr-1" @click="deleteTreeNumList(item.Seq, index)">
                        <i class="fa fa-minus"></i>
                    </a>

                </td>
            </tr>

        </tbody>
    </table>

    </div>
    <div class="table-responsive">
    <table class="table table1">
        <thead class="insearch">
            <tr>
                <th style="width: 100px;">年度</th>
                <th style="width: 100px;"><strong>月份</strong></th>
                <th style="width: 300px;"><strong>預定面積(公頃)</strong></th>
                <th><strong>累計預定面積(公頃)</strong></th>
                <th><strong>實際面積(公頃)</strong></th>
                <th><strong>累計實際面積(公頃)</strong></th>
                <!-- <th><strong>實際株數</strong></th>
                <th><strong>累計實際株數</strong></th> -->
            </tr>
        </thead>
        <tbody>
            <tr  v-for="(item, index) in treeMonths" :key="index">
                <td class="text-left">{{ item.Year }}</td>
                <td>{{ (startMonth +index - 1) %12 +1 }}</td>
                <td><input type="number" placeholder="0"  class="form-control" value="10" v-model.lazy="item.ScheduledArea" @change="CalculateAcc('ScheduledArea',index)"></td>
                <td>{{item.AccScheduledArea.toFixed(2)}}</td>
                <td><input type="number" placeholder="0" class="form-control" value="18"  v-model.lazy="item.ActualArea" @change="CalculateAcc('ActualArea',index)"></td>
                <td>{{item.AccActualArea.toFixed(2)}}</td>
                <!-- <td><input type="number" placeholder="0"  class="form-control" value="125" v-model.lazy="item.ActualPlantNum" @change="CalculateAcc('ActualPlantNum',index)"></td>
                <td>{{item.AccActualPlantNum}}</td> -->
            </tr>

        </tbody>
    </table>
    </div>
    <p style="font-size:18px">
            預定種植面積(公頃) : {{AreaTotal}}，每月累計面積(公頃) : <span :style="`color:${AreaDetectColor}`">{{treeMonths.length > 0 ? treeMonths[treeMonths.length - 1].AccScheduledArea.toFixed(2) : 0}}</span>
        </p>

</div>

</template>

<script>

import Comm from "../../Common/Common2";
export default {

    props : ["engSeq", "isSave", "edit", "startMonth", "monthDiff", "createType", "treePlantMainSeq", "AreaTotal", "treePlantMain"],
    emits: ["afterSave", "QualityDetect", "planteScheduleDateChange"],
    data : () => {
        return {
            selectTree : [],
            treeList: [],
            treeNumList :[],
            treeMonths:[],
            endMonth : 0,
            AreaDetectColor : null,
            AccSchArea : 0,
            ScheduledStart : null
        
        }
    },
    watch:{


        overArea : {
            handler(value)
            {
                console.log(value,"sdf");
                if(value) this.AreaDetectColor ='red';
                else this.AreaDetectColor = "black";
            },
            immediate :true
        },
        treePlantMainSeq : {
            async handler(value)
            {
                //自行勾稽新增
                console.log(value);
                if(value != 0 && this.edit)
                {
                    this.treeNumList = (await window.myAjax.post("Tree/getTreeNumList", { engSeq :this.engSeq,  treePlantMainSeq : value  })).data ;
                    this.treeMonths = (await window.myAjax.post("Tree/getTreePlantMonths", { engSeq :this.engSeq, treePlantMainSeq : value })).data;
                    this.calculateTreeMonthsAcc(this.treeMonths);
                }
            }
        },
        isSave : {
            async handler(value)
            {            

                console.log(this.treeNumList);
                this.treeNumList.forEach(e =>{
                    if( !parseInt(e.TreeTypeSeq)  && e.TreeTypeName == null)
                    {
                        e.TreeTypeName =  e.TreeTypeSeq;      
                    }

                 } );

                if(value != 0  )
                {
                    if(this.edit)
                    {
               
                        await window.myAjax.post("Tree/updateTreePlantMonths", { list :this.treeMonths, engSeq : this.engSeq, 
                            treePlantSeq : value ? value : null });
                        await window.myAjax.post("Tree/updateTreeNumList",  { list : this.treeNumList, engSeq : this.engSeq,
                            treePlantSeq : value ? value : null});

                        
                    }
                    else {
                        await window.myAjax.post("Tree/insertTreePlantMonths", { list :this.treeMonths, engSeq : this.engSeq,
                            treePlantSeq : value ? value : null});
                        await window.myAjax.post("Tree/insertTreeNumList", {list:this.treeNumList, engSeq : this.engSeq,
                            treePlantSeq : value ? value : null });
      
                    }
                    
                    this.$emit("afterSave");
                    this.treeNumList = (await window.myAjax.post("Tree/getTreeNumList", { engSeq :this.engSeq,  treePlantMainSeq : value  })).data ;
                }

            }
        },
        treePlantMain : {
            handler(value)
            {
                var monthDiff =  Comm.monthDiff(value.ScheduledPlantDate, value.ScheduledCompletionDate);
                console.log(monthDiff, this.treeMonths.length  );
                if(monthDiff == 0 && this.treeMonths.length == 0)
                {
                        this.treeMonths.push(                        {
                            ScheduledArea : 0,
                            AccScheduledArea : 0,
                            ActualArea : 0,
                            AccActualArea : 0,
                            ActualPlantNum : 0,
                            AccActualPlantNum : 0
                        });
                        this.treeMonths[0].Year = new Date(value.ScheduledPlantDate).getFullYear() - 1911;
                }
                if(this.treePlantMain.ScheduledPlantDate > this.treePlantMain.ScheduledCompletionDate )
                {
                    alert("日期不允許");
                    this.treeMonths = [];
                    this.$emit("planteScheduleDateChange");
                }
                
            },
            deep: true,
            immediate : true
        },
        monthDiff :{
            handler(value, old)
            {
                // if(this.createType == 1)
                // {
                //     return ;
                // }

                if(this.treePlantMain.ScheduledPlantDate > this.treePlantMain.ScheduledCompletionDate ) return ;
                
                var monthDiff =  Comm.monthDiff(this.treePlantMain.ScheduledPlantDate, this.treePlantMain.ScheduledCompletionDate);
                console.log(monthDiff);
                var newTreeMonths = [];
                for(var i = 0 ; i < monthDiff +1; i++)
                {

                    newTreeMonths[i] = this.treeMonths[i] ??
                    {
                        ScheduledArea : 0,
                        AccScheduledArea : 0,
                        ActualArea : 0,
                        AccActualArea : 0,
                        ActualPlantNum : 0,
                        AccActualPlantNum : 0
                    }
                    
                }
                this.treeMonths = newTreeMonths;
            


                // if(!this.edit)
                // {
                //     console.log(value, old, "f", this.edit);
                //     this.treeMonths = [];

                //     for(var i = 0 ; i<= value; i++)
                //     {

                //         this.treeMonths.push(
                //             {
                //                 ScheduledArea : 0,
                //                 AccScheduledArea : 0,
                //                 ActualArea : 0,
                //                 AccActualArea : 0,
                //                 ActualPlantNum : 0,
                //                 AccActualPlantNum : 0
                //             }
                //         )
                //         console.log("F", this.treeMonths);
                //     }

                // }
                // else if(value > old ){
                //     for(i = 0 ; i < value - old; i++)
                //     {

                //         this.treeMonths.push(
                //             {
                //                 ScheduledArea : 0,
                //                 AccScheduledArea : 0,
                //                 ActualArea : 0,
                //                 AccActualArea : 0,
                //                 ActualPlantNum : 0,
                //                 AccActualPlantNum : 0
                //             }
                //         )
                //     }
                // }
                // else{
                //     this.treeMonths = this.treeMonths.slice(0, value+1);
                // }
                let [startYear, startMonth] = 
                    this.ScheduledStartDate.split('/')
                    .map(e => parseInt(e));
                console.log("ff", startYear, startMonth);
                this.treeMonths.forEach( (element, index) => {
                    
                        element.Year = startYear +  Math.floor( (startMonth -1 + index) /12 );
                })
                this.calculateTreeMonthsAcc(this.treeMonths);

            }
        }
    },
    methods: {

        checkTree(tree)
        {
            if(tree.TreeTypeSeq ==null)
            {
                alert("此樹種未被認定");
            }
        },
        SetTreeScheduledStart(treePlantMain)
        {
            this.ScheduledStart = treePlantMain.ScheduledPlantDate;
        },
        GetDataFromPcces(dataFromPcces)
        {
            console.log("GetDataFromPcces");
                this.treeNumList  = dataFromPcces.treeNumList;
                // let treeMonthList  = dataFromPcces.treeMonthList;
                // this.calculateTreeMonthsAcc(treeMonthList);

        },
        CalculateAcc(str,index)
        {
            if(index < 0) return ;

            for(var i = index ;i <this.treeMonths.length; i++)
            {
                this.treeMonths[i]["Acc"+str] =  
                ( i == 0 ? 0 : parseFloat(this.treeMonths[i-1]["Acc"+str]) )+ 
                
                (isNaN(parseInt(this.treeMonths[i][str])) ? 0 : parseFloat(this.treeMonths[i][str]));
            }
            this.AccSchArea =  this.treeMonths[this.treeMonths.length -1].AccScheduledArea
        } ,
        async addTreeNumList()
        {
        //    var item  =  (await window.myAjax.post("Tree/insertTreeNumList", {engSeq : this.engSeq })).data;
           this.treeNumList.push({
                ScheduledPlantNum : 0 ,
                TreeTypeSeq : null,

                ActualPlantNum : 0
           });
        },
        async editTreeNumList(m)
        {
            await window.myAjax.post("Tree/updateTreeNumList",  { m : m  })
        },
        async deleteTreeNumList(Seq, index)
        {
            if(!Seq)
            {
                this.treeNumList = 
                        this.treeNumList.slice(0, index)
                        .concat(this.treeNumList.slice(index+1));
            }
            else if( confirm("確定要刪除，按下確定後會直接刪除，而不是等到按儲存之後?") )
            {
                if( (await window.myAjax.post("Tree/deleteTreeNumList", {treeNumSeq : Seq})).data == true)
                {
                    this.treeNumList = 
                        this.treeNumList.slice(0, index)
                        .concat(this.treeNumList.slice(index+1));
                }   
            }

        },
        calculateTreeMonthsAcc(treeMonths = [])
        {
            this.treeMonths = treeMonths.reduce((a, e) => {
                        a.AccActualArea += e.ActualArea;
                        a.AccScheduledArea += e.ScheduledArea;
                        a.AccActualPlantNum += e.ActualPlantNum;
                        e.AccActualArea = a.AccActualArea;
                        e.AccScheduledArea = a.AccScheduledArea;
                        e.AccActualPlantNum = a.AccActualPlantNum; 
                        a.resultTreeMonth.push(e);
                        return a;
                    }, {
                        AccActualArea : 0,
                        AccScheduledArea : 0,
                        AccActualPlantNum : 0,
                        resultTreeMonth : []
                    }).resultTreeMonth;
        },
        async GetTreeData()
        {
            if(this.treePlantMainSeq)
            {

                this.treeNumList = (await window.myAjax.post("Tree/getTreeNumList", { engSeq :this.engSeq,  treePlantMainSeq : this.treePlantMainSeq })).data ;
                

                let treeMonths = (await window.myAjax.post("Tree/getTreePlantMonths", { engSeq :this.engSeq, treePlantMainSeq : this.treePlantMainSeq })).data;

                this.calculateTreeMonthsAcc(treeMonths);
            }

        }
    },
    computed:{
        TreeYear()
        {
            var yearStart = new Date(this.treePlantMain.ScheduledPlantDate).getFullYear() - 1911; 
            var yearEnd = new Date(this.treePlantMain.ScheduledCompletionDate).getFullYear() - 1911; 
            var s = "";
            for(var i = yearStart; i <= yearEnd; i++)
            {
                s+= "/" + i;
            }
            return s.slice(1, s.length);

        },
        treeNumListView()
        {
            return this.treeNumList.filter(e => e.TreeTypeSeq != null);
        },
        ScheduledStartDate()
        {
            return Comm.ToROCDate(this.treePlantMain.ScheduledPlantDate);
        },
        overArea()
        {
                return this.AccSchArea > this.AreaTotal;
        }
    },
    async mounted()
    {
        this.treeList = (await window.myAjax.post("Tree/getTreeList")).data;
        this.treeList.forEach(e => {
            if(e.Seq == 0)
            {   
                e.Seq = e.Name;
            }

        })

        if(this.treePlantMainSeq)
        {
            await this.GetTreeData();
            this.CalculateAcc('ActualArea', this.treeMonths.length);
        }

     
    }
    
}

</script>

<style>
.el-input
{
    font-size: 1rem !important;
    font-weight: bolder !important;
}
.el-select-dropdown__item{
    font-size: 1rem !important;
    font-weight: bolder !important;
}

</style>