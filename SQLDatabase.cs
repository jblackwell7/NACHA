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
                            SQLInsertAddendaRecord(addenda, batch, batch.BatchHeader.SECCode, connectionString);
                        }
                    }
                }
                SQLInsertBatchControlRecord(batch.BatchControl, batch, connectionString);
            }
            SQLInsertFileControlRecord(root.FileContents.AchFile.FileControl, root.FileContents.AchFile.FileHeader, connectionString);
        }
        private static void SQLInsertFileHeaderRecord(FileHeaderRecord fh, string connectionString)
        {
            string procName = GetStoredProc(fh.RecType);
            SqlParameter[] parameters = {
                    new SqlParameter("@FileheaderId", fh.FileheaderId),
                    new SqlParameter("@RecordType", (int)fh.RecType),
                    new SqlParameter("@PriorityCode", fh.PriorityCode),
                    new SqlParameter("@ImmedDestination", fh.ImmedDestination),
                    new SqlParameter("@ImmedOrigin", fh.ImmedOrigin),
                    new SqlParameter("@FileCreationDate", fh.FileCreationDate),
                    new SqlParameter("@FileCreationTime", fh.FileCreationTime),
                    new SqlParameter("@FileIDModifier", fh.FileIDModifier),
                    new SqlParameter("@RecSize", fh.RecSize),
                    new SqlParameter("@BlockingFactor", fh.BlockingFactor),
                    new SqlParameter("@FormatCode", fh.FormatCode),
                    new SqlParameter("@ImmedDestinationName", fh.ImmedDestinationName),
                    new SqlParameter("@ImmedOriginName", fh.ImmedOriginName),
                    new SqlParameter("@RefCode", fh.RefCode)
                    };
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertBatchHeaderRecord(BatchHeaderRecord bh, Root root, string connectionString)
        {
            string procName = GetStoredProc(bh.RecType);
            SqlParameter[] parameters = {
                new SqlParameter("@BatchHeaderId", bh.BchHeaderId),
                new SqlParameter("@FileheaderId", root.FileContents.AchFile.FileHeader.FileheaderId),
                new SqlParameter("@RecordType", (int)bh.RecType),
                new SqlParameter("@ServiceClassCode", (int)bh.ServiceClassCode),
                new SqlParameter("@CompanyName", bh.CoName),
                new SqlParameter("@CompanyDiscretionaryData", bh.CoDiscretionaryData),
                new SqlParameter("@CompanyID", bh.CoId),
                new SqlParameter("@StandardEntryClassCode", bh.SECCode.ToString()),
                new SqlParameter("@CompanyEntryDescription", bh.CoEntDescription),
                new SqlParameter("@CompanyDescriptiveDate", bh.CoDescriptiveDate),
                new SqlParameter("@EffectiveEntryDate", bh.EffectiveEntDate),
                new SqlParameter("@SettlementDate", bh.SettlementDate),
                new SqlParameter("@OriginatorStatusCode", bh.OriginatorStatusCode),
                new SqlParameter("@OriginatingDFIId", bh.OriginatingDFIId),
                new SqlParameter("@BatchNumber", bh.BchNum),
            };
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertEntryDetailRecord(EntryDetailRecord ed, Batch batch, StandardEntryClassCode sec, string connectionString)
        {
            SqlParameter[] parameters = GetEntryDetailParameters(ed, batch, sec);
            string procName = GetStoredProc(ed.RecType);
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertAddendaRecord(Addenda ad, Batch batch, StandardEntryClassCode sec, string connectionString)
        {
            SqlParameter[] parameters = GetAddendaParameters(ad, batch, sec);
            string procName = GetStoredProc(ad.RecType);
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertBatchControlRecord(BatchControlRecord bc, Batch batch, string connectionString)
        {
            string procName = GetStoredProc(bc.RecType);
            SqlParameter[] parameters = {
                    new SqlParameter("@BatchControlId", bc.BchControlId),
                    new SqlParameter("@BatchHeaderId", batch.BatchHeader.BchHeaderId),
                    new SqlParameter("@RecordType", (int)bc.RecType),
                    new SqlParameter("@ServiceClassCode", (int)bc.ServiceClassCode),
                    new SqlParameter("@EntryAddendaCount", bc.EntAddendaCnt),
                    new SqlParameter("@EntryHash", bc.EntHash),
                    new SqlParameter("@TotalDebitAmount", bc.TotBchDrEntAmt),
                    new SqlParameter("@TotalCreditAmount", bc.TotBchCrEntAmt),
                    new SqlParameter("@CompanyID", bc.CoId),
                    new SqlParameter("@MessageAuthenticationCode", bc.MsgAuthCode),
                    new SqlParameter("@Reserved", bc.Reserved),
                    new SqlParameter("@OriginatingDFIId", bc.OriginatingDFIId),
                    new SqlParameter("@BatchNumber", bc.BchNum),
            };
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertFileControlRecord(FileControlRecord fc, FileHeaderRecord fh, string connectionString)
        {string procName = GetStoredProc(fc.RecType);
            SqlParameter[] parameters = {
                new SqlParameter("@FileControlId", fc.FileControlId),
                new SqlParameter("@FileheaderId", fh.FileheaderId),
                new SqlParameter("@RecordType", (int)fc.RecType),
                new SqlParameter("@BatchCount", fc.BchCnt),
                new SqlParameter("@BlockCount", fc.BlockCnt),
                new SqlParameter("@EntryAddendaCount", fc.EntAddendaCnt),
                new SqlParameter("@EntryHash", fc.EntHash),
                new SqlParameter("@TotalDebitAmount", fc.TotFileDrEntAmt),
                new SqlParameter("@TotalCreditAmount", fc.TotFileCrEntAmt),
                new SqlParameter("@Reserved", fc.Reserved),
            };

            ExecuteProc(connectionString, "sp_InsertFileControlRecord", parameters);
        }
        private static SqlParameter[] GetEntryDetailParameters(EntryDetailRecord ed, Batch batch, StandardEntryClassCode sec)
        {
            return null;
        }
        private static SqlParameter[] GetAddendaParameters(Addenda ad, Batch batch, StandardEntryClassCode sec)
        {
            return null;
        }
        private static string GetStoredProc(RecordType recType)
        {
            //TODO: ACH-22 Add StandardEntryClassCode logic
            //TODO: ACH-23 RecordType.ed and AdTypeCode for RecordType.ad
            //TODO: ACH-24 Add stored procs for to support ACH-23

            switch (recType)
            {
                case RecordType.fh:
                    return "sp_InsertFileHeaderRecord";
                case RecordType.bh:
                    return "sp_InsertBatchHeaderRecord";
                case RecordType.ed:
                    return "sp_InsertEntryDetailRecord";
                case RecordType.ad:
                    return "sp_InsertAddendaRecord";
                case RecordType.bc:
                    return "sp_InsertBatchControlRecord";
                case RecordType.fc:
                    return "sp_InsertFileControlRecord";
                default:
                    throw new InvalidOperationException($"There is not a valid Stored Procedure for Record Type '{recType}' is not supported");
            }
        }
    }
}