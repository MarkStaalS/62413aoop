using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace webScraper.DataOperations
{
    class recordsDataAccessLayer
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
        #region SQL insert
        public bool InsertRecord(string name, string artist, string genre,string url, string pathUrl)
        {
            //laves om så den kan tage et record objekt som indput
            OpenConnection();
            InsertGenre(genre);
            InsertArtist(artist);
            //Checks weather the record exsists in the table if not adds it
            string sql;
            if (!RecordExsists( name, artist, genre, url))
            {
                sql = "INSERT INTO records" +
                "(recordName,recordArtist,recordGenre,recordUrl,recordPathUrl)" +
                "Values" +
                "(@name, @artist, @genre, @url, @pathUrl)";

                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@name",
                        Value = name,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@artist",
                        Value = artist,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@genre",
                        Value = genre,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@url",
                        Value = url,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@pathUrl",
                        Value = pathUrl,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);
                    cmd.CommandType = CommandType.Text;
                    //Error handling
                    try
                    {
                        cmd.ExecuteNonQuery();
                        CloseConnection();
                        return true;
                    }
                    catch
                    {
                        CloseConnection();
                        return false;
                    }
                }
            }
            else
            {
                CloseConnection();
                return false;
            }
        }
        private void InsertGenre(string genre)
        {
            //Checks weather the genre exsists in the table if not adds it
            string sql;
            if (!GenreExsists(genre))
            {
                sql = "INSERT INTO genre" +
                        $"(genre) Values (@genre)";
                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@genre",
                        Value = genre,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void InsertArtist(string artist)
        {
            //Checks weather the artist exsists in the table if not adds it
            string sql;
            if (!ArtistExsists(artist))
            {
                sql = "INSERT INTO artists" +
                        $"(artist) Values (@artist)";
                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@artist",
                        Value = artist,
                        SqlDbType = SqlDbType.Char
                    };
                    cmd.Parameters.Add(parameter);
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion
        #region SQL select
        public string GetLatestId()
        {
            OpenConnection();
            string sql = $"SELECT TOP 1 Id FROM records " +
                        "ORDER BY Id DESC";
            string id;
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        id = null;
                    }
                    else
                    {
                        id = dataReader["Id"].ToString();
                    }
                }
            }
            CloseConnection();
            return id;
        }
        private bool RecordExsists(string name, string artist, string genre,string url)
        {
            string sql = $"SELECT * FROM records WHERE (recordName = @name AND " +
                $"recordArtist = @artist AND recordGenre = @genre AND recordUrl = @url)";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@artist",
                    Value = artist,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@genre",
                    Value = genre,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@url",
                    Value = url,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                }
            }
        }
        private bool GenreExsists(string genre)
        {
            string sql = $"SELECT * FROM genre WHERE genre = @genre";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@genre",
                    Value = genre,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);
                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        private bool ArtistExsists(string artist)
        {
            string sql = $"SELECT * FROM artists WHERE artist = @artist";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@artist",
                    Value = artist,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);
                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        #endregion
        #region SQL update
        public void updateRecordPtah(string Id, string RecordPath)
        {
            //Checks weather the artist exsists in the table if not adds it
            string sql;
            OpenConnection();
            sql = "UPDATE records SET recordPathUrl = @RecordPath WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@RecordPath",
                    Value = RecordPath,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = Id,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                cmd.CommandType = CommandType.Text;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                }
                CloseConnection();
            }
        }
        #endregion
        #region Utility tools
        public void ResetDatabase()
        {
            OpenConnection();
            //Deletes rows from tables
            string sql = "DELETE FROM records";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            //Resets id for records
            sql = "DBCC CHECKIDENT('records', RESEED, 0)";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sql = "DELETE FROM genre";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            CloseConnection();
        }
        #endregion
    }
}
