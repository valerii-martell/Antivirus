using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows;

namespace Antivirus
{
    static class DbWorker
    {
        private static string connectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\App_Data\\AntivirusDatabase.mdf;Integrated Security = True";

        public static void InsertTrustedProcess(string processMainFilePath)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "INSERT INTO TrustedProcesses (ProcessMainFilePath) VALUES (@ProcessMainFilePath)";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Данный процес не может быть добавлен в список исключений", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        public static List<string> GetTrustedProcesses()
        {

            List<string> result = new List<string>();
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT * FROM TrustedProcesses;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                using (SqlDataReader dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        result.Add(dbReader["ProcessMainFilePath"].ToString());
                    }
                }
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Невозможно получить список доверенных процесов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }

        public static void DeleteTrustedProcess(string processMainFilePath)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "DELETE FROM TrustedProcesses WHERE ProcessMainFilePath = @ProcessMainFilePath";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Данный процес не может быть удален из списка исключений", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ClearTrustedProcesses()
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "DELETE FROM TrustedProcesses;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Список исключений не может быть очищен", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool IsProcessInTrustedProcesses(string processMainFilePath)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT COUNT(*) FROM TrustedProcesses WHERE ProcessMainFilePath = @ProcessMainFilePath";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                int count = Convert.ToInt32(dbCommand.ExecuteScalar().ToString());
                dbConnection.Close();
                if (count == 0) return false; else return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsProcessInProcessesMD5(string processMainFilePath)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT COUNT(*) FROM ProcessesMD5 WHERE ProcessMainFilePath = @ProcessMainFilePath";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                int count = Convert.ToInt32(dbCommand.ExecuteScalar().ToString());
                dbConnection.Close();
                if (count == 0) return false; else return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CompareProcessesMD5(string processMainFilePath, string newMD5)
        {
            bool buffer = false;
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT * FROM ProcessesMD5 WHERE ProcessMainFilePath = @ProcessMainFilePath";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                using (SqlDataReader dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        if (dbReader["MD5"].ToString() == newMD5)
                        {
                            buffer = true;
                        }
                        else
                        {
                            buffer = false;
                        }
                    }
                }
                dbConnection.Close();
                return buffer;
            }
            catch
            {
                return false;
            }
        }

        public static void InsertProcessMD5(FileMD5 fileMD5)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "INSERT INTO ProcessesMD5 (ProcessMainFilePath, MD5, Date) VALUES (@ProcessMainFilePath, @MD5, @Date)";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", fileMD5.MainFilePath);
                dbCommand.Parameters.AddWithValue("@MD5", fileMD5.Md5);
                dbCommand.Parameters.AddWithValue("@Date", fileMD5.Date);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Контрольная сумма даного процесса не может быть добавлена в базу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void DeleteProcessMD5(string processMainFilePath)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "DELETE FROM ProcessesMD5 WHERE ProcessMainFilePath = @ProcessMainFilePath";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Контрольная сумма даного процесса не может быть удалена из базы", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ClearProcessesMD5()
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "DELETE FROM ProcessesMD5;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Список контрольных сумм не может быть очищен", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void UpdateProcessMD5(FileMD5 fileMD5)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "UPDATE ProcessesMD5 SET MD5 = @md5, Date=@Date WHERE ProcessMainFilePath = @ProcessMainFilePath;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", fileMD5.MainFilePath);
                dbCommand.Parameters.AddWithValue("@MD5", fileMD5.Md5);
                dbCommand.Parameters.AddWithValue("@Date", fileMD5.Date);
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Контрольная сумма даного файла не может быть обновлена", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<FileMD5> GetProcessesMD5()
        {

            List<FileMD5> result = new List<FileMD5>();
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT * FROM ProcessesMD5;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                using (SqlDataReader dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        result.Add(new FileMD5(
                            dbReader["ProcessMainFilePath"].ToString(), 
                            dbReader["MD5"].ToString(),
                            DateTime.Parse(dbReader["Date"].ToString())));
                    }
                }
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Невозможно получить список контрольных сумм файлов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }

        public static FileMD5 GetProcessMD5(string processMainFilePath)
        {

            FileMD5 result = new FileMD5();
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);
                string query = "SELECT * FROM ProcessesMD5 WHERE ProcessMainFilePath = @ProcessMainFilePath;";
                SqlCommand dbCommand = new SqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@ProcessMainFilePath", processMainFilePath);
                dbConnection.Open();
                using (SqlDataReader dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        result.MainFilePath = dbReader["ProcessMainFilePath"].ToString();
                        result.Md5 = dbReader["MD5"].ToString();
                        result.Date = DateTime.Parse(dbReader["Date"].ToString());
                    }
                }
                dbConnection.Close();
            }
            catch
            {
                MessageBox.Show("Невозможно получить список контрольных сумм файлов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }
    }
}
