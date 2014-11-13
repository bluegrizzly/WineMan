using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Holiday : BaseController
    {
        public int id;
        public DateTime date;
        public string reason;

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                DateTime.TryParse(dr["date"].ToString(), out date);
                reason = dr["reason"].ToString();
            }
        }

        public static bool CreateRecord(Holiday holiday)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(holiday.id, true) +
                                    AddDateParameter(holiday.date) +
                                    AddStringParameter(holiday.reason); 
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO holidays VALUES (" + values + ")", con))
                    {
                        cmd.ExecuteNonQuery();

                        holiday.id = (int)cmd.LastInsertedId;
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
                return false;
            }
        }

        public static List<Holiday> GetAllRecords()
        {
            List<Holiday> holidays = new List<Holiday>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM holidays", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Holiday holiday = new Holiday();
                        holiday.FillData(dr);
                        holidays.Add(holiday);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return holidays;
        }
    }
}