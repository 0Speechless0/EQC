module.exports = {

    outputDir: '../Content/dist',
    filenameHashing: false,
    productionSourceMap: process.env.NODE_ENV == 'production',
    publicPath: process.env.NODE_ENV == 'production' ? '/Content/dist' : '/',
    configureWebpack: {
        devtool: process.env.NODE_ENV == 'production' ? 'source-map' : '',

        optimization: {
            splitChunks: false
        },
        resolve: {
            alias: {
                'vue$': 'vue/dist/vue.esm.js'
            }
        }
    },
    devServer: {
        proxy: {
            '/webapi': {
                target: '//111.222.333.444:8888',// 你要請求的後端介面ip+port
                changeOrigin: true,// 允許跨域，在本地會建立一個虛擬服務端，然後傳送請求的資料，並同時接收請求的資料，這樣服務端和服務端進行資料的互動就不會有跨域問題
                ws: true,// 開啟webSocket
                pathRewrite: {
                    '^/webapi': '',// 替換成target中的地址
                }
            }
        }
    },

    
}