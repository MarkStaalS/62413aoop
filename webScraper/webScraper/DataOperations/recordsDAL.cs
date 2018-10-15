using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using webScraper.Models;

namespace webScraper.DataOperations
{
    class recordsDAL
    {
        private SqlConnection _SqlConnection = null;
        private readonly string _connectionString = ConfigurationManager.AppSettings["connectionString"];
        private void OpenConnection()
        {
            _SqlConnection = new SqlConnection()
            {
                ConnectionString = _connectionString
            };
            _SqlConnection.Open();
        }
        private void CloseConnection()
        {
            if (_SqlConnection?.State != ConnectionState.Closed)
            {
                _SqlConnection?.Close();
            }
        }
        public void InsertRecord(string name, string artist, string genre,string url, string pathUrl)
        {
            OpenConnection();
            string sql = "INSERT INTO records" +
                "(recordName,recordArtist,recordGenre,recordUrl,recordPathUrl)" +
                "Values" +
                $"('{name}','{artist}','{genre}','{url}','{pathUrl}')";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            CloseConnection();
        }
    }
}
