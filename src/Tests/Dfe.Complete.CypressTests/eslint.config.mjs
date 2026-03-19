import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginCypress from "eslint-plugin-cypress";
import globals from "globals";

export default tseslint.config(
    {
        ignores: [
            "node_modules/**",
            "cypress/videos/**",
            "cypress/reports/**",
            "cypress/downloads/**",
            "zap-reports/**",
        ],
    },
    eslint.configs.recommended,
    tseslint.configs.recommended,
    pluginCypress.configs.recommended,
    {
        languageOptions: {
            globals: {
                ...globals.node,
            },
        },
        rules: {
            "cypress/unsafe-to-chain-command": "off",
            "@typescript-eslint/no-namespace": [
                "error",
                { allowDeclarations: true },
            ],
            "@typescript-eslint/no-explicit-any": "warn",
            "@typescript-eslint/no-unused-expressions": "warn",
        },
    },
    {
        files: ["**/*.js"],
        rules: {
            "@typescript-eslint/no-require-imports": "off",
        },
    },
);
