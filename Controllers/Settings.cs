using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Settings : BaseController
    {
        const int c_CurrentVersion = 100;
        public int NbStations;
        public int Version;
        public int MinStationHour;
        public int MaxStationHour;

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
                        bool parsed = Int32.TryParse(dr["version"].ToString(), out Version);
                        System.Diagnostics.Debug.Assert(parsed);

                        parsed = Int32.TryParse(dr["nb_stations"].ToString(), out NbStations);
                        System.Diagnostics.Debug.Assert(parsed);

                        parsed = Int32.TryParse(dr["min_station_hour"].ToString(), out MinStationHour);
                        System.Diagnostics.Debug.Assert(parsed);
                        parsed = Int32.TryParse(dr["max_station_hour"].ToString(), out MaxStationHour);
                        System.Diagnostics.Debug.Assert(parsed);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                }
                con.Close();
            }
        }
    }
}