using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Step : BaseController
    {
        public const string c_dbName = "steps";
        public int id;
        public string name;
        public string name_french;
        public int final_step;

        public void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                name = dr["name"].ToString();
                name_french = dr["name_french"].ToString();

                parsed = Int32.TryParse(dr["final_step"].ToString(), out final_step);
                System.Diagnostics.Debug.Assert(parsed);
            }
        }

        public static List<Step> GetRecords(string sqlQuery)
        {
            List<Step> steps = new List<Step>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Step step = new Step();
                        step.FillData(dr);
                        steps.Add(step);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return steps;
        }

        public static Step GetRecordById(string id)
        {
            List<Step> steps = GetRecords("SELECT DISTINCT * FROM steps WHERE id = '" + id + "'");
            System.Diagnostics.Debug.Assert(steps.Count == 1);
            if (steps.Count == 1)
                return steps[0];
            return null;
        }

        public static Step GetLastStep()
        {
            List<Step> steps = GetRecords("SELECT DISTINCT * FROM steps WHERE final_step = 1");
            System.Diagnostics.Debug.Assert(steps.Count == 1);
            if (steps.Count == 1)
                return steps[0];
            return null;
        }

        public static List<Step> GetAllRecords()
        {
            return GetRecords("SELECT * FROM steps");
        }

        public static string GetStepName(int id)
        {
            string retName="";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM steps WHERE id="+ id.ToString(), con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        Step step = new Step();
                        step.FillData(dr);
                        retName = step.name;
                    }
                    dr.Close();
                }
                con.Close();
            }

            return retName;
        }
    }
}