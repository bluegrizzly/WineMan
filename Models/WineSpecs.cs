using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class WineSpecs
    {
        public void GetAllWineBrands(out System.Data.DataSet objDs)
        {
            objDs = new System.Data.DataSet();

            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id, name FROM wine_brands";
            MySqlDataAdapter dAdapter = new MySqlDataAdapter();
            dAdapter.SelectCommand = cmd;
            con.Open();
            dAdapter.Fill(objDs);
            con.Close();
        }

        public void GetAllWineTypes(int brandID, out System.Data.DataSet objDs)
        {
            objDs = new System.Data.DataSet();

            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id, name FROM wine_types WHERE brand_id =@brand_id";
            cmd.Parameters.AddWithValue("@brand_id", brandID);
            MySqlDataAdapter dAdapter = new MySqlDataAdapter();
            dAdapter.SelectCommand = cmd;
            con.Open();
            dAdapter.Fill(objDs);
            con.Close();
        }
        public void GetAllWineCategories(out System.Data.DataSet objDs)
        {
            objDs = new System.Data.DataSet();

            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            //cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT id, name FROM wine_categories WHERE step=1";
            MySqlDataAdapter dAdapter = new MySqlDataAdapter();
            dAdapter.SelectCommand = cmd;
            con.Open();
            dAdapter.Fill(objDs);
            con.Close();
        }
    }
}