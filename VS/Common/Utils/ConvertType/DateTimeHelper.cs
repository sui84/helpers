using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Common.Utils.ConvertType
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

        /// <summary>
        /// 当前日期
        /// </summary>
        /// <returns></returns>
        public static string GetToday()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 当前日期自定义格式
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetToday(string format)
        {
            return DateTime.Now.ToString(format);
        }
        /// <summary>
        /// 当前日期 加添加，减天数 -1、1
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GetDate(int i)
        {
            DateTime dt = DateTime.Now;
            dt = dt.AddDays(i);
            return dt.ToString("yyyy-MM-dd");
        }

        public static string GetNumberWeekDay(DateTime dt)
        {
            int y = dt.Year;
            int m = dt.Month;
            int d = dt.Day;
            if (m < 3)
            {
                m += 12;
                y--;
            }
            if (y % 400 == 0 || y % 100 != 0 && y % 4 == 0)
                d--;
            else
                d += 1;
            int val = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
            return val.ToString();
        }

        public string GetChineseWeekDay(int y, int m, int d)
        {
            string[] weekstr = { "日", "一", "二", "三", "四", "五", "六" };
            if (m < 3)
            {
                m += 12;
                y--;
            }
            if (y % 400 == 0 || y % 100 != 0 && y % 4 == 0)
                d--;
            else
                d += 1;
            return "星期" + weekstr[(d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7];
        }

        #region 返回本年有多少天

        /// <summary>返回本年有多少天</summary>
        /// <param name="iYear">年份</param>
        /// <returns>本年的天数</returns>
        public static int GetDaysOfYear(int iYear)
        {
            return IsRuYear(iYear) ? 366 : 365;
        }

        /// <summary>本年有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>本天在当年的天数</returns>
        public static int GetDaysOfYear(DateTime dt)
        {
            return IsRuYear(dt.Year) ? 366 : 365;
        }

        #endregion

        #region 返回本月有多少天
        /// <summary>本月有多少天</summary>
        /// <param name="iYear">年</param>
        /// <param name="Month">月</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(int iYear, int Month)
        {
            var days = 0;
            switch (Month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    days = IsRuYear(iYear) ? 29 : 28;
                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;
        }


        /// <summary>本月有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(DateTime dt)
        {
            //--------------------------------//
            //--从dt中取得当前的年，月信息  --//
            //--------------------------------//
            int days = 0;
            int year = dt.Year;
            int month = dt.Month;

            //--利用年月信息，得到当前月的天数信息。
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    days = IsRuYear(year) ? 29 : 28;
                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }
            return days;
        }
        #endregion

        #region 返回当前日期的 （星期名称or星期编号）
        /// <summary>返回当前日期的星期名称</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期名称</returns>
        public static string GetWeekNameOfDay(DateTime dt)
        {
            string week = string.Empty;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = "星期一";
                    break;
                case DayOfWeek.Tuesday:
                    week = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    week = "星期三";
                    break;
                case DayOfWeek.Thursday:
                    week = "星期四";
                    break;
                case DayOfWeek.Friday:
                    week = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    week = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    week = "星期日";
                    break;
            }
            return week;
        }


        /// <summary>返回当前日期的星期编号</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期数字编号</returns>
        public static int GetWeekNumberOfDay(DateTime dt)
        {
            int week = 0;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = 1;
                    break;
                case DayOfWeek.Tuesday:
                    week = 2;
                    break;
                case DayOfWeek.Wednesday:
                    week = 3;
                    break;
                case DayOfWeek.Thursday:
                    week = 4;
                    break;
                case DayOfWeek.Friday:
                    week = 5;
                    break;
                case DayOfWeek.Saturday:
                    week = 6;
                    break;
                case DayOfWeek.Sunday:
                    week = 7;
                    break;
            }
            return week;
        }
        #endregion

        #region 获取某一年有多少周
        /// <summary>
        /// 获取某一年有多少周
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>该年周数</returns>
        public static int GetWeekAmount(int year)
        {
            var end = new DateTime(year, 12, 31); //该年最后一天
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday); //该年星期数
        }
        #endregion

        #region 获取某一日期是该年中的第几周
        /// <summary>
        /// 获取某一日期是该年中的第几周
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>该日期在该年中的周数</returns>
        public static int GetWeekOfYear(DateTime dt)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
        #endregion

        #region 根据某年的第几周获取这周的起止日期
        /// <summary>
        /// 根据某年的第几周获取这周的起止日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOrder"></param>
        /// <param name="firstDate"></param>
        /// <param name="lastDate"></param>
        /// <returns></returns>
        public static void WeekRange(int year, int weekOrder, ref DateTime firstDate, ref DateTime lastDate)
        {
            //当年的第一天
            var firstDay = new DateTime(year, 1, 1);

            //当年的第一天是星期几
            int firstOfWeek = Convert.ToInt32(firstDay.DayOfWeek);

            //计算当年第一周的起止日期，可能跨年
            int dayDiff = (-1) * firstOfWeek + 1;
            int dayAdd = 7 - firstOfWeek;

            firstDate = firstDay.AddDays(dayDiff).Date;
            lastDate = firstDay.AddDays(dayAdd).Date;

            //如果不是要求计算第一周
            if (weekOrder != 1)
            {
                int addDays = (weekOrder - 1) * 7;
                firstDate = firstDate.AddDays(addDays);
                lastDate = lastDate.AddDays(addDays);
            }
        }
        #endregion

        #region 返回两个日期之间相差的天数
        /// <summary>
        /// 返回两个日期之间相差的天数
        /// </summary>
        /// <param name="dtfrm">两个日期参数</param>
        /// <param name="dtto">两个日期参数</param>
        /// <returns>天数</returns>
        public static int DiffDays(DateTime dtfrm, DateTime dtto)
        {
            TimeSpan tsDiffer = dtto.Date - dtfrm.Date;
            return tsDiffer.Days;
        }
        #endregion

        #region 判断当前年份是否是闰年
        /// <summary>判断当前年份是否是闰年，私有函数</summary>
        /// <param name="iYear">年份</param>
        /// <returns>是闰年：True ，不是闰年：False</returns>
        private static bool IsRuYear(int iYear)
        {
            //形式参数为年份
            //例如：2003
            int n = iYear;
            return (n % 400 == 0) || (n % 4 == 0 && n % 100 != 0);
        }
        #endregion

        #region 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期
        /// <summary>
        /// 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期。
        /// </summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>日期对象</returns>
        public static DateTime ToDate(string strInput)
        {
            DateTime oDateTime;

            try
            {
                oDateTime = DateTime.Parse(strInput);
            }
            catch (Exception)
            {
                oDateTime = DateTime.Today;
            }

            return oDateTime;
        }
        #endregion

        #region 将日期对象转化为格式字符串
        /// <summary>
        /// 将日期对象转化为格式字符串
        /// </summary>
        /// <param name="oDateTime">日期对象</param>
        /// <param name="strFormat">
        /// 格式：
        ///		"SHORTDATE"===短日期
        ///		"LONGDATE"==长日期
        ///		其它====自定义格式
        /// </param>
        /// <returns>日期字符串</returns>
        public static string ToString(DateTime oDateTime, string strFormat)
        {
            string strDate;

            try
            {
                switch (strFormat.ToUpper())
                {
                    case "SHORTDATE":
                        strDate = oDateTime.ToShortDateString();
                        break;
                    case "LONGDATE":
                        strDate = oDateTime.ToLongDateString();
                        break;
                    default:
                        strDate = oDateTime.ToString(strFormat);
                        break;
                }
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }

            return strDate;
        }
        #endregion
    }
}
