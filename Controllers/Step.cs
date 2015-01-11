using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Step : BaseController
    {
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

        public static Step GetRecord(string id)
        {
            Step ret = new Step();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM steps WHERE id = '" + id + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        TransactionStep tx = new TransactionStep();
                        ret.FillData(dr);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return ret;
        }

        public static List<Step> GetAllRecords()
        {
            List<Step> steps = new List<Step>();
            
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM steps", con))
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