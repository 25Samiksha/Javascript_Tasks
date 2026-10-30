
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ExpenseTracker
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlDashboardMonth.SelectedValue =
                    DateTime.Now.Month.ToString();

                LoadDashboard();
            }
        }

        protected void ddlDashboardMonth_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings[
                    "ExpenseTrackerConnection"].ConnectionString;

            int selectedMonth =
                Convert.ToInt32(ddlDashboardMonth.SelectedValue);

            int selectedYear = DateTime.Now.Year;

            decimal income = 0m;
            decimal expenses = 0m;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                con.Open();

                // Get income for the selected month and current year.
                string incomeQuery = @"
                    SELECT ISNULL(SUM(Amount), 0)
                    FROM MonthlyIncome
                    WHERE IncomeMonth = @Month
                      AND IncomeYear = @Year";

                using (SqlCommand cmd =
                    new SqlCommand(incomeQuery, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@Month", selectedMonth);

                    cmd.Parameters.AddWithValue(
                        "@Year", selectedYear);

                    income = Convert.ToDecimal(
                        cmd.ExecuteScalar());
                }

                // Get expenses for the selected month and current year.
                string expenseQuery = @"
                    SELECT ISNULL(SUM(Amount), 0)
                    FROM Expenses
                    WHERE MONTH(ExpenseDate) = @Month
                      AND YEAR(ExpenseDate) = @Year";

                using (SqlCommand cmd =
                    new SqlCommand(expenseQuery, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@Month", selectedMonth);

                    cmd.Parameters.AddWithValue(
                        "@Year", selectedYear);

                    expenses = Convert.ToDecimal(
                        cmd.ExecuteScalar());
                }
            }

            // Calculate the actual balance.
            decimal actualSavings = income - expenses;

            // Display zero instead of negative savings.
            decimal savings = Math.Max(0m, actualSavings);

            // Calculate budget usage.
            // This can exceed 100% when expenses exceed income.
            decimal budgetUsed = 0m;

            if (income > 0m)
            {
                budgetUsed = (expenses / income) * 100m;
            }

            lblDashboardIncome.Text =
                "₹" + income.ToString("N2");

            lblDashboardExpenses.Text =
                "₹" + expenses.ToString("N2");

            lblDashboardSavings.Text =
                "₹" + savings.ToString("N2");

            lblDashboardBudget.Text =
                budgetUsed.ToString("N1") + "%";
        }
    }
}
