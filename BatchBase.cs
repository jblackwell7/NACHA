namespace NACHAParser
{
    public abstract class BatchBase : IBatch
    {
        #region Fields
        protected BatchHeaderRecord? BatchHeader;
        protected List<EntryDetailRecord>? entryRecord;
        protected BatchControlRecord? batchControl;
        protected Batch? currentBatch;

        #endregion

        #region Methods

        public void SetBatch(Batch batch)
        {
            currentBatch = batch;
        }
        public abstract BatchHeaderRecord ProcessBatchHeader(string line, int lineNumber, StandardEntryClassCode sec);
        public abstract void ProcessEntryDetail(string line, string nextLine, int lineNumber);
        public abstract void ProcessAddenda(string line, int lineNumber);
        public abstract void ProcessBatchControl(string line, Root root);

        #endregion
    }
}