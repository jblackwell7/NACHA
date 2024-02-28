using Newtonsoft.Json;
namespace NACHAParser
{
    public class Root
    {
        #region Properties

        [JsonProperty("fileContents")]
        public FileContents? FileContents { get; set; }

        #endregion
    }
    public class FileContents
    {
        #region Properties

        [JsonProperty("fileId")]
        public string FileId { get; set; } = string.Empty;

        [JsonProperty("achFile")]
        public AchFile AchFile { get; set; }

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
        public FileHeaderRecord? FileHeader { get; set; }

        [JsonProperty("batches")]
        public List<Batch>? Batches { get; set; }

        [JsonProperty("fTrailer")]
        public FileControlRecord? FileControl { get; set; }

        #endregion
    }

}