using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WineMan
{
    public class BaseController 
    {
        protected static string AddDateParameter(DateTime value, bool first = false)
        {
            string retStr;
            if (first)
                retStr = "'"; 
            else
                retStr = ",'";
            
            retStr += value.ToString("s") + "'";
            return retStr;
        }
        protected static string AddIntParameter(int value, bool first = false)
        {
            if (first)
                return value.ToString();
            else
                return "," + value.ToString();
        }
        protected static string AddStringParameter(string value, bool first = false)
        {
            if (first)
                return "'" + value + "'";
            else
                return "," + "'" + value + "'";
        }
    }
}