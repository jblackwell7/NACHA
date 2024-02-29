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
                default:
                    throw new NotImplementedException($"Standard Entry Class Code '{sec}' is not supported");
            }
        }

        #endregion
    }
}