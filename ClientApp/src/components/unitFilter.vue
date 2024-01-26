<template>
    <div >
        <div :class="`col-12 col-sm-${Math.trunc(12/(maxUnitLevel+1)) - 1} mt-3`"  v-for="(unitOption, index2) in unitOptions" :key="index2">

            <select    class="form-control" v-model="subUnit[index2+(_startLevel ?? 0)]" @change="unitChange(index2)" >
                <option  value="" selected > 全部單位 </option>
                <option v-for="(option, index) in unitOption" :value="option" :key="index" >{{ option }}</option>
            </select>
        </div>

    </div>



</template>

<script>
export default {
    props : ["newUnitLevelOptions", "maxUnitLevel", "startLevel", "initSubUnit" ],
    emits : ["afterUnitChange"],
    data : () => {
        return {
            unitOptions : [],
            unitLevel : -1,
            subUnit : [],

        }
    },
    watch: {
        newUnitLevelOptions : {
            handler(value, old)
            {
                console.log(this.init);
                if (
                    this.newUnitLevelOptions.length > 0  
                    && this.unitLevel < this.maxUnitLevel 
                    && this.subUnit[this.unitLevel] != "" 
                )
                {
                    console.log(this.newUnitLevelOptions, this.subUnit);
                    
                    this.unitOptions.push(this.newUnitLevelOptions );
                    this.unitLevel++;

                }
            }
        }
    },
    computed:{
        _startLevel()
        {
            return this.startLevel ?? 0;
        }
    },
    methods :{
        unitChange(index)
        {
            console.log(index, this.unitLevel);
            if(index >= this.unitLevel )
            {
                this.$emit("afterUnitChange",  this.subUnit);
            }
            else {
                this.unitOptions = this.unitOptions.slice(0, index+1);
                for(var i = this.maxUnitLevel ;i >= this._startLevel; i--)
                {
                    if(i - this._startLevel == index)  break;
                    this.subUnit[i] = "";

                }

                this.$emit("afterUnitChange", this.subUnit);


            }

            this.unitLevel = index + this._startLevel;
        },

    },
    mounted()
    {
        this.unitLevel = this._startLevel - 1 ;
        if(this.initSubUnit )
        {
            this.subUnit =  this.initSubUnit 
            for( i = 0;i <= this.maxUnitLevel; i++) 
            this.subUnit[i]= this.initSubUnit[i] ? this.initSubUnit[i] : "";

        }
        else
        {
            for(var i = this._startLevel;i <= this.maxUnitLevel; i++) this.subUnit[i]= "";
        }

        this.$emit("afterUnitChange",  this.subUnit);
        console.log(this.subUnit, this._startLevel);

    }

}


</script>  
