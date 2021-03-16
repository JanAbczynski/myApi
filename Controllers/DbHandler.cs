using Commander.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public class DbHandler
    {
        private string connectionString = "Server=(localdb)\\MyLocalDB; Initial Catalog=Commander_DB; User ID=CommanderAPI; Password=qwe123qwe;";

        public void ExecuteQuery(string query, Dictionary<string, object> queryParams = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = new SqlCommand(query);
                if (queryParams != null)
                {
                    foreach (var pair in queryParams)
                    {
                        sda.SelectCommand.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                }
                sda.SelectCommand.CommandText = query;
                sda.SelectCommand.Connection = connection;
                connection.Open();
                sda.SelectCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public DataSet GetSetFromDb(string query, Dictionary<string, object> queryParams = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                DataSet dataSet = new DataSet();
                sda.SelectCommand = new SqlCommand(query);
                if (queryParams != null)
                {
                    foreach (var pair in queryParams)
                    {
                        sda.SelectCommand.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                }
                sda.SelectCommand.CommandText = query;
                sda.SelectCommand.Connection = connection;
                sda.Fill(dataSet, "tab");   
                return dataSet;
            }
        }

        public string AddParamsToQuery(string query, Dictionary<string, object> paramsName)
        {
            string rebuildedQuery = query;
            StringBuilder newQuery = new StringBuilder(query);
            newQuery.Append(" ");
            int numberOfParams = paramsName.Count;
            int actualparam = 0;
            foreach (var param in paramsName)
            {
                actualparam++;
                newQuery.Append(param.Key);
                if (actualparam < numberOfParams)
                {
                    newQuery.Append(", ");
                }
            }
            return newQuery.ToString();
        }


        public void GenerateProcedure(string sqlProc, Dictionary<string, object> queryParams)
        {
            StringBuilder newQuery = new StringBuilder("CREATE PROCEDURE");
            newQuery.Append(sqlProc.Replace("exec ", ""));
            newQuery.Append(" (");
            int numberOfParams = queryParams.Count;
            int actualparam = 0;
            foreach (var param in queryParams)
            {
                actualparam++;
                newQuery.Append(param.Key + " nvarchar(50) = null");
                if (actualparam < numberOfParams)
                {
                    newQuery.Append(", ");
                }
            }
            newQuery.Append(") AS BEGIN      END");
            Console.WriteLine(newQuery);
        }

        public void GenerateQuerryValues(string sqlProc, Dictionary<string, object> queryParams)
        {
            StringBuilder newQuery = new StringBuilder();
            int numberOfParams = queryParams.Count;
            int actualparam = 0;
            newQuery.Append("(");
            foreach (var param in queryParams)
            {
                actualparam++;
                newQuery.Append(param.Key.Replace("@", "") );
                if (actualparam < numberOfParams)
                {
                    newQuery.Append(",  ");
                }
            }
            newQuery.Append(")");

            StringBuilder newQuery2 = new StringBuilder();
            actualparam = 0;
            newQuery2.Append("(");
            foreach (var param in queryParams)
            {
                actualparam++;
                newQuery2.Append(param.Key);
                if (actualparam < numberOfParams)
                {
                    newQuery2.Append(", ");
                }
            }
            newQuery2.Append(")");
            Console.WriteLine(newQuery);
            Console.WriteLine(newQuery2);
        }

    }

    public class T
    {
    }
}
