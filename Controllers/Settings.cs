using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Settings : BaseController
    {
        const string c_dbName = "settings";
        const int c_CurrentVersion = 100;
        public int NbStations;
        public int version;
        public int MinStationHour;
        public int MaxStationHour;
        public int hour_intervale;
        public bool auto_print;
        public string backup_path;
        public string default_printer;

        public Settings()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM settings WHERE version LIKE '" + c_CurrentVersion.ToString() + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        bool parsed = Int32.TryParse(dr["version"].ToString(), out version);
                        System.Diagnostics.Debug.Assert(parsed);

                        parsed = Int32.TryParse(dr["nb_stations"].ToString(), out NbStations);
                        System.Diagnostics.Debug.Assert(parsed);

                        parsed = Int32.TryParse(dr["min_station_hour"].ToString(), out MinStationHour);
                        System.Diagnostics.Debug.Assert(parsed);
                        parsed = Int32.TryParse(dr["max_station_hour"].ToString(), out MaxStationHour);
                        System.Diagnostics.Debug.Assert(parsed);

                        parsed = Int32.TryParse(dr["hour_intervale"].ToString(), out hour_intervale);
                        System.Diagnostics.Debug.Assert(parsed);

                        int autoprintInt=0;
                        parsed = Int32.TryParse(dr["auto_print"].ToString(), out autoprintInt);
                        System.Diagnostics.Debug.Assert(parsed);
                        auto_print = autoprintInt > 0 ? true : false;

                        backup_path = dr["backup_path"].ToString();

                        default_printer = dr["default_printer"].ToString();
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                }
                con.Close();
            }
        }

        public bool Save()
        {
            bool ret = false;
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = "UPDATE " + c_dbName + " SET default_printer='" + default_printer + "'" +
                        ", backup_path='" + backup_path + "'" +
                        ", auto_print=" + (auto_print ? "1" : "0") +
                        " WHERE version="+version;
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        int result = cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }
            return ret;
        }

        public int GetHour(DayOfWeek day, bool start)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return 0;
                case DayOfWeek.Monday:
                    return start ? 10 : 18;
                case DayOfWeek.Tuesday:
                    return start ? 10 : 18;
                case DayOfWeek.Wednesday:
                    return start ? 10 : 18;
                case DayOfWeek.Thursday:
                    return start ? 10 : 20;
                case DayOfWeek.Friday:
                    return start ? 10 : 20;
                case DayOfWeek.Saturday:
                    return start ? 10 : 17;
            }
            return 0;
        }
    }
}