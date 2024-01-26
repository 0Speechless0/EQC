<template>
  
  <div>
    <ImportTemplate route="NSActivity"
      :hasEdit="true"
      :hasDelete="true"
      :hasSearch="true" 
      searchSign="可輸入通報工程查詢"/>
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