using Newtonsoft.Json;

namespace NACHAParser
{
    public class FileControlRecord
    {
        #region Properties

        [JsonProperty("fileControlId")]
        public string FileControlId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("bchCount")]
        public string BchCnt { get; set; } = string.Empty;

        [JsonProperty("blockCount")]
        public string BlockCnt { get; set; } = string.Empty;

        [JsonProperty("entryAddendaCount")]
        public string EntAddendaCnt { get; set; } = string.Empty;

        [JsonProperty("entryHash")]
        public string EntHash { get; set; } = string.Empty;

        [JsonProperty("totalFileDebitEntryDollarAmount")]
        public string TotFileDrEntAmt { get; set; } = string.Empty;

        [JsonProperty("totalFileCreditEntryDollarAmount")]
        public string TotFileCrEntAmt { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public FileControlRecord()
        {
            FileControlId = Guid.NewGuid().ToString();
            Console.WriteLine($"FileControlId: '{FileControlId}'");
        }

        #endregion

        #region Methods

        public FileControlRecord ParseFileControl(string line)
        {
            RecType = (RecordType)int.Parse(line.Substring(0, 1));
            BchCnt = line.Substring(1, 6);
            BlockCnt = line.Substring(7, 6);
            EntAddendaCnt = line.Substring(13, 8);
            EntHash = line.Substring(21, 10);
            TotFileDrEntAmt = line.Substring(31, 12);
            TotFileCrEntAmt = line.Substring(43, 12);
            Reserved = line.Substring(55, 39).Trim();
            return this;
        }

        #endregion
    }
}
