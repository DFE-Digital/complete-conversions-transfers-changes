import { defineConfig } from "cypress";
import { generateZapReport } from "cypress/plugins/generateZapReport";
import path from "node:path";
import webpackPreprocessor from "@cypress/webpack-preprocessor";
import webpack from "webpack";

export default defineConfig({
    allowCypressEnv: false,
    defaultCommandTimeout: 5000,
    pageLoadTimeout: 20000,
    watchForFileChanges: false,
    chromeWebSecurity: false,
    video: false,
    retries: {
        runMode: 1,
    },
    userAgent: "Complete/1.0 Cypress",
    reporter: "cypress-multi-reporters",
    reporterOptions: {
        reporterEnabled: "mochawesome",
        mochawesomeReporterOptions: {
            reportDir: "cypress/reports/mocha",
            quite: true,
            overwrite: false,
            html: false,
            json: true,
        },
    },
    e2e: {
        excludeSpecPattern: ["*/**/legacy"],
        setupNodeEvents(on, config) {
            const configProcessScopedVariables: Record<string, unknown> = {};

            // Cypress's built-in preprocessor uses tsconfig-paths-webpack-plugin,
            // which infinite-loops because our local "cypress/" directory shares
            // its name with the "cypress" npm package.
            //
            // We replace it with @cypress/webpack-preprocessor using a config
            // modelled on Cypress's internal "batteries-included" defaults:
            //  - ProvidePlugin for process/Buffer globals
            //  - resolve.fallback for Node.js core modules
            //  - resolve.modules with project root first (replaces tsconfig-paths)
            on(
                "file:preprocessor",
                webpackPreprocessor({
                    webpackOptions: {
                        mode: "development",
                        node: {
                            global: true,
                            __filename: true,
                            __dirname: true,
                        },
                        resolve: {
                            extensions: [".ts", ".tsx", ".js", ".jsx", ".mjs", ".json"],
                            modules: [path.resolve(__dirname), "node_modules"],
                            fallback: {
                                buffer: require.resolve("buffer/"),
                                process: require.resolve("process/browser"),
                                stream: require.resolve("stream-browserify"),
                                util: require.resolve("util/"),
                                child_process: false,
                                dns: false,
                                fs: false,
                                net: false,
                                os: false,
                                path: false,
                                tls: false,
                            },
                        },
                        plugins: [
                            new webpack.ProvidePlugin({
                                Buffer: ["buffer", "Buffer"],
                                process: "process/browser",
                            }),
                        ],
                        module: {
                            rules: [
                                {
                                    test: /\.tsx?$/,
                                    exclude: [/node_modules/],
                                    use: [
                                        {
                                            loader: "ts-loader",
                                            options: {
                                                transpileOnly: true,
                                            },
                                        },
                                    ],
                                },
                            ],
                        },
                    },
                }),
            );

            on("before:run", () => {
                process.env = config.env;
            });

            on("after:run", async () => {
                if (process.env.zapReport) {
                    await generateZapReport();
                }
            });

            on("task", {
                log(message) {
                    console.log(message);
                    return null;
                },
                set(keySet: Record<string, unknown>) {
                    Object.entries(keySet).forEach(([key, value]) => {
                        configProcessScopedVariables[key] = value;
                    });
                    return null;
                },
                get(keys: string[]) {
                    const variablesToReturn: Record<string, unknown> = {};
                    keys.forEach((key) => {
                        variablesToReturn[key] = configProcessScopedVariables[key];
                    });
                    return variablesToReturn ;
                },
            });

            config.baseUrl = config.env.url;
            config.expose = {
                url: config.env.url,
                api: config.env.api,
                username: config.env.username,
            };

            return config;
        },
    },
});
