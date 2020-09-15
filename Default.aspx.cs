using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppointmentCalendarDemo
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.AppointmentCalendarUserControl.MinimumDate = DateTime.Today;
            this.AppointmentCalendarUserControl.MaximumDate = DateTime.Today.AddMonths(1);
        }
    }
}