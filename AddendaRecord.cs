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
        public string ReturnReasonCode { get; set; } = string.Empty;

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
        public string ChangeCode { get; set; } = string.Empty;

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
            if (adEntry.Addenda.AdTypeCode == AddendTypeCode.StandardAddenda)
            {
                adEntry.Addenda.PaymtRelatedInfo = line.Substring(3, 80);
                adEntry.Addenda.AddendaSeqNum = line.Substring(83, 4);
                adEntry.Addenda.EntDetailSeqNum = line.Substring(87, 7);
            }
            else if (adEntry.Addenda.AdTypeCode == AddendTypeCode.ReturnAddenda)
            {
                adEntry.Addenda.ReturnReasonCode = line.Substring(3, 3);
                adEntry.Addenda.OrigTraceNum = line.Substring(6, 15);
                adEntry.Addenda.DateOfDeath = line.Substring(21, 6);
                adEntry.Addenda.OrigReceivingDFIId = line.Substring(27, 8);
                adEntry.Addenda.AddendaInfo = line.Substring(35, 44);
                adEntry.Addenda.AdTraceNum = line.Substring(79, 15);
            }
            else if (adEntry.Addenda.AdTypeCode == AddendTypeCode.NOCAddenda)
            {
                adEntry.Addenda.ChangeCode = line.Substring(3, 2);
                adEntry.Addenda.OrigTraceNum = line.Substring(6, 15);
                adEntry.Addenda.Reserved1 = line.Substring(21, 6);
                adEntry.Addenda.OrigReceivingDFIId = line.Substring(27, 8);
                adEntry.Addenda.CorrectedData = line.Substring(35, 29);
                adEntry.Addenda.Reserved2 = line.Substring(64, 15);
                adEntry.Addenda.AdTraceNum = line.Substring(79, 15);
            }
            return adEntry;
        }

        #endregion
    }
}
