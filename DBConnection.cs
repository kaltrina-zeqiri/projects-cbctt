using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Controls;
using System.IO;

namespace UniversityTimetabling
{
    static class DBConnection
    {
        private static string _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        private static string _database = "FIEKDatabase";
        /**
         * Get an item from a table
         * @params table string
         * @params id int
         * 
         * @returns ArrayList
        */
        static public ArrayList Get(string table, int id)
        {
            string Query = "SELECT * FROM " + table + " WHERE id = " + id;
            var dt = new DataTable();
            ArrayList result = new ArrayList() ;
            SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True;Connect Timeout=30");
            try
            {
                objconn.Open();
                SqlCommand objcomand = new SqlCommand(Query, objconn);
                using (SqlDataAdapter adapter = new SqlDataAdapter(objcomand))
                {
                    adapter.Fill(dt);
                }
                   foreach (DataRow row in dt.Rows)
                   {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                        dictionary[row.Table.Columns[i].ColumnName] = row[i].ToString();
                    result.Add(dictionary); 
                   }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                return null;
            }
            finally
            {
                objconn.Close();
            }        
        }

        /**
         * Select items from a table
         * @params table string
         * @params columns string
         * @params conditions string, default null
         * 
         * @returns DataTable
        */
        static public DataTable Select(string table, string columns = "'*'", string conditions = null, string groupBy = null, string orderBy = null)
        {
            string Query = "";
            var dt = new DataTable();
            ArrayList result = new ArrayList();

            if (conditions == null)
                Query = "SELECT " + columns + " FROM " + table;
            else
                Query = "SELECT " + columns + " FROM " + table + " WHERE " + conditions;

            if (groupBy != null)
                Query += " GROUP BY " + groupBy;

            if (orderBy != null)
                Query += " ORDER BY " + orderBy;
            SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True;Connect Timeout=30");
            try
            {
                objconn.Open();
                SqlCommand objcomand = new SqlCommand(Query, objconn);
                using (SqlDataAdapter adapter = new SqlDataAdapter(objcomand))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                return null;
            }
            finally
            {
                objconn.Close();
            }
        }

        /**
        * Select items from a table
        * @params table string
        * @params columns string
        * @params conditions string, default null
        * 
        * @returns DataTable
       */
        static public DataTable RAW_Select(string query)
        {
            var dt = new DataTable();
            ArrayList result = new ArrayList();

            SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True;Connect Timeout=30");
            try
            {
                objconn.Open();
                SqlCommand objcomand = new SqlCommand(query, objconn);
                using (SqlDataAdapter adapter = new SqlDataAdapter(objcomand))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                return null;
            }
            finally
            {
                objconn.Close();
            }
        }

        /**
         * Select items to Fill Combobox
         * @params table string
         * @params combobox Combobox
        */
        static public void FillCombo(string table, ComboBox combobox, string displayField = "name", string conditions = null, string orders = null)
        {
            string Query = "SELECT * FROM " + table;
            if (conditions != null) Query += " WHERE " + conditions;
			if (orders != null) Query += " ORDER BY " + orders;
			SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True;Connect Timeout=30");
            try
            {
                objconn.Open();
                SqlCommand objcomand = new SqlCommand(Query, objconn);
                SqlDataReader reader = objcomand.ExecuteReader();
                string[] displayFields = displayField.Split(',');
                while (reader.Read())
                {
                    int nameIndex, idIndex;
                    string getName = "";
                    foreach (string dField in displayFields)
                    {
                        nameIndex = reader.GetOrdinal(dField);
                        getName += (getName == "") ? reader.GetString(nameIndex) : " " + reader.GetString(nameIndex);
                    }

                    idIndex = reader.GetOrdinal("id");
                    int getId = reader.GetInt32(idIndex);

                    combobox.Items.Add(new { id = getId, name = getName});
                    combobox.SelectedValuePath = "id";
                    combobox.DisplayMemberPath = "name";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
            }
            finally
            {
                objconn.Close();
            }
        }

        /**
        * Create an item
        * @params table string
        * @params columns string
        * @params values string
        * 
        * @returns int
       */
        static public int Create(string table, string columns, string values)
        {
            using (SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True"))
            {
                String Query = "INSERT INTO " + table + " (" + columns + ") VALUES (" + values + ")";

                try
                {
                    objconn.Open();
                    SqlCommand objcomand = new SqlCommand(Query, objconn);
                    int result = objcomand.ExecuteNonQuery();
                    int identity = 0;
                    // Check Error
                    if (result <= 0)
                        MessageBox.Show("Error inserting data into Database!", "Error:", MessageBoxButton.OK);
                    else
                    {
                        objcomand.Parameters.Clear();
                        objcomand.CommandText = "SELECT @@IDENTITY";

                        identity = objcomand.ExecuteScalar().Equals(DBNull.Value) ? 10 : Convert.ToInt32(objcomand.ExecuteScalar());
                    }
                    return identity;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                    return 0;
                }
                finally
                {
                    objconn.Close();
                }
            }
        }

        /**
        * Update an item
        * @params table string
        * @params values string
        * @params id int
        * 
        * @returns int
        */
        static public int Update(string table, string values, int id = 0, string conditions = null)
        {
            using (SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True"))
            {
                String Query = "UPDATE " + table + " SET " + values + " WHERE ";
                if (id > 0) Query += "id = '" + id + "'";
                else if (conditions != null) Query += conditions;

                try
                {
                    objconn.Open();
                    SqlCommand objcomand = new SqlCommand(Query, objconn);
                    int result = objcomand.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                        MessageBox.Show("Error on updating data!", "Error:", MessageBoxButton.OK);

                    return result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                    return 0;
                }
                finally
                {
                    objconn.Close();
                }
            }
        }

        /**
          * Delete an item
          * @params table string
          * @params id int
          * 
          * @returns int
          */
        static public int Delete(string table, int id = 0, string conditions = null)
        {
            using (SqlConnection objconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _basePath + "\\" + _database + ".mdf;Integrated Security=True"))
            {
                String Query = "DELETE FROM " + table;
                if (id > 0) Query += " WHERE id = '" + id + "'";
                else if (conditions != null) Query += " WHERE " + conditions;

                try
                {
                    objconn.Open();
                    SqlCommand objcomand = new SqlCommand(Query, objconn);
                    int result = objcomand.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                        MessageBox.Show("Error on deleting data!", "Error:", MessageBoxButton.OK);

                    return result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);
                    return 0;
                }
                finally
                {
                    objconn.Close();
                }
            }
        }
    }
}
