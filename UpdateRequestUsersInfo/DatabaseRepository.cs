using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace UpdateUsersLogins
{
    public class DatabaseRepository: IDatabaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DatabaseRepository(string connectionString, ILogger logger)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            _connectionString = connectionString;
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }
        public IEnumerable<User> GetUsers()
        {
            var newUsers = new List<User>();
            var connection = new SqlConnection(_connectionString);
            try
            {
                _logger.Log("Connection opening...", LogLevel.Notice);
                connection.Open();
                _logger.Log("Connection opened", LogLevel.Notice);
                const string selectQuery = @"SELECT IdRequestUser, Login,
                        Snp, Post, Phone, Department, Unit, Office
                    FROM dbo.RequestUsers
                    WHERE Deleted <> 1";
                var command = new SqlCommand(selectQuery, connection); 
                _logger.Log("Execute query...", LogLevel.Notice);
                var sqlReader = command.ExecuteReader();
                if (!sqlReader.HasRows) return newUsers;
                _logger.Log("Filling users table...", LogLevel.Notice);
                while (sqlReader.Read())
                {
                    newUsers.Add(new User
                    {
                        IdUser = (int)sqlReader["IdRequestUser"],
                        Login = sqlReader["Login"] == DBNull.Value ? null : (string)sqlReader["Login"],
                        Snp = sqlReader["Snp"] == DBNull.Value ? null : (string)sqlReader["Snp"],
                        Post = sqlReader["Post"] == DBNull.Value ? null : (string)sqlReader["Post"],
                        Phone = sqlReader["Phone"] == DBNull.Value ? null : (string)sqlReader["Phone"],
                        Department = sqlReader["Department"] == DBNull.Value ? null : (string)sqlReader["Department"],
                        Unit = sqlReader["Unit"] == DBNull.Value ? null : (string)sqlReader["Unit"],
                        Office = sqlReader["Office"] == DBNull.Value ? null : (string)sqlReader["Office"]
                    });
                }
                sqlReader.Close();
                return newUsers;
            }
            catch (SqlException e)
            {
                _logger.Log(string.Format("Database error: {0}", e.Message), LogLevel.Error);
                return null;
            }
            finally
            {
                _logger.Log("Connection closing...", LogLevel.Notice);
                connection.Close();
                _logger.Log("Connection closed", LogLevel.Notice);
            }   
        }

        public bool UpdateUser(int idUser, User userInfo)
        {
            var connection = new SqlConnection(_connectionString);
            int affectedRows;
            try
            {
                _logger.Log("Connection opening...", LogLevel.Notice);
                connection.Open();
                _logger.Log("Connection opened", LogLevel.Notice);

                const string updateQuery = @"UPDATE dbo.RequestUsers 
                        SET
                          Login = @Login, Snp = @Snp, Post = @Post,
                          Phone = @Phone, 
                          Office = @Office
                        WHERE
                          IdRequestUser = @IdUser AND Deleted <> 1";
                var command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddRange(new[]
                {
                    new SqlParameter {ParameterName = "@Login", 
                        Value = (object)userInfo.Login ?? DBNull.Value},
                    new SqlParameter {ParameterName = "@Snp", 
                        Value = (object)userInfo.Snp ?? DBNull.Value},
                    new SqlParameter {ParameterName = "@Post", 
                        Value = (object)userInfo.Post ?? DBNull.Value},
                    new SqlParameter {ParameterName = "@Phone", 
                        Value = (object)userInfo.Phone ?? DBNull.Value},
                    new SqlParameter {ParameterName = "@Office", 
                        Value = (object)userInfo.Office ?? DBNull.Value},
                    new SqlParameter {ParameterName = "@IdUser", 
                        Value = idUser}
                });
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException e)
            {
                _logger.Log(string.Format("Database error: {0}", e.Message), LogLevel.Error);
                return false;
            }
            finally
            {
                _logger.Log("Connection closing...", LogLevel.Notice);
                connection.Close();
                _logger.Log("Connection closed", LogLevel.Notice);
            }
            return affectedRows > 0;
        }
    }
}
