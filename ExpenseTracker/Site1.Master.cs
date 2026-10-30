
using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace ExpenseTracker
{
    public partial class Site1 : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetActiveClass(string pageName)
        {
            string currentPage = Path.GetFileName(
                Request.AppRelativeCurrentExecutionFilePath);

            if (string.Equals(
                currentPage,
                pageName,
                StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }

            return "";
        }
    }
}
