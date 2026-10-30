
<%@ Page Title="Income" Language="C#"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="Income.aspx.cs"
    Inherits="ExpenseTracker.Income" %>

<asp:Content ID="IncomeHead" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .income-container {
            padding: 30px;
            color: #263445;
        }

        .income-container h1 {
            margin-top: 0;
            color: #263f61;
            font-size: 32px;
        }

        .subtitle {
            color: #718096;
            margin-bottom: 25px;
        }

        .filter-card, .summary-card, .progress-card {
            background: #fff;
            border: 1px solid #e5eaf1;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 3px 12px rgba(0,0,0,0.04);
        }

        .filter-card {
            margin-bottom: 25px;
        }

        .filter-row {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }

        .filter-field {
            flex: 1;
            min-width: 180px;
        }

        .filter-field label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #526579;
        }

        .filter-dropdown {
            width: 100%;
            max-width: 300px;
            padding: 11px;
            border: 1px solid #ccd5e0;
            border-radius: 6px;
            background: #fff;
        }

        .summary-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 20px;
            margin-bottom: 25px;
        }

        .summary-card h3 {
            margin-top: 0;
            margin-bottom: 18px;
            color: #718096;
            font-size: 15px;
        }

        .amount {
            font-size: 27px;
            font-weight: bold;
            color: #263f61;
            overflow-wrap: anywhere;
        }

        .income-card {
            border-top: 4px solid #39966b;
        }

        .expense-card {
            border-top: 4px solid #df6565;
        }

        .balance-card {
            border-top: 4px solid #4986d6;
        }

        .savings-card {
            border-top: 4px solid #d6a442;
        }

        .progress-card h3 {
            margin-top: 0;
            color: #718096;
        }

        .progress-track {
            width: 100%;
            height: 20px;
            background: #e8edf4;
            border-radius: 10px;
            overflow: hidden;
            margin: 15px 0;
        }

        .progress-fill {
            display: block;
            width: 0%;
            height: 20px;
            background: #39966b;
            border-radius: 10px;
        }

        .status-message {
            display: block;
            margin-top: 15px;
            line-height: 1.6;
        }

        .warning {
            display: block;
            margin-top: 15px;
            padding: 14px;
            border-radius: 8px;
            background: #fff3f0;
            color: #b42318;
            border: 1px solid #ffd2cc;
        }

        .success {
            color: #218653;
        }

        .error-message {
            display: block;
            margin-top: 15px;
            color: #b42318;
        }

        .manage-button {
            display: inline-block;
            margin-top: 25px;
            padding: 12px 20px;
            background: #315f96;
            color: #fff;
            text-decoration: none;
            border-radius: 6px;
        }

        .manage-button:hover {
            background: #244b78;
            color: #fff;
        }

        @media (max-width: 700px) {
            .income-container {
                padding: 18px;
            }

            .summary-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>

</asp:Content>

<asp:Content ID="IncomeContent"
    ContentPlaceHolderID="MainContent" runat="server">

    <div class="income-container">

        <h1>Income Summary</h1>

        <p class="subtitle">
            Track your monthly income, expenses and available savings.
        </p>

        <div class="filter-card">

            <h3>Select Month and Year</h3>

            <div class="filter-row">

                <div class="filter-field">

                    <asp:Label ID="lblMonth"
                        runat="server"
                        Text="Select Month:"
                        AssociatedControlID="ddlIncomeMonth" />

                    <asp:DropDownList ID="ddlIncomeMonth"
                        runat="server"
                        CssClass="filter-dropdown"
                        AutoPostBack="True"
                        OnSelectedIndexChanged="Filter_SelectedIndexChanged">

                        <asp:ListItem Text="January" Value="1" />
                        <asp:ListItem Text="February" Value="2" />
                        <asp:ListItem Text="March" Value="3" />
                        <asp:ListItem Text="April" Value="4" />
                        <asp:ListItem Text="May" Value="5" />
                        <asp:ListItem Text="June" Value="6" />
                        <asp:ListItem Text="July" Value="7" />
                        <asp:ListItem Text="August" Value="8" />
                        <asp:ListItem Text="September" Value="9" />
                        <asp:ListItem Text="October" Value="10" />
                        <asp:ListItem Text="November" Value="11" />
                        <asp:ListItem Text="December" Value="12" />

                    </asp:DropDownList>

                </div>

                <div class="filter-field">

                    <asp:Label ID="lblYear"
                        runat="server"
                        Text="Select Year:"
                        AssociatedControlID="ddlIncomeYear" />

                    <asp:DropDownList ID="ddlIncomeYear"
                        runat="server"
                        CssClass="filter-dropdown"
                        AutoPostBack="True"
                        OnSelectedIndexChanged="Filter_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

            </div>

        </div>

        <div class="summary-grid">

            <div class="summary-card income-card">
                <h3>Monthly Income</h3>
                <div class="amount">
                    <asp:Label ID="lblMonthlyIncome"
                        runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="summary-card expense-card">
                <h3>Total Expenses</h3>
                <div class="amount">
                    <asp:Label ID="lblTotalExpenses"
                        runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="summary-card balance-card">
                <h3>Available Balance</h3>
                <div class="amount">
                    <asp:Label ID="lblAvailableBalance"
                        runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="summary-card savings-card">
                <h3>Savings Percentage</h3>
                <div class="amount">
                    <asp:Label ID="lblSavingsPercentage"
                        runat="server" Text="0.0%" />
                </div>
            </div>

        </div>

        <div class="progress-card">

            <h3>Monthly Expense Progress</h3>

            <div class="amount">
                <asp:Label ID="lblExpensePercentage"
                    runat="server" Text="0.0%" />
            </div>

            <div class="progress-track">

                <asp:Panel ID="pnlExpenseProgress"
                    runat="server"
                    CssClass="progress-fill" />

            </div>

            <asp:Label ID="lblFinancialStatus"
                runat="server"
                CssClass="status-message" />

            <asp:Label ID="lblOverspending"
                runat="server"
                Visible="False"
                CssClass="warning" />

            <asp:Label ID="lblError"
                runat="server"
                CssClass="error-message" />

        </div>

        <asp:HyperLink ID="lnkManageExpenses"
            runat="server"
            NavigateUrl="~/ExpenseTracker.aspx"
            CssClass="manage-button"
            Text="Manage Expenses" />

    </div>

</asp:Content>
