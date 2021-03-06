﻿using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using webScraper.Models;
using System;

namespace webScraper.DataOperations
{
    /// <summary>
    /// Provides an interface for inserting and updating records in the database. As well as clearing the database tables 
    /// </summary>
    class RecordsDataAccessLayer
    {
        private SqlConnection _SqlConnection = null;

        private string GetConnectionString() //Uses current directory to set the path for the database in the App.config file
        {
            string appConfig = Environment.CurrentDirectory; //Get current directory
            appConfig = appConfig.Replace("\\bin\\Debug", "\\records.mdf"); //Gets directory for the database
            string connectionStringSection = ConfigurationManager.AppSettings["connectionString"]; //Gets current connectionString containing a placeholder
            connectionStringSection = connectionStringSection.Replace("@ConnectionString", appConfig); //Changes placeholder to current path
            return connectionStringSection;
        }
        //OpenConnection and CloseConnection methods. Source: A. Troelsen, P Japikse, Pro C# 7, Mínnesota, 2017,p 383
        private void OpenConnection()
        {
            _SqlConnection = new SqlConnection()
            {
                ConnectionString = GetConnectionString()
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
        private delegate void Insert(Record record);
        public bool InsertRecord(Record record)
        {
            OpenConnection();
            //Checks weather the record exists in the table if not, adds it
            if (!RecordExsists(record))
            {
                //Delegate to add relational information regarding the record
                Insert Insert = InsertArtist;
                Insert += InsertGenre;
                Insert += InsertCountry;
                Insert += InsertLabel;
                Insert(record);
                //Resets the autoincrementing primary key of the records table to ensure no gaps
                string sql = "DECLARE @MaxId AS INT " +
                "SELECT @MaxId = (SELECT TOP 1 Id FROM records ORDER BY Id DESC) " +
                "IF(@MaxId > 1) " +
                "DBCC CHECKIDENT('records', RESEED, @MaxId)";
                using (SqlCommand SqlCommand = new SqlCommand(sql, _SqlConnection))
                {
                    SqlCommand.CommandType = CommandType.Text;
                    SqlCommand.ExecuteNonQuery();
                }
                sql = "INSERT INTO records" +
                "(name, artist, genre, pathUrl, country, label, released)" +
                "VALUES" +
                "(@name, @artist, @genre, @pathUrl, @country, @label, @released)";
                using (SqlCommand SqlCommand = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter Parameter = new SqlParameter
                    {
                        ParameterName = "@name",
                        Value = record.Name,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);

                    Parameter = new SqlParameter
                    {
                        ParameterName = "@artist",
                        Value = record.Artist,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);
                    Parameter = new SqlParameter
                    {
                        ParameterName = "@genre",
                        Value = record.Genre,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);
                    
                    Parameter = new SqlParameter
                    {
                        ParameterName = "@pathUrl",
                        Value = record.PathUrl,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);

                    Parameter = new SqlParameter
                    {
                        ParameterName = "@country",
                        Value = record.Country,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);

                    Parameter = new SqlParameter
                    {
                        ParameterName = "@label",
                        Value = record.Label,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);

                    Parameter = new SqlParameter
                    {
                        ParameterName = "@released",
                        Value = record.Released,
                        SqlDbType = SqlDbType.Char
                    };
                    SqlCommand.Parameters.Add(Parameter);
                    SqlCommand.CommandType = CommandType.Text;
                    try //Error handling
                    {
                        SqlCommand.ExecuteNonQuery();
                        CloseConnection();
                        return true;
                    }
                    catch
                    {
                        Console.WriteLine("Error: DB Error");
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
        private void InsertGenre(Record record)
        {
            string genre = record.Genre;
            //Checks weather the genre exists in the table if not adds it
            if (!GenreExsists(genre))
            {
                string sql = "INSERT INTO genre" +
                        $"(genre) VALUES (@genre)";
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
        private void InsertArtist(Record record)
        {
            string artist = record.Artist;
            //Checks weather the artist exists in the table if not adds it
            if (!ArtistExsists(artist))
            {
                string sql = "INSERT INTO artists" +
                        $"(artist) VALUES (@artist)";
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
        private void InsertCountry(Record record)
        {
            string country = record.Country;
            //Checks weather the artist exists in the table if not adds it
            if (!CountryExsists(country))
            {
                string sql = "INSERT INTO country" +
                        $"(country) VALUES (@country)";
                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@country",
                        Value = country,
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
        private void InsertLabel(Record record)
        {
            string label = record.Label;
            //Checks weather the artist exists in the table if not adds it
            if (!LabelExsists(label))
            {
                string sql = "INSERT INTO label" +
                        $"(label) VALUES (@label)";
                using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
                {
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@label",
                        Value = label,
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
        public void InsertTrackList(string id, Record record)
        {
            foreach (Track track in record.Tracklist)
            {
                InsertTrack(id, track);
            }
        }
        private void InsertTrack(string id, Track track)
        {
            OpenConnection();

            string sql = "INSERT INTO tracks" +
            "(number, name, duration, record)" +
            "VALUES" +
            "(@number, @name, @duration, @record)";

            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@number",
                    Value = track.Number,
                    SqlDbType = SqlDbType.Int
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = track.Name,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@duration",
                    Value = track.Duration,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@record",
                    Value = id,
                    SqlDbType = SqlDbType.Int
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
        private bool RecordExsists(Record record)
        {
            string sql = $"SELECT * FROM records WHERE (name = @name AND " +
                $"artist = @artist AND genre = @genre)";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = record.Name,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@artist",
                    Value = record.Artist,
                    SqlDbType = SqlDbType.Char
                };
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@genre",
                    Value = record.Genre,
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
        private bool CountryExsists(string country)
        {
            string sql = $"SELECT * FROM country WHERE country = @country";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@country",
                    Value = country,
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
        private bool LabelExsists(string label)
        {
            string sql = $"SELECT * FROM label WHERE label = @label";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@label",
                    Value = label,
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
        public void UpdateRecordPtah(string Id, string RecordPath)
        {
            //Checks weather the artist exists in the table if not adds it
            string sql;
            OpenConnection();
            sql = "UPDATE records SET pathUrl = @RecordPath WHERE Id = @Id";
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
            string sql = "DELETE FROM tracks";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            //Resets id for records
            sql = "DBCC CHECKIDENT('tracks', RESEED, 0)";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sql = "DELETE FROM records";
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
            sql = "DELETE FROM artists";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sql = "DELETE FROM country";
            using (SqlCommand cmd = new SqlCommand(sql, _SqlConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sql = "DELETE FROM label";
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
