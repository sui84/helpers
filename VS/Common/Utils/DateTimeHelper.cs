using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
