using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace AppointmentCalendarDemo.UserControls
{
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

    [Serializable]
    public class AppointmentCalendarDay
    {
        public DateTime? Day { get; set; }
        public AppointmentCalendarDayStatus Status { get; set; }
    }

    [Serializable]
    public class AppointmentCalendarSlot
    {
        public DateTime EndTime { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public AppointmentCalendarSlotStatus Status { get; set; }
    }

    public partial class AppointmentCalendarUserControl : System.Web.UI.UserControl
    {
        public event EventHandler OnDaySelected;

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

        public DateTime CurrentMonth { get; set; }

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

        protected DateTime? DisplayedMonth
        {
            get
            {
                if (ViewState["DisplayedMonth"] == null)
                {
                    return DateTime.Today;
                }

                return (DateTime)ViewState["DisplayedMonth"];
            }
            set
            {
                ViewState["DisplayedMonth"] = value;
            }
        }

        private List<AppointmentCalendarDay> _displayDays = new List<AppointmentCalendarDay>();

        public override void DataBind()
        {
            if (!DisplayedMonth.HasValue)
            {
                DisplayedMonth = AppointmentDays.First().Day.Value;
            }

            _displayDays = CloneDays(AppointmentDays);

            _displayDays = _displayDays.Where(d => d.Day.Value.Month == DisplayedMonth.Value.Month).ToList();

            AddMissingDays(_displayDays);
            AddEmptyDays(_displayDays);
            AddHolidayDays(_displayDays);
            

            btnPrevMonth.Enabled = (DisplayedMonth.Value.Month > AppointmentDays.Where(d => d.Day.HasValue).First().Day.Value.Month);
            btnNextMonth.Enabled = (DisplayedMonth.Value.Month < AppointmentDays.Where(d => d.Day.HasValue).Last().Day.Value.Month);

            rpMonthDays.DataSource = _displayDays;
            rpMonthDays.DataBind();

            rpSlots.DataSource = AppointmentSlots;
            rpSlots.DataBind();
        }


        protected string GetHijriDate(object dataItem)
        {
            var calendarDay = (AppointmentCalendarDay)dataItem;

            if (calendarDay != null && calendarDay.Day.HasValue)
            {
                return calendarDay.Day.Value.ToString("dd MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ar-SA"));
            }

            return string.Empty;
        }

        protected string HijriDsiplayMonths
        {
            get
            {
                var hijriMonths = _displayDays.Where(i => i.Day.HasValue)
               .Select(d => d.Day.Value.ToString("MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("ar-SA")))
               .Distinct();

                var hijriDisplayMonths = string.Empty;

                hijriMonths.ForEach(h => hijriDisplayMonths = $"{hijriDisplayMonths} {h}");

                return (hijriDisplayMonths);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            rpMonthDays.ItemCommand += RpMonthDays_ItemCommand;
            rpSlots.ItemCommand += RpSlots_ItemCommand;
        }

        protected void rpMonthDays_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            LinkButton dayButton = (LinkButton)e.Item.FindControl("btnDay");

            if (dayButton != null)
            {
                var appointmentCalendarDay = ((AppointmentCalendarDay)e.Item.DataItem);

                //var status = ((AppointmentCalendarDay)e.Item.DataItem).Status;

                if (appointmentCalendarDay.Day.HasValue)
                {
                    if (appointmentCalendarDay.Day.Value == SelectedDay)
                    {
                        dayButton.CssClass = $"{dayButton.CssClass} selected";
                    }

                    if (appointmentCalendarDay.Day.Value.Date == DateTime.Today)
                    {
                        dayButton.CssClass = $"{dayButton.CssClass} today";
                    }
                }
                else
                {
                    dayButton.CssClass = $"{dayButton.CssClass} {appointmentCalendarDay.Status.ToString().ToLower()}";
                }


                if (appointmentCalendarDay.Status == AppointmentCalendarDayStatus.Holiday)
                {
                    dayButton.Enabled = false;
                    dayButton.CssClass = $"{dayButton.CssClass} holiday";
                }

                if (appointmentCalendarDay.Status == AppointmentCalendarDayStatus.Empty)
                {
                    dayButton.Enabled = false;
                    dayButton.CssClass = $"{dayButton.CssClass} empty";
                }
            }
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

        private void AddEmptyDays(List<AppointmentCalendarDay> displayDays)
        {
            var monthFirstDay = (int)displayDays[0].Day.Value.DayOfWeek;

            var dummyDaysCount = monthFirstDay - (int)DayOfWeek.Saturday;

            dummyDaysCount += 7;

            for (int i = 1; i < dummyDaysCount; i++)
            {
                displayDays.Insert(0, new AppointmentCalendarDay { Day = null, Status = AppointmentCalendarDayStatus.Empty });
            }

            var monthLasttDay = displayDays.Last().Day.Value.DayOfWeek;

            dummyDaysCount = (int)DayOfWeek.Saturday - (int)monthLasttDay;

            for (int i = 0; i < dummyDaysCount; i++)
            {
                displayDays.Add(new AppointmentCalendarDay { Day = null, Status = AppointmentCalendarDayStatus.Empty });
            }

            foreach (var displayDay in displayDays)
            {
                if (!displayDay.Day.HasValue || displayDay.Day < MinimumDate || displayDay.Day > MaximumDate)
                {
                    displayDay.Status = AppointmentCalendarDayStatus.Empty;
                }
            }
        }

        private void AddHolidayDays(List<AppointmentCalendarDay> days)
        {
            days.ForEach(day =>
            {
                if (day.Day?.DayOfWeek == DayOfWeek.Friday || day.Day?.DayOfWeek == DayOfWeek.Saturday)
                {
                    day.Status = AppointmentCalendarDayStatus.Holiday;
                }
            });
        }

        private List<AppointmentCalendarDay> AddMissingDays(List<AppointmentCalendarDay> displayDays)
        {
            var minDate = displayDays.Min(i => i.Day?.Date);

            if (minDate.HasValue && minDate.Value.Day > 1)
            {
                for (int i = minDate.Value.Day - 1; i > 0; i--)
                {
                    displayDays.Insert(0, new AppointmentCalendarDay
                    {
                        Day = new DateTime(minDate.Value.Year, minDate.Value.Month, i),
                        Status = AppointmentCalendarDayStatus.Empty
                    });
                }
            }

            var maxDate = displayDays.Max(i => i.Day?.Date);

            if (maxDate.HasValue && maxDate.Value.Day < DateTime.DaysInMonth(maxDate.Value.Year, maxDate.Value.Month))
            {
                for (int i = maxDate.Value.Day + 1; i <= DateTime.DaysInMonth(maxDate.Value.Year, maxDate.Value.Month); i++)
                {
                    displayDays.Add( new AppointmentCalendarDay
                    {
                        Day = new DateTime(maxDate.Value.Year, maxDate.Value.Month, i),
                        Status = AppointmentCalendarDayStatus.Empty
                    });
                }
            }

            return displayDays;
        }

        private List<AppointmentCalendarDay> CloneDays(List<AppointmentCalendarDay> days)
        {
            var displayDays = new List<AppointmentCalendarDay>();

            foreach (var day in days)
            {
                displayDays.Add(new AppointmentCalendarDay { Day = day.Day, Status = day.Status });
            }

            return displayDays;
        }

        private void RpMonthDays_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DaySelected")
            {
                SelectedDay = DateTime.Parse(e.CommandArgument.ToString());

                OnDaySelected(this, EventArgs.Empty);
            }
        }

        private void RpSlots_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SlotSelected")
            {
                SelectedSlot = DateTime.Parse(e.CommandArgument.ToString());

                Response.Write(SelectedSlot);
            }
        }

        protected void BtnPrevMonth_Click(object sender, EventArgs e)
        {
            DisplayedMonth = DisplayedMonth.Value.AddMonths(-1);
            DataBind();
        }

        protected void BtnNextMonth_Click(object sender, EventArgs e)
        {
            DisplayedMonth = DisplayedMonth.Value.AddMonths(1);
            DataBind();
        }
    }
}