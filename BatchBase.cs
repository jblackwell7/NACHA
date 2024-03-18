namespace NACHAParser
{
    public abstract class BatchBase : IBatch
    {
        #region Fields
        protected BatchHeaderRecord? BatchHeader;
        protected List<EntryDetailRecord>? entryRecord;
        protected BatchControlRecord? batchControl;

        #endregion

        #region Methods

        public abstract BatchHeaderRecord ProcessBatchHeader(string line, int lineNumber, ACHFile achFile, StandardEntryClassCode sec);
        public abstract void ProcessEntryDetail(string line, string nextLine, ACHFile achFile, int lineNumber);
        public abstract void ProcessAddenda(string line, ACHFile achFile, int lineNumber);
        public abstract BatchControlRecord ProcessBatchControl(string line, Root root, ACHFile achFile);

        #endregion
    }
}