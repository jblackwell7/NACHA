using System.Text;
using Microsoft.Data.SqlClient;

namespace NACHAParser
{
    public class SQLDatabase
    {
        public static void ExecuteProc(string connectionString, string procName, SqlParameter[] parameters)
        {
            var sb = new StringBuilder($"Executing {procName} with parameters: ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(procName, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddRange(parameters);

                foreach (SqlParameter p in parameters)
                {
                    var value = p.Value is DBNull ? "NULL" : $"'{p.Value}'";
                    sb.Append($"{p.ParameterName}={value}, ");
                }
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Store Proc: '{procName}' executed successfully: {sb}\n");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error executing Stored Proc '{procName}': {sb}\n");
                    foreach (SqlError error in ex.Errors)
                    {
                        Console.WriteLine($"SQL Error. Stored Proc '{procName}': Msg {error.Number},\nLevel {error.Class},\nState {error.State},\nLine {error.LineNumber}\n{error.Message}");
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public static void SQLInsertData(string connectionString, Root root)
        {
        }
        private static void SQLInsertFileHeaderRecord()
        {
        }
        private static void SQLInsertBatchHeaderRecord()
        {
        }
        private static void SQLInsertEntryDetailRecord()
        {
        }
        private static void SQLInsertAddendaRecord()
        {
        }
        private static void SQLInsertBatchControlRecord()
        {
        }
        private static void SQLInsertFileControlRecord()
        {
        }
        private static string GetStoredProcs()
        {
            return null;
        }
    }
}