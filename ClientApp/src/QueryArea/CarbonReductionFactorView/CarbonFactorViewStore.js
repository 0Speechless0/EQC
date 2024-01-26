import {reactive, computed} from "vue";

export const store = reactive({
    factorList : [],
    factorListOrg : [],
    codeOption : [],
    codeOptionStack : [],
    optionSelectedStack : [],
    getNewOption(front, selectedOption) {
        let factorList = this.factorListOrg.filter(e => e.Code.startsWith(selectedOption) );

        this.codeOption = new Set(factorList.map(e => e.Code.slice(0, front)));


        this.codeOptionStack = this.codeOptionStack.slice(0, front-1);
        this.optionSelectedStack = this.optionSelectedStack.slice(0, front-2);
        if(this.codeOption.size >= 2 ) 
        {
            this.codeOptionStack.push(this.codeOption);

        }
        this.optionSelectedStack.push(selectedOption);
        this.factorList = factorList;
        
    },
    async getFactorList (type) 
    {
        this.factorListOrg = ( await window.myAjax.post("CarbonReduction/GetList", { type : type }) ).data.items;
        this.factorList = this.factorListOrg;
        this.codeOptionStack= [];
        this.optionSelectedStack = [];
        this.codeOptionStack.push(new Set(this.factorListOrg.map(e => e.Code.slice(0, 1))))
    }

}) 