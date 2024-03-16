using System.Text;
using Microsoft.Data.SqlClient;

namespace NACHAParser
{
    public class SQLDatabase
    {
        public static void ExecuteProc(string connectionString, string procName, SqlParameter[] parameters)
        {
            var sb = new StringBuilder($"EXEC {procName} ");

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
                sb.Length -= 2;
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
                        Console.WriteLine($"SQL Error. Stored Proc '{procName}': Msg {error.Number},\nLevel {error.Class},\nState {error.State},\nLine {error.LineNumber}\n{error.Message}\n");
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
                    SQLInsertEntryDetailRecord(entry, batch, connectionString);
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
                new SqlParameter("@OriginatorStatusCode", (int)bh.OriginatorStatusCode),
                new SqlParameter("@OriginatingDFIId", bh.OriginatingDFIId),
                new SqlParameter("@BatchNumber", bh.BchNum),
            };
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertEntryDetailRecord(EntryDetailRecord ed, Batch batch, string connectionString)
        {
            SqlParameter[] parameters = GetEntryDetailParameters(ed, batch, batch.BatchHeader.SECCode);
            string procName = GetStoredProc(ed.RecType);
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertAddendaRecord(Addenda ad, Batch batch, string connectionString)
        {
            SqlParameter[] parameters = GetAddendaParameters(ad, batch);
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
        {
            string procName = GetStoredProc(fc.RecType);
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

            ExecuteProc(connectionString, procName, parameters);
        }
        private static SqlParameter[] GetEntryDetailParameters(EntryDetailRecord ed, Batch batch, StandardEntryClassCode sec)
        {
            //TODO: ACH-26 Add logic for additional batches
            List<SqlParameter> parameters = new List<SqlParameter>();
            switch (sec)
            {
                case StandardEntryClassCode.PPD:
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", (int)ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", (int)ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", (int)ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.WEB:
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", (int)ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", (int)ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", (int)ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.POP:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("CheckSerialNumber", ed.CheckSerialNum));
                    parameters.Add(new SqlParameter("TerminalCity", ed.TerminalCity));
                    parameters.Add(new SqlParameter("TerminalState", ed.TerminalState));
                    parameters.Add(new SqlParameter("DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.POS:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("CardTransactionType", ed.CardTransTypeCode));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.CCD:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.COR:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.ACK:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("OriginalTraceNumber", ed.OriginalTraceNum));
                    parameters.Add(new SqlParameter("ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.ATX:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("OriginalTraceNumber", ed.OriginalTraceNum));
                    parameters.Add(new SqlParameter("ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.TEL:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.CTX:
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("NumberOfAddendaRecords", ed.NumOfAddendaRecords));
                    parameters.Add(new SqlParameter("ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.aDRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    parameters.Add(new SqlParameter("RESERVED", ed.Reserved));
                    break;
            }
            return parameters.ToArray();
        }
        private static SqlParameter[] GetAddendaParameters(Addenda ad, Batch batch)
        {
            //TODO: ACH-25 Returns, Add COR, Contested COR, Dishonor &  Contested Support
            List<SqlParameter> parameters = new List<SqlParameter>();
            var lastEntry = batch.EntryRecord.LastOrDefault();
            switch (ad.AdTypeCode)
            {
                case AddendaTypeCode.StandardAddenda:
                    parameters.Add(new SqlParameter("Addenda05Id", ad.Addenda05Id));
                    parameters.Add(new SqlParameter("EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", batch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", (int)ad.RecType));
                    parameters.Add(new SqlParameter("AddendaTypeCode", (int)ad.AdTypeCode));
                    parameters.Add(new SqlParameter("PaymentRelatedInformation", ad.PaymtRelatedInfo));
                    parameters.Add(new SqlParameter("AddendaSequenceNumber", ad.AddendaSeqNum));
                    parameters.Add(new SqlParameter("EntryDetailSequenceNumber", ad.EntDetailSeqNum));
                    break;
            }
            return parameters.ToArray();
        }
        private static string GetStoredProc(RecordType recType)
        {
            //root.FileContents.AchFile.Batches[0].BatchHeader.SECCode.ToString();
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
                    return "sp_InsertWEBEntryDetailRecord";
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