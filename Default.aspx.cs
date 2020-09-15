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
        private readonly AppointmentsManager _appointmentsManager = new AppointmentsManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ucAppointmentCalendarUserControl.MinimumDate = DateTime.Today;
            this.ucAppointmentCalendarUserControl.MaximumDate = DateTime.Today.AddMonths(1);
            ucAppointmentCalendarUserControl.OnDaySelected += UcAppointmentCalendarUserControl_OnDaySelected;

            if (!Page.IsPostBack)
            {
                var days = _appointmentsManager.GetAppointmentDays(DateTime.Now);

                ucAppointmentCalendarUserControl.AppointmentDays = days.Select(d => new UserControls.AppointmentCalendarDay
                {
                    Day = d.Date,
                    Status = (UserControls.AppointmentCalendarDayStatus)Enum.Parse(typeof(UserControls.AppointmentCalendarDayStatus), d.Status.ToString())
                }).ToList();

                ucAppointmentCalendarUserControl.DataBind();
            }
        }

        private void UcAppointmentCalendarUserControl_OnDaySelected(object sender, EventArgs e)
        {
            ucAppointmentCalendarUserControl.AppointmentSlots = _appointmentsManager.GetAppointmentDaySlots(ucAppointmentCalendarUserControl.SelectedDay.Value)
                .Select(s => new UserControls.AppointmentCalendarSlot
                {
                    Status = (UserControls.AppointmentCalendarSlotStatus)Enum.Parse(typeof(UserControls.AppointmentCalendarSlotStatus), s.Status.ToString()),
                    EndTime = s.EndTime,
                    StartTime = s.StartTime
                }).ToList() ;
            ucAppointmentCalendarUserControl.DataBind();
        }
    }
}