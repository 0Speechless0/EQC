<template>
  
    <div>
      <ImportTemplate route="PerformanceScore"
        :hasEdit="true"
        :hasDelete="true"
        :hasSearch="true" 
        :exceptField="['PrjXMLSeq']"
        :additionDemandCols="['標案編號', '執行機關編號']"
        :disabledEdtiCols="['PSIssueNo', 'PSIssueDate']"
        searchSign="可輸入發文文號查詢"/>
    </div>
  </template>
  
  
  <script>
  import ImportTemplate from "../../components/ImportTemplate";
  export default {
    components: {
      ImportTemplate
    }
  }
  </script>