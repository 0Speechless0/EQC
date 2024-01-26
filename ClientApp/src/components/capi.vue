<template>
    <div>
        <div>
            <h1>全球匯率</h1>
            <div>{{capiData}}</div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            let capiData = ''

            return {
                capiData: capiData
            }
        },
        created() {
            let self = this
            console.log(self)
            self.$http.get('https://tw.rter.info/capi.php')
                .then(response => {
                    self.capiData = response
                })
        }
    }
</script>