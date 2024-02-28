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
                default:
                    throw new NotImplementedException($"Standard Entry Class Code '{sec}' is not supported");
            }
        }

        #endregion
    }
}