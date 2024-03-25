using System.Text;


namespace NACHAParser
{
    public static class CSVFileWriter
    {
        #region Constants


        public const string FileHeader = "RecordType,PriorityCode,ImmediateDestination,ImmediateOrigin,FileCreationDate,FileCreationTime,FileIdModifier,RecordSize,BlockingFactor,FormatCode,ImmediateDestinationName,ImmediateOriginName,ReferenceCode";
        public const string BatchHeader = "Record Type,Service Class Code,Company Name,Company Discretionary Data,Company Identification,Standard Entry Class Code,Company Entry Description,Company Descriptive Date,Effective Date,Settlement Date (Julian),Originator Status Code,Originating DFI Identification,Batch Number";

        #region EntryDetail Headers

        public const string ACKeDetailsHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Original Entry Trace Number,Receiving Trace Number,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string ARCeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Total Amount, Original Entry Trace Number, Receiving Company Name, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string ATXeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Total Amount, Original Entry Trace Number, Number of Addenda Records, Receiving Company Name, Reserved, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string BOCeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Check Serial Number, Individual Name, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string CCDeDtailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,ID Number,Receiving Company Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string CIEeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Individual Name, Individual Identification Number, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string COReDetailsHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string CTXeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Number of Addenda Records,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string CTXNOCeDetailsHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Number of Addenda Records,Receiving Company Name or ID Number,Reserved,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string DNEeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Individual Identification Number, Individual Name, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string ENReDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Individual Identification Number, Number of Addenda Records, Reserved, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string MTEeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual Name, Individual ID Number, Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string POPeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Check Serial Number,Terminal City, Terminal State, Individual Name/Receiving Company Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string POSeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Card Transaction Type,Terminal City, Terminal State, Card Transaction Type,Addenda Record Indicator,Trace Number";
        public const string PPDeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Payment Type Code,Addenda Record Indicator,Trace Number";
        public const string RCKeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Check Serial Number, Individual Name, Discretionary Data, Addenda Record Indicator, Trace Number";
        public const string SHReDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Card Expiration Date, Document Reference Number, Individual Card Account Number, Card Transaction Type Code, Addenda Record Indicator, Trace Number";
        public const string TELeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Payment Type Code,Addenda Record Indicator,Trace Number";
        public const string TRCeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Check Serial Number, Process Control Field, Item Research Number, Item Type Indicator, Addenda Record Indicator, Trace Number";
        public const string TRXeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Total Amount, Identification Number, Number of Addenda Records, Receiving Company, Reserved, Item Type Indicator, Addenda Record Indicator, Trace Number";
        public const string WEBTELeDetailHeader = "Record Type,Transaction Code,Receiving DFI Identification,Check Digit,DFI Account Number,Amount,Individual ID Number,Individual Name,Discretionary Data,Addenda Record Indicator,Trace Number";
        public const string XCKeDetailsHeader = "Record Type, Transaction Code, Receiving DFI Identification, Check Digit, DFI Account Number, Amount, Check Serial Number, Process Control Field, Item Research Number, Item Type Indicator, Addenda Record Indicator, Trace Number";

        #endregion

        #region AddendaHeaders

        public const string AddendaHeader = "Record Type,Payment Related Information,Addenda Sequence Number,Entry Detail Sequence Number";
        public const string CORAddendaHeader = "Record Type,Addenda Type Code,Change Code,Original Entry Trace Number,Reserved,Original Receiving DFI Identification,Corrected Data,Reserved,Trace Number";
        public const string POSAddendaHeader = "Record Type, Addenda Type Code, Reference Information #1, Reference Information #2, Terminal Identification Code, Transaction Serial Number, Transaction Date, Authorization Code or Card Expiration Date, Terminal Location, Terminal City, Terminal State, Trace Number";
        public const string SHRAddendaHeader = "Record Type, Addenda Type Code, Reference Information #1, Reference Information #2, Terminal Identification Code, Transaction Serial Number, Transaction Date, Authorization Code or Card Expiration Date, Terminal Location, Terminal City, Terminal State, Trace Number";
        public const string MTEAddendaHeader = "Record Type, Addenda Type Code, Transaction Description, Network Identification Code, Terminal Identification Code, Transaction Serial Number, Transaction Date, Transaction Time, Terminal Location, Terminal City, Terminal State, Trace Number";
        public const string ReturnAddendaHeader = "Record Type,Addenda Type Code,Return Reason Code,Original Entry Trace Number,Date of Death,Original Receiving DFI Identification,Addenda Information,Trace Number";
        public const string DishonorReturnAddendaHeader = "Record Type,Addenda Type Code,DisHonrorReturnCode,Original Entry Trace Number,Reserved,Original Receiving DFI Identification,Reserved,Return Trace Number,Return Settlement Date,Dis Honor Return Reason Code,Addenda Information,Trace Number";
        public const string ContestedDishonorReturnAddendaHeader = "Record Type,Addenda Type Code,Contested Dishonor Return Reason Code,Original Entry Trace Number,Date Original Entry Returned,Original Receiving DFI Identification,Original Settlement Date,Return Trace Number,Return Settlement Date,Return Reason Code,Dishonor Return Settlement Date,Dishonor Return Reason Code, Reserved,Trace Number";
        public const string RefusedCORAddendaHeader = "Record Type, Addenda Type Code, Change Code, Refused COR Code, Original Entry Trace Number, Reserved, Original Receiving DFI Identification, Corrected Data, Change Code, COR Trace Number, Reserved, Trace Number";

        #endregion

        public const string BatchControlHeader = "Record Type,Service Class Code,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount,Total Credit Entry Dollar Amount,Company Identification,Message Authentication Code,Reserved,Originating DFI Identificiation,Batch Number";
        public const string FileControlHeader = "Record Type,Batch Count,Block Count,Entry/Addenda Count,Entry Hash,Total Debit Entry Dollar Amount in File,Total Credit Entry Dollar Amount in File,Reserved";

        #endregion

        #region Methods

        public static void WriteCsvFile(Root root, string outputFile)
        {
            var sb = new StringBuilder();
            var fh = root.FileContents.ACHFile.FileHeader;
            CSVFHRecords(sb, fh);
            foreach (var batch in root.FileContents.ACHFile.Batches)
            {
                CSVWriteBHRecords(sb, batch);
                foreach (var etR in batch.EntryRecord)
                {
                    CSVWriteEDRecords(sb, batch.BatchHeader, etR);
                    foreach (var addenda in etR.AddendaRecord)
                    {
                        CSVADRecords(sb, addenda, batch.BatchHeader);
                    }
                }
                CsvBCRecords(sb, batch.BatchControl);
            }
            CsvWriteFCRecords(sb, root.FileContents.ACHFile.FileControl);
            File.WriteAllText(outputFile, sb.ToString());
        }
        public static void CSVFHRecords(StringBuilder sb, FileHeaderRecord fh)
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
        public static void CSVWriteBHRecords(StringBuilder sb, Batch b)
        {
            var bh = b.BatchHeader;
            sb.AppendLine(BatchHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                (int)bh.RecType,
                (int)bh.ServiceClassCode,
                bh.CoName,
                bh.CoDiscretionaryData,
                bh.CoId.Trim(),
                (int)bh.SECCode,
                bh.CoEntDescription,
                bh.CoDescriptiveDate,
                bh.EffectiveEntDate,
                bh.SettlementDate,
                (int)bh.OriginatorStatusCode,
                bh.OriginatingDFIId,
                bh.BchNum
                ));
        }
        public static void CSVWriteEDRecords(StringBuilder sb, BatchHeaderRecord bh, EntryDetailRecord ed)
        {
            switch (bh.SECCode)
            {
                case StandardEntryClassCode.ACK:
                    sb.AppendLine(ACKeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.OriginalTraceNum,
                    ed.ReceiverCoName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.ATX:
                    sb.AppendLine(ATXeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.TotalAmt,
                    ed.OriginalTraceNum,
                    ed.NumOfAddendaRecords,
                    ed.ReceiverCoName.Trim(),
                    ed.Reserved.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.ARC:
                    sb.AppendLine(ARCeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.BOC:
                    sb.AppendLine(BOCeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.CCD:
                    sb.AppendLine(CCDeDtailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.ReceiverCoName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.CIE:
                    sb.AppendLine(CIEeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.COR:
                    sb.AppendLine(COReDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                       (int)ed.RecType,
                       (int)ed.TransCode,
                       ed.RDFIId,
                       ed.CheckDigit,
                       ed.DFIAcctNum.Trim(),
                       ed.Amt,
                       ed.IndivIdNum.Trim(),
                       ed.ReceiverCoName.Trim(),
                       ed.DiscretionaryData.Trim(),
                       (int)ed.adRecIndicator,
                       ed.TraceNum
                       ));
                    break;
                case StandardEntryClassCode.CTX:
                    sb.AppendLine(CTXeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.NumOfAddendaRecords,
                    ed.ReceiverCoName.Trim(),
                    ed.Reserved.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.DNE:
                    sb.AppendLine(DNEeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.ENR:
                    sb.AppendLine(ENReDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.NumOfAddendaRecords,
                    ed.Reserved.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.MTE:
                    sb.AppendLine(MTEeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.POP:
                    sb.AppendLine(POPeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.TerminalCity,
                    ed.TerminalState,
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.POS:
                    sb.AppendLine(POSeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.IndivName.Trim(),
                    ed.CardTransTypeCode,
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.PPD:
                    sb.AppendLine(PPDeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.ReceiverCoName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.RCK:
                    sb.AppendLine(RCKeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.IndivName.Trim(),
                    ed.DiscretionaryData.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.SHR:
                    sb.AppendLine(SHReDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CardExpirationDate.Trim(),
                    ed.DocRefNum.Trim(),
                    ed.IndivCardAcctNum.Trim(),
                    ed.CardTransTypeCode.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.TEL:
                    sb.AppendLine(TELeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.ReceiverCoName.Trim(),
                    ed.PaymtTypeCode.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.TRC:
                    sb.AppendLine(TRCeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.ProcessControlField,
                    ed.ItemResearchNum,
                    ed.ItemTypeIndicator,
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.TRX:
                    sb.AppendLine(TRXeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.TotalAmt,
                    ed.IndivIdNum,
                    ed.NumOfAddendaRecords,
                    ed.ReceiverCoName.Trim(),
                    ed.Reserved.Trim(),
                    ed.ItemTypeIndicator,
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.WEB:
                    sb.AppendLine(WEBTELeDetailHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.IndivIdNum.Trim(),
                    ed.IndivName.Trim(),
                    ed.PaymtTypeCode.Trim(),
                    (int)ed.adRecIndicator,
                    ed.TraceNum
                    ));
                    break;
                case StandardEntryClassCode.XCK:
                    sb.AppendLine(XCKeDetailsHeader);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    (int)ed.RecType,
                    (int)ed.TransCode,
                    ed.RDFIId,
                    ed.CheckDigit,
                    ed.DFIAcctNum.Trim(),
                    ed.Amt,
                    ed.CheckSerialNum,
                    ed.ProcessControlField,
                    ed.ItemResearchNum,
                    ed.DiscretionaryData,
                    ed.ItemTypeIndicator,
                    ed.TraceNum
                    ));
                    break;
                default:
                    throw new System.NotImplementedException($"Standard Entry Class Code '{bh.SECCode}' is not supported");
            }
        }
        public static void CSVADRecords(StringBuilder sb, AddendaRecord ad, BatchHeaderRecord bh)
        {
            if (ad != null)
            {
                switch (ad.AdTypeCode)
                {
                    case AddendaTypeCode.Addenda02:
                        if (bh.SECCode == StandardEntryClassCode.POS)
                        {
                            sb.AppendLine(POSAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            (int)ad.RecType,
                            (int)ad.AdTypeCode,
                            ad.RefInfo1,
                            ad.RefInfo2,
                            ad.TerminalIDCode,
                            ad.TransSerialNum,
                            ad.TransDate,
                            ad.AuthCodeOrExpDate,
                            ad.TerminalLoc,
                            ad.TerminalCity,
                            ad.TerminalState,
                            ad.AdTraceNum
                            ));
                            break;
                        }
                        else if (bh.SECCode == StandardEntryClassCode.SHR)
                        {
                            sb.AppendLine(SHRAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            (int)ad.RecType,
                            (int)ad.AdTypeCode,
                            ad.RefInfo1,
                            ad.RefInfo2,
                            ad.TerminalIDCode,
                            ad.TransSerialNum,
                            ad.TransDate,
                            ad.AuthCodeOrExpDate,
                            ad.TerminalLoc,
                            ad.TerminalCity,
                            ad.TerminalState,
                            ad.AdTraceNum
                            ));
                            break;
                        }
                        else if (bh.SECCode == StandardEntryClassCode.MTE)
                        {
                            sb.AppendLine(MTEAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            (int)ad.RecType,
                            (int)ad.AdTypeCode,
                            ad.TransDescription,
                            ad.NetworkIdCode,
                            ad.TerminalIDCode,
                            ad.TransSerialNum,
                            ad.TransDate,
                            ad.TransTime,
                            ad.TerminalLoc,
                            ad.TerminalCity,
                            ad.TerminalState,
                            ad.AdTraceNum
                            ));
                            break;
                        }
                        else
                        {
                            throw new System.NotImplementedException($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                        }
                    case AddendaTypeCode.Addenda05:
                        sb.AppendLine(AddendaHeader);
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                        (int)ad.RecType,
                        (int)ad.AdTypeCode,
                        ad.PaymtRelatedInfo,
                        ad.AddendaSeqNum,
                        ad.EntDetailSeqNum
                        ));
                        break;
                    case AddendaTypeCode.Addenda98:
                        bool isRefusedCOR = ad.IsRefusedCORCode(ad.ChangeCode);
                        if (isRefusedCOR == false)
                        {
                            sb.AppendLine(CORAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                (int)ad.RecType,
                                (int)ad.AdTypeCode,
                                ad.ChangeCode,
                                ad.OrigTraceNum,
                                ad.Reserved1,
                                ad.OrigReceivingDFIId,
                                ad.CorrectedData,
                                ad.Reserved2,
                                ad.AdTraceNum
                                ));
                            break;
                        }
                        else if (isRefusedCOR == true)
                        {
                            sb.AppendLine(RefusedCORAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            (int)ad.RecType,
                            (int)ad.AdTypeCode,
                            ad.RefusedCORCode,
                            ad.OrigTraceNum,
                            ad.Reserved1,
                            ad.OrigReceivingDFIId,
                            ad.CorrectedData,
                            ad.ChangeCode,
                            ad.CorTraceSeqNum,
                            ad.Reserved2,
                            ad.AdTraceNum
                            ));
                            break;
                        }
                        else
                        {
                            throw new System.NotImplementedException($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                        }
                    case AddendaTypeCode.Addenda99:
                        if(Enum.IsDefined(typeof(ReturnCode), ad.ReturnReasonCode))
                        {
                            sb.AppendLine(ReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                (int)ad.RecType,
                                (int)ad.AdTypeCode,
                                ad.ReturnReasonCode,
                                ad.OrigTraceNum,
                                ad.DateOfDeath,
                                ad.OrigReceivingDFIId,
                                ad.AddendaInfo,
                                ad.AdTraceNum
                                ));
                        }
                        else if (Enum.IsDefined(typeof(ReturnCode), ad.DisHonorReturnReasonCode))
                        {
                            sb.AppendLine(DishonorReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                           (int)ad.RecType,
                            ad.AdTypeCode,
                            ad.DisHonorReturnReasonCode,
                            ad.OrigTraceNum,
                            ad.Reserved1,
                            ad.OrigReceivingDFIId,
                            ad.Reserved2,
                            ad.ReturnTraceNum,
                            ad.ReturnSettlementDate,
                            ad.DReturnReasonCode,
                            ad.AddendaInfo,
                            ad.AdTraceNum
                            ));
                        }
                        else if (Enum.IsDefined(typeof(ReturnCode), ad.ContestedDisHonorReturnReasonCode))
                        {
                            sb.AppendLine(ContestedDishonorReturnAddendaHeader);
                            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                            (int)ad.RecType,
                            (int)ad.AdTypeCode,
                            ad.ContestedDisHonorReturnReasonCode,
                            ad.OrigTraceNum,
                            ad.DateOriginalEntryReturned,
                            ad.OrigReceivingDFIId,
                            ad.OriginalSettlementDate,
                            ad.ReturnTraceNum,
                            ad.ReturnSettlementDate,
                            ad.DReturnReasonCode,
                            ad.DisHonrorReturnTraceNum,
                            ad.ReturnSettlementDate,
                            ad.CReturnReasonCode,
                            ad.Reserved1,
                            ad.AdTraceNum
                            ));
                        }
                        break;
                    default:
                        throw new System.NotImplementedException($"Addenda Type Code '{ad.AdTypeCode}' is not supported");
                }
            }
        }
        public static void CsvBCRecords(StringBuilder sb, BatchControlRecord bc)
        {
            sb.AppendLine(BatchControlHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                (int)bc.RecType,
                (int)bc.ServiceClassCode,
                bc.EntAddendaCnt,
                bc.EntHash,
                bc.TotBchDrEntAmt,
                bc.TotBchCrEntAmt,
                bc.CoId,
                bc.MsgAuthCode,
                bc.Reserved,
                bc.OriginatingDFIId,
                bc.BchNum
                ));
        }
        public static void CsvWriteFCRecords(StringBuilder sb, FileControlRecord fc)
        {
            sb.AppendLine(FileControlHeader);
            sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                (int)fc.RecType,
                fc.BchCnt,
                fc.BlockCnt,
                fc.EntAddendaCnt,
                fc.EntHash,
                fc.TotFileDrEntAmt,
                fc.TotFileCrEntAmt,
                fc.Reserved
                ));
        }
        #endregion
    }
}