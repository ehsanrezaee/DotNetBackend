using System.Globalization;

namespace ErSoftDev.Common.Utilities
{
    public static class DateExtension
    {
        public static string GrepToPersian(this string grep)
        {
            DateTime grepConvert;
            grepConvert = Convert.ToDateTime(grep);

            PersianCalendar persianDate = new PersianCalendar();
            string outDate = persianDate.GetYear(grepConvert) + "/";
            if (persianDate.GetMonth(grepConvert).ToString().Length == 1) outDate += "0";
            outDate = outDate + persianDate.GetMonth(grepConvert) + "/";
            if (persianDate.GetDayOfMonth(grepConvert).ToString().Length == 1) outDate += "0";
            outDate += persianDate.GetDayOfMonth(grepConvert);

            return outDate;
        }

        public static string GrepToPersianWithTime(this string grep)
        {
            DateTime grepConvert;
            grepConvert = Convert.ToDateTime(grep);

            PersianCalendar persianDate = new PersianCalendar();
            string outDate = persianDate.GetYear(grepConvert) + "/";
            if (persianDate.GetMonth(grepConvert).ToString().Length == 1) outDate += "0";
            outDate = outDate + persianDate.GetMonth(grepConvert) + "/";
            if (persianDate.GetDayOfMonth(grepConvert).ToString().Length == 1) outDate += "0";
            outDate += persianDate.GetDayOfMonth(grepConvert);

            outDate += " " + grepConvert.TimeOfDay;

            return outDate;
        }
    }
}
