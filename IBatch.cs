namespace NACHAParser
{
    public interface IBatch
    {
        #region Methods

        BatchHeaderRecord ProcessBatchHeader(string line, int lineNumber, ACHFile achFile, StandardEntryClassCode sec);
        void ProcessEntryDetail(string line, string nextLine, int lineNumber);
        void ProcessAddenda(string line, int LineNumber);
        BatchControlRecord ProcessBatchControl(string line, Root root, ACHFile achFile);

        #endregion
    }
}