namespace CMS.Shared.Handlers
{
    using System;
    using Enums;

    public class DateTimeHandler
    {
        public static string ToDbFilterDate(string date)
        {
            var parts = date.Split("-");
            return parts[2] + "-" + parts[1] + "-" + parts[0];
        }

        public static DateTime ToLocale(DateTime dateUtc, string timeZone) => TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById(timeZone));

        public static DateTime? ToDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            if (!date.Contains("-"))
            {
                return null;
            }

            var dt = date.Split('-');
            return new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]));
        }

        public static DateTime ToNotNullDate(string date)
        {
            var dt = date.Split('-');
            return new DateTime(Convert.ToInt32(dt[0]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[2]));
        }

        public static DateTime? ToDateTime(string dateTime)
        {
            // 2019-04-10 10:50 PM
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            var dtS = dateTime.Split('-');
            var dtY = dtS[2].Split(' ');
            var dtH = dtY[1].Trim().Split(':');
            var hour = int.Parse(dtH[0]);
            if (dtY[2].ToUpper() == "PM")
            {
                hour = (hour % 12) + 12;
            }

            var convertedDateTime = new DateTime(int.Parse(dtS[0]), int.Parse(dtS[1]), int.Parse(dtY[0]), hour, int.Parse(dtH[1]), 0);
            return convertedDateTime;
        }

        public static DateTimeOffset? ToDateTimeOffset(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            var dtS = dateTime.Split('-');
            var dtY = dtS[2].Split(' ');
            var dtH = dtY[1].Trim().Split(':');
            var hour = int.Parse(dtH[0]);
            if (dtY[2].ToUpper() == "PM")
            {
                hour = (hour % 12) + 12;
            }

            var convertedDateTime = new DateTimeOffset(int.Parse(dtY[0]), int.Parse(dtS[1]), int.Parse(dtS[0]), hour, int.Parse(dtH[1]), 0, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));
            return convertedDateTime;
        }

        public static int Difference(DateTime startDate, DateTime endDate, CalenderType calendarType)
        {
            TimeSpan timeSpan;
            switch (calendarType)
            {
                case CalenderType.Years:
                    var difference = endDate.Year - startDate.Year;
                    if (startDate > endDate.AddYears(-difference))
                    {
                        difference--;
                    }

                    return difference;
                case CalenderType.Hours:
                    timeSpan = endDate - startDate;
                    return (int)timeSpan.TotalHours;
                case CalenderType.Minutes:
                    timeSpan = endDate - startDate;
                    return (int)timeSpan.TotalMinutes;
                default:
                    return 0;
            }
        }

        public static int DifferenceInMinutes(TimeSpan startTime, TimeSpan endTime)
        {
            var totalHours = endTime - startTime;
            var midNight = new TimeSpan(24, 0, 0);
            if (startTime <= endTime)
            {
                return (int)totalHours.TotalMinutes;
            }

            var f1 = midNight - startTime;
            totalHours = f1 + endTime;
            return (int)totalHours.TotalMinutes;
        }
    }
}