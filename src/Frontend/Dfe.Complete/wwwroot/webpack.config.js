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

					from: '*.{png,svg,ico}',
					to: path.join(__dirname, 'assets/images'),
					context: 'node_modules/govuk-frontend/dist/govuk/assets/rebrand/images'
				},
				{
					from: 'manifest.json',
					to: path.join(__dirname, 'assets/images'),
					context: 'node_modules/govuk-frontend/dist/govuk/assets/rebrand'
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