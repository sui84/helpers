using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Common.Utils
{
    // format :
    //string date = DateTime.Today.ToString("yyyy MMMM", System.Globalization.DateTimeFormatInfo.InvariantInfo );
    //2013 April
    //DateTime.Today.ToString("dddd, dd MMMM yyyy HH:mm:ss")
    //Sunday, 07 April 2013 00:00:00
    //DateTime.Today.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'")
    //Sun, 07 Apr 2013 00:00:00 GMT
    //DateTime.Today.ToString("yyyyMMdd hhmmssfff")
    //20130407 120000000
    //DateTime.Now.ToString("yyyyMMdd hhmmssfff")
    //20130407 041007557

    public class DateTimeHelper
    {
        // foo = true ? (DateTime?)null : new DateTime(0);
        public DateTime? GetDateByString(string dateStr){
            try
            {
                DateTime result = DateTime.ParseExact(dateStr, "yyyy/M/d", System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DateTime? GetDateByString(string dateStr , string format)
        {
            DateTime dt;
            if (DateTime.TryParse(dateStr, out dt))
            {
                return dt;
            }
            return dt;
        }

        private static string defaultNullText = "-";

        public static int CalculateAge(DateTime birthday)
        {
            // get the difference in years
            DateTime today = DateTime.Today;
            int years = today.Year - birthday.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if (today.Month < birthday.Month ||
                (today.Month == birthday.Month &&
                today.Day < birthday.Day))
                years--;
            return years;
        }

        public static DateTime ParseExact(string s, string format)
        {
            if (string.IsNullOrEmpty(s))
            {
                return DateTime.MinValue;
            }
            if (format == "MMM-yyyy")
            {
                format = "d-MMM-yyyy";
                s = "01-" + s;
            }
            return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
        }


        public static DateTime Parse(string s, string format)
        {
            return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
        }

        public static DateTime? QuietParse(string s, string format)
        {
            try
            {
                return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime ConvertFromHundredYearDate(int d)
        {
            return ConvertFromHundredYearDate((double)d);
        }

        public static DateTime ConvertFromHundredYearDate(decimal d)
        {
            return ConvertFromHundredYearDate((double)d);
        }

        public static DateTime ConvertFromHundredYearDate(double d)
        {
            return DateTime.FromOADate(d + 1);
        }

        public static int ConvertToHundredYearDate(DateTime date)
        {
            return (int)(date.ToOADate() - 1);
        }

        public static DateTime? TryParse(string strDateTime)
        {
            DateTime? result = null;
            if (!string.IsNullOrEmpty(strDateTime))
            {
                DateTime resultDate;
                bool isValid = DateTime.TryParse(strDateTime, CultureInfo.CurrentCulture, DateTimeStyles.None, out resultDate);
                if (isValid)
                {
                    result = resultDate;
                }
            }
            return result;
        }

        /// <summary>
        /// Convert from integer to date. (i.e. 82477 -> 08/24/1977)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static DateTime ConvertFromMDYInteger(int i)
        {
            string s = i.ToString("000000");
            return ParseExact(s, "MMddyy");
        }

        /// <summary>
        /// Convert from integer to date. (i.e. 82477 -> 08/24/1977)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static DateTime ConvertFromMDYInteger(decimal i)
        {
            string s = i.ToString("000000");
            return ParseExact(s, "MMddyy");
        }

        /// <summary>
        /// Convert from integer to datetime. (i.e. 92511, 95439 -> 09/25/2011 09:54:39)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertFromMDYAndHMSInteger(int date, int time)
        {
            string s = date.ToString("000000") + time.ToString("000000");
            return ParseExact(s, "MMddyyHHmmss");
        }

        /// <summary>
        /// Convert from integer to datetime. (i.e. 92511, 95439 -> 09/25/2011 09:54:39)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertFromMDYAndHMSInteger(decimal date, decimal time)
        {
            string s = date.ToString("000000") + time.ToString("000000");
            return ParseExact(s, "MMddyyHHmmss");
        }

        #region OLE Date Handling

        /// <summary>
        /// Returns a date string in specified format equivalent to a OLE Automation Date.
        /// Returns specified nullText if the oaDateValue is 0.
        /// </summary>
        /// <param name="oaDateValue">A OLE Automation Date.</param>
        /// <param name="format">A DateTime format string.</param>
        /// <param name="nullText">A string will return when oaDateValue is 0.</param>
        /// <returns></returns>
        public static string OADateToString(decimal oaDateValue, string format, string nullText)
        {
            string dateStr = nullText;
            if (oaDateValue != 0)
            {
                DateTime d = DateTime.FromOADate(Convert.ToDouble(oaDateValue) + 1);
                dateStr = d.ToString(format);
            }
            return dateStr;
        }

        /// <summary>
        /// Returns a date string in specified format equivalent to a OLE Automation Date.
        /// Returns defaul nullText "-" if the oaDateValue is 0.
        /// </summary>
        /// <param name="oaDateValue">A OLE Automation Date.</param>
        /// <param name="format">A DateTime format string.</param>
        /// <returns></returns>
        public static string OADateToString(decimal oaDateValue, string format)
        {
            return OADateToString(oaDateValue, format, defaultNullText);
        }

        #endregion
    }
}
