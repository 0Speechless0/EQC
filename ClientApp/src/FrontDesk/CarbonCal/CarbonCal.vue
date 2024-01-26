<template>
    <div>


            <div class="row mb-4">
                <div class="col">
                    <input type="text" class="form-control" v-model="code"/>
                </div>
                <div class="col">
                    <select class="form-control" v-model="unit">
                        <option v-for="option, index in unitOption" :value="option" :key="index">
                            {{option}}
                        </option>
                        <option value="%" selected>
                            非上述
                        </option>
                    </select>
                </div>

                <div class="col">
                    <button @click="getData" class="btn btn-success">
                        查詢
                    </button>
                </div>
                
                <div class="col" v-if="result.error" style="color:red">
                    {{result.Memo}}
                </div>

                
            </div>



        <div class="container">
            <table class="table">
                <thead>
                    <tr>
                        <th scope="col">編碼</th>
                        <th scope="col">項目及說明</th>
                        <th scope="col">單位</th>
                        <th scope="col">碳排係數</th>
                        <th scope="col">備註</th>
                    </tr>

                </thead>
                <tbody>
                    <tr v-for="(item, index ) in items" :key="index">
                    
                        <td>
                            {{ item.RefItemCode }}
                        </td>
                        <td>
                            {{ item.Description }}
                        </td>
                        <td>
                            {{ item.ResultUnit }}
                        </td>
                        <td>
                            {{ item.ResultKgCo2e }}
                        </td>
                        <td v-if="!item.error">
                            {{ item.Memo }}
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>

    </div>


</template>

<script>
import axios from "axios";
export default {
    data: () => {
        return {
            unitOption: [],
            items: [],
            result: {},
            code : null,
            unit : "%"
        }
    },
    methods: {
        async getData() {
            this.items = [];
            let res = await axios.get("EQMCarbonEmissionCal/getFactor", {
                params: {
                    code :this.code,
                    compare_unit : this.unit
                }

            });
            this.unitOption = res.data.unitOption;

            if(this.code != null) {
                this.result = res.data.data;
                if(this.result.RStatusCode >= 97)  this.items.push(res.data.data);     

            }         


        }
    },

    mounted() {
        this.getData();
    }
}
</script>