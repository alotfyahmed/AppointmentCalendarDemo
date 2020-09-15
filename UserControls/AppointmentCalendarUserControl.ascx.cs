using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppointmentCalendarDemo.UserControls
{
    public partial class AppointmentCalendarUserControl : System.Web.UI.UserControl
    {
        private readonly AppointmentsManager _appointmentsManager = new AppointmentsManager();

        public DateTime? SelectedSlot
        {
            get
            {
                if (ViewState["SelectedSlot"] == null)
                {
                    return null;
                }

                return (DateTime)ViewState["SelectedSlot"];
            }
            set
            {
                ViewState["SelectedSlot"] = value;
            }
        }

        public DateTime MinimumDate
        {
            get
            {
                if (ViewState["MinimumDate"] == null)
                {
                    return DateTime.Today;
                }

                return (DateTime)ViewState["MinimumDate"];
            }
            set
            {
                ViewState["MinimumDate"] = value;
            }
        }

        public DateTime MaximumDate
        {
            get
            {
                if (ViewState["MaximumDate"] == null)
                {
                    return DateTime.Today;
                }

                return (DateTime)ViewState["MaximumDate"];
            }
            set
            {
                ViewState["MaximumDate"] = value;
            }
        }

        public List<AppointmentDay> AppointmentDays { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            rpMonthDays.ItemCommand += RpMonthDays_ItemCommand;
            rpSlots.ItemCommand += RpSlots_ItemCommand;

            if (!Page.IsPostBack)
            {
                var days = _appointmentsManager.GetAppointmentDays(DateTime.Now).Select(d => new AppointmentCalendarMonthDay
                {
                    Day = d.Date,
                    Status = (AppointmentCalendarDayStatus)Enum.Parse(typeof(AppointmentCalendarDayStatus), d.Status.ToString())
                }).ToList();

                SetEmptyDays(days);
                AddHolidayDays(days);

                rpMonthDays.DataSource = days;
                rpMonthDays.DataBind();
            }
        }
        public override void DataBind()
        {
            base.DataBind();
        }

        private void RpSlots_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SlotSelected")
            {
                Response.Write(e.CommandArgument.ToString());
            }
        }

        private void RpMonthDays_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DaySelected")
            {
                rpSlots.DataSource = new AppointmentsManager().GetAppointmentDaySlots(DateTime.Parse(e.CommandArgument.ToString()));
                rpSlots.DataBind();
            }
        }

        private void AddHolidayDays(List<AppointmentCalendarMonthDay> days)
        {
            foreach (var day in days)
            {
                if (day.Day?.DayOfWeek == DayOfWeek.Friday || day.Day?.DayOfWeek == DayOfWeek.Saturday)
                {
                    day.Status = AppointmentCalendarDayStatus.Holiday;
                }
            }
        }

        private void SetEmptyDays(List<AppointmentCalendarMonthDay> days)
        {
            foreach (var calendarDay in days)
            {
                if (calendarDay.Day < MinimumDate || calendarDay.Day > MaximumDate)
                {
                    calendarDay.Status = AppointmentCalendarDayStatus.Empty;
                }
            }

            var monthFirstDay = (int)days[0].Day.Value.DayOfWeek;

            var dummyDaysCount = (int)monthFirstDay - (int)DayOfWeek.Saturday;

            dummyDaysCount += 7;

            for (int i = 1; i < dummyDaysCount; i++)
            {
                days.Insert(0, new AppointmentCalendarMonthDay { Day = null, Status = AppointmentCalendarDayStatus.Empty });
            }
        }

        public DateTime CurrentMonth { get; set; }

        protected void DaySelected(object sender, EventArgs e)
        {
            //if (e != null)
            //{
            //    Response.Write(DateTime.Parse(e.ToString()));
            //}
        }
    }

    internal class AppointmentCalendarMonthDay
    {
        public DateTime? Day { get; set; }
        public AppointmentCalendarDayStatus Status { get; set; }
    }

    internal class AppointmentCalendarWeekDay
    {
        public string Name { get; set; }
    }

    public enum AppointmentCalendarDayStatus
    {
        Available,
        PartiallyAvailable,
        NotAvailable,
        Holiday,
        Empty
    }
}