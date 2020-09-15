using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace AppointmentCalendarDemo.UserControls
{
    public partial class AppointmentCalendarUserControl : System.Web.UI.UserControl
    {
        public event EventHandler OnDaySelected;

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

        public DateTime? SelectedDay
        {
            get
            {
                if (ViewState["SelectedDay"] == null)
                {
                    return null;
                }

                return (DateTime)ViewState["SelectedDay"];
            }
            private set
            {
                ViewState["SelectedDay"] = value;
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

        public List<AppointmentCalendarDay> AppointmentDays
        {
            get
            {
                if (ViewState["AppointmentDays"] == null)
                {
                    return null;
                }

                return (List<AppointmentCalendarDay>)ViewState["AppointmentDays"];
            }
            set
            {
                ViewState["AppointmentDays"] = value;
            }
        }

        public List<AppointmentCalendarSlot> AppointmentSlots
        {
            get
            {
                if (ViewState["AppointmentSlots"] == null)
                {
                    return null;
                }

                return (List<AppointmentCalendarSlot>)ViewState["AppointmentSlots"];
            }
            set
            {
                ViewState["AppointmentSlots"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            rpMonthDays.ItemCommand += RpMonthDays_ItemCommand;
            rpSlots.ItemCommand += RpSlots_ItemCommand;
        }

        public override void DataBind()
        {
            var displayDays = SetEmptyDays(AppointmentDays);
            displayDays = AddHolidayDays(displayDays);

            rpMonthDays.DataSource = displayDays;
            rpMonthDays.DataBind();

            rpSlots.DataSource = this.AppointmentSlots;
            rpSlots.DataBind();
        }

        private void RpSlots_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SlotSelected")
            {
                SelectedSlot = DateTime.Parse(e.CommandArgument.ToString());
                
                //Button slotButton = (Button)e.Item.FindControl("btnSlot");

                //slotButton.CssClass = $"li selected";
            }
        }

        private void RpMonthDays_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DaySelected")
            {
                this.SelectedDay = DateTime.Parse(e.CommandArgument.ToString());

                this.OnDaySelected(this, EventArgs.Empty);
            }
        }

        private List<AppointmentCalendarDay> AddHolidayDays(List<AppointmentCalendarDay> days)
        {
            days.ForEach(day =>
            {
                if (day.Day?.DayOfWeek == DayOfWeek.Friday || day.Day?.DayOfWeek == DayOfWeek.Saturday)
                {
                    day.Status = AppointmentCalendarDayStatus.Holiday;
                }
            });

            return days;
        }

        private List<AppointmentCalendarDay> SetEmptyDays(List<AppointmentCalendarDay> days)
        {
            var displayDays = new List<AppointmentCalendarDay>();

            var monthFirstDay = (int)days[0].Day.Value.DayOfWeek;

            var dummyDaysCount = (int)monthFirstDay - (int)DayOfWeek.Saturday;

            dummyDaysCount += 7;

            for (int i = 1; i < dummyDaysCount; i++)
            {
                displayDays.Insert(0, new AppointmentCalendarDay { Day = null, Status = AppointmentCalendarDayStatus.Empty });
            }

            foreach (var day in days)
            {
                displayDays.Add(new AppointmentCalendarDay { Day = day.Day, Status = day.Status });
            }

            foreach (var displayDay in displayDays)
            {
                if (!displayDay.Day.HasValue || displayDay.Day < MinimumDate || displayDay.Day > MaximumDate)
                {
                    displayDay.Status = AppointmentCalendarDayStatus.Empty;
                }
            }

            return displayDays;
        }

        public DateTime CurrentMonth { get; set; }

        protected void DaySelected(object sender, EventArgs e)
        {
            //if (e != null)
            //{
            //    Response.Write(DateTime.Parse(e.ToString()));
            //}
        }

        protected void rpSlots_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Button slotButton = (Button)e.Item.FindControl("btnSlot");

            if (slotButton != null)
            {
                var appointmentSlot = ((AppointmentCalendarSlot)e.Item.DataItem);

                if (appointmentSlot.StartTime == SelectedSlot)
                {
                    slotButton.CssClass = $"{slotButton.CssClass} selected";
                }
                else
                {
                    slotButton.CssClass = $"{slotButton.CssClass} {appointmentSlot.Status.ToString().ToLower()}";
                }

                if (appointmentSlot.Status == AppointmentCalendarSlotStatus.NotAvailable)
                {
                    slotButton.Enabled = false;
                }
            }
        }

        protected void rpMonthDays_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Button dayButton = (Button)e.Item.FindControl("btnDay");

            if (dayButton != null)
            {
                var status = ((AppointmentCalendarDay)e.Item.DataItem).Status;

                if (((AppointmentCalendarDay)e.Item.DataItem).Day.HasValue && ((AppointmentCalendarDay)e.Item.DataItem).Day.Value == SelectedDay)
                {
                    dayButton.CssClass = $"{dayButton.CssClass} selected";
                }
                else
                {
                    dayButton.CssClass = $"{dayButton.CssClass} {status.ToString().ToLower()}";
                }

                if (status == AppointmentCalendarDayStatus.Holiday || status == AppointmentCalendarDayStatus.Empty)
                {
                    dayButton.Enabled = false;
                }


            }
        }
    }

    [Serializable]
    public class AppointmentCalendarDay
    {
        public DateTime? Day { get; set; }
        public AppointmentCalendarDayStatus Status { get; set; }
    }

    [Serializable]
    public class AppointmentCalendarSlot
    {
        public AppointmentCalendarSlotStatus Status { get; set; }
        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }
    }

    public enum AppointmentCalendarDayStatus
    {
        Available,
        PartiallyAvailable,
        NotAvailable,
        Holiday,
        Empty
    }

    public enum AppointmentCalendarSlotStatus
    {
        Available,
        NotAvailable,
        PartiallyAvailable
    }

}