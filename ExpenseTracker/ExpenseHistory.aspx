
<%@ Page Title="Expense History" Language="C#"
    MasterPageFile="~/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="ExpenseHistory.aspx.cs"
    Inherits="ExpenseTracker.ExpenseHistory" %>

<asp:Content ID="HistoryHead" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .history-container {
            padding: 30px;
        }

        .history-title {
            color: #263f61;
            margin-top: 0;
            margin-bottom: 8px;
        }

        .history-subtitle {
            color: #718096;
            margin-bottom: 25px;
        }

        .filter-section {
            background: white;
            padding: 20px;
            border-radius: 10px;
            border: 1px solid #e5eaf1;
            margin-bottom: 22px;
        }

        .month-dropdown {
            padding: 10px;
            border: 1px solid #ccd5e0;
            border-radius: 5px;
            min-width: 170px;
            margin-left: 8px;
        }

        .history-table {
            width: 100%;
            border-collapse: collapse;
            background: white;
        }

        .history-table th {
            background: #315f96;
            color: white;
            padding: 13px;
            text-align: left;
        }

        .history-table td {
            padding: 12px;
            border-bottom: 1px solid #e5eaf1;
        }

        .history-table tr:nth-child(even) {
            background: #f7f9fc;
        }

        .history-table input[type="text"] {
            width: 110px;
            padding: 6px;
            border: 1px solid #ccd5e0;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .action-edit {
            display: inline-block;
            background: #315f96;
            color: white;
            padding: 7px 12px;
            border-radius: 4px;
            text-decoration: none;
            margin: 2px;
        }

        .action-delete {
            display: inline-block;
            background: #d9534f;
            color: white;
            padding: 7px 12px;
            border-radius: 4px;
            text-decoration: none;
            margin: 2px;
        }

        .action-edit:hover,
        .action-delete:hover {
            color: white;
            opacity: 0.85;
        }

        .status-message {
            display: block;
            margin-top: 15px;
            color: #526579;
        }

        @media (max-width: 700px) {
            .history-container {
                padding: 15px;
                overflow-x: auto;
            }

            .history-table {
                min-width: 850px;
            }
        }
    </style>

</asp:Content>

<asp:Content ID="HistoryContent"
    ContentPlaceHolderID="MainContent" runat="server">

    <div class="history-container">

        <h1 class="history-title">Expense History</h1>

        <p class="history-subtitle">
            View, edit and delete your previously recorded expenses.
        </p>

        <div class="filter-section">

            <asp:Label ID="lblHistoryMonth" runat="server"
                Text="Filter by month:"
                AssociatedControlID="ddlHistoryMonth" />

            <asp:DropDownList ID="ddlHistoryMonth"
                runat="server"
                CssClass="month-dropdown"
                AutoPostBack="True"
                OnSelectedIndexChanged="ddlHistoryMonth_SelectedIndexChanged">

                <asp:ListItem Text="All Months" Value="0" />
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

        <asp:GridView ID="gvExpenseHistory" runat="server"
            AutoGenerateColumns="False"
            CssClass="history-table"
            GridLines="None"
            DataKeyNames="ExpenseId"
            EmptyDataText="No expenses found for the selected month."
            CellPadding="0"
            OnRowEditing="gvExpenseHistory_RowEditing"
            OnRowCancelingEdit="gvExpenseHistory_RowCancelingEdit"
            OnRowUpdating="gvExpenseHistory_RowUpdating"
            OnRowDeleting="gvExpenseHistory_RowDeleting">

            <Columns>

                <asp:BoundField DataField="ExpenseId"
                    HeaderText="ID"
                    ReadOnly="True" />

                <asp:TemplateField HeaderText="Expense Name">
                    <ItemTemplate>
                        <%# Eval("ExpenseName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditExpenseName"
                            runat="server"
                            Text='<%# Bind("ExpenseName") %>'
                            MaxLength="100" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Amount (₹)">
                    <ItemTemplate>
                        <%# Eval("Amount", "{0:N2}") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditAmount"
                            runat="server"
                            Text='<%# Bind("Amount", "{0:0.00}") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Expense Date">
                    <ItemTemplate>
                        <%# Eval("ExpenseDate", "{0:dd-MM-yyyy}") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditExpenseDate"
                            runat="server"
                            Text='<%# Bind("ExpenseDate", "{0:yyyy-MM-dd}") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>

                        <asp:LinkButton ID="btnEdit"
                            runat="server"
                            Text="Edit"
                            CommandName="Edit"
                            CausesValidation="False"
                            CssClass="action-edit" />

                        <asp:LinkButton ID="btnDelete"
                            runat="server"
                            Text="Delete"
                            CommandName="Delete"
                            CausesValidation="False"
                            CssClass="action-delete"
                            OnClientClick="return confirm('Are you sure you want to permanently delete this expense?');" />

                    </ItemTemplate>

                    <EditItemTemplate>

                        <asp:LinkButton ID="btnUpdate"
                            runat="server"
                            Text="Update"
                            CommandName="Update"
                            CssClass="action-edit" />

                        <asp:LinkButton ID="btnCancel"
                            runat="server"
                            Text="Cancel"
                            CommandName="Cancel"
                            CausesValidation="False"
                            CssClass="action-delete" />

                    </EditItemTemplate>
                </asp:TemplateField>

            </Columns>

        </asp:GridView>

        <asp:Label ID="lblHistoryMessage"
            runat="server"
            CssClass="status-message" />

    </div>

</asp:Content>
