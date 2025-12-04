const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
	mode: 'production',
	plugins: [
		new CopyPlugin({
			patterns: [
				{ from: path.join(__dirname, 'node_modules/govuk-frontend/dist/govuk/assets'), to: path.join(__dirname, 'assets') },
				{ from: path.join(__dirname, 'node_modules/accessible-autocomplete/dist'), to: path.join(__dirname, 'dist') },
				{ from: path.resolve(__dirname, 'node_modules/@ministryofjustice/frontend/moj/assets'), to: path.resolve(__dirname, 'assets') },
				{ from: path.resolve(__dirname, 'node_modules/@ministryofjustice/frontend/moj/assets'), to: path.resolve(__dirname, 'dist/assets') },
				{ from: path.join(__dirname, 'node_modules/govuk-frontend/dist/govuk/assets'), to: path.join(__dirname, 'dist/assets') },
				// Copy custom DfE images
				{ from: path.join(__dirname, 'govuk/assets'), to: path.join(__dirname, 'assets') },
				{ from: path.join(__dirname, 'govuk/assets'), to: path.join(__dirname, 'dist/assets') },
				// Copy rebrand favicon files to override govuk-frontend favicons (must be after govuk-frontend copy)
				{ from: path.join(__dirname, 'assets/rebrand/images/favicon.ico'), to: path.join(__dirname, 'dist/assets/images/favicon.ico') },
				{ from: path.join(__dirname, 'assets/rebrand/images/favicon.svg'), to: path.join(__dirname, 'dist/assets/images/favicon.svg') },
			],
		})
	],
	output: {
		path: path.resolve(__dirname, 'dist'),
		filename: 'site.js',
	}
};