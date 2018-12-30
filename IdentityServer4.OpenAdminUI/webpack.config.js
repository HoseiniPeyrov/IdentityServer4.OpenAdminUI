var path = require('path');
var webpack = require('webpack');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = {
    entry: path.resolve(__dirname, 'Admin/ClientApp/main.js'),
    plugins: [
        new VueLoaderPlugin()
    ],
    output: {
        path: path.resolve(__dirname, './wwwroot/js'),
        publicPath: '/wwwroot/js/',
        filename: 'admin-app.js'
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    //loaders: {
                    //    // Since sass-loader (weirdly) has SCSS as its default parse mode, we map
                    //    // the "scss" and "sass" values for the lang attribute to the right configs here.
                    //    // other preprocessors should work out of the box, no loader config like this necessary.
                    //    'css': 'vue-style-loader!css-loader',
                    //    'scss': 'vue-style-loader!css-loader!sass-loader',
                    //    'sass': 'vue-style-loader!css-loader!sass-loader?indentedSyntax',
                    //}
                    // other vue-loader options go here
                }
            },
            //{
            //    test: /\.tsx?$/,
            //    loader: 'ts-loader',
            //    exclude: /node_modules/,
            //    options: {
            //        appendTsSuffixTo: [/\.vue$/],
            //    }
            //},
            //{
            //    test: /\.(png|jpg|gif|svg)$/,
            //    loader: 'file-loader',
            //    options: {
            //        name: '[name].[ext]?[hash]'
            //    }
            //},
            {
                test: /\.(c|sa|sc)ss$/,
                use: [
                    'vue-style-loader',
                    'css-loader',
                    //{
                    //    loader: 'sass-loader',
                    //    options: {
                    //        indentedSyntax: true
                    //    }
                    //}
                ]
            }
        ]
    },
    resolve: {
        extensions: ['.ts', '.js', '.vue', '.json'],
        alias: {
            'vue$': 'vue/dist/vue.esm.js',
            '@': path.resolve(__dirname, 'Admin/ClientApp')
        }
    },
    devServer: {
        historyApiFallback: true,
        noInfo: true
    },
    performance: {
        hints: false
    },
    devtool: '#eval-source-map'
};

if (process.env.NODE_ENV === 'production') {
    module.exports.devtool = '#source-map';
    // http://vue-loader.vuejs.org/en/workflow/production.html
    module.exports.plugins = (module.exports.plugins || []).concat([
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        }),
        new webpack.optimize.UglifyJsPlugin({
            sourceMap: true,
            compress: {
                warnings: false
            }
        }),
        new webpack.LoaderOptionsPlugin({
            minimize: true
        })
    ]);
}