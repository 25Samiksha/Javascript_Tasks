
<%@ Page Title="Dashboard" Language="C#"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="ExpenseTracker.Dashboard" %>

<asp:Content ID="DashboardHead" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <style type="text/css">
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f3f6fb;
            color: #263445;
        }

        .main {
            flex: 1;
            min-width: 0;
            padding: 35px;
        }

        .main h1 {
            margin-top: 0;
            font-size: 32px;
        }

        .subtitle {
            color: #718096;
            margin-bottom: 25px;
        }

        .month-filter {
            margin-bottom: 28px;
        }

        .month-dropdown {
            padding: 10px 14px;
            border: 1px solid #ccd5e0;
            border-radius: 6px;
            background: white;
            min-width: 170px;
            font-size: 14px;
        }

        .cards {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 22px;
        }

        .card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 3px 12px rgba(0, 0, 0, 0.06);
            border: 1px solid #e5eaf1;
        }

        .card h3 {
            color: #68778b;
            font-size: 15px;
            margin-top: 0;
            margin-bottom: 18px;
        }

        .amount {
            font-size: 28px;
            font-weight: bold;
            color: #263f61;
            overflow-wrap: anywhere;
        }

        .income { border-top: 4px solid #39966b; }
        .expenses { border-top: 4px solid #df6565; }
        .savings { border-top: 4px solid #4986d6; }
        .budget { border-top: 4px solid #d6a442; }

        .bottom-link {
            margin-top: 30px;
        }

        .bottom-link a {
            display: inline-block;
            background: #315f96;
            color: white;
            text-decoration: none;
            padding: 12px 20px;
            border-radius: 6px;
        }

        .bottom-link a:hover {
            background: #244b78;
        }

        @media (max-width: 700px) {
            .main { padding: 20px; }
            .cards { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>

<asp:Content ID="DashboardContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="main">

        <h1>Dashboard</h1>

        <p class="subtitle">Overview of your monthly finances</p>

        <div class="month-filter">

            <asp:Label ID="lblSelectMonth" runat="server"
                Text="Select Month:"
                AssociatedControlID="ddlDashboardMonth" />

            <asp:DropDownList ID="ddlDashboardMonth"
                runat="server"
                CssClass="month-dropdown"
                AutoPostBack="True"
                OnSelectedIndexChanged="ddlDashboardMonth_SelectedIndexChanged">

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

        <div class="cards">

            <div class="card income">
                <h3>Monthly Income</h3>
                <div class="amount">
                    <asp:Label ID="lblDashboardIncome" runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="card expenses">
                <h3>Total Expenses</h3>
                <div class="amount">
                    <asp:Label ID="lblDashboardExpenses" runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="card savings">
                <h3>Remaining Savings</h3>
                <div class="amount">
                    <asp:Label ID="lblDashboardSavings" runat="server" Text="₹0.00" />
                </div>
            </div>

            <div class="card budget">
                <h3>Budget Used</h3>
                <div class="amount">
                    <asp:Label ID="lblDashboardBudget" runat="server" Text="0%" />
                </div>
            </div>

        </div>

        <div class="bottom-link">
            <a href="ExpenseTracker.aspx">Manage Expenses</a>
        </div>

    </div>

</asp:Content>
