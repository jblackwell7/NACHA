using Newtonsoft.Json;

namespace NACHAParser
{
    #region Properties
    public class FileTrailer
    {
        [JsonProperty("fileTrailerId")]
        public string FileTrailerId { get; set; } = string.Empty;

        [JsonProperty("fileControl")]
        public FileControlRecord FControl { get; set; } = new FileControlRecord();

        #endregion

        #region Constructors

        public FileTrailer()
        {
            FileTrailerId = Guid.NewGuid().ToString();
            FControl = new FileControlRecord();
        }

        #endregion
    }
    public class FileControlRecord
    {
        #region Properties

        [JsonProperty("fileControlId")]
        public string FileControlId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("bchCnt")]
        public string BchCnt { get; set; } = string.Empty;

        [JsonProperty("blockCnt")]
        public string BlockCnt { get; set; } = string.Empty;

        [JsonProperty("entAddendaCnt")]
        public string EntAddendaCnt { get; set; } = string.Empty;

        [JsonProperty("entHash")]
        public string EntHash { get; set; } = string.Empty;

        [JsonProperty("totFileDrEntAmt")]
        public string TotFileDrEntAmt { get; set; } = string.Empty;

        [JsonProperty("totFileCrEntAmt")]
        public string TotFileCrEntAmt { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public FileControlRecord()
        {
            FileControlId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods

        public static FileControlRecord ParseFileControl(string line)
        {
            return new FileControlRecord
            {
                RecType = (RecordTypes)int.Parse(line.Substring(0, 1)),
                BchCnt = line.Substring(1, 6),
                BlockCnt = line.Substring(7, 6),
                EntAddendaCnt = line.Substring(13, 8),
                EntHash = line.Substring(21, 10),
                TotFileDrEntAmt = line.Substring(31, 12),
                TotFileCrEntAmt = line.Substring(43, 12),
                Reserved = line.Substring(55, 39)
            };
        }

        #endregion
    }
}
