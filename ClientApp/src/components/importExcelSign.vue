<template>
	
    <div>

      <b-card   style="font-size:25px; background-color:#FFF0F5;width: 99%;margin: auto;" >
    <b-card-title style="font-size:50px;color:red;">匯入限制</b-card-title>
    <b-card-text></b-card-text>
        <b-card-text>說明: 匯入資料由資料列、非資料列、標題列組成，資料列被分為不同欄，</b-card-text>
        <b-card-text>1.資料列中第一欄必須有項次，第一欄的值必須是數字</b-card-text>
        <b-card-text>2.非資料列中每一格不可只有數字，如果是，請在數字最左加#</b-card-text>
        <b-card-text>3.標題列必定在第一筆資料列之上，標題列各欄之值為對應之每個資料列中同欄序的欄位名稱</b-card-text>
        <b-card-text>4.{{preWord}}{{demandText}}</b-card-text>
        <b-card-text>5.資料列的欄數必須相同，資料列除該欄為項次外都是匯入的資料</b-card-text>
    <b-card-text>6.匯入必須是xlsx檔</b-card-text>
        <b-card-text>7.資料列每一欄必須有值，空白請用--取代</b-card-text>
                <b-card-text>8.冒號請使用全形</b-card-text>
    <b-card-text></b-card-text>
    <b-card-text></b-card-text>
    <b-card-text style="font-weight: bold;color:blue" >若要新增其他欄位值的匯入，請通知程式開發人員</b-card-text>

      </b-card>
    </div>
</template>

<script >
	import axios from "axios";
	export default{
		props :["route", "additionDemandCols"],
		data: ()=> {
			return {
				preWord : "excel匯入檔標題列必須包括以下欄為名稱:",
				demandText: "",
				field : []
			}
		},
		async mounted() {
			let res = await window.myAjax.post(this.route+"/getDemandFields");
						console.log("mounted Excelsign", res.data);
			let text = "";
			this.field = res.data.data;
			this.field.forEach((element) => {
					if(text != "") text += "、";
					text += element.fieldShowName;
			});
			console.log("text ", text);
			if(this.additionDemandCols)
				this.demandText = this.additionDemandCols.reduce((a, c) =>a+ "、"+ c,text);

		}

	}
</script>