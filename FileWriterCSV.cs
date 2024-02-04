using System.Text;
using NACHAParser;

namespace NachaFileParser
{
    public static class CSVFileWriter
    {
        #region Constants
        public const string FileHeader = "RecordType,PriorityCode,ImmediateDestination,ImmediateOrigin,FileCreationDate,FileCreationTime,FileIdModifier,RecordSize,BlockingFactor,FormatCode,ImmediateDestinationName,ImmediateOriginName,ReferenceCode";
        public const string BatchHeader = "Record Type,Service Class Code,Company Name,Company Discretionary Data,Company Identification,Standard Entry Class Code,Company Entry Description,Company Descriptive Date,Effective Date,Settlement Date (Julian),Originator Status Code,Originating DFI Identification,Batch Number";
        public const string WEBTELeDHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual Identification Number,Individual Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string CCDPPDeDHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual Identification Number,Individual Name,Payment Type Code,Addenda Record Indicator,Trace Number";
        public const string AddendaHeader = "Record Type,Payment Related Information,Addenda Sequence Number,Entry Detail Sequence Number";
        public const string NOCAddendaHeader = "Record Type,Addenda Type Code,Change Code,Original Entry Trace Number,Reserved,Original Receiving DFI Identification,Corrected Data,Reserved,Trace Number";
        public const string ReturnAddendaHeader = "Record Type,Addenda Type Code,Return Reason Code,Original Entry Trace Number,Date of Death,Original Receiving DFI Identification,Addenda Information,Trace Number";
        public const string BatchControlHeader = "Record Type,Service Class Code,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount,Total Credit Entry Dollar Amount,Company Identification,Message Authentication Code,Reserved,Originating DFI Identificiation,Batch Number";
        public const string FileControlHeader = "Record Type,Batch Count,Block Count,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount in File,Total Credit Entry Dollar Amount in File,Reserved";

        #endregion

        #region Methods
        public static void WriteCsvFile(Root root, string outputFile)
        {
            var sb = new StringBuilder();

            var fHead = root.FileContents.AchFile.FHeader;

            CsvFHRecords(sb, fHead);

            foreach (var btH in root.FileContents.AchFile.Batches)
            {
                CsvWriteBHRecords(sb, btH);

                foreach (var etR in btH.EntryRecords)
                {
                    CsvWriteEDRecords(sb, btH.BatchHeader, etR);

                    foreach (var addenda in etR.EntryDetails.AddendaRecords)
                    {
                        CsvADRecords(sb, addenda);
                    }
                }
                CsvBCRecords(sb, btH.BatchTrailer.BControl);
            }
            CsvWriteFCRecords(sb, root.FileContents.AchFile.FTrailer.FControl);

            File.WriteAllText(outputFile, sb.ToString());
        }
        public static void CsvFHRecords(StringBuilder sb, FileHeaderRecord fH)
        {
            sb.AppendLine(CSVFileWriter.FileHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                (int)fH.RecType,
                fH.PriorityCode,
                fH.ImmedDestination,
                fH.ImmedOrigin,
                fH.FileCreationDate,
                fH.FileCreationTime,
                fH.FileIDModifier,
                fH.RecSize,
                fH.BlockingFactor,
                fH.FormatCode,
                fH.ImmedDestinationName,
                fH.ImmedOriginName,
                fH.RefCode
                ));
        }
        public static void CsvWriteBHRecords(StringBuilder sb, Batch bHR)
        {
            var bH = bHR.BatchHeader;

            sb.AppendLine(CSVFileWriter.BatchHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                (int)bH.RecType,
                (int)bH.ServiceClassCode,
                bH.CoName,
                bH.CoDiscretionaryData,
                bH.CoId,
                (int)bH.StandardEntryClass,
                bH.CoEntDescription,
                bH.CoDescriptiveDate,
                bH.EffectiveEntDate,
                bH.SettlementDate,
                bH.OriginatorStatusCode,
                bH.OriginatingDFIId,
                bH.BchNum
                ));
        }
        public static void CsvWriteEDRecords(StringBuilder sb, BatchHeaderRecord bhSEC, EntryDetailRecord eR)
        {
            var eDetails = eR.EntryDetails;
            if (bhSEC.StandardEntryClass == StandardEntryClassCode.WEB || bhSEC.StandardEntryClass == StandardEntryClassCode.TEL)
            {
                sb.AppendLine(CSVFileWriter.WEBTELeDHeader);
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                (int)eDetails.RecType,
                eDetails.TransCode,
                eDetails.RDFIId,
                eDetails.CheckDigit,
                eDetails.DFIAcctNum,
                eDetails.Amt,
                eDetails.IndivIdNum,
                eDetails.IndivName,
                eDetails.PaymtTypeCode,
                eDetails.aDRecIndicator,
                eDetails.TraceNum
                ));
            }
            else if (bhSEC.StandardEntryClass == StandardEntryClassCode.CCD || bhSEC.StandardEntryClass == StandardEntryClassCode.PPD || bhSEC.StandardEntryClass == StandardEntryClassCode.COR)
            {
                sb.AppendLine(CSVFileWriter.CCDPPDeDHeader);
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                (int)eDetails.RecType,
                eDetails.TransCode,
                eDetails.RDFIId,
                eDetails.CheckDigit,
                eDetails.DFIAcctNum,
                eDetails.Amt,
                eDetails.IndivIdNum,
                eDetails.IndivName,
                eDetails.DiscretionaryData,
                eDetails.aDRecIndicator,
                eDetails.TraceNum
                ));
            }
        }
        public static void CsvADRecords(StringBuilder sb, AddendaRecord aD)
        {
            var adR = aD.Addenda;
            if (adR.AdTypeCode == AddendTypeCode.StandardAddenda)
            {
                sb.AppendLine(CSVFileWriter.AddendaHeader);
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                (int)adR.RecType,
                (int)adR.AdTypeCode,
                adR.PaymtRelatedInfo,
                adR.AddendaSeqNum,
                adR.EntDetailSeqNum
                ));
            }
            else if (adR.AdTypeCode == AddendTypeCode.ReturnAddenda)
            {
                sb.AppendLine(CSVFileWriter.ReturnAddendaHeader);
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                    (int)adR.RecType,
                    (int)adR.AdTypeCode,
                    adR.ReturnReasonCode,
                    adR.OrigTraceNum,
                    adR.DateOfDeath,
                    adR.OrigReceivingDFIId,
                    adR.AddendaInfo,
                    adR.AdTraceNum
                    ));
            }
            else if (adR.AdTypeCode == AddendTypeCode.NOCAddenda)
            {
                sb.AppendLine(CSVFileWriter.NOCAddendaHeader);
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    (int)adR.RecType,
                    (int)adR.AdTypeCode,
                    adR.ChangeCode,
                    adR.OrigTraceNum,
                    adR.Reserved1,
                    adR.OrigReceivingDFIId,
                    adR.CorrectedData,
                    adR.Reserved2,
                    adR.AdTraceNum
                    ));
            }
        }
        public static void CsvBCRecords(StringBuilder sb, BatchControlRecord bC)
        {
            sb.AppendLine(CSVFileWriter.BatchControlHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                (int)bC.RecType,
                (int)bC.ServiceClass,
                bC.EntAddendaCnt,
                bC.EntHash,
                bC.TotBchDrEntAmt,
                bC.TotBchCrEntAmt,
                bC.CoId,
                bC.MsgAuthCode,
                bC.Reserved,
                bC.OriginatingDFIId,
                bC.BchNum
                ));
        }
        public static void CsvWriteFCRecords(StringBuilder sb, FileControlRecord fC)
        {
            sb.AppendLine(CSVFileWriter.FileControlHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                (int)fC.RecType,
                fC.BchCnt,
                fC.BlockCnt,
                fC.EntAddendaCnt,
                fC.EntHash,
                fC.TotFileDrEntAmt,
                fC.TotFileCrEntAmt,
                fC.Reserved
                ));
        }

        #endregion
    }
}