using Newtonsoft.Json;

namespace NACHAParser
{
    public class IatBatchHeader
    {
        #region Properties

        [JsonProperty("bchHeaderId")]
        public string BchHeaderId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public int RecType { get; set; }

        [JsonProperty("serviceClassCode")]
        public int ServiceClassCode { get; set; }

        [JsonProperty("iatIndicator")]
        public string IatIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignExchangeIndicator")]
        public string ForeignExchangeIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignRefIndicator")]
        public string ForeignRefIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignRef")]
        public int ForeignRef { get; set; }

        [JsonProperty("isoDestionationCountryCode")]
        public int IsoDestionationCountryCode { get; set; }

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("originatorId")]
        public string OriginatorId { get; set; } = string.Empty;

        [JsonProperty("standardEntryClass")]
        public string StandardEntryClass { get; set; } = string.Empty;

        [JsonProperty("coEntryDescription")]
        public string CoEntryDescription { get; set; } = string.Empty;

        [JsonProperty("isoOriginatingCurrencyCode")]
        public string IsoOriginatingCurrencyCode { get; set; } = string.Empty;

        [JsonProperty("isoDestionationCurrencyCode")]
        public string IsoDestionationCurrencyCode { get; set; } = string.Empty;

        [JsonProperty("effectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;

        [JsonProperty("SettlementDate")]
        public string SettlementDate { get; set; } = string.Empty;

        [JsonProperty("originatorStatusCode")]
        public string OriginatorStatusCode { get; set; } = string.Empty;

        [JsonProperty("goIdOriginatingDfiId")]
        public string GoIdOriginatingDfiId { get; set; } = string.Empty;

        [JsonProperty("bchNum")]
        public string BchNum { get; set; } = string.Empty;

        #endregion
    }
    public class IatEntryDetails
    {
        #region Properties

        [JsonProperty("iatEntDetailsId")]
        public string IatEntDetailsId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public int RecType { get; set; }

        [JsonProperty("transCode")]
        public string TransCode { get; set; } = string.Empty;

        [JsonProperty("goReceivingDFIId")]
        public string GoReceivingDFIId { get; set; } = string.Empty;

        [JsonProperty("checkDigit")]
        public string CheckDigit { get; set; } = string.Empty;

        [JsonProperty("addendaRecordsNum")]
        public string AddendaRecordsNum { get; set; } = string.Empty;

        [JsonProperty("reserved1")]
        public string Reserved1 { get; set; } = string.Empty;

        [JsonProperty("amt")]
        public string Amt { get; set; } = string.Empty;

        [JsonProperty("foreignDfiNum")]
        public string ForeignDfiNum { get; set; } = string.Empty;

        [JsonProperty("reserved2")]
        public string Reserved2 { get; set; } = string.Empty;

        [JsonProperty("GatewayOFACIndicator")]
        public string GatewayOFACIndicator { get; set; } = string.Empty;

        [JsonProperty("addendaRecordIndicator")]
        public string AddendaRecordIndicator { get; set; } = string.Empty;

        [JsonProperty("traceNum")]
        public string TraceNum { get; set; } = string.Empty;
        [JsonProperty("iatAddendaRecords")]
        public IatAddendaRecords IatAddendaRecords { get; set; } = new IatAddendaRecords();

        #endregion
    }
    public class IatntryDetailRecord
    {
        #region Properties

        [JsonProperty("iatEntRecId")]
        public string IatEntRecId { get; set; } = string.Empty;

        [JsonProperty("iatEntryDetails")]
        public IatEntryDetails IatEntryDetails { get; set; } = new IatEntryDetails();
    }
    public class IatAddenda10
    {
        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("transactionTypeCode")]
        public string TransactionTypeCode { get; set; } = string.Empty;

        [JsonProperty("foreignPaymentAmount")]
        public string ForeignPaymentAmount { get; set; } = string.Empty;

        [JsonProperty("foreignTraceNumber")]
        public string ForeignTraceNumber { get; set; } = string.Empty;

        [JsonProperty("receivingName")]
        public string ReceivingName { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda11
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorName")]
        public string OriginatorName { get; set; } = string.Empty;

        [JsonProperty("originatorStreetAddress")]
        public string OriginatorStreetAddress { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda12
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorCityState")]
        public string OriginatorCityState { get; set; } = string.Empty;

        [JsonProperty("originatorCountryPostalCode")]
        public string OriginatorCountryPostalCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda13
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorDfiName")]
        public string OriginatorDfiName { get; set; } = string.Empty;

        [JsonProperty("originatorIdNumQualifier")]
        public string OriginatorIdNumQualifier { get; set; } = string.Empty;

        [JsonProperty("originatorDfiId")]
        public string OriginatorDfiId { get; set; } = string.Empty;

        [JsonProperty("originatorDfiBranchCountryCode")]
        public string OriginatorDfiBranchCountryCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda14
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("ReceivingDfiName")]
        public string ReceivingDfiName { get; set; } = string.Empty;

        [JsonProperty("receiverIdNumQualifier")]
        public string ReceiverIdNumQualifier { get; set; } = string.Empty;

        [JsonProperty("receiverDfiId")]
        public string ReceiverDfiId { get; set; } = string.Empty;

        [JsonProperty("receiverDfiBranchCountryCode")]
        public string ReceiverDfiBranchCountryCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda15
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("receiverId")]
        public string ReceiverId { get; set; } = string.Empty;

        [JsonProperty("ReceivingStreetAddress")]
        public string ReceivingStreetAddress { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddenda16
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("receiverCityState")]
        public string ReceiverCityState { get; set; } = string.Empty;

        [JsonProperty("receiverCountryPostalCode")]
        public string ReceiverCountryPostalCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IatAddendaRecords
    {
        #region Properties

        [JsonProperty("iatAddendaRecId")]
        public string IatAddendaRecId { get; set; } = string.Empty;

        [JsonProperty("iatAddenda10")]
        public IatAddenda10 IatAddenda10 { get; set; } = new IatAddenda10();

        [JsonProperty("iatAddenda11")]
        public IatAddenda11 IatAddenda11 { get; set; } = new IatAddenda11();

        [JsonProperty("iatAddenda12")]
        public IatAddenda12 IatAddenda12 { get; set; } = new IatAddenda12();

        [JsonProperty("iatAddenda13")]
        public IatAddenda13 IatAddenda13 { get; set; } = new IatAddenda13();

        [JsonProperty("iatAddenda14")]
        public IatAddenda14 IatAddenda14 { get; set; } = new IatAddenda14();

        [JsonProperty("iatAddenda15")]
        public IatAddenda15 IatAddenda15 { get; set; } = new IatAddenda15();

        [JsonProperty("iatAddenda16")]
        public IatAddenda16 IatAddenda16 { get; set; } = new IatAddenda16();

        #endregion
    }
    public class IatBatchControlRecord
    {
        #region Properties

        [JsonProperty("iatBchControlId")]
        public string IatBchControlId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public int RecType { get; set; }

        [JsonProperty("serviceClass")]
        public ServiceClass ServiceClass { get; set; }

        [JsonProperty("entAddendaCnt")]
        public string EntAddendaCnt { get; set; } = string.Empty;

        [JsonProperty("entHash")]
        public string EntHash { get; set; } = string.Empty;

        [JsonProperty("totBchDrEntAmt")]
        public string TotBchDrEntAmt { get; set; } = string.Empty;

        [JsonProperty("totBchCrEntAmt")]
        public string TotBchCrEntAmt { get; set; } = string.Empty;

        [JsonProperty("coId")]
        public string CoId { get; set; } = string.Empty;

        [JsonProperty("messageAuthCode")]
        public string MessageAuthCode { get; set; } = string.Empty;

        [JsonProperty("Reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("bchNum")]
        public string BchNum { get; set; } = string.Empty;

        #endregion
    }
    public class IatBatchTrailer
    {
        #region Properties

        [JsonProperty("iatBchTrailerId")]
        public string IatBchTrailerId { get; set; } = string.Empty;

        [JsonProperty("iatBatchControlRecord")]
        public IatBatchControlRecord IatBatchControlRecord { get; set; } = new IatBatchControlRecord();

        #endregion
    }
}