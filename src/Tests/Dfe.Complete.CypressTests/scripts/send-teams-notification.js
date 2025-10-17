import { post } from "axios";
import { existsSync, readFileSync } from "fs";
import { join } from "path";

// used in GitHub Actions to send Cypress test results to Microsoft Teams channel via webhook
async function sendTeamsNotification() {
    try {
        // Read the combined report JSON to get test statistics and failures
        let reportStats = { tests: 0, passes: 0, failures: 0 };
        let failedTests = [];

        const reportPath = join(process.cwd(), "mochareports", "report.json");

        if (existsSync(reportPath)) {
            const reportData = JSON.parse(readFileSync(reportPath, "utf8"));

            if (reportData && reportData.stats) {
                reportStats = {
                    tests: reportData.stats.tests || 0,
                    passes: reportData.stats.passes || 0,
                    failures: reportData.stats.failures || 0,
                };
            }

            // Extract failed tests with their error messages
            if (reportData && reportData.results) {
                failedTests = extractFailedTests(reportData.results);
            }
        } else {
            console.warn("Report file not found at:", reportPath);
        }

        // Define card style based on test results
        const hasFailures = reportStats.failures > 0;
        const style = hasFailures ? "attention" : "good";
        const statusText = hasFailures ? "**Cypress Test Run Failed** ❌" : "**Cypress Test Run Passed** ✅";

        // Build the adaptive card body
        const cardBody = [
            {
                type: "TextBlock",
                wrap: true,
                text: statusText,
                size: "large",
                horizontalAlignment: "center",
            },
            {
                type: "TextBlock",
                wrap: true,
                text: "**Branch:** " + process.env.GITHUB_REF,
            },
            {
                type: "TextBlock",
                wrap: true,
                text: "**Workflow:** " + process.env.GITHUB_WORKFLOW,
            },
            {
                type: "TextBlock",
                wrap: true,
                text: "**Environment:** " + process.env.ENVIRONMENT,
            },
            {
                type: "FactSet",
                facts: [
                    { title: "Total Tests:", value: reportStats.tests.toString() },
                    { title: "Passed:", value: reportStats.passes.toString() },
                    { title: "Failed:", value: reportStats.failures.toString() },
                ],
            },
        ];

        // Add failed test details if there are any failures
        if (hasFailures && failedTests.length > 0) {
            cardBody.push({
                type: "TextBlock",
                wrap: true,
                text: "**Failed Tests:**",
                weight: "bolder",
                spacing: "medium",
            });

            // Add each failed test as a separate text block
            failedTests.forEach((test, index) => {
                // Limit to first 10 failed tests to avoid message size limits
                if (index < 10) {
                    cardBody.push({
                        type: "TextBlock",
                        wrap: true,
                        text: `**${index + 1}.** ${test.fullTitle}`,
                        weight: "bolder",
                        spacing: "small",
                    });

                    // Add error message, truncated if too long
                    const errorMessage = truncateText(test.errorMessage, 500);
                    cardBody.push({
                        type: "TextBlock",
                        wrap: true,
                        text: `*Error:* ${errorMessage}`,
                        spacing: "none",
                        isSubtle: true,
                    });
                }
            });

            // Add note if there are more failures than displayed
            if (failedTests.length > 10) {
                cardBody.push({
                    type: "TextBlock",
                    wrap: true,
                    text: `*... and ${failedTests.length - 10} more failed tests. See full report for details.*`,
                    spacing: "small",
                    isSubtle: true,
                });
            }
        }

        // Add information link
        cardBody.push({
            type: "TextBlock",
            wrap: true,
            text:
                "**See more information:** [" +
                process.env.INFORMATION_LINK +
                "](" +
                process.env.INFORMATION_LINK +
                ")",
            spacing: "medium",
        });

        // Create Teams message card
        const message = {
            type: "message",
            attachments: [
                {
                    contentType: "application/vnd.microsoft.card.adaptive",
                    contentUrl: null,
                    content: {
                        $schema: "http://adaptivecards.io/schemas/adaptive-card.json",
                        type: "AdaptiveCard",
                        version: "1.2",
                        body: [
                            {
                                type: "Container",
                                style: style,
                                items: cardBody,
                            },
                        ],
                    },
                },
            ],
        };

        // Send the notification
        await post(process.env.TEAMS_WEBHOOK_URL, message);
        console.log("Message sent to Teams successfully");
    } catch (error) {
        console.error("Error sending notification to Teams:", error);
        process.exit(1);
    }
}

/**
 * Recursively extract failed tests from the Cypress report structure
 */
function extractFailedTests(results) {
    const failedTests = [];

    function processTestItem(item) {
        // Check if this is a test with fail: true
        if (item.fail === true && item.fullTitle && item.err) {
            failedTests.push({
                fullTitle: item.fullTitle,
                errorMessage: cleanErrorMessage(item.err.message || "No error message available"),
            });
        }

        // Process nested tests
        if (item.tests && Array.isArray(item.tests)) {
            item.tests.forEach(processTestItem);
        }

        // Process nested suites
        if (item.suites && Array.isArray(item.suites)) {
            item.suites.forEach(processTestItem);
        }
    }

    results.forEach(processTestItem);
    return failedTests;
}

/**
 * Clean and format error messages for better readability
 */
function cleanErrorMessage(errorMessage) {
    if (!errorMessage) return "No error message available";

    // Remove stack traces and keep only the main error message
    const lines = errorMessage.split("\n");
    const relevantLines = [];

    for (const line of lines) {
        // Stop at stack trace indicators
        if (line.includes("    at ") || line.includes("From Your Spec Code:")) {
            break;
        }

        // Skip lines that are just URLs or technical details
        if (!line.includes("https://on.cypress.io/") && !line.includes("webpack://") && line.trim() !== "") {
            relevantLines.push(line.trim());
        }
    }

    return relevantLines.join(" ").trim() || errorMessage.split("\n")[0];
}

/**
 * Truncate text to specified length with ellipsis
 */
function truncateText(text, maxLength) {
    if (!text || text.length <= maxLength) return text;
    return text.substring(0, maxLength) + "...";
}

sendTeamsNotification();
