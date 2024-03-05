using System.Text;


namespace NACHAParser
{
    public static class CSVFileWriter
    {
        #region Constants
        public const string FileHeader = "RecordType,PriorityCode,ImmediateDestination,ImmediateOrigin,FileCreationDate,FileCreationTime,FileIdModifier,RecordSize,BlockingFactor,FormatCode,ImmediateDestinationName,ImmediateOriginName,ReferenceCode";
        public const string BatchHeader = "Record Type,Service Class Code,Company Name,Company Discretionary Data,Company Identification,Standard Entry Class Code,Company Entry Description,Company Descriptive Date,Effective Date,Settlement Date (Julian),Originator Status Code,Originating DFI Identification,Batch Number";
        public const string WEBTELeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string TELDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Payment Type Code,Addenda Record Indicator,Trace Number";
        public const string CCDeDtailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,ID Number,Receiving Company Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string PPDeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Payment Type Code,Addenda Record Indicator,Trace Number";
        public const string POPDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Check Serial Number,Terminal City, Terminal State, Individual Name/Receiving Company Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string POSDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Card Transaction Type,Terminal City, Terminal State, Card Transaction Type,Addenda Record Indicator,Trace Number";
        public const string CTXeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Number of Addenda Records,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string CTXNOCeDetailsHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Number of Addenda Records,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string NOCeDetailsHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string ReturnAddendaHeader = "Record Type,Addenda Type Code,Return Reason Code,Original Entry Trace Number,Date of Death,Original Receiving DFI Identification,Addenda Information,Trace Number";
        public const string DishonorReturnAddendaHeader = "Record Type,Addenda Type Code,DisHonrorReturnCode,Original Entry Trace Number,Reserved,Original Receiving DFI Identification,Reserved,Return Trace Number,Return Settlement Date,Dis Honor Return Reason Code,Addenda Information,Trace Number";
        public const string ContestedDishonorReturnAddendaHeader = "Record Type,Addenda Type Code,Contested Dishonor Return Reason Code,Original Entry Trace Number,Date Original Entry Returned,Original Receiving DFI Identification,Original Settlement Date,Return Trace Number,Return Settlement Date,Return Reason Code,Dishonor Return Settlement Date,Dishonor Return Reason Code, Reserved,Trace Number";
        public const string AddendaHeader = "Record Type,Payment Related Information,Addenda Sequence Number,Entry Detail Sequence Number";
        public const string NOCAddendaHeader = "Record Type,Addenda Type Code,Change Code,Original Entry Trace Number,Reserved,Original Receiving DFI Identification,Corrected Data,Reserved,Trace Number";
        public const string BatchControlHeader = "Record Type,Service Class Code,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount,Total Credit Entry Dollar Amount,Company Identification,Message Authentication Code,Reserved,Originating DFI Identificiation,Batch Number";
        public const string FileControlHeader = "Record Type,Batch Count,Block Count,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount in File,Total Credit Entry Dollar Amount in File,Reserved";
        #endregion
        #region Methods
        public static void WriteCsvFile(Root root, string outputFile)
        {
            var sb = new StringBuilder();
            var fh = root.FileContents.AchFile.FileHeader;
            CsvFHRecords(sb, fh);
            foreach (var batch in root.FileContents.AchFile.Batches)
            {
                CsvWriteBHRecords(sb, batch);
                foreach (var etR in batch.EntryRecord)
                {
                    CsvWriteEDRecords(sb, batch.BatchHeader, etR);
                    foreach (var addenda in etR.AddendaRecord)
                    {
                        CsvADRecords(sb, addenda);
                    }
                }
                CsvBCRecords(sb, batch.BatchControl);
            }
            CsvWriteFCRecords(sb, root.FileContents.AchFile.FileControl);
            File.WriteAllText(outputFile, sb.ToString());
        }
        public static void CsvFHRecords(StringBuilder sb, FileHeaderRecord fh)
        {
            sb.AppendLine(FileHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                (int)fh.RecType,
                fh.PriorityCode,
                fh.ImmedDestination,
                fh.ImmedOrigin,
                fh.FileCreationDate,
                fh.FileCreationTime,
                fh.FileIDModifier,
                fh.RecSize,
                fh.BlockingFactor,
                fh.FormatCode,
                fh.ImmedDestinationName,
                fh.ImmedOriginName,
                fh.RefCode
                ));
        }
        public static void CsvWriteBHRecords(StringBuilder sb, Batch b)
        {
            var bh = b.BatchHeader;
            sb.AppendLine(BatchHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                (int)bh.RecType,
                (int)bh.ServiceClassCode,
                bh.CoName,
                bh.CoDiscretionaryData,
                bh.CoId,
                (int)bh.SECCode,
                bh.CoEntDescription,
                bh.CoDescriptiveDate,
                bh.EffectiveEntDate,
                bh.SettlementDate,
                bh.OriginatorStatusCode,
                bh.OriginatingDFIId,
                bh.BchNum
                ));
        }
        public static void CsvWriteEDRecords(StringBuilder sb, BatchHeaderRecord bh, EntryDetailRecord ed)
        {
            var eDetails = ed;
            switch (bh.SECCode)
            {
                case StandardEntryClassCode.WEB:
                    sb.AppendLine(WEBTELeDetailHeader);
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
                    break;
                case StandardEntryClassCode.CCD:
                    sb.AppendLine(CCDeDtailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.IndivIdNum,
                    eDetails.ReceiverCoName,
                    eDetails.DiscretionaryData,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.PPD:
                    sb.AppendLine(PPDeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.IndivIdNum,
                    eDetails.ReceiverCoName,
                    eDetails.DiscretionaryData,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.TEL:
                    sb.AppendLine(TELDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.IndivIdNum,
                    eDetails.ReceiverCoName,
                    eDetails.PaymtTypeCode,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.COR:
                    sb.AppendLine(NOCeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                       (int)eDetails.RecType,
                       eDetails.TransCode,
                       eDetails.RDFIId,
                       eDetails.CheckDigit,
                       eDetails.DFIAcctNum,
                       eDetails.Amt,
                       eDetails.IndivIdNum,
                       eDetails.ReceiverCoName,
                       eDetails.DiscretionaryData,
                       eDetails.aDRecIndicator,
                       eDetails.TraceNum
                       ));
                    break;
                case StandardEntryClassCode.POS:
                    sb.AppendLine(POSDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.IndivIdNum,
                    eDetails.IndivName,
                    eDetails.CardTransTypeCode,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.POP:
                    sb.AppendLine(POPDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.CheckSerialNum,
                    eDetails.TerminalCity,
                    eDetails.TerminalState,
                    eDetails.IndivName,
                    eDetails.DiscretionaryData,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.CTX:
                    sb.AppendLine(CTXeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    (int)eDetails.RecType,
                    eDetails.TransCode,
                    eDetails.RDFIId,
                    eDetails.CheckDigit,
                    eDetails.DFIAcctNum,
                    eDetails.Amt,
                    eDetails.NumOfAddendaRecords,
                    eDetails.ReceiverCoName,
                    eDetails.Reserved,
                    eDetails.DiscretionaryData,
                    eDetails.aDRecIndicator,
                    eDetails.TraceNum
                    ));
                    break;
                default:
                    throw new System.NotImplementedException($"Standard Entry Class Code '{bh.SECCode}' is not supported");
            }
        }
        public static void CsvADRecords(StringBuilder sb, Addenda ad)
        {
            if (ad != null)
            {
                var aDetails = ad;
                switch (ad.AdTypeCode)
                {
                    case AddendaTypeCode.StandardAddenda:
                        sb.AppendLine(AddendaHeader);
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                        (int)aDetails.RecType,
                        (int)aDetails.AdTypeCode,
                        aDetails.PaymtRelatedInfo,
                        aDetails.AddendaSeqNum,
                        aDetails.EntDetailSeqNum
                        ));
                        break;
                    case AddendaTypeCode.ReturnAddenda:
                        if (aDetails.ReturnReasonCode != ReturnCode.Unknown && aDetails.DisHonorReturnReasonCode == ReturnCode.Unknown && aDetails.ContestedDisHonorReturnReasonCode == ReturnCode.Unknown)
                        {
                            sb.AppendLine(ReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                (int)aDetails.RecType,
                                (int)aDetails.AdTypeCode,
                                aDetails.ReturnReasonCode,
                                aDetails.OrigTraceNum,
                                aDetails.DateOfDeath,
                                aDetails.OrigReceivingDFIId,
                                aDetails.AddendaInfo,
                                aDetails.AdTraceNum
                                ));
                        }
                        else if (aDetails.DisHonorReturnReasonCode != ReturnCode.Unknown && aDetails.ContestedDisHonorReturnReasonCode == ReturnCode.Unknown && aDetails.ReturnReasonCode == ReturnCode.Unknown)
                        {
                            sb.AppendLine(DishonorReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                           (int)aDetails.RecType,
                            aDetails.AdTypeCode,
                            aDetails.DisHonorReturnReasonCode,
                            aDetails.OrigTraceNum,
                            aDetails.Reserved1,
                            aDetails.OrigReceivingDFIId,
                            aDetails.Reserved2,
                            aDetails.ReturnTraceNum,
                            aDetails.ReturnSettlementDate,
                            aDetails.DReturnReasonCode,
                            aDetails.AddendaInfo,
                            aDetails.AdTraceNum
                            ));
                        }
                        else if (aDetails.ContestedDisHonorReturnReasonCode != ReturnCode.Unknown && aDetails.DisHonorReturnReasonCode == ReturnCode.Unknown && aDetails.ReturnReasonCode == ReturnCode.Unknown)
                        {
                            sb.AppendLine(ContestedDishonorReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                            (int)aDetails.RecType,
                            (int)aDetails.AdTypeCode,
                            aDetails.ContestedDisHonorReturnReasonCode,
                            aDetails.OrigTraceNum,
                            aDetails.DateOriginalEntryReturned,
                            aDetails.OrigReceivingDFIId,
                            aDetails.OriginalSettlementDate,
                            aDetails.ReturnTraceNum,
                            aDetails.ReturnSettlementDate,
                            aDetails.DReturnReasonCode,
                            aDetails.DisHonrorReturnTraceNum,
                            aDetails.ReturnSettlementDate,
                            aDetails.CReturnReasonCode,
                            aDetails.Reserved1,
                            aDetails.AdTraceNum
                            ));
                        }
                        break;
                    case AddendaTypeCode.NOCAddenda:
                        sb.AppendLine(NOCAddendaHeader);
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                            (int)aDetails.RecType,
                            (int)aDetails.AdTypeCode,
                            aDetails.ChangeCode,
                            aDetails.OrigTraceNum,
                            aDetails.Reserved1,
                            aDetails.OrigReceivingDFIId,
                            aDetails.CorrectedData,
                            aDetails.Reserved2,
                            aDetails.AdTraceNum
                            ));
                        break;
                    default:
                        throw new System.NotImplementedException($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                }
            }
        }
        public static void CsvBCRecords(StringBuilder sb, BatchControlRecord bC)
        {
            sb.AppendLine(BatchControlHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                (int)bC.RecType,
                (int)bC.ServiceClassCode,
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
            sb.AppendLine(FileControlHeader);
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