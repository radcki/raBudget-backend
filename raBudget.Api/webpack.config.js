/// <binding />
const path = require('path');
var webpack = require("webpack");
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const autoprefixer = require('autoprefixer');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (env = {}, argv = {}) => {
    const isProd = argv.mode === 'production';
    const config = {
        entry: {
            main: './Scripts/main.ts',
        },
        resolve: {
            extensions: [".js", ".ts", ".css", ".scss"]
        },
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    loader: 'ts-loader',
                    exclude: /node_modules/,
                },
                {
                    test: /\.js$/,
                    enforce: 'pre',
                    use: ['source-map-loader'],
                    exclude: /node_modules/,
                },
                {
                    test: /\.s?(c|a)ss$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        { loader: 'css-loader' },
                        {
                            loader: 'postcss-loader',
                            options: {
                            }
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                implementation: require('sass'),
                                webpackImporter: false,
                                sassOptions: {
                                    includePaths: ['./node_modules']
                                },
                            },
                        },
                    ]
                },
            ]
        },
        optimization: {
            splitChunks: {
                chunks: 'all',
                minChunks: 1,
                maxAsyncRequests: 30,
                maxInitialRequests: Infinity,
                automaticNameDelimiter: '~',
                enforceSizeThreshold: 50000,
                cacheGroups: {
                    vendor: {
                        test: /[\\/]node_modules[\\/]/,
                        name(module) {
                            const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];
                            return `vendor.${packageName.replace('@', '')}`;
                        },
                    },
                }
            },
            minimize: true
        },
        plugins: [
            new CleanWebpackPlugin(),
            new HtmlWebpackPlugin({
                template: './Pages/Shared/_Layout.ejs',
                filename: '../../Pages/Shared/_Layout.cshtml',
                publicPath: '~/dist',
                inject: false,
                minify: false,
            }),
            new webpack.SourceMapDevToolPlugin({
                filename: '[name].[hash][ext].map',
                publicPath: '/',
                fileContext: 'public',
            }),
            new MiniCssExtractPlugin({
                filename: '[name].[contenthash].css'
            })
        ],
        output: {
            filename: '[name].[hash].js',
            path: path.resolve(__dirname, 'wwwroot/dist'),
            publicPath: '/',
        }
    };

    //if (!isProd) {
    //    config.devServer = {
    //        index: '',
    //        contentBase: path.resolve(__dirname, '../wwwroot/dist'),
    //        proxy: {
    //            context: () => true, 
    //            target: 'https://localhost:5001',
    //            secure: false 
    //        },
    //        hot: true 
    //    }
    //}
    return config;
};