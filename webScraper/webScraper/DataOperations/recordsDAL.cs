using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using webScraper.Models;
using System;

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
            InsertGenre(genre);
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
        private void InsertGenre(string genre)
        {
            //Checks weather the genre exsists in the table if not adds it
            bool exsists = true;
            string sql = $"SELECT * FROM genre WHERE genre = '{genre}'";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        exsists = false;
                    }
                }
            }
            if (!exsists)
            {
                sql = "INSERT INTO genre" +
                        $"(genre) Values ('{genre}')";
                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
