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
        public ACHFile? AchFile { get; set; }

        #endregion

        #region Constructors
        public FileContents()
        {
            FileId = Guid.NewGuid().ToString();
            Console.WriteLine($"FileId: '{FileId}'");
        }
        #endregion
    }
    public class ACHFile
    {
        #region Fields

        private Batch? _currentBatch;
        private List<Batch>? _batches = new List<Batch>();

        #endregion

        #region Properties

        [JsonProperty("achFileId")]
        public string AchFileId { get; set; } = string.Empty;

        [JsonProperty("fileHeader")]
        public FileHeaderRecord? FileHeader { get; set; }

        [JsonProperty("batches")]
        public IReadOnlyList<Batch>? Batches
        {
            get
            {
                return _batches;
            }
        }

        [JsonProperty("fileControl")]
        public FileControlRecord? FileControl { get; set; }

        public Batch? CurrentBatch
        {
            get
            {
                return _currentBatch;
            }
            private set
            {
                _currentBatch = value;
            }
        }

        #endregion

        #region Constructors

        public ACHFile()
        {
            AchFileId = Guid.NewGuid().ToString();
            Console.WriteLine($"AchFileId: '{AchFileId}'");
        }

        #endregion

        #region Methods

        public void AddBatch(Batch batch)
        {
            _batches.Add(batch);
        }
        public void SetCurrentBatch(Batch batch)
        {
            if (_batches.Contains(batch))
            {
                CurrentBatch = batch;
            }
            else
            {
                throw new Exception("Batch not found in list of batches");
            }
        }
        public Batch GetCurrentBatch()
        {
            if (CurrentBatch == null)
            {
                throw new Exception("CurrentBatch is null");
            }
            else
            {
                return CurrentBatch;
            }
        }

        #endregion
    }
}