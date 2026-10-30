
<%@ Page Title="Months Summary" Language="C#"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="MonthsSummary.aspx.cs"
    Inherits="ExpenseTracker.MonthsSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .summary-container {
            padding: 25px;
        }

        .summary-title {
            font-size: 28px;
            font-weight: bold;
            color: #2f6f9f;
            margin-bottom: 5px;
        }

        .summary-subtitle {
            color: #666;
            margin-bottom: 20px;
        }

        .summary-card {
            background: #eaf3fb;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 8px;
            border: 1px solid #ddd;
        }

        .summary-label {
            font-size: 16px;
            color: #555;
        }

        .summary-amount {
            font-size: 26px;
            font-weight: bold;
            color: #2f6f9f;
        }

        .summary-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .summary-table th {
            background: #2f6f9f;
            color: white;
            padding: 10px;
            border: 1px solid #ddd;
        }

        .summary-table td {
            padding: 10px;
            border: 1px solid #ddd;
        }

        .summary-message {
            margin-top: 15px;
            color: #555;
        }

        .pdf-button {
            background: #2f6f9f;
            color: white;
            border: none;
            padding: 10px 18px;
            border-radius: 5px;
            cursor: pointer;
            margin-bottom: 20px;
        }

        /* Print only the report */
        @media print {

            @page {
                size: A4;
                margin: 15mm;
            }

            body {
                margin: 0 !important;
                padding: 0 !important;
                background: white !important;
            }

            /* Hide everything initially */
            body * {
                visibility: hidden !important;
            }

            /* Show only the report and its contents */
            .summary-container,
            .summary-container * {
                visibility: visible !important;
            }

            .summary-container {
                position: absolute !important;
                left: 0 !important;
                top: 0 !important;
                width: 100% !important;
                padding: 0 !important;
                margin: 0 !important;
                box-sizing: border-box !important;
            }

            /* Hide the PDF button */
            .no-print {
                display: none !important;
                visibility: hidden !important;
            }

            .summary-title {
                color: black !important;
                font-size: 24px;
                text-align: center;
            }

            .summary-subtitle {
                color: black !important;
                text-align: center;
            }

            .summary-card {
                background: white !important;
                border: 1px solid #999 !important;
                border-radius: 0 !important;
                page-break-inside: avoid;
            }

            .summary-label,
            .summary-amount,
            .summary-message {
                color: black !important;
            }

            .summary-table {
                width: 100% !important;
                border-collapse: collapse !important;
                font-size: 11px;
            }

            .summary-table th {
                background: #eeeeee !important;
                color: black !important;
                border: 1px solid #999 !important;
                padding: 8px !important;
            }

            .summary-table td {
                color: black !important;
                border: 1px solid #999 !important;
                padding: 8px !important;
            }

            .summary-table tr {
                page-break-inside: avoid;
            }
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="summary-container">

        <div class="summary-title">
            Monthly Expense Summary
        </div>

        <div class="summary-subtitle">
            View your expenses month by month.
        </div>

        <div class="no-print">
            <asp:Button
                ID="btnDownloadPDF"
                runat="server"
                Text="Download PDF"
                CssClass="pdf-button"
                OnClientClick="window.print(); return false;" />
        </div>

        <div class="summary-card">

            <div class="summary-label">
                Overall Expenses
            </div>

            <asp:Label
                ID="lblOverallExpenses"
                runat="server"
                CssClass="summary-amount"
                Text="₹0.00">
            </asp:Label>

        </div>

        <asp:GridView
            ID="gvMonthsSummary"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="summary-table"
            GridLines="None"
            EmptyDataText="No monthly expense records found.">

            <Columns>

                <asp:BoundField
                    DataField="MonthName"
                    HeaderText="Month" />

                <asp:BoundField
                    DataField="ExpenseYear"
                    HeaderText="Year" />

                <asp:BoundField
                    DataField="TotalTransactions"
                    HeaderText="Total Transactions" />

                <asp:BoundField
                    DataField="TotalExpenses"
                    HeaderText="Total Expenses"
                    DataFormatString="{0:N2}" />

            </Columns>

        </asp:GridView>

        <div class="summary-message">
            <asp:Label
                ID="lblSummaryMessage"
                runat="server">
            </asp:Label>
        </div>

    </div>

</asp:Content>
