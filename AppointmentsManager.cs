using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentCalendarDemo
{
    public class AppointmentsManager
    {
        public TimeSpan DayStart { get; set; }
        public TimeSpan DayEnd { get; set; }
        public TimeSpan SlotSize { get; set; }

        public AppointmentsManager()
        {
            DayStart = new TimeSpan(8, 0, 0);
            DayEnd = new TimeSpan(14, 0, 0);
            SlotSize = new TimeSpan(1, 0, 0);
        }
        public List<AppointmentSlot> GetAppointmentDaySlots(DateTime date)
        {
            var day = new DateTime(date.Year, date.Month, date.Day);

            var appointmentSlots = new List<AppointmentSlot>();

            var slot = DayStart;

            while (slot < DayEnd)
            {
                appointmentSlots.Add(new AppointmentSlot
                {
                    StartTime = day + slot,
                    EndTime = day + slot + SlotSize,
                    Status = AppointmentSlotStatus.Available
                });

                slot += SlotSize;
            }

            appointmentSlots[0].Status = AppointmentSlotStatus.NotAvailable;
            appointmentSlots[1].Status = AppointmentSlotStatus.PartiallyAvailable;

            return appointmentSlots;
        }

        public List<AppointmentDay> GetAppointmentDays()
        {
            var appointmentDays = new List<AppointmentDay>();

            var dayDate = DateTime.Today;

            for (int i = 1; i <= 70; i++)
            {
                appointmentDays.Add(new AppointmentDay
                {
                    Date = dayDate,
                    Status = (dayDate.DayOfWeek == DayOfWeek.Friday || dayDate.DayOfWeek == DayOfWeek.Saturday) ? AppointmentDayStatus.Holiday : AppointmentDayStatus.Available
                });

                dayDate = dayDate.AddDays(1);
            }

            return appointmentDays;
        }
    }

    public class AppointmentSlot
    {
        public AppointmentSlotStatus Status { get; set; }
        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }
    }

    public class AppointmentDay
    {
        public DateTime Date { get; set; }
        public AppointmentDayStatus Status { get; set; }
    }

    public enum AppointmentSlotStatus
    {
        Available,
        NotAvailable,
        PartiallyAvailable
    }

    public enum AppointmentDayStatus
    {
        Available,
        PartiallyAvailable,
        NotAvailable,
        Holiday
    }
}