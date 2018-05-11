'use strict'
const {
    VueLoaderPlugin
} = require('vue-loader'),
    path = require('path');

module.exports = {
    mode: 'development',
    entry: {
        main: './Vue/views/main.js',
        person: './Vue/views/person.js',
    },
    output: {
        path: path.join(__dirname, 'wwwroot'),
        filename: "dist/[name].min.js"
    },
    module: {
        rules: [{
            test: /\.vue$/,
            loader: 'vue-loader'
        },
        // this will apply to both plain `.js` files
        // AND `<script>` blocks in `.vue` files
        {
            test: /\.js$/,
            loader: 'babel-loader'
        },
        // this will apply to both plain `.css` files
        // AND `<style>` blocks in `.vue` files
        {
            test: /\.css$/,
            use: [
                'vue-style-loader',
                'css-loader'
            ]
        }
        ]
    },
    plugins: [
        new VueLoaderPlugin()
    ]
}