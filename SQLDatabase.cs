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
            SQLInsertFileHeaderRecord(root.FileContents.AchFile.FileHeader, connectionString);
            foreach (var batch in root.FileContents.AchFile.Batches)
            {
                SQLInsertBatchHeaderRecord(batch.BatchHeader, root, connectionString);
                foreach (var entry in batch.EntryRecord)
                {
                    SQLInsertEntryDetailRecord(entry, batch, batch.BatchHeader.SECCode, connectionString);
                    if (entry.AddendaRecord != null)
                    {
                        foreach (var addenda in entry.AddendaRecord)
                        {
                            SQLInsertAddendaRecord(addenda, batch, connectionString);
                        }
                    }
                }
                SQLInsertBatchControlRecord(batch.BatchControl, batch, connectionString);
            }
            SQLInsertFileControlRecord(root.FileContents.AchFile.FileControl, root.FileContents.AchFile.FileHeader, connectionString);
        }
        private static void SQLInsertFileHeaderRecord(FileHeaderRecord fh, string connectionString)
        {
        }
        private static void SQLInsertBatchHeaderRecord(BatchHeaderRecord bh, Root root, string connectionString)
        {
        }
        private static void SQLInsertEntryDetailRecord(EntryDetailRecord ed, Batch batch, StandardEntryClassCode sec, string connectionString)
        {
        }
        private static void SQLInsertAddendaRecord(Addenda ad, Batch batch, string connectionString)
        {
        }
        private static void SQLInsertBatchControlRecord(BatchControlRecord bc, Batch batch, string connectionString)
        {
        }
        private static void SQLInsertFileControlRecord(FileControlRecord fc, FileHeaderRecord fh, string connectionString)
        {
        }
        private static SqlParameter[] GetEntryDetailParameters(EntryDetailRecord ed, Batch batch, StandardEntryClassCode sec)
        {
            return null;
        }
        private static SqlParameter[] GetAddendaParameters(Addenda ad, Batch batch, StandardEntryClassCode sec)
        {
            return null;
        }
        private static string GetStoredProcs()
        {
            return null;
        }
    }
}