using Newtonsoft.Json;

namespace NACHAParser
{
    public class Root
    {
        #region Properties

        [JsonProperty("fileContents")]
        public FileContents FileContents { get; set; } = new FileContents();

        #endregion
    }
    public class FileContents
    {
        #region Properties

        [JsonProperty("fileId")]
        public string FileId { get; set; } = string.Empty;

        [JsonProperty("achFile")]
        public AchFile AchFile { get; set; } = new AchFile();

        #endregion

        #region Constructors
        public FileContents()
        {
            FileId = Guid.NewGuid().ToString();
            AchFile = new AchFile();
        }

        #endregion
    }
    public class AchFile
    {
        #region Properties

        [JsonProperty("fHeader")]
        public FileHeaderRecord FHeader { get; set; } = new FileHeaderRecord();

        [JsonProperty("batches")]
        public List<Batch> Batches { get; set; } = new List<Batch>();

        [JsonProperty("fTrailer")]
        public FileTrailer FTrailer { get; set; } = new FileTrailer();

        #endregion
    }
    public class Batch
    {
        #region Properties

        [JsonProperty("bchId")]
        public string BchId { get; set; } = string.Empty;

        [JsonProperty("BatchHeader")]
        public BatchHeaderRecord BatchHeader { get; set; } = new BatchHeaderRecord();

        [JsonProperty("entryDetailRecords")]
        public List<EntryDetailRecord> EntryRecords { get; set; } = new List<EntryDetailRecord>();

        [JsonProperty("batchTrailer")]
        public BatchTrailer BatchTrailer { get; set; } = new BatchTrailer();

        [JsonProperty("iatBatchHeader")]
        public IatBatchHeader IatBatchHeader { get; set; } = new IatBatchHeader();

        [JsonProperty("iatntryDetailRecords")]
        public List<IatntryDetailRecord> IatntryDetailRecords { get; set; } = new List<IatntryDetailRecord>();

        [JsonProperty("iatBatchTrailer")]
        public IatBatchTrailer IatBatchTrailer { get; set; } = new IatBatchTrailer();

        #endregion

        #region Constructors
        public Batch()
        {
            BchId = Guid.NewGuid().ToString();
        }

        #endregion
    }
}