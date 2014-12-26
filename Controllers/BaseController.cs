using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WineMan
{
    public class BaseController 
    {
        protected static string AddDateParameter(DateTime value, bool first = false, string paramName = "")
        {
            string res = "";
            if (!first)
                res += ",";
            if (paramName.Length > 0)
                res += (paramName + "=");

            res += "'" + value.ToString("s") + "'";
            return res;
        }
        protected static string AddIntParameter(int value, bool first = false, string paramName="")
        {
            string res="";
            if (!first)
                res = ",";
            if (paramName.Length > 0)
                res += (paramName + "=");
            res+=value.ToString();
            return res;
        }
        protected static string AddStringParameter(string value, bool first = false, string paramName="")
        {
            string res = "";
            if (!first)
                res = ",";
            if (paramName.Length > 0)
                res += (paramName + "=");

            res += "'" + value + "'";
            return res;
        }
    }
}