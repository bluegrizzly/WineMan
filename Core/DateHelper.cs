using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WineMan
{
    public class DateHelper
    {
        static public int GetRendezVousHour(string datestr)
        {
            string hour = datestr.Substring(0, 2);
            int result = 0;
            bool parsed = Int32.TryParse(hour, out result);
            System.Diagnostics.Debug.Assert(parsed);
            return result;
        }

        static public int GetRendezVousMin(string datestr)
        {
            string min = datestr.Substring(3, 2);
            int result = 0;
            bool parsed = Int32.TryParse(min, out result);
            System.Diagnostics.Debug.Assert(parsed);
            return result;
        }

        static public string GetHFormatedDate(DateTime datetime)
        {
            string res = datetime.ToString("HH");
            res += "h";
            res += datetime.ToString("mm");
            return res;
        }
    }
}