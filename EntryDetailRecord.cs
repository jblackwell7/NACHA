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

        [JsonProperty("ReceiverCompanyName")]
        public string ReceiverCoName { get; set; } = string.Empty;

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
            Console.WriteLine($"EntDetailsId: '{EntDetailsId}'");
        }

        #endregion

        #region Methods
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