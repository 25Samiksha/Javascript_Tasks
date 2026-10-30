
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ExpenseTracker
{
    public partial class MonthsSummary : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings[
                "ExpenseTrackerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMonthsSummary();
            }
        }

        private void LoadMonthsSummary()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT
                            DATENAME(MONTH, ExpenseDate) AS MonthName,
                            YEAR(ExpenseDate) AS ExpenseYear,
                            MONTH(ExpenseDate) AS ExpenseMonth,
                            COUNT(*) AS TotalTransactions,
                            SUM(Amount) AS TotalExpenses
                        FROM Expenses
                        GROUP BY
                            YEAR(ExpenseDate),
                            MONTH(ExpenseDate),
                            DATENAME(MONTH, ExpenseDate)
                        ORDER BY
                            YEAR(ExpenseDate) DESC,
                            MONTH(ExpenseDate) DESC";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter =
                            new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvMonthsSummary.DataSource = dt;
                            gvMonthsSummary.DataBind();

                            decimal overallExpenses = 0;

                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["TotalExpenses"] != DBNull.Value)
                                {
                                    overallExpenses +=
                                        Convert.ToDecimal(
                                            row["TotalExpenses"]);
                                }
                            }

                            lblOverallExpenses.Text =
                                "₹" + overallExpenses.ToString("N2");

                            lblSummaryMessage.Text =
                                "Monthly records found: " +
                                dt.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gvMonthsSummary.DataSource = null;
                gvMonthsSummary.DataBind();

                lblOverallExpenses.Text = "₹0.00";

                lblSummaryMessage.Text =
                    "Unable to load the monthly summary. " +
                    "Please check the database connection and table.";

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
