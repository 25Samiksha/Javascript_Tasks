
<%@ Page Title="Expenses" Language="C#"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="ExpenseTracker.aspx.cs"
    Inherits="ExpenseTracker.ExpenseTracker" %>

<asp:Content ID="PageHead"
    ContentPlaceHolderID="head"
    runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="ExpenseTracker.css" rel="stylesheet" type="text/css" />
    <script src="ExpenseTracker.js" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="PageContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="container">

        <h1>MY EXPENSE TRACKER</h1>

        <p class="page-description">Manage your expenses</p>

        <div class="expense-form">

            <h2>Add New Expense</h2>

            <div class="form-group">

                <asp:Label ID="lblmonth" runat="server"
                    AssociatedControlID="ddlmonths"
                    Text="Select Month" />

                <asp:DropDownList ID="ddlmonths" runat="server"
                    CssClass="form-control"
                    AutoPostBack="True"
                    OnSelectedIndexChanged="ddlmonths_SelectedIndexChanged">

                    <asp:ListItem Text="Select Month" Value="" />
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

            <div class="form-group">

                <asp:Label ID="lblExpenseDate" runat="server"
                    AssociatedControlID="txtExpenseDate"
                    Text="Select Expense Date" />

                <asp:TextBox ID="txtExpenseDate" runat="server"
                    CssClass="form-control"
                    TextMode="Date" />

            </div>

            <div class="form-group">

                <asp:Label ID="lblpms" runat="server"
                    AssociatedControlID="txtpms"
                    Text="Pocket Money / Salary" />

                <asp:TextBox ID="txtpms" runat="server"
                    CssClass="form-control"
                    TextMode="Number"
                    min="0"
                    step="1"
                    ReadOnly="True"
                    Height="34px"
                    Width="567px" />

                <asp:Button ID="btnSaveIncome" runat="server"
                    Text="Save Monthly Income"
                    CssClass="add-button"
                    OnClick="btnSaveIncome_Click"
                    Enabled="False" />

                <asp:Button ID="btnUpdateIncome" runat="server"
                    Text="Update Income"
                    CssClass="add-button"
                    OnClick="btnUpdateIncome_Click"
                    Visible="False" />

                <asp:Label ID="lblIncomeStatus" runat="server" />

            </div>

            <div class="form-group">

                <asp:Label ID="lblExpname" runat="server"
                    AssociatedControlID="txtExpName"
                    Text="Expense Name" />

                <asp:TextBox ID="txtExpName" runat="server"
                    CssClass="form-control expense-name"
                    MaxLength="100" />

            </div>

            <div class="form-group">

                <asp:Label ID="lblAmount" runat="server"
                    AssociatedControlID="txtAmount"
                    Text="Expense Amount" />

                <asp:TextBox ID="txtAmount" runat="server"
                    CssClass="form-control expense-amount"
                    TextMode="Number"
                    min="1"
                    step="1" />

            </div>

            <asp:Button ID="btnAddExp" runat="server"
                Text="+ Add Expense"
                CssClass="add-button expense-add"
                OnClick="btnAddExp_Click" />

            <div class="form-group">

                <asp:Label ID="lblpm" runat="server"
                    Text="Monthly Income: ₹0.00"
                    CssClass="total-amount"
                    Visible="False" />

            </div>

            <div class="form-group">

                <asp:Label ID="lbltotalexp" runat="server"
                    Text="Total Expenses: ₹0.00"
                    CssClass="total-amount"
                    Visible="False" />

            </div>

            <div class="form-group">

                <asp:Label ID="lblsaving" runat="server"
                    Text="Remaining Savings: ₹0.00"
                    CssClass="total-amount"
                    Visible="False" />

            </div>

        </div>

    </div>

</asp:Content>
