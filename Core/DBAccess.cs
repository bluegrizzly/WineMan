using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;

namespace WineMan.Core
{
    public class DBAccess
    {
        static public void GetJSONRecords(HttpContext context, string dbName, string sqlCmd=null)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    con.Open();
                    //
                    // The following code uses an SqlCommand based on the SqlConnection.
                    //
                    string retString = @"{";
                    if (sqlCmd == null)
                        sqlCmd = "SELECT * FROM " + dbName + " ORDER BY id";

                    using (MySqlCommand command = new MySqlCommand(sqlCmd , con))
                    {
                        int nbRows = int.MaxValue;
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            int currentRow = 0;

                            retString += @"""total"": """ + nbRows.ToString() + @""",";
                            retString += @"""page"": ""1"",";
                            retString += @"""records"": """ + nbRows.ToString() + @""",";
                            retString += @"""rows"" : [ ";

                            bool firstRow = true;
                            while (reader.Read())
                            {
                                if (!firstRow)
                                    retString += ",";
                                firstRow = false;
                                retString += @"{""id"":" + currentRow.ToString() + @", ""cell"" :[";

                                bool firstRow2 = true;
                                for (int colomnNumber = 0; colomnNumber < reader.FieldCount; colomnNumber++)
                                {
                                    //string colName = reader.GetName(colomnNumber);
                                    if (!firstRow2)
                                        retString += ",";
                                    firstRow2 = false;

                                    try
                                    {
                                        reader.GetString(colomnNumber);
                                        string valueStr = reader.GetString(colomnNumber);
                                        valueStr = valueStr.Replace("\r", "");
                                        retString += @"""" + valueStr + @""" ";
                                    }
                                    catch
                                    {
                                        retString += @"""""";
                                    }
                                }

                                retString += "]}";
                                currentRow++;
                            }
                            retString += "]}";
                            reader.Close();
                        }
                    }
                    context.Response.Write(retString);
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }
        }

        static public bool IsRecordExistWithSameName(HttpContext context, string dbName, System.Collections.Specialized.NameValueCollection forms, bool explicitSetID= false)
        {
            bool ret = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                // Open the SqlConnection.
                con.Open();

                try
                {
                    // Create the command
                    string sqlCmd;
                    if (dbName == Customer.c_dbName) // Special case for customers
                        sqlCmd = Customer.GetDoublonValidationSqlQuery(forms);
                    else if (dbName == Wine_Category.c_dbName)
                        sqlCmd = Wine_Category.GetDoublonValidationSqlQuery(forms);
                    else if (dbName == Wine_Type.c_dbName)
                        sqlCmd = Wine_Type.GetDoublonValidationSqlQuery(forms);
                    else
                        sqlCmd = "SELECT * FROM " + dbName + " WHERE name='" + forms.Get("name").ToString() + "'";

                    using (MySqlCommand command = new MySqlCommand(sqlCmd, con))
                    {
                        MySqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                            ret = true;

                        reader.Close();
                    }
                }
                catch { }
                con.Close();
            }
            return ret;
        }

        static public bool AddRecord(HttpContext context, string dbName, System.Collections.Specialized.NameValueCollection forms, bool explicitSetID= false)
        {
            bool ret = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                // Open the SqlConnection.
                con.Open();

                // Create the command
                string sqlCmd = "INSERT INTO " + dbName + " (";
                string values = " (";
                bool firstRow=true;

                // Get the list of columns for this table
                DataTable schema = null;
                using (var schemaCommand = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM " + dbName, con))
                {
                    using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        schema = reader.GetSchemaTable();
                    }
                }
                foreach (DataRow col in schema.Rows)
                {
                    string colName = col.Field<String>("ColumnName");

                    if (!explicitSetID && colName == "id")
                        continue;

                    string value = "";
                    try
                    {
                        string valueRaw = forms.Get(colName).ToString();

                        if (colName == "active")
                            value = valueRaw;
                        else
                            value = "'" + valueRaw + "'";
                    }
                    catch 
                    {
                        continue;
                    }

                    if (!firstRow)
                    {
                        sqlCmd += ",";
                        values += ",";
                    }
                    firstRow = false;
                    sqlCmd += colName;
                    values += value;
                }
                sqlCmd += ") VALUES " + values + ")";

                //execute the command
                using (MySqlCommand command = new MySqlCommand(sqlCmd, con))
                {
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        ret = true;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Assert(false, e.Message);
                        ret = false; 
                    }
                }

                con.Close();
            }

            return ret;
        }

        static public bool EditRecord(HttpContext context, string dbName, System.Collections.Specialized.NameValueCollection forms, string idToEdit)
        {
            bool ret = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                // Open the SqlConnection.
                con.Open();

                // Create the command
                string sqlCmd = "UPDATE " + dbName + " SET ";
                bool firstRow = true;

                // Get the list of columns for this table
                DataTable schema = null;
                using (var schemaCommand = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM " + dbName, con))
                {
                    using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        schema = reader.GetSchemaTable();
                    }
                }
                foreach (DataRow col in schema.Rows)
                {
                    string colName = col.Field<String>("ColumnName");
                    if (colName == "id")
                        continue;

                    if (!firstRow)
                    {
                        sqlCmd += ",";
                    }
                    firstRow = false;

                    try
                    {
                        // COLNAME=VALUE,
                        if (colName == "active")
                            sqlCmd += colName + "=" + forms.Get(colName).ToString();
                        else
                            sqlCmd += colName + "='" + forms.Get(colName).ToString() + "'";
                    }
                    catch
                    {
                        if (!firstRow)
                            sqlCmd = sqlCmd.TrimEnd(',');
                        continue;
                    }
                }
                sqlCmd += " WHERE id=" + idToEdit;

                //execute the command
                using (MySqlCommand command = new MySqlCommand(sqlCmd, con))
                {
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        ret = true;
                    }
                    catch { ret = false; }
                }

                con.Close();
            }

            return ret;
        }

        static public bool DeleteRecord(HttpContext context, string dbName, string idToDelete)
        {
            bool ret = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                // Open the SqlConnection.
                con.Open();

               string sqlCmd = "DELETE FROM " + dbName + " WHERE id=" + idToDelete;
               using (MySqlCommand command = new MySqlCommand(sqlCmd, con))
               {
                   try
                   {
                       int result = command.ExecuteNonQuery();
                       ret = result>0;
                   }
                   catch { ret = false; }
               }
               con.Close();
            }

            return ret;
        }
    }
}