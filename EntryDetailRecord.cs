using Newtonsoft.Json;

namespace NACHAParser
{
    public class EntryDetailRecord
    {
        #region Properties

        [JsonProperty("entDetailsId")]
        public string EntDetailsId { get; set; } = string.Empty;

        [JsonProperty("lineNum")]
        public int LineNum { get; set; }

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("transCode")]
        public TransactionCode TransCode { get; set; }

        [JsonProperty("rDFIId")]
        public string RDFIId { get; set; } = string.Empty;

        [JsonProperty("checkDigit")]
        public char CheckDigit { get; set; }

        [JsonProperty("dFIAcctNum")]
        public string DFIAcctNum { get; set; }= string.Empty;

        [JsonProperty("amt")]
        public string Amt { get; set; } = string.Empty;

        [JsonProperty("indivIdNum")]
        public string IndivIdNum { get; set; } = string.Empty;

        [JsonProperty("indivName")]
        public string IndivName { get; set; } = string.Empty;

        [JsonProperty("checkSerialNum")]
        public string CheckSerialNum { get; set; } = string.Empty;

        [JsonProperty("terminalCity")]
        public string TerminalCity { get; set; } = string.Empty;

        [JsonProperty("terminalState")]
        public string TerminalState { get; set; } = string.Empty;

        [JsonProperty("cardTransTypeCode")]
        public string CardTransTypeCode { get; set; } = string.Empty;

        [JsonProperty("discretionaryData")]
        public string? DiscretionaryData { get; set; }

        [JsonProperty("paymtTypeCode")]
        public string? PaymtTypeCode { get; set; }

        [JsonProperty("addendaRecordIndicator")]
        public AddendaRecordIndicator aDRecIndicator { get; set; }

        [JsonProperty("traceNum")]
        public string TraceNum { get; set; } = string.Empty;

        [JsonProperty("addendaRecords")]
        public List<Addenda> AddendaRecord { get; set; } = new List<Addenda>();

        #endregion

        #region Constructors

        public EntryDetailRecord()
        {
            EntDetailsId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods
        // // public static EntryDetailRecord ParseEntryDetail(string line, BatchHeaderRecord batchHeader, int lineNumber)
        // // {

        // //     EntryDetailRecord entry = new EntryDetailRecord
        // //     {
        // //         RecType = (RecordType)int.Parse(line.Substring(0, 1)),
        // //         TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
        // //         RDFIId = line.Substring(3, 8),
        // //         CheckDigit = line[11],
        // //         DFIAcctNum = line.Substring(12, 17),
        // //         Amt = line.Substring(29, 10),
        // //         aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
        // //         TraceNum = line.Substring(79, 15)
        // //     };
        // //     ProcessEntryDetailSEC(batchHeader.SECCode, entry, line);
        // //     return entry;
        // // }
        // private static void ProcessEntryDetailSEC(StandardEntryClassCode sec, EntryDetailRecord details, string line)
        // {
        //     switch (sec)
        //     {
        //         case StandardEntryClassCode.WEB:
        //             details.IndivIdNum = line.Substring(39, 15);
        //             details.IndivName = line.Substring(54, 22);
        //             details.PaymtTypeCode = line.Substring(76, 2);
        //             break;
        //         case StandardEntryClassCode.PPD:
        //             details.IndivIdNum = line.Substring(39, 15);
        //             details.IndivName = line.Substring(54, 22);
        //             details.DiscretionaryData = line.Substring(76, 2);
        //             break;
        //         case StandardEntryClassCode.POS:
        //             details.CheckSerialNum = line.Substring(39, 15);
        //             details.TerminalCity = line.Substring(48, 4);
        //             details.TerminalState = line.Substring(52, 2);
        //             details.CardTransTypeCode = line.Substring(76, 2);
        //             break;
        //         case StandardEntryClassCode.POP:
        //             details.CheckSerialNum = line.Substring(39, 9);
        //             details.TerminalCity = line.Substring(48, 4);
        //             details.TerminalState = line.Substring(52, 2);
        //             details.DiscretionaryData = line.Substring(76, 2);
        //             break;
        //         default:
        //             throw new InvalidOperationException($"Standard Entry Class Code '{sec}' is not supported");
        //     }
        // }
        public bool IsTransCodeReturnOrNOC()
        {
            switch (TransCode)
            {
                case TransactionCode.CheckingReturnNOCCredit:
                case TransactionCode.CheckingReturnNOCDebit:
                case TransactionCode.SavingReturnNOCCredit:
                case TransactionCode.SavingsReturnNOCDebit:
                case TransactionCode.GLReturnNoCCredit:
                case TransactionCode.GLReturnNOCDebit:
                case TransactionCode.LoanReturnNOCredit:
                case TransactionCode.LoanReturnNOCDebit:
                    return true;
                default:
                    return false;
            }
        }
        public static int CountEntryDetailRecords(Batch batch)
        {
            int eDcount = batch.EntryRecord.Count;
            int aDcount = 0;

            foreach (var et in batch.EntryRecord)
            {
                aDcount += et.AddendaRecord.Count;
            }
            int eDaDTotal = eDcount + aDcount;
            return eDaDTotal;
        }

        #endregion
    }
}