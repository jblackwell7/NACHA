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
            if (root != null)
            {
                if (root.FileContents!.ACHFile != null)
                {
                    SQLInsertFileHeaderRecord(root.FileContents.ACHFile.FileHeader!, root.FileContents.ACHFile, connectionString);
                    foreach (var batch in root.FileContents.ACHFile.Batches!)
                    {
                        SQLInsertBatchHeaderRecord(batch.BatchHeader!, root, connectionString);
                        foreach (var entry in batch.EntryRecord!)
                        {
                            SQLInsertEntryDetailRecord(entry, root.FileContents.ACHFile, connectionString);
                            if (entry.AddendaRecord != null)
                            {
                                foreach (var addenda in entry.AddendaRecord)
                                {
                                    SQLInsertAddendaRecord(addenda, root.FileContents.ACHFile, connectionString);
                                }
                            }
                        }
                        SQLInsertBatchControlRecord(batch.BatchControl!, root.FileContents.ACHFile, connectionString);
                    }
                    SQLInsertFileControlRecord(root.FileContents.ACHFile.FileControl!, root.FileContents.ACHFile.FileHeader!, root.FileContents.ACHFile, connectionString);
                }
                else
                {
                    throw new Exception("ACH File is null");
                }
            }
            else
            {
                throw new Exception("Root is null");
            }
        }
        private static void SQLInsertFileHeaderRecord(FileHeaderRecord fh, ACHFile achFile, string connectionString)
        {
            string procName = GetStoredProc(fh.RecType, achFile);
            SqlParameter[] parameters = {
                    new SqlParameter("@AchFileId", achFile.AchFileId),
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
            string procName = GetStoredProc(bh.RecType, root.FileContents.ACHFile);
            SqlParameter[] parameters = {
                new SqlParameter("@AchFileId", root.FileContents.ACHFile.AchFileId),
                new SqlParameter("@BatchHeaderId", bh.BchHeaderId),
                new SqlParameter("@FileheaderId", root.FileContents.ACHFile.FileHeader.FileheaderId),
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
        private static void SQLInsertEntryDetailRecord(EntryDetailRecord ed, ACHFile achFile, string connectionString)
        {
            SqlParameter[] parameters = GetEntryDetailParameters(ed, achFile);
            string procName = GetStoredProc(ed.RecType, achFile);
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertAddendaRecord(AddendaRecord ad, ACHFile achFile, string connectionString)
        {
            string procName = GetStoredProc(ad.RecType, achFile);
            SqlParameter[] parameters = GetAddendaParameters(ad, achFile, procName);
            ExecuteProc(connectionString, procName, parameters);
        }
        private static void SQLInsertBatchControlRecord(BatchControlRecord bc, ACHFile achFile, string connectionString)
        {
            string procName = GetStoredProc(bc.RecType, achFile);
            SqlParameter[] parameters = {
                    new SqlParameter("@AchFileId", achFile.AchFileId),
                    new SqlParameter("@BatchControlId", bc.BchControlId),
                    new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId),
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
        private static void SQLInsertFileControlRecord(FileControlRecord fc, FileHeaderRecord fh, ACHFile achFile, string connectionString)
        {
            string procName = GetStoredProc(fc.RecType, achFile);
            SqlParameter[] parameters = {
                new SqlParameter("@AchFileId", achFile.AchFileId),
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
        private static SqlParameter[] GetEntryDetailParameters(EntryDetailRecord ed, ACHFile achFile)
        {
            //TODO: ACH-26 Add logic for additional batches
            List<SqlParameter> parameters = new List<SqlParameter>();
            switch (achFile.CurrentBatch.BatchHeader.SECCode)
            {
                case StandardEntryClassCode.ACK:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ed.OriginalTraceNum));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.ARC:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ed.OriginalTraceNum));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.ATX:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ed.OriginalTraceNum));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.CCD:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.CIE:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.COR:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("Amount", ed.Amt));
                    parameters.Add(new SqlParameter("IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.CTX:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
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
                    parameters.Add(new SqlParameter("AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("TraceNumber", ed.TraceNum));
                    parameters.Add(new SqlParameter("RESERVED", ed.Reserved));
                    break;
                case StandardEntryClassCode.DNE:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.ENR:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@NumberOfAddendaRecords", ed.NumOfAddendaRecords));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@Reserved", ed.Reserved));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.MTE:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.POP:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@CheckSerialNumber", ed.CheckSerialNum));
                    parameters.Add(new SqlParameter("@TerminalCity", ed.TerminalCity));
                    parameters.Add(new SqlParameter("@TerminalState", ed.TerminalState));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.POS:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@CardTransactionType", ed.CardTransTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.PPD:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", (int)ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", (int)ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", (int)ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.RCK:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@CheckSerialNumber", ed.CheckSerialNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.SHR:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@CardExpirationDate", ed.CardExpirationDate));
                    parameters.Add(new SqlParameter("@DocumentReferenceNumber", ed.DocRefNum));
                    parameters.Add(new SqlParameter("@IndividualCardAccountNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@CardTransactionType", ed.CardTransTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.TEL:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.TRC:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@CheckSerialNumber", ed.CheckSerialNum));
                    parameters.Add(new SqlParameter("@ProcessControlField", ed.ProcessControlField));
                    parameters.Add(new SqlParameter("@ItemResearchNumber", ed.ItemResearchNum));
                    parameters.Add(new SqlParameter("@ItemTypeIndicator", ed.ItemTypeIndicator));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.TRX:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@TotalAmount", ed.TotalAmt));
                    parameters.Add(new SqlParameter("@IndividualNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@ReceivingCompanyName", ed.ReceiverCoName));
                    parameters.Add(new SqlParameter("@Reserved", ed.Reserved));
                    parameters.Add(new SqlParameter("@ItemTypeIndicator", ed.ItemTypeIndicator));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.WEB:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", (int)ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", (int)ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@IndividualIdNumber", ed.IndivIdNum));
                    parameters.Add(new SqlParameter("@IndividualName", ed.IndivName));
                    parameters.Add(new SqlParameter("@PaymentTypeCode", ed.PaymtTypeCode));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", (int)ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                case StandardEntryClassCode.XCK:
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@EntryDetailId", ed.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ed.RecType));
                    parameters.Add(new SqlParameter("@TransactionCode", ed.TransCode));
                    parameters.Add(new SqlParameter("@ReceivingDFIId", ed.RDFIId));
                    parameters.Add(new SqlParameter("@CheckDigit", ed.CheckDigit));
                    parameters.Add(new SqlParameter("@DFIAccountNumber", ed.DFIAcctNum));
                    parameters.Add(new SqlParameter("@Amount", ed.Amt));
                    parameters.Add(new SqlParameter("@CheckSerialNumber", ed.CheckSerialNum));
                    parameters.Add(new SqlParameter("@ProcessControlField", ed.ProcessControlField));
                    parameters.Add(new SqlParameter("@ItemResearchNumber", ed.ItemResearchNum));
                    parameters.Add(new SqlParameter("@DiscretionaryData", ed.DiscretionaryData));
                    parameters.Add(new SqlParameter("@AddendaRecordIndicator", ed.adRecIndicator));
                    parameters.Add(new SqlParameter("@TraceNumber", ed.TraceNum));
                    break;
                default:
                    throw new Exception($"Standard Entry Class Code '{achFile.CurrentBatch.BatchHeader.SECCode}' is not supported");
            }
            return parameters.ToArray();
        }
        private static SqlParameter[] GetAddendaParameters(AddendaRecord ad, ACHFile achFile, string procName)
        {
            //TODO: ACH-25 Returns, Add COR, Contested COR, Dishonor &  Contested Support
            List<SqlParameter> parameters = new List<SqlParameter>();
            var lastEntry = achFile.CurrentBatch.EntryRecord.LastOrDefault();
            switch (procName)
            {
                case "sp_InsertAddendaRecord":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("RecordType", (int)ad.RecType));
                    parameters.Add(new SqlParameter("AddendaTypeCode", (int)ad.AdTypeCode));
                    parameters.Add(new SqlParameter("PaymentRelatedInformation", ad.PaymtRelatedInfo));
                    parameters.Add(new SqlParameter("AddendaSequenceNumber", ad.AddendaSeqNum));
                    parameters.Add(new SqlParameter("EntryDetailSequenceNumber", ad.EntDetailSeqNum));
                    break;
                case "sp_InsertAddendaRecord_COR":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("RecordType", (int)ad.RecType));
                    parameters.Add(new SqlParameter("AddendaTypeCode", (int)ad.AdTypeCode));
                    parameters.Add(new SqlParameter("ChanceCode", ad.ChangeCode));
                    parameters.Add(new SqlParameter("OriginalTraceNumber", ad.OrigTraceNum));
                    parameters.Add(new SqlParameter("OriginalReceivingDFIId", ad.OrigReceivingDFIId));
                    parameters.Add(new SqlParameter("Reserved", ad.Reserved1));
                    parameters.Add(new SqlParameter("CorrectedData", ad.CorrectedData));
                    parameters.Add(new SqlParameter("Reserved2", ad.Reserved2));
                    parameters.Add(new SqlParameter("AddendaTraceNumber", ad.AdTraceNum));
                    break;
                case "sp_InsertAddendaRecord_MTE":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("@EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ad.RecType));
                    parameters.Add(new SqlParameter("@AddendaTypeCode", ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@TransactionDescription", ad.TransDescription));
                    parameters.Add(new SqlParameter("@NetworkIdentificationCode", ad.NetworkIdCode));
                    parameters.Add(new SqlParameter("@TerminalIdCode", ad.TerminalIDCode));
                    parameters.Add(new SqlParameter("@TransactionSerialNumber", ad.TransSerialNum));
                    parameters.Add(new SqlParameter("@TransactionDate", ad.TransDate));
                    parameters.Add(new SqlParameter("@TransactionTime", ad.TransTime));
                    parameters.Add(new SqlParameter("@TerminalLocation", ad.TerminalLoc));
                    parameters.Add(new SqlParameter("@TerminalCity", ad.TerminalCity));
                    parameters.Add(new SqlParameter("@TerminalState", ad.TerminalState));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                break;
                case "sp_InsertAddendaRecord_SHR":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("RecordType", (int)ad.RecType));
                    parameters.Add(new SqlParameter("AddendaTypeCode", (int)ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@ReferenceInformation1", ad.RefInfo1));
                    parameters.Add(new SqlParameter("@ReferenceInformation2", ad.RefInfo2));
                    parameters.Add(new SqlParameter("@TerminalIdCode", ad.TerminalIDCode));
                    parameters.Add(new SqlParameter("@TransactionSerialNumber", ad.TransSerialNum));
                    parameters.Add(new SqlParameter("@TransactionDate", ad.TransDate));
                    parameters.Add(new SqlParameter("@AuthorizationCodeOrExpireDate", ad.AuthCodeOrExpDate));
                    parameters.Add(new SqlParameter("@TerminalLocation", ad.TerminalLoc));
                    parameters.Add(new SqlParameter("@TerminalCity", ad.TerminalCity));
                    parameters.Add(new SqlParameter("@TerminalState", ad.TerminalState));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                break;
                case "sp_InsertAddendaRecord_POS":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("@EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ad.RecType));
                    parameters.Add(new SqlParameter("@AddendaTypeCode", ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@ReferenceInformation1", ad.RefInfo1));
                    parameters.Add(new SqlParameter("@ReferenceInformation2", ad.RefInfo2));
                    parameters.Add(new SqlParameter("@TerminalIdCode", ad.TerminalIDCode));
                    parameters.Add(new SqlParameter("@TransactionSerialNumber", ad.TransSerialNum));
                    parameters.Add(new SqlParameter("@TransactionDate", ad.TransDate));
                    parameters.Add(new SqlParameter("@AuthorizationCodeOrExpireDate", ad.AuthCodeOrExpDate));
                    parameters.Add(new SqlParameter("@TerminalLocation", ad.TerminalLoc));
                    parameters.Add(new SqlParameter("@TerminalCity", ad.TerminalCity));
                    parameters.Add(new SqlParameter("@TerminalState", ad.TerminalState));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                    break;
                case "sp_InsertAddendaRecord_RefusedCOR":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("RecordType", (int)ad.RecType));
                    parameters.Add(new SqlParameter("AddendaTypeCode", (int)ad.AdTypeCode));
                    parameters.Add(new SqlParameter("ChanceCode", ad.ChangeCode));
                    parameters.Add(new SqlParameter("RefusedCORCode", ad.RefusedCORCode));
                    parameters.Add(new SqlParameter("OriginalTraceNumber", ad.OrigTraceNum));
                    parameters.Add(new SqlParameter("OriginalReceivingDFIId", ad.OrigReceivingDFIId));
                    parameters.Add(new SqlParameter("Reserved", ad.Reserved1));
                    parameters.Add(new SqlParameter("CorrectedData", ad.CorrectedData));
                    parameters.Add(new SqlParameter("Reserved2", ad.Reserved2));
                    parameters.Add(new SqlParameter("CORTraceSequenceNumber", ad.CorTraceSeqNum));
                    parameters.Add(new SqlParameter("AddendaTraceNumber", ad.AdTraceNum));
                    break;
                case "sp_InsertAddendaRecord_Return":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("@EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ad.RecType));
                    parameters.Add(new SqlParameter("@AddendaTypeCode", ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ad.OrigTraceNum));
                    parameters.Add(new SqlParameter("@OriginalReceivingDFIId", ad.OrigReceivingDFIId));
                    parameters.Add(new SqlParameter("@AddendaInformation", ad.AddendaInfo));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                    break;
                case "sp_InsertAddendaRecord_Dishonor":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("@EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ad.RecType));
                    parameters.Add(new SqlParameter("@AddendaTypeCode", ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@DishonorReturnReasonCode", ad.DisHonorReturnReasonCode));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ad.OrigTraceNum));
                    parameters.Add(new SqlParameter("@OriginalReceivingDFIId", ad.OrigReceivingDFIId));
                    parameters.Add(new SqlParameter("@Reserved", ad.Reserved1));
                    parameters.Add(new SqlParameter("@ReturnTraceNumber", ad.ReturnTraceNum));
                    parameters.Add(new SqlParameter("@ReturnSettlementDate", ad.ReturnSettlementDate));
                    parameters.Add(new SqlParameter("@Reserved2", ad.Reserved2));
                    parameters.Add(new SqlParameter("@ReturnCode", ad.DReturnReasonCode));
                    parameters.Add(new SqlParameter("@AddendaInformation", ad.AddendaInfo));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                    break;
                case "sp_InsertAddendaRecord_ContestedDishonor":
                    parameters.Add(new SqlParameter("@AchFileId", achFile.AchFileId));
                    parameters.Add(new SqlParameter("@AddendaId", ad.AddendaId));
                    parameters.Add(new SqlParameter("@EntryDetailId", lastEntry.EntDetailsId));
                    parameters.Add(new SqlParameter("@BatchHeaderId", achFile.CurrentBatch.BatchHeader.BchHeaderId));
                    parameters.Add(new SqlParameter("@RecordType", ad.RecType));
                    parameters.Add(new SqlParameter("@AddendaTypeCode", ad.AdTypeCode));
                    parameters.Add(new SqlParameter("@ContestedDishonorCode", ad.ContestedDisHonorReturnReasonCode));
                    parameters.Add(new SqlParameter("@OriginalTraceNumber", ad.OrigTraceNum));
                    parameters.Add(new SqlParameter("@DateOriginalEntryReturned", ad.DateOriginalEntryReturned));
                    parameters.Add(new SqlParameter("@OriginalReceivingDFIId", ad.OrigReceivingDFIId));
                    parameters.Add(new SqlParameter("@OriginalSettlementDate", ad.OriginalSettlementDate));
                    parameters.Add(new SqlParameter("@ReturnTraceNumber", ad.ReturnTraceNum));
                    parameters.Add(new SqlParameter("@ReturnSettlementDate", ad.ReturnSettlementDate));
                    parameters.Add(new SqlParameter("@ReturnCode", ad.CReturnReasonCode));
                    parameters.Add(new SqlParameter("@DishonorReturnReasonCode", ad.DReturnReasonCode));
                    parameters.Add(new SqlParameter("@DisHonrorReturnTraceNumber", ad.DisHonrorReturnTraceNum));
                    parameters.Add(new SqlParameter("@DisHonrorReturnSettlementDate", ad.DisHonrorReturnSettlementDate));
                    parameters.Add(new SqlParameter("@Reserved", ad.Reserved1));
                    parameters.Add(new SqlParameter("@AddendaInformation", ad.AddendaInfo));
                    parameters.Add(new SqlParameter("@AddendaTraceNumber", ad.AdTraceNum));
                    break;
                default:
                    throw new Exception($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
            }
            return parameters.ToArray();
        }
        private static string GetStoredProc(RecordType recType, ACHFile achFile)
        {
            var lastEntry = achFile.CurrentBatch.EntryRecord.LastOrDefault();
            var ad = lastEntry.AddendaRecord.LastOrDefault();
            var bh = achFile.CurrentBatch.BatchHeader;

            switch (recType)
            {
                case RecordType.fh:
                    return "sp_InsertFileHeaderRecord";
                case RecordType.bh:
                    return "sp_InsertBatchHeaderRecord";
                case RecordType.ed:
                    if (bh.SECCode == StandardEntryClassCode.ACK)
                    {
                        return "sp_InsertEntryDetailRecord_ACK";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.ARC)
                    {
                        return "sp_InsertEntryDetailRecord_ARC";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.ATX)
                    {
                        return "sp_InsertEntryDetailRecord_ATX";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.BOC)
                    {
                        return "sp_InsertEntryDetailRecord_BOC";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.CCD)
                    {
                        return "sp_InsertEntryDetailRecord_CCD";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.CIE)
                    {
                        return "sp_InsertEntryDetailRecord_CIE";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.COR)
                    {
                        return "sp_InsertEntryDetailRecord_COR";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.DNE)
                    {
                        return "sp_InsertEntryDetailRecord_DNE";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.ENR)
                    {
                        return "sp_InsertEntryDetailRecord_ENR";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.CTX)
                    {
                        return "sp_InsertEntryDetailRecord_CTX";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.MTE)
                    {
                        return "sp_InsertEntryDetailRecord_MTE";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.POP)
                    {
                        return "sp_InsertEntryDetailRecord_POP";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.POS)
                    {
                        return "sp_InsertEntryDetailRecord_POS";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.PPD)
                    {
                        return "sp_InsertEntryDetailRecord_PPD";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.RCK)
                    {
                        return "sp_InsertEntryDetailRecord_RCK";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.SHR)
                    {
                        return "sp_InsertEntryDetailRecord_SHR";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.TEL)
                    {
                        return "sp_InsertEntryDetailRecord_TEL";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.TRC)
                    {
                        return "sp_InsertEntryDetailRecord_TRC";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.TRX)
                    {
                        return "sp_InsertEntryDetailRecord_TRX";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.WEB)
                    {
                        return "sp_InsertEntryDetailRecord_WEB";
                    }
                    else if (bh.SECCode == StandardEntryClassCode.XCK)
                    {
                        return "sp_InsertEntryDetailRecord_XCK";
                    }
                    else
                    {
                        throw new Exception($"Standard Entry Class Code '{bh.SECCode}' is not supported");
                    }
                case RecordType.ad:
                    if (lastEntry.adRecIndicator != AddendaRecordIndicator.Unknown || lastEntry.adRecIndicator != AddendaRecordIndicator.NoAddenda)
                    {
                        if (ad.AdTypeCode == AddendaTypeCode.Addenda02)
                        {
                            if (bh.SECCode == StandardEntryClassCode.POS)
                            {
                                return "sp_InsertAddendaRecord_POS";
                            }
                            else if (bh.SECCode == StandardEntryClassCode.MTE)
                            {
                                return "sp_InsertAddendaRecord_MTE";
                            }
                            else if (bh.SECCode == StandardEntryClassCode.SHR)
                            {
                                return "sp_InsertAddendaRecord_SHR";
                            }
                            else
                            {
                                throw new Exception($"Standard Entry Class Code '{bh.SECCode}' is not supported");
                            }
                        }
                        else if (ad.AdTypeCode == AddendaTypeCode.Addenda05)
                        {
                            return "sp_InsertAddendaRecord";
                        }
                        else if (ad.AdTypeCode == AddendaTypeCode.Addenda99)
                        {
                            if (Enum.IsDefined(typeof(ReturnCode),ad.ReturnReasonCode))
                            {
                                return "sp_InsertAddendaRecord_Return";
                            }
                            else if (Enum.IsDefined(typeof(ReturnCode),ad.DisHonorReturnReasonCode))
                            {
                                return "sp_InsertAddendaRecord_Dishonor";
                            }
                            else if (Enum.IsDefined(typeof(ReturnCode),ad.ContestedDisHonorReturnReasonCode))
                            {
                                return "sp_InsertAddendaRecord_ContestedDishonor";
                            }
                            else
                            {
                                throw new Exception("Addenda Return Code is not supported");
                            }
                        }
                        else if (ad.AdTypeCode == AddendaTypeCode.Addenda98)
                        {
                            if (ad.IsRefusedCORCode(ad.ChangeCode) == false)
                            {
                                return "sp_InsertAddendaRecord_COR";
                            }
                            else if (ad.IsRefusedCORCode(ad.ChangeCode) == true)
                            {
                                return "sp_InsertAddendaRecord_ResfusedCOR";
                            }
                            else
                            {
                                throw new Exception($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                            }
                        }
                        else
                        {
                            throw new Exception($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                        }
                    }
                    else
                    {
                        throw new($"Addenda Record Indicator is not supported for Record Type '{recType}'");
                    }
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