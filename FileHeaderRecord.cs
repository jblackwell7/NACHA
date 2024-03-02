using Newtonsoft.Json;

namespace NACHAParser
{

    public class FileHeaderRecord
    {
        #region Properties

        [JsonProperty("filehdrId")]
        public string FilehdrId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("priorityCode")]
        public string PriorityCode { get; set; } = string.Empty;

        [JsonProperty("immedDestination")]
        public string ImmedDestination { get; set; } = string.Empty;

        [JsonProperty("immedOrigin")]
        public string ImmedOrigin { get; set; } = string.Empty;

        [JsonProperty("fileCreationDate")]
        public string FileCreationDate { get; set; } = string.Empty;

        [JsonProperty("fileCreationTime")]
        public string FileCreationTime { get; set; } = string.Empty;

        [JsonProperty("fileIDModifier")]
        public char FileIDModifier { get; set; }

        [JsonProperty("recSize")]
        public string RecSize { get; set; } = string.Empty;

        [JsonProperty("blockingFactor")]
        public string BlockingFactor { get; set; } = string.Empty;

        [JsonProperty("formatCode")]
        public char FormatCode { get; set; }

        [JsonProperty("immedDestinationName")]
        public string ImmedDestinationName { get; set; } = string.Empty;

        [JsonProperty("immedOriginName")]
        public string ImmedOriginName { get; set; } = string.Empty;

        [JsonProperty("refCode")]
        public string RefCode { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public FileHeaderRecord()
        {
            FilehdrId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method parses a line of text representing the file header record of a ACH file and creates a new FileHeaderRecord object with the parsed data.
        /// </summary>
        /// <param name="line">A string representing a single line of text containing the file header record data of an ACH file</param>
        /// <returns>A new FileHeaderRecord object with the parsed data from the input line</returns>
        public static FileHeaderRecord ParseFileHeader(string line)
        {
            return new FileHeaderRecord
            {
                RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                PriorityCode = line.Substring(1, 2),
                ImmedDestination = line.Substring(3, 10).Trim(),
                ImmedOrigin = line.Substring(13, 10).Trim(),
                FileCreationDate = line.Substring(23, 6),
                FileCreationTime = line.Substring(29, 4),
                FileIDModifier = line[33],
                RecSize = line.Substring(34, 3),
                BlockingFactor = line.Substring(37, 2),
                FormatCode = line[39],
                ImmedDestinationName = line.Substring(40, 23).Trim(),
                ImmedOriginName = line.Substring(63, 23).Trim(),
                RefCode = line.Substring(86, 8)
            };
        }
        #endregion
    }
}