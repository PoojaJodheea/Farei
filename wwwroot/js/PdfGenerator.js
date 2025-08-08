function generatePdf() {
    const tableHtml = document.getElementById("dataTable");
    if (!tableHtml) {
        alert("Table not found!");
        return;
    }

    const styles = `
        <style>
            table { width: 100%; border-collapse: collapse; }
            th, td { border: 1px solid #000; padding: 8px; text-align: left; }
            th { background-color: #f2f2f2; }
        </style>
    `;

    const fullHtml = `
        <html>
            <head>
                <meta charset="UTF-8">
                ${styles}
            </head>
            <body>
                ${tableHtml.outerHTML}
            </body>
        </html>
    `;

    fetch('/Request/GeneratePdfFromHtml', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ htmlContent: fullHtml })
    })
        .then(response => {
            if (!response.ok) throw new Error("PDF generation failed.");
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = "table.pdf";
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            alert("Failed to generate PDF: " + error.message);
        });
}