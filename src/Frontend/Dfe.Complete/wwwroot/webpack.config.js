const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
	mode: 'production',
	entry: ["./src/index.js", "./src/index.scss"],
	plugins: [
		new MiniCssExtractPlugin({ filename: 'site.css' }),
		new CopyWebpackPlugin({
			patterns: [
				{
					from: 'src/assets/images/govuk-crest.svg',
					to: 'assets/images/govuk-crest.svg'
				}
			]
		})
	],
	module: {
		rules: [
			{
				test: /\.s[ac]ss$/i,
				use: [
					// Creates `style` nodes from JS strings
					MiniCssExtractPlugin.loader,
					// Translates CSS into CommonJS
					{
						loader: "css-loader",
						options: {
							url: true
						}
					},
					// Compiles Sass to CSS
					"sass-loader",
				],
			},
			{ test: /\.css$/, use: ['style-loader', 'css-loader'] },
			{
				test: /\.(woff2?)$/i,
				type: 'asset/resource',
				dependency: { not: ['url'] }
			},
			{
				test: /\.(gif|svg)$/i,
				exclude: /govuk-crest\.svg$/,
				use: [
					{
						loader: 'file-loader',
						options: {
							emitFile: true,
							name: '/dist/assets/images/[name].[ext]',
							esModule: false
						}
					}
				]
			},
		]
	},
	output: {
		path: path.resolve(__dirname, 'dist'),
		filename: 'site.js',
	}
};