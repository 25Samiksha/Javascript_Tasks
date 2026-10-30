
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace ExpenseTracker
{
    public partial class ExpenseHistory : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings[
                "ExpenseTrackerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlHistoryMonth.SelectedValue = "0";
                BindExpenseHistory();
            }
        }

        protected void ddlHistoryMonth_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            gvExpenseHistory.EditIndex = -1;
            BindExpenseHistory();
        }

        private void BindExpenseHistory()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    int selectedMonth =
                        Convert.ToInt32(
                            ddlHistoryMonth.SelectedValue);

                    string query;

                    if (selectedMonth == 0)
                    {
                        query = @"
                            SELECT ExpenseId,
                                   ExpenseName,
                                   Amount,
                                   ExpenseDate
                            FROM Expenses
                            ORDER BY ExpenseDate DESC,
                                     ExpenseId DESC";
                    }
                    else
                    {
                        query = @"
                            SELECT ExpenseId,
                                   ExpenseName,
                                   Amount,
                                   ExpenseDate
                            FROM Expenses
                            WHERE MONTH(ExpenseDate) = @Month
                            ORDER BY ExpenseDate DESC,
                                     ExpenseId DESC";
                    }

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        if (selectedMonth != 0)
                        {
                            cmd.Parameters.Add(
                                "@Month", SqlDbType.Int).Value =
                                selectedMonth;
                        }

                        using (SqlDataAdapter adapter =
                            new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();

                            adapter.Fill(dt);

                            gvExpenseHistory.DataSource = dt;
                            gvExpenseHistory.DataBind();

                            lblHistoryMessage.Text =
                                "Total records: " + dt.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gvExpenseHistory.DataSource = null;
                gvExpenseHistory.DataBind();

                lblHistoryMessage.Text =
                    "Unable to load expense history. " +
                    "Please check your database connection.";

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        protected void gvExpenseHistory_RowEditing(
            object sender, GridViewEditEventArgs e)
        {
            gvExpenseHistory.EditIndex = e.NewEditIndex;
            BindExpenseHistory();
        }

        protected void gvExpenseHistory_RowCancelingEdit(
            object sender, GridViewCancelEditEventArgs e)
        {
            gvExpenseHistory.EditIndex = -1;
            BindExpenseHistory();
        }

        protected void gvExpenseHistory_RowUpdating(
            object sender, GridViewUpdateEventArgs e)
        {
            int expenseId = Convert.ToInt32(
                gvExpenseHistory.DataKeys[e.RowIndex].Value);

            GridViewRow row = gvExpenseHistory.Rows[e.RowIndex];

            TextBox txtName =
                (TextBox)row.FindControl("txtEditExpenseName");

            TextBox txtAmount =
                (TextBox)row.FindControl("txtEditAmount");

            TextBox txtDate =
                (TextBox)row.FindControl("txtEditExpenseDate");

            string expenseName = txtName.Text.Trim();
            string amountText = txtAmount.Text.Trim();
            string dateText = txtDate.Text.Trim();

            decimal amount;
            DateTime expenseDate;

            if (string.IsNullOrWhiteSpace(expenseName))
            {
                lblHistoryMessage.Text =
                    "Expense name cannot be empty.";
                e.Cancel = true;
                return;
            }

            if (!decimal.TryParse(
                amountText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out amount) || amount <= 0)
            {
                lblHistoryMessage.Text =
                    "Enter a valid amount greater than zero.";
                e.Cancel = true;
                return;
            }

            if (!DateTime.TryParseExact(
                dateText,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out expenseDate))
            {
                lblHistoryMessage.Text =
                    "Enter the date in yyyy-MM-dd format.";
                e.Cancel = true;
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        UPDATE Expenses
                        SET ExpenseName = @ExpenseName,
                            Amount = @Amount,
                            ExpenseDate = @ExpenseDate
                        WHERE ExpenseId = @ExpenseId";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@ExpenseName", SqlDbType.NVarChar, 100)
                            .Value = expenseName;

                        SqlParameter amountParameter =
                            cmd.Parameters.Add(
                                "@Amount", SqlDbType.Decimal);

                        amountParameter.Precision = 10;
                        amountParameter.Scale = 2;
                        amountParameter.Value = amount;

                        cmd.Parameters.Add(
                            "@ExpenseDate", SqlDbType.DateTime)
                            .Value = expenseDate;

                        cmd.Parameters.Add(
                            "@ExpenseId", SqlDbType.Int)
                            .Value = expenseId;

                        con.Open();

                        int affected = cmd.ExecuteNonQuery();

                        gvExpenseHistory.EditIndex = -1;
                        BindExpenseHistory();

                        lblHistoryMessage.Text = affected > 0
                            ? "Expense updated successfully."
                            : "Expense not found. No record was updated.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblHistoryMessage.Text =
                    "Unable to update expense. Check the entered values.";

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        protected void gvExpenseHistory_RowDeleting(
            object sender, GridViewDeleteEventArgs e)
        {
            int expenseId = Convert.ToInt32(
                gvExpenseHistory.DataKeys[e.RowIndex].Value);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query =
                        "DELETE FROM Expenses WHERE ExpenseId = @ExpenseId";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@ExpenseId", SqlDbType.Int)
                            .Value = expenseId;

                        con.Open();

                        int affected = cmd.ExecuteNonQuery();

                        gvExpenseHistory.EditIndex = -1;
                        BindExpenseHistory();

                        lblHistoryMessage.Text = affected > 0
                            ? "Expense deleted successfully."
                            : "Expense not found. No record was deleted.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblHistoryMessage.Text =
                    "Unable to delete this expense.";

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
