using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class OpenHours : BaseController
    {
        public int id;
        public string day;
        public int hour_start;
        public int hour_end;

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                day = dr["day"].ToString();

                parsed = Int32.TryParse(dr["hour_start"].ToString(), out hour_start);
                System.Diagnostics.Debug.Assert(parsed);
                parsed = Int32.TryParse(dr["hour_end"].ToString(), out hour_end);
                System.Diagnostics.Debug.Assert(parsed);
            }
        }

        public static List<OpenHours> GetAllRecords()
        {
            List<OpenHours> allData = new List<OpenHours>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM openhours", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        OpenHours oh = new OpenHours();
                        oh.FillData(dr);
                        allData.Add(oh);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return allData;
        }

        public static void GetDayOpenHour(System.DayOfWeek day, out int start, out int end)
        {
            OpenHours oh = new OpenHours();
            start = 0;
            end = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM openhours WHERE day = '" + day.ToString() + "'";
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        oh.FillData(dr);
                        start = oh.hour_start;
                        end = oh.hour_end;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                    dr.Close();
                }
                con.Close();
            }
        }
    }
}