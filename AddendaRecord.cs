using Newtonsoft.Json;

namespace NACHAParser
{
    public class Addenda
    {
        #region Properties

        [JsonProperty("addendaId")]
        public string AddendaId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendaTypeCode AdTypeCode { get; set; }

        [JsonProperty("returnReasonCode")]
        public ReturnCode ReturnReasonCode { get; set; }

        [JsonProperty("disHonorReturnReasonCode")]
        public ReturnCode DisHonorReturnReasonCode { get; set; }

        [JsonProperty("contestedDisHonorReturnReasonCode")]
        public ReturnCode ContestedDisHonorReturnReasonCode { get; set; }

        [JsonProperty("refusedCORCode")]
        public ChangeCode RefusedCORCode { get; set; }

        [JsonProperty("changeCode")]
        public ChangeCode ChangeCode { get; set; }

        [JsonProperty("corTraceSeqNumber")]
        public string CorTraceSeqNum { get; set; } = string.Empty;

        [JsonProperty("paymtRelatedInfo")]
        public string? PaymtRelatedInfo { get; set; }

        [JsonProperty("addendaSeqNumber")]
        public string AddendaSeqNum { get; set; } = string.Empty;

        [JsonProperty("entDetailSeqNumber")]
        public string EntDetailSeqNum { get; set; } = string.Empty;

        [JsonProperty("dishonorReturnReasonCode")]
        public string DReturnReasonCode { get; set; } = string.Empty;

        [JsonProperty("contestedReturnCode")]
        public string CReturnReasonCode { get; set; } = string.Empty;

        [JsonProperty("dateOriginalEntryReturned")]
        public string DateOriginalEntryReturned { get; set; } = string.Empty;

        [JsonProperty("originalSettlementDate")]
        public string OriginalSettlementDate { get; set; } = string.Empty;

        [JsonProperty("disHonrorReturnTraceNumber")]
        public string DisHonrorReturnTraceNum { get; set; } = string.Empty;

        [JsonProperty("disHonrorReturnSettlementDate")]
        public string DisHonrorReturnSettlementDate { get; set; } = string.Empty;

        [JsonProperty("returnTraceNumber")]
        public string ReturnTraceNum { get; set; } = string.Empty;

        [JsonProperty("returnSettlementDate")]
        public string ReturnSettlementDate { get; set; } = string.Empty;

        [JsonProperty("origTraceNumber")]
        public string OrigTraceNum { get; set; } = string.Empty;

        [JsonProperty("dateOfDeath")]
        public string DateOfDeath { get; set; } = string.Empty;

        [JsonProperty("origReceivingDFIId")]
        public string OrigReceivingDFIId { get; set; } = string.Empty;

        [JsonProperty("addendaInfo")]
        public string AddendaInfo { get; set; } = string.Empty;

        [JsonProperty("addendaTraceNum")]
        public string AdTraceNum { get; set; } = string.Empty;

        [JsonProperty("correctedData")]
        public string CorrectedData { get; set; } = string.Empty;

        [JsonProperty("reserved1")]
        public string Reserved1 { get; set; } = string.Empty;

        [JsonProperty("reserved2")]
        public string Reserved2 { get; set; } = string.Empty;

        [JsonProperty("refInfo1")]
        public string RefInfo1 { get; set; } = string.Empty;

        [JsonProperty("refInfo2")]
        public string RefInfo2 { get; set; } = string.Empty;

        [JsonProperty("terminalIDCode")]
        public string TerminalIDCode { get; set; } = string.Empty;

        [JsonProperty("transSerialNumber")]
        public string TransSerialNum { get; set; } = string.Empty;

        [JsonProperty("transactionDate")]
        public string TransDate { get; set; } = string.Empty;

        [JsonProperty("authCodeOrExpDate")]
        public string AuthCodeOrExpDate { get; set; } = string.Empty;

        [JsonProperty("terminalLocation")]
        public string TerminalLoc { get; set; } = string.Empty;

        [JsonProperty("terminalCity")]
        public string TerminalCity { get; set; } = string.Empty;

        [JsonProperty("terminalState")]
        public string TerminalState { get; set; } = string.Empty;

        [JsonProperty("transactionDescription")]
        public string TransDescription { get; set; } = string.Empty;

        [JsonProperty("networkIdCode")]
        public string NetworkIdCode { get; set; } = string.Empty;

        [JsonProperty("transactionTime")]
        public string TransTime { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public Addenda()
        {
            AddendaId = Guid.NewGuid().ToString();
            Console.WriteLine($"AddendaId: '{AddendaId}'");
        }
        #endregion
        #region Methods
        public static AddendaTypeCode ParseAddendaType(string value)
        {
            switch (value)
            {
                case "02":
                    return AddendaTypeCode.Addenda02;
                case "05":
                    return AddendaTypeCode.Addenda05;
                case "98":
                    return AddendaTypeCode.Addenda98;
                case "99":
                    return AddendaTypeCode.Addenda99;
                default:
                    throw new InvalidOperationException($"Addenda Type Code '{value}' is not supported");
            }
        }
        public bool IsContestedDishonor(EntryDetailRecord entry, ReturnCode rc)
        {
            bool isDishonorReturnCode = IsRCContestedReturnCode(rc);
            bool isDishonorTransactionCode = entry.IsTransCodeReturnOrNOC();
            if (isDishonorReturnCode && isDishonorTransactionCode == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsRCContestedReturnCode(ReturnCode rc)
        {
            switch (rc)
            {
                case ReturnCode.R71:
                case ReturnCode.R72:
                case ReturnCode.R73:
                case ReturnCode.R74:
                case ReturnCode.R75:
                case ReturnCode.R76:
                case ReturnCode.R77:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsDisHonor(EntryDetailRecord entry, ReturnCode rc)
        {
            bool isDishonorReturnCode = IsRCDishonorReturnCode(rc);
            bool isDishonorTransactionCode = entry.IsTransCodeReturnOrNOC();
            if (isDishonorReturnCode && isDishonorTransactionCode == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsRCDishonorReturnCode(ReturnCode rc)
        {
            switch (rc)
            {
                case ReturnCode.R61:
                case ReturnCode.R62:
                case ReturnCode.R67:
                case ReturnCode.R68:
                case ReturnCode.R69:
                case ReturnCode.R70:
                case ReturnCode.R77:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsRefusedCORCode(ChangeCode cc)
        {
            switch (cc)
            {
                case ChangeCode.C61:
                case ChangeCode.C62:
                case ChangeCode.C63:
                case ChangeCode.C64:
                case ChangeCode.C65:
                case ChangeCode.C66:
                case ChangeCode.C67:
                case ChangeCode.C68:
                case ChangeCode.C69:
                    return true;
                default:
                    return false;
            }
        }
        public ChangeCode ParseChangeCode(string value)
        {
            switch (value)
            {
                case "C01":
                    return ChangeCode.C01;
                case "C02":
                    return ChangeCode.C02;
                case "C03":
                    return ChangeCode.C03;
                case "C05":
                    return ChangeCode.C05;
                case "C06":
                    return ChangeCode.C06;
                case "C07":
                    return ChangeCode.C07;
                case "C08":
                    return ChangeCode.C08;
                case "C09":
                    return ChangeCode.C09;
                case "C13":
                    return ChangeCode.C13;
                case "C14":
                    return ChangeCode.C14;
                case "C61":
                    return ChangeCode.C61;
                case "C62":
                    return ChangeCode.C62;
                case "C63":
                    return ChangeCode.C63;
                case "C64":
                    return ChangeCode.C64;
                case "C65":
                    return ChangeCode.C65;
                case "C66":
                    return ChangeCode.C66;
                case "C67":
                    return ChangeCode.C67;
                case "C68":
                    return ChangeCode.C68;
                case "C69":
                    return ChangeCode.C69;
                default:
                    throw new ArgumentException($"'{value}' is not a valid ChangeCode.");
            }
        }
        public ReturnCode ParseReturnCode(string value)
        {
            switch (value)
            {
                case "R01":
                    return ReturnCode.R01;
                case "R02":
                    return ReturnCode.R02;
                case "R03":
                    return ReturnCode.R03;
                case "R04":
                    return ReturnCode.R04;
                case "R05":
                    return ReturnCode.R05;
                case "R06":
                    return ReturnCode.R06;
                case "R07":
                    return ReturnCode.R07;
                case "R08":
                    return ReturnCode.R08;
                case "R09":
                    return ReturnCode.R09;
                case "R10":
                    return ReturnCode.R10;
                case "R11":
                    return ReturnCode.R11;
                case "R12":
                    return ReturnCode.R12;
                case "R13":
                    return ReturnCode.R13;
                case "R14":
                    return ReturnCode.R14;
                case "R15":
                    return ReturnCode.R15;
                case "R16":
                    return ReturnCode.R16;
                case "R17":
                    return ReturnCode.R17;
                case "R18":
                    return ReturnCode.R18;
                case "R19":
                    return ReturnCode.R19;
                case "R20":
                    return ReturnCode.R20;
                case "R21":
                    return ReturnCode.R21;
                case "R22":
                    return ReturnCode.R22;
                case "R23":
                    return ReturnCode.R23;
                case "R24":
                    return ReturnCode.R24;
                case "R25":
                    return ReturnCode.R25;
                case "R26":
                    return ReturnCode.R26;
                case "R27":
                    return ReturnCode.R27;
                case "R28":
                    return ReturnCode.R28;
                case "R29":
                    return ReturnCode.R29;
                case "R30":
                    return ReturnCode.R30;
                case "R31":
                    return ReturnCode.R31;
                case "R32":
                    return ReturnCode.R32;
                case "R33":
                    return ReturnCode.R33;
                case "R34":
                    return ReturnCode.R34;
                case "R35":
                    return ReturnCode.R35;
                case "R36":
                    return ReturnCode.R36;
                case "R37":
                    return ReturnCode.R37;
                case "R38":
                    return ReturnCode.R38;
                case "R39":
                    return ReturnCode.R39;
                case "R40":
                    return ReturnCode.R40;
                case "R41":
                    return ReturnCode.R41;
                case "R42":
                    return ReturnCode.R42;
                case "R43":
                    return ReturnCode.R43;
                case "R44":
                    return ReturnCode.R44;
                case "R45":
                    return ReturnCode.R45;
                case "R46":
                    return ReturnCode.R46;
                case "R47":
                    return ReturnCode.R47;
                case "R50":
                    return ReturnCode.R50;
                case "R51":
                    return ReturnCode.R51;
                case "R52":
                    return ReturnCode.R52;
                case "R53":
                    return ReturnCode.R53;
                case "R61":
                    return ReturnCode.R61;
                case "R62":
                    return ReturnCode.R62;
                case "R67":
                    return ReturnCode.R67;
                case "R68":
                    return ReturnCode.R68;
                case "R69":
                    return ReturnCode.R69;
                case "R70":
                    return ReturnCode.R70;
                case "R71":
                    return ReturnCode.R71;
                case "R72":
                    return ReturnCode.R72;
                case "R73":
                    return ReturnCode.R73;
                case "R75":
                    return ReturnCode.R75;
                case "R76":
                    return ReturnCode.R76;
                case "R77":
                    return ReturnCode.R77;
                default:
                    throw new ArgumentException($"'{value}' is not a valid ReturnCode.");
            }
        }
        #endregion
    }
}