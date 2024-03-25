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
        public string CheckDigit { get; set; }

        [JsonProperty("dFIAcctNumber")]
        public string DFIAcctNum { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public int Amt { get; set; }

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

        public StandardEntryClassCode SECCode { get; set; }

        public ACHFile AchFile { get; set; }

        #endregion

        #region Constructors

        public EntryDetailRecord()
        {
            EntDetailsId = Guid.NewGuid().ToString();
            Console.WriteLine($"EntDetailsId: '{EntDetailsId}'");
        }

        #endregion

        #region Methods

        public bool ValidateEntryDetail(string nextLine)
        {
            if (IsMandatoryField())
            {
                if (IsMandatoryAddenda(nextLine))
                {
                    return true;
                }
                else
                {
                    return IsAddendaRecordIndicator(nextLine, isMandatory: false);
                }
            }
            else
            {
                throw new Exception("Entry Detail Record is missing required fields.");
            }
        }
        public bool IsMandatoryField()
        {
            if (Enum.IsDefined(typeof(TransactionCode), TransCode)
            && !string.IsNullOrWhiteSpace(RDFIId)
            && !string.IsNullOrWhiteSpace(CheckDigit)
            && !string.IsNullOrWhiteSpace(DFIAcctNum)
            && !string.IsNullOrWhiteSpace(TraceNum))
            {
                return true;
            }
            else
            {
                throw new Exception("Entry Detail Record is missing required fields.");
            }
        }
        public bool IsMandatoryAddenda(string nextLine)
        {
            switch (SECCode)
            {
                case StandardEntryClassCode.COR:
                case StandardEntryClassCode.DNE:
                case StandardEntryClassCode.ENR:
                case StandardEntryClassCode.POS:
                case StandardEntryClassCode.SHR:
                case StandardEntryClassCode.MTE:
                case StandardEntryClassCode.TRX:
                    return IsAddendaRecordIndicator(nextLine, isMandatory: true);
                default:
                    return true;
            }
        }
        public bool IsAddendaRecordIndicator(string nextLine, bool isMandatory)
        {
            char recType = '7';
            if (isMandatory is true && aDRecIndicator != AddendaRecordIndicator.Addenda)
            {
                throw new Exception($"Addenda Record is mandatory for Standard Entry Class Code '{SECCode}'. Addenda Record Indicator should be '1'.");
            }
            else if (aDRecIndicator == AddendaRecordIndicator.NoAddenda && nextLine.StartsWith(recType))
            {
                throw new Exception("Addenda Record should be '1'.");
            }
            else if (aDRecIndicator == AddendaRecordIndicator.Addenda && !nextLine.StartsWith(recType))
            {
                throw new Exception("Entry Detail is missing Addenda Record");
            }
            else if (aDRecIndicator == AddendaRecordIndicator.Addenda && nextLine.StartsWith(recType))
            {
                if (isMandatory is true)
                {
                    if (AddendaCount() == 0)
                    {
                        throw new Exception($"Addenda Record is missing. Addenda Record is mandatory for Standard Entry Class Code '{SECCode}'.");
                    }
                    else if (AddendaCount() > 1)
                    {
                        throw new Exception($"Only one Addenda Record is allowed for Standard Entry Class Code '{SECCode}'.");
                    }
                }
                else if (AddendaCount() == 0)
                {
                    throw new Exception($"Addenda Record Indicator is '{aDRecIndicator}', but Addenda Record is missing.");
                }
                else if (SECCode == StandardEntryClassCode.CTX || SECCode == StandardEntryClassCode.ENR || SECCode == StandardEntryClassCode.TRX)
                {
                    if (AddendaCount() >= 9999)
                    {
                        throw new Exception($"Addenda Count exceeds the number of addenda record for Standard Entry Class Code '{SECCode}'.");
                    }
                }
                else if (AddendaCount() > 1)
                {
                    throw new Exception($"Only one Addenda Record is allowed for Standard Entry Class Code '{SECCode}'.");
                }
            }

            return true;
        }
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