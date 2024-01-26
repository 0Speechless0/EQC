<template>
    <div>
        <div class="table-responsive">
            <table class="table table1">
                <thead class="insearch">
                    <tr>
                        <th><strong>項目</strong></th>
                        <th><strong>樹種</strong></th>
                        <th><strong>株數</strong></th>
                        <th><strong>植樹經費</strong></th>
                        <th><strong>預定株數</strong></th>
                        <th><strong>預定累計株數</strong></th>
                        <th><strong>實際株數</strong></th>
                        <th><strong>實際累計株數</strong></th>
                        <th><strong>預定面積</strong></th>
                        <!-- <th><strong>預定累計面積</strong></th> -->
                        <th><strong>實際面積</strong></th>
                        <!-- <th><strong>實際累計面積</strong></th> -->
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in items" :key="index">
                        <td style="width: 100px;" align="right">{{ item.PayItem}}</td>
                        <td style="width: 200px;" align="right">
                            {{ item.TreeName }}
                        </td>

                        <td style="width: 100px;" align="right">{{ item.Quality }}</td>
                        <td style="width: 100px;" align="right">{{ item.TreeAmount }}</td>
                        <td style="width: 200px;" align="right" >
                            <div v-for="(item, index) in item.ScheduledPlantValue" :key="index" class="row">
                                <div class="col">
                                    {{item.date}}
                                </div>
                                <div class="col" style="color:blue">
                                    {{item.count}}
                                </div>
                            </div>
                        </td>
                        <td style="width: 200px;" align="right">{{ item.AccSchNum}}</td>
                        <td style="width: 200px;" align="right">
                            <div v-for="(item, index) in item.ActualPlantValue" :key="index" class="row">
                                <div class="col">
                                    {{item.date}}
                                </div>
                                <div class="col" style="color:blue">
                                    {{item.count}}
                                </div>
                            </div>
                        </td>
                        <td style="width: 200px;" align="right">{{ item.AccActualNum}}</td>
                        <!-- <td style="width: 200px;" align="right">{{ computeArea(itFem, "ScheduledPlantNum")}}</td> -->
                        <td style="width: 200px;" align="right">{{ computeArea(item, "AccSchNum") }}</td>
                        <!-- <td style="width: 200px;" align="right">{{ computeArea(item, "ActualPlantNum")}}</td> -->
                        <td style="width: 200px;" align="right">{{ computeArea(item, "AccActualNum")}}</td>
                    </tr>
                    
                </tbody>
            </table>
        </div> 
        <p :style="`color:${errorLabel.color}`" v-if="loading"> {{errorLabel.text ?? '資料載入中，請稍後....'  }}</p>


</div>

</template>

<script>

export default {

    props : ["engSeq", "AreaTotal", "isSave", "edit"],
    emits : ["afterSave", "afterLoad", "QualityDetect" ],
    
    data : () => {
        return {
            loading : false,
            errorLabel : {
                text : null,
                color : "green"
            },
            items : [],
            areaTotal : 0,
            qualityTotal : 0,
            qualityColor : null  

        }
    },
    watch : {
        isSave : {
            async handler(value)
            {
                if(value != 0)
                {
                    // if(this.edit)
                    // {
                    //     await window.myAjax.post("Tree/updateTreePlantMonths", { list :this.items, engSeq : this.engSeq});
                    // }
                    // else {
                    //     await window.myAjax.post("Tree/insertTreePlantMonths", { list :this.items, engSeq : this.engSeq});
                    // }


                    this.$emit("afterSave");
                }

            }
        },
        engSeq :{
            async handler(value)
            {
                if(value != null)
                {
                    console.log("aaa", value);
                    this.data = (await window.myAjax.post("Tree/getTreePlantMonthsPcces", {engSeq : this.engSeq})).data;
                }
            }
        }
    },
    computed: {
        // overQuality()
        // {
        //     this.emit("QualityDetect", this.qualityTotal > this.AreaTotal);
        //     return this.qualityTotal > this.AreaTotal;
        // }
    },
    methods:{
        computeArea(item, col)
        {
            return this.AreaTotal ?  ((item[col]/this.qualityTotal)*this.AreaTotal).toFixed(2) : 0;
        },
        // computeActualArea(item)
        // {

        //     item.ActualArea = this.AreaTotal ?  (item["ActualPlantNum"]/item.Quality)*this.AreaTotal  : 0;
        //     return item.ActualArea ;
        // },
        // computeScheduledArea(item)
        // {
        //     return this.AreaTotal ?  (item["ScheduledPlantNum"]/item.Quality)*this.AreaTotal : 0;
        //     return item.ScheduledArea;
        // }
    },

    async mounted()
    {

            this.loading = true;
            try {
                this.items = (await window.myAjax.post("Tree/getTreePlantMonthsPcces", {engSeq : this.engSeq})).data;
                if(this.items < 0)
                {
                    switch(this.items){
                        case -1 : this.errorLabel = { text :"無植樹資料", color : "red"};break;
                    }
                    this.items = [];
                    return ;
                }
                console.log("1", this.items);
                this.items.forEach(element => {
                    element.ScheduledPlantValue = 
                        element.ScheduledPlantValue.slice(1).split('，')
                        .map(e => e.split(':'))
                        .map(e =>  { return {date : e[0], count : e[1] } } );
                    element.ActualPlantValue = 
                        element.ActualPlantValue.slice(1).split('，')
                        .map(e => e.split(':'))
                        .map(e =>  { return {date : e[0], count : e[1] } } );
                    this.qualityTotal += element.Quality; 
                })

                this.items.forEach(element => {
                    var targetScheduledPlantValue = element.ScheduledPlantValue.filter(
                        (e,i) => !( element.ActualPlantValue[i].count ==0 && parseInt(e.count) == 0 ) );
                    element.ActualPlantValue =  element.ActualPlantValue.filter(
                        (e,i) => (i+1 > element.ScheduledPlantValue.length && parseInt(e.count) != 0) 
                        ||  !( i+1 <= element.ScheduledPlantValue.length && element.ScheduledPlantValue[i].count ==0 && parseInt(e.count) == 0)   );
                    element.ScheduledPlantValue = targetScheduledPlantValue;
                })
                // this.items.forEach(elenment => {
                //     elenment.ScheduledPlantValue = elenment.ScheduledPlantValue.slice(start);
                //     elenment.ActualPlantValue = elenment.ActualPlantValue.slice(start);
                // })
                // console.log("4", this.items);
            } catch (error) {
                console.log("aa", error);
                this.loading = false;
            }


            this.loading = false;
            this.$emit("afterLoad", this.items);
        // else this.items = (await window.myAjax.post("Tree/getTreePlantMonths", {engSeq : this.engSeq})).data;
    }
}
</script>
