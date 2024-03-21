namespace NACHAParser
{
    public class BatchFactory
    {
        #region Methods
        public IBatch CreateBatch(StandardEntryClassCode sec)
        {
            switch (sec)
            {
                case StandardEntryClassCode.PPD:
                    return new PPDBatch();
                case StandardEntryClassCode.CCD:
                    return new CCDBatch();
                case StandardEntryClassCode.TEL:
                    return new TELBatch();
                case StandardEntryClassCode.WEB:
                    return new WEBBatch();
                case StandardEntryClassCode.POP:
                    return new POPBatch();
                case StandardEntryClassCode.POS:
                    return new POSBatch();
                case StandardEntryClassCode.CTX:
                    return new CTXBatch();
                case StandardEntryClassCode.COR:
                    return new CORBatch();
                case StandardEntryClassCode.ACK:
                    return new ACKBatch();
                case StandardEntryClassCode.ATX:
                    return new ATXBatch();
                case StandardEntryClassCode.ARC:
                    return new ARCBatch();
                case StandardEntryClassCode.BOC:
                    return new BOCBatch();
                case StandardEntryClassCode.CIE:
                    return new CIEBatch();
                case StandardEntryClassCode.DNE:
                    return new DNEBatch();
                case StandardEntryClassCode.ENR:
                    return new ENRBatch();
                case StandardEntryClassCode.MTE:
                    return new MTEBatch();
                case StandardEntryClassCode.RCK:
                    return new RCKBatch();
                case StandardEntryClassCode.TRC:
                    return new TRCBatch();
                case StandardEntryClassCode.TRX:
                    return new TRXBatch();
                case StandardEntryClassCode.XCK:
                    return new XCKBatch();
                case StandardEntryClassCode.SHR:
                    return new SHRBatch();
                default:
                    throw new NotImplementedException($"Standard Entry Class Code '{sec}' is not supported");
            }
        }
        public StandardEntryClassCode ParseSEC(string value)
        {
            switch (value)
            {
                case "CCD":
                    return StandardEntryClassCode.CCD;
                case "PPD":
                    return StandardEntryClassCode.PPD;
                case "WEB":
                    return StandardEntryClassCode.WEB;
                case "TEL":
                    return StandardEntryClassCode.TEL;
                case "COR":
                    return StandardEntryClassCode.COR;
                case "POP":
                    return StandardEntryClassCode.POP;
                case "POS":
                    return StandardEntryClassCode.POS;
                case "BOC":
                    return StandardEntryClassCode.BOC;
                case "ARC":
                    return StandardEntryClassCode.ARC;
                case "RCK":
                    return StandardEntryClassCode.RCK;
                case "MTE":
                    return StandardEntryClassCode.MTE;
                case "SHR":
                    return StandardEntryClassCode.SHR;
                case "CTX":
                    return StandardEntryClassCode.CTX;
                case "IAT":
                    return StandardEntryClassCode.IAT;
                case "ENR":
                    return StandardEntryClassCode.ENR;
                case "TRC":
                    return StandardEntryClassCode.TRC;
                case "ADV":
                    return StandardEntryClassCode.ADV;
                case "XCK":
                    return StandardEntryClassCode.XCK;
                case "ACK":
                    return StandardEntryClassCode.ACK;
                default:
                    throw new ArgumentException($"'{value}' is not a valid StandardEntryClassCode.");
            };
        }

        #endregion
    }
}