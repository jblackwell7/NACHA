using Newtonsoft.Json;

namespace NACHAParser
{
    public class AddendaRecord
    {
        #region Properties

        [JsonProperty("addendaRecId")]
        public string AddendaRecId { get; set; } = string.Empty;

        [JsonProperty("addenda")]
        public Addenda Addenda { get; set; } = new Addenda();

        #endregion

        #region Constructors
        public AddendaRecord()
        {
            AddendaRecId = Guid.NewGuid().ToString();
            Addenda = new Addenda();
        }

        #endregion
    }
    public class Addenda
    {
        #region Properties

        [JsonProperty("addenda05Id")]
        public string Addenda05Id { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AdTypeCode { get; set; }

        [JsonProperty("paymtRelatedInfo")]
        public string? PaymtRelatedInfo { get; set; }

        [JsonProperty("addendaSeqNum")]
        public string AddendaSeqNum { get; set; } = string.Empty;
        [JsonProperty("entDetailSeqNum")]
        public string EntDetailSeqNum { get; set; } = string.Empty;
        [JsonProperty("returnReasonCode")]
        public ReturnCode ReturnReasonCode { get; set; }

        [JsonProperty("origTraceNumber")]
        public string OrigTraceNum { get; set; } = string.Empty;

        [JsonProperty("dateOfDeath")]
        public string DateOfDeath { get; set; } = string.Empty;

        [JsonProperty("origReceivingDFIId")]
        public string OrigReceivingDFIId { get; set; } = string.Empty;

        [JsonProperty("addendaInfo")]
        public string AddendaInfo { get; set; } = string.Empty;

        [JsonProperty("adTraceNum")]
        public string AdTraceNum { get; set; } = string.Empty;
        [JsonProperty("ChangeCode")]
        public ChangeCode ChangeCode { get; set; }

        [JsonProperty("correctedData")]
        public string CorrectedData { get; set; } = string.Empty;

        [JsonProperty("reserved1")]
        public string Reserved1 { get; set; } = string.Empty;
        [JsonProperty("reserved2")]
        public string Reserved2 { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public Addenda()
        {
            Addenda05Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods

        public static AddendaRecord ParseAddenda(string line, int lineNumber)
        {
            AddendaRecord adEntry = new AddendaRecord
            {
                Addenda = new Addenda
                {
                    RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                    AdTypeCode = (AddendTypeCode)int.Parse(line.Substring(1, 2)),
                }
            };

            ProcessAddendaDetails(adEntry, line);
            return adEntry;
        }

        private static void ProcessAddendaDetails(AddendaRecord adEntry, string line)
        {
            switch (adEntry.Addenda.AdTypeCode)
            {
                case AddendTypeCode.StandardAddenda:
                    adEntry.Addenda.PaymtRelatedInfo = line.Substring(3, 80);
                    adEntry.Addenda.AddendaSeqNum = line.Substring(83, 4);
                    adEntry.Addenda.EntDetailSeqNum = line.Substring(87, 7);
                    break;
                case AddendTypeCode.ReturnAddenda:
                    adEntry.Addenda.ReturnReasonCode = (ReturnCode)int.Parse(line.Substring(3, 3));
                    adEntry.Addenda.OrigTraceNum = line.Substring(6, 15);
                    adEntry.Addenda.DateOfDeath = line.Substring(21, 6);
                    adEntry.Addenda.OrigReceivingDFIId = line.Substring(27, 8);
                    adEntry.Addenda.AddendaInfo = line.Substring(35, 44);
                    adEntry.Addenda.AdTraceNum = line.Substring(79, 15);
                    break;
                case AddendTypeCode.NOCAddenda:
                    adEntry.Addenda.ChangeCode = (ChangeCode)int.Parse(line.Substring(3, 2));
                    adEntry.Addenda.OrigTraceNum = line.Substring(6, 15);
                    adEntry.Addenda.Reserved1 = line.Substring(21, 6);
                    adEntry.Addenda.OrigReceivingDFIId = line.Substring(27, 8);
                    adEntry.Addenda.CorrectedData = line.Substring(35, 29);
                    adEntry.Addenda.Reserved2 = line.Substring(64, 15);
                    adEntry.Addenda.AdTraceNum = line.Substring(79, 15);
                    break;
                default:
                    throw new Exception("Invalid Addenda Record");
            }
        }
        public static bool IsTCReturn(TransactionCode tc)
        {
            switch (tc)
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
        public static bool IsRCDishonor(ReturnCode rc)
        {
            switch (rc)
            {
                case ReturnCode.R61:
                case ReturnCode.R62:
                case ReturnCode.R67:
                case ReturnCode.R68:
                case ReturnCode.R69:
                case ReturnCode.R70:
                case ReturnCode.R71:
                case ReturnCode.R72:
                case ReturnCode.R73:
                case ReturnCode.R75:
                case ReturnCode.R76:
                case ReturnCode.R77:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}
