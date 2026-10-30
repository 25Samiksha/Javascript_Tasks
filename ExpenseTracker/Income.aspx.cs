
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace ExpenseTracker
{
    public partial class Income : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "ExpenseTrackerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadYears();

                ddlIncomeMonth.SelectedValue =
                    DateTime.Now.Month.ToString();

                ddlIncomeYear.SelectedValue =
                    DateTime.Now.Year.ToString();

                LoadIncomeSummary();
            }
        }

        private void LoadYears()
        {
            ddlIncomeYear.Items.Clear();

            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 5;
                 year <= currentYear + 5;
                 year++)
            {
                ddlIncomeYear.Items.Add(
                    new ListItem(
                        year.ToString(),
                        year.ToString()));
            }
        }

        protected void Filter_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            LoadIncomeSummary();
        }

        private void LoadIncomeSummary()
        {
            lblError.Text = "";
            lblOverspending.Text = "";
            lblOverspending.Visible = false;

            int month = Convert.ToInt32(
                ddlIncomeMonth.SelectedValue);

            int year = Convert.ToInt32(
                ddlIncomeYear.SelectedValue);

            decimal income = 0;
            decimal expenses = 0;

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();


                    string incomeQuery = @"
                        SELECT ISNULL(SUM(Amount), 0)
                        FROM MonthlyIncome
                        WHERE IncomeMonth = @Month
                          AND IncomeYear = @Year";

                    using (SqlCommand cmd =
                        new SqlCommand(incomeQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@Month", SqlDbType.Int).Value = month;

                        cmd.Parameters.Add(
                            "@Year", SqlDbType.Int).Value = year;

                        income = Convert.ToDecimal(
                            cmd.ExecuteScalar());
                    }


                    string expenseQuery = @"
                        SELECT ISNULL(SUM(Amount), 0)
                        FROM Expenses
                        WHERE ExpenseDate >= @StartDate
                          AND ExpenseDate < @EndDate";

                    using (SqlCommand cmd =
                        new SqlCommand(expenseQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@StartDate", SqlDbType.DateTime).Value =
                            startDate;

                        cmd.Parameters.Add(
                            "@EndDate", SqlDbType.DateTime).Value =
                            endDate;

                        expenses = Convert.ToDecimal(
                            cmd.ExecuteScalar());
                    }
                }

                decimal balance = income - expenses;

                if (balance < 0)
                {
                    balance = 0;
                }

                decimal savingsPercentage = 0;

                if (income > 0 && income > expenses)
                {
                    savingsPercentage =
                        (income - expenses) / income * 100;
                }

                decimal expensePercentage = 0;

                if (income > 0)
                {
                    expensePercentage =
                        expenses / income * 100;
                }

                lblMonthlyIncome.Text =
                    "₹" + income.ToString("N2");

                lblTotalExpenses.Text =
                    "₹" + expenses.ToString("N2");

                lblAvailableBalance.Text =
                    "₹" + balance.ToString("N2");

                lblSavingsPercentage.Text =
                    savingsPercentage.ToString("N1") + "%";

                lblExpensePercentage.Text =
                    expensePercentage.ToString("N1") + "%";

                decimal barPercentage = expensePercentage;

                if (barPercentage < 0)
                {
                    barPercentage = 0;
                }

                if (barPercentage > 100)
                {
                    barPercentage = 100;
                }

                pnlExpenseProgress.Style["width"] =
                    barPercentage.ToString(
                        "0.0",
                        CultureInfo.InvariantCulture) + "%";

                if (income <= 0 && expenses <= 0)
                {
                    pnlExpenseProgress.Style["background"] =
                        "#9aa7b5";

                    lblFinancialStatus.Text =
                        "No income or expenses recorded for this month.";
                }
                else if (income <= 0 && expenses > 0)
                {
                    pnlExpenseProgress.Style["background"] =
                        "#d9534f";

                    lblFinancialStatus.Text =
                        "Expenses have been recorded, but no income is saved.";

                    lblOverspending.Text =
                        "Expenses exceed the recorded income by ₹" +
                        expenses.ToString("N2");

                    lblOverspending.Visible = true;
                }
                else if (expenses > income)
                {
                    pnlExpenseProgress.Style["background"] =
                        "#d9534f";

                    lblFinancialStatus.Text =
                        "Your expenses have exceeded your monthly income.";

                    lblOverspending.Text =
                        "Overspending: ₹" +
                        (expenses - income).ToString("N2") +
                        ". Your available balance is ₹0.00.";

                    lblOverspending.Visible = true;
                }
                else if (expenses == income)
                {
                    pnlExpenseProgress.Style["background"] =
                        "#d6a442";

                    lblFinancialStatus.Text =
                        "You have used 100% of your income. No balance remains.";
                }
                else if (expensePercentage >= 80)
                {
                    pnlExpenseProgress.Style["background"] =
                        "#d6a442";

                    lblFinancialStatus.Text =
                        "Warning: You have used most of your monthly income.";
                }
                else
                {
                    pnlExpenseProgress.Style["background"] =
                        "#39966b";

                    lblFinancialStatus.Text =
                        "Your expenses are within your income.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text =
                    "Unable to load the summary. Please check your " +
                    "database connection and table column names.";

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
