using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDrivenProgram
{

    // This is the code that connects the database to the program.
    public class AccountsData
    {
        static string connectionString = ConfigurationManager.AppSettings["connectionString"];

        public static List<T> GetData<T>(string SQL, CommandType cmdType)
        {

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {

                var objProps = Activator.CreateInstance<T>().GetType().GetProperties();
                var returnList = new List<T>();

                using (SqlCommand sqlCmd = new SqlCommand(SQL, sqlCon))
                {

                    sqlCmd.CommandType = cmdType;
                    sqlCmd.CommandTimeout = 30000;

                    try
                    {
                        sqlCon.Open();

                        SqlDataReader reader = sqlCmd.ExecuteReader();

                        var columns = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(row => row["ColumnName"].ToString().ToLower()).ToList();

                        while (reader.Read())
                        {
                            var thisRow = Activator.CreateInstance<T>();
                            foreach (var prop in objProps)
                            {
                                if (columns.Contains(prop.Name.ToLower()))
                                {
                                    if (reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                    {
                                        prop.SetValue(thisRow, null, null);
                                    }
                                    else
                                    {
                                        prop.SetValue(thisRow, reader[prop.Name], null);
                                    }
                                }
                            }
                            returnList.Add(thisRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (sqlCon.State != ConnectionState.Closed)
                        {
                            sqlCon.Close();
                        }
                    }

                }

                return returnList;

            }
        }


        public static int ExecuteSqlNonQuery(string sql, CommandType cmdType)
        {
            int num = 0;

            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sql, sqlCon))
                    {
                        sqlCmd.CommandType = cmdType;
                        sqlCmd.CommandTimeout = 30000;

                        sqlCon.Open();

                        num = sqlCmd.ExecuteNonQuery();
                    }
                }

                return num;

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);

                return num;
            }
        }

    }
}
