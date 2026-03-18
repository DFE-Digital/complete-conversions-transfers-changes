import ZapClient from "zaproxy";

export async function generateZapReport() {
    const zapUrl = new URL(process.env.zapUrl || "https://zap:8080");
    const zapOptions = {
        apiKey: process.env.zapApiKey,
        proxy: {
            host: zapUrl.hostname,
            port: Number.parseInt(zapUrl.port, 10),
        },
    };
    const zaproxy = new ZapClient(zapOptions);
    // Wait for passive scanner to finish scanning before generating report
    let recordsRemaining = 100;
    while (recordsRemaining !== 0) {
        await zaproxy.pscan
            .recordsToScan()
            .then((resp) => {
                try {
                    recordsRemaining = Number.parseInt(resp.recordsToScan, 10);
                } catch (err) {
                    if (err instanceof Error) {
                        console.log(`Error converting result: ${err.message}`);
                    } else {
                        console.log("Unknown error during results conversion");
                    }
                    recordsRemaining = 0;
                }
            })
            .catch((err) => {
                console.log(`Error from the ZAP Passive Scan API: ${err}`);
                recordsRemaining = 0;
            });
    }

    await zaproxy.reports
        .generate({
            title: "Report",
            template: "traditional-html",
            reportfilename: "ZAP-Report.html",
            reportdir: "/zap/wrk",
        })
        .then((resp) => {
            console.log(`${JSON.stringify(resp)}`);
        })
        .catch((err) => {
            console.log(`Error from ZAP Report API: ${err}`);
        });
}
