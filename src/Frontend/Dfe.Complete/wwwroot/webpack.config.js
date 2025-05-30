const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
module.exports = {
	mode: 'production',
	entry: ["./src/index.js", "./src/index.scss"],
	plugins: [
		new MiniCssExtractPlugin({ filename: 'site.css' }),
	],
	module: {
		rules: [
			{
				test: /\.s[ac]ss$/i,
				use: [
					// Creates `style` nodes from JS strings
					MiniCssExtractPlugin.loader,
					// Translates CSS into CommonJS
					"css-loader",
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
				use: [
					{
						loader: 'file-loader',
						options: {
							emitFile: false,
							name: '/dist/assets/images/[name].[ext]'
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