namespace NACHAParser
{
    public class BatchFactory
    {
        #region Methods
        public static IBatch CreateBatch(StandardEntryClassCode sec)
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
                default:
                    throw new NotImplementedException($"Standard Entry Class Code '{sec}' is not supported");
            }
        }
        #endregion
    }
}