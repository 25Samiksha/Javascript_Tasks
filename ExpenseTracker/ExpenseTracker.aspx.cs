
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace ExpenseTracker
{
    public partial class ExpenseTracker : System.Web.UI.Page
    {
        private string connectionString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["ExpenseTrackerConnection"]
            .ConnectionString;

        private bool ShowSummary
        {
            get
            {
                return ViewState["ShowSummary"] != null &&
                    (bool)ViewState["ShowSummary"];
            }
            set
            {
                ViewState["ShowSummary"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowSummary = false;
                LoadMonthlyIncome();
            }

            SetExpenseDateRange();
        }

        private void SetExpenseDateRange()
        {
            if (string.IsNullOrEmpty(ddlmonths.SelectedValue))
            {
                txtExpenseDate.Attributes.Remove("min");
                txtExpenseDate.Attributes.Remove("max");
                return;
            }

            int month = Convert.ToInt32(ddlmonths.SelectedValue);
            int year = DateTime.Today.Year;

            DateTime firstDay = new DateTime(year, month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            txtExpenseDate.Attributes["min"] =
                firstDay.ToString("yyyy-MM-dd");

            txtExpenseDate.Attributes["max"] =
                lastDay.ToString("yyyy-MM-dd");
        }

        protected void ddlmonths_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            txtExpenseDate.Text = "";

            SetExpenseDateRange();
            LoadMonthlyIncome();
        }

        private bool TryGetSelectedMonth(out int month)
        {
            month = 0;

            if (string.IsNullOrEmpty(ddlmonths.SelectedValue))
            {
                ShowMessage("Please select a month.");
                return false;
            }

            month = Convert.ToInt32(ddlmonths.SelectedValue);
            return true;
        }

        private bool TryGetExpenseDate(
            int month, out DateTime expenseDate)
        {
            expenseDate = DateTime.MinValue;

            if (!DateTime.TryParseExact(
                txtExpenseDate.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out expenseDate))
            {
                ShowMessage("Please select an expense date.");
                return false;
            }

            if (expenseDate.Month != month ||
                expenseDate.Year != DateTime.Today.Year)
            {
                ShowMessage(
                    "Please select a date from the selected month " +
                    "in the current year.");
                return false;
            }

            return true;
        }

        protected void btnAddExp_Click(object sender, EventArgs e)
        {
            int month;

            if (!TryGetSelectedMonth(out month))
                return;

            string expenseName = txtExpName.Text.Trim();

            if (string.IsNullOrWhiteSpace(expenseName))
            {
                ShowMessage("Please enter an expense name.");
                return;
            }

            if (expenseName.Length > 100)
            {
                ShowMessage(
                    "Expense name cannot exceed 100 characters.");
                return;
            }

            int amount;

            if (!int.TryParse(txtAmount.Text.Trim(), out amount) ||
                amount <= 0)
            {
                ShowMessage(
                    "Please enter a valid whole-number expense amount.");
                return;
            }

            DateTime expenseDate;

            if (!TryGetExpenseDate(month, out expenseDate))
                return;

            // Check the actual remaining balance before adding.
            decimal remainingBalance = GetRemainingBalance(month);

            if (remainingBalance <= 0m || amount > remainingBalance)
            {
                ShowMessage(
                    "Insufficient balance! Your available balance is ₹" +
                    Math.Max(0m, remainingBalance).ToString("N2") +
                    ". Please enter a lower expense amount or update your income.");

                return;
            }

            string query = @"
                INSERT INTO dbo.Expenses
                    (ExpenseName, Amount, ExpenseDate, IncomeMonth)
                VALUES
                    (@ExpenseName, @Amount, @ExpenseDate, @IncomeMonth)";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@ExpenseName", SqlDbType.NVarChar, 100)
                    .Value = expenseName;

                cmd.Parameters.Add(
                    "@Amount", SqlDbType.Int).Value = amount;

                cmd.Parameters.Add(
                    "@ExpenseDate", SqlDbType.Date)
                    .Value = expenseDate.Date;

                cmd.Parameters.Add(
                    "@IncomeMonth", SqlDbType.Int).Value = month;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            ShowSummary = true;

            txtExpName.Text = "";
            txtAmount.Text = "";
            txtExpenseDate.Text = "";

            LoadMonthlyIncome();

            ShowMessage("Expense added successfully.");
        }

        private decimal GetRemainingBalance(int month)
        {
            int year = DateTime.Today.Year;
            decimal monthlyIncome = 0m;
            decimal totalExpenses = 0m;

            string incomeQuery = @"
                SELECT Amount
                FROM dbo.MonthlyIncome
                WHERE IncomeYear = @Year
                  AND IncomeMonth = @Month";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(incomeQuery, con))
            {
                cmd.Parameters.Add(
                    "@Year", SqlDbType.Int).Value = year;

                cmd.Parameters.Add(
                    "@Month", SqlDbType.Int).Value = month;

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    monthlyIncome = Convert.ToDecimal(result);
            }

            string expenseQuery = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM dbo.Expenses
                WHERE ExpenseDate >= @StartDate
                  AND ExpenseDate < @EndDate";

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(expenseQuery, con))
            {
                cmd.Parameters.Add(
                    "@StartDate", SqlDbType.Date).Value = startDate;

                cmd.Parameters.Add(
                    "@EndDate", SqlDbType.Date).Value = endDate;

                con.Open();

                totalExpenses =
                    Convert.ToDecimal(cmd.ExecuteScalar());
            }

            return monthlyIncome - totalExpenses;
        }

        private void LoadMonthlyIncome()
        {
            if (string.IsNullOrEmpty(ddlmonths.SelectedValue))
            {
                txtpms.Text = "";
                txtpms.ReadOnly = true;

                btnSaveIncome.Enabled = false;
                btnSaveIncome.Visible = true;

                btnUpdateIncome.Visible = false;
                btnUpdateIncome.Text = "Update Income";

                lblIncomeStatus.Text = "";

                ViewState["MonthlyIncome"] = 0m;

                UpdateBalance();
                return;
            }

            int month = Convert.ToInt32(ddlmonths.SelectedValue);
            int year = DateTime.Today.Year;

            string query = @"
                SELECT Amount
                FROM dbo.MonthlyIncome
                WHERE IncomeYear = @Year
                  AND IncomeMonth = @Month";

            object income;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@Year", SqlDbType.Int).Value = year;

                cmd.Parameters.Add(
                    "@Month", SqlDbType.Int).Value = month;

                con.Open();
                income = cmd.ExecuteScalar();
            }

            if (income != null && income != DBNull.Value)
            {
                decimal monthlyIncome = Convert.ToDecimal(income);

                ViewState["MonthlyIncome"] = monthlyIncome;

                txtpms.Text = monthlyIncome.ToString("0");
                txtpms.ReadOnly = true;

                btnSaveIncome.Enabled = false;
                btnSaveIncome.Visible = false;

                btnUpdateIncome.Visible = true;
                btnUpdateIncome.Text = "Update Income";

                lblIncomeStatus.Text = "Income already saved.";
            }
            else
            {
                ViewState["MonthlyIncome"] = 0m;

                txtpms.Text = "";
                txtpms.ReadOnly = false;

                btnSaveIncome.Enabled = true;
                btnSaveIncome.Visible = true;

                btnUpdateIncome.Visible = false;
                btnUpdateIncome.Text = "Update Income";

                lblIncomeStatus.Text = "Enter income for this month.";
            }

            UpdateBalance();
        }

        protected void btnSaveIncome_Click(object sender, EventArgs e)
        {
            int month;

            if (!TryGetSelectedMonth(out month))
                return;

            int incomeAmount;

            if (!int.TryParse(txtpms.Text.Trim(), out incomeAmount) ||
                incomeAmount < 0)
            {
                ShowMessage(
                    "Please enter a valid whole-number income amount.");
                return;
            }

            int year = DateTime.Today.Year;

            // Save the income only if this month's income does not exist.
            string query = @"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.MonthlyIncome
                    WHERE IncomeYear = @Year
                      AND IncomeMonth = @Month
                )
                BEGIN
                    INSERT INTO dbo.MonthlyIncome
                        (IncomeYear, IncomeMonth, Amount)
                    VALUES
                        (@Year, @Month, @Amount)
                END";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@Year", SqlDbType.Int).Value = year;

                cmd.Parameters.Add(
                    "@Month", SqlDbType.Int).Value = month;

                SqlParameter amountParameter =
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal);

                amountParameter.Precision = 18;
                amountParameter.Scale = 2;
                amountParameter.Value = incomeAmount;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadMonthlyIncome();

            ShowMessage("Monthly income saved successfully.");
        }

        protected void btnUpdateIncome_Click(object sender, EventArgs e)
        {
            // First click: enable editing of the saved income.
            if (btnUpdateIncome.Text == "Update Income")
            {
                int month;

                if (!TryGetSelectedMonth(out month))
                    return;

                txtpms.ReadOnly = false;

                btnSaveIncome.Visible = false;
                btnSaveIncome.Enabled = false;

                btnUpdateIncome.Text = "Save Updated Income";

                lblIncomeStatus.Text =
                    "Edit the income and click Save Updated Income.";

                return;
            }

            // Second click: validate and save the edited income.
            int selectedMonth;

            if (!TryGetSelectedMonth(out selectedMonth))
                return;

            int incomeAmount;

            if (!int.TryParse(txtpms.Text.Trim(), out incomeAmount) ||
                incomeAmount < 0)
            {
                ShowMessage(
                    "Please enter a valid whole-number income amount.");
                return;
            }

            int year = DateTime.Today.Year;

            string query = @"
                UPDATE dbo.MonthlyIncome
                SET Amount = @Amount
                WHERE IncomeYear = @Year
                  AND IncomeMonth = @Month";

            int rowsUpdated;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@Year", SqlDbType.Int).Value = year;

                cmd.Parameters.Add(
                    "@Month", SqlDbType.Int).Value = selectedMonth;

                SqlParameter amountParameter =
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal);

                amountParameter.Precision = 18;
                amountParameter.Scale = 2;
                amountParameter.Value = incomeAmount;

                con.Open();
                rowsUpdated = cmd.ExecuteNonQuery();
            }

            if (rowsUpdated > 0)
            {
                LoadMonthlyIncome();

                ShowMessage("Monthly income updated successfully.");
            }
            else
            {
                LoadMonthlyIncome();

                ShowMessage(
                    "No saved income record was found for this month. " +
                    "Please save the income first.");
            }
        }

        private void UpdateBalance()
        {
            lblpm.Visible = ShowSummary;
            lbltotalexp.Visible = ShowSummary;
            lblsaving.Visible = ShowSummary;

            if (!ShowSummary)
                return;

            decimal monthlyIncome = 0m;
            decimal totalExpenses = 0m;

            if (ViewState["MonthlyIncome"] != null)
            {
                monthlyIncome =
                    Convert.ToDecimal(ViewState["MonthlyIncome"]);
            }

            if (!string.IsNullOrEmpty(ddlmonths.SelectedValue))
            {
                int month = Convert.ToInt32(ddlmonths.SelectedValue);
                int year = DateTime.Today.Year;

                string query = @"
                    SELECT ISNULL(SUM(Amount), 0)
                    FROM dbo.Expenses
                    WHERE ExpenseDate >= @StartDate
                      AND ExpenseDate < @EndDate";

                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1);

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@StartDate", SqlDbType.Date).Value = startDate;

                    cmd.Parameters.Add(
                        "@EndDate", SqlDbType.Date).Value = endDate;

                    con.Open();

                    totalExpenses =
                        Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }

            ViewState["TotalExpenses"] = totalExpenses;

            // Display zero instead of negative savings.
            decimal savings = Math.Max(
                0m, monthlyIncome - totalExpenses);

            lblpm.Text =
                "Monthly Income: ₹" + monthlyIncome.ToString("N2");

            lbltotalexp.Text =
                "Total Expenses: ₹" + totalExpenses.ToString("N2");

            lblsaving.Text =
                "Remaining Savings: ₹" + savings.ToString("N2");
        }

        private void ShowMessage(string message)
        {
            string safeMessage =
                message.Replace("\\", "\\\\")
                       .Replace("'", "\\'")
                       .Replace("\r", "")
                       .Replace("\n", "\\n");

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "ExpenseMessage",
                "alert('" + safeMessage + "');",
                true);
        }
    }
}
