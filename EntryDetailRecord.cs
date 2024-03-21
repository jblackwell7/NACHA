using Newtonsoft.Json;

namespace NACHAParser
{
    public class EntryDetailRecord
    {
        #region Properties

        [JsonProperty("entDetailsId")]
        public string EntDetailsId { get; set; } = string.Empty;

        [JsonIgnore]
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

        [JsonProperty("dFIAcctNumber")]
        public string DFIAcctNum { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public string Amt { get; set; } = string.Empty;

        [JsonProperty("totalAmount")]
        public string TotalAmt { get; set; } = string.Empty;

        [JsonProperty("indivIdNumber")]
        public string IndivIdNum { get; set; } = string.Empty;

        [JsonProperty("numOfAddendaRecords")]
        public int NumOfAddendaRecords { get; set; }

        [JsonProperty("individualName")]
        public string IndivName { get; set; } = string.Empty;

        [JsonProperty("receiverCompanyName")]
        public string ReceiverCoName { get; set; } = string.Empty;

        [JsonProperty("checkSerialNumber")]
        public string CheckSerialNum { get; set; } = string.Empty;

        [JsonProperty("terminalCity")]
        public string TerminalCity { get; set; } = string.Empty;

        [JsonProperty("terminalState")]
        public string TerminalState { get; set; } = string.Empty;

        [JsonProperty("cardTransTypeCode")]
        public string CardTransTypeCode { get; set; } = string.Empty;

        [JsonProperty("cardExpirationDate")]
        public string CardExpirationDate { get; set; } = string.Empty;

        [JsonProperty("documentReferenceNumber")]
        public string DocRefNum { get; set; } = string.Empty;

        [JsonProperty("IndividualCardAcctNumber")]
        public string IndivCardAcctNum { get; set; } = string.Empty;

        [JsonProperty("discretionaryData")]
        public string DiscretionaryData { get; set; } = string.Empty;

        [JsonProperty("paymtTypeCode")]
        public string PaymtTypeCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("addendaRecordIndicator")]
        public AddendaRecordIndicator aDRecIndicator { get; set; }

        [JsonProperty("traceNumber")]
        public string TraceNum { get; set; } = string.Empty;

        [JsonProperty("originalTraceNumber")]
        public string OriginalTraceNum { get; set; } = string.Empty;

        [JsonProperty("processControlField")]
        public string ProcessControlField { get; set; } = string.Empty;

        [JsonProperty("itemResearchNumber")]
        public string ItemResearchNum { get; set; } = string.Empty;

        [JsonProperty("itemTypeIndicator")]
        public string ItemTypeIndicator { get; set; } = string.Empty;

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
        public int AddendaCount()
        {
            return AddendaRecord.Count;
        }
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

        #endregion
    }
}