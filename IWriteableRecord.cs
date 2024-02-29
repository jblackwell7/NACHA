

namespace NACHAParser
{
    /// <summary>
    /// Represents a record that can be written to a 'StreamWriter'
    /// </summary>
    public interface IWriteableRecord
    {
        /// <summary>
        /// Writes the record to the 'StreamWriter'
        /// </summary>
        /// <param name="writer">The 'StreamWriter' object used to output the records</param>
        void WriteRecord(StreamWriter writer);
    }
    public interface IDataWriter
    {
        void WriteData(Root achData, string outputFile, string outputJsonFIle);
    }
}