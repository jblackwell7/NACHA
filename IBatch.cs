namespace NACHAParser
{
    public interface IBatch
    {
        #region Methods

        void ProcessBatchHeader(string line, int lineNumber, StandardEntryClassCode sec);
        void ProcessEntryDetail(string line, string nextLine, int lineNumber);
        void ProcessAddenda(string line);
        void ProcessBatchControl(string line, Root root);

        #endregion
    }
}