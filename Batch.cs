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

        #endregion

        #region Constructors
        public Batch()
        {
            BchId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods

                public bool ValidateBatch()
        {
            bool sc = IsServiceClassCode(this.BatchHeader.ServiceClassCode);
            StandardEntryClassCode sec = ParseSEC(this.BatchHeader.SECCode.ToString());
            try
            {
                if (sc == true && sec != StandardEntryClassCode.CCD || sec != StandardEntryClassCode.PPD || sec != StandardEntryClassCode.WEB || sec != StandardEntryClassCode.TEL || sec != StandardEntryClassCode.COR || sec != StandardEntryClassCode.POP || sec != StandardEntryClassCode.POS)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Parses the Standard Entry Class (SEC) code from a string and returns the corresponding <see cref="StandardEntryClassCode"/> enum value.
        /// </summary>
        /// <remarks>
        /// This method maps the provided string value to a specific  enum. If the provided value does not match any known SEC codes, an <see cref="ArgumentException"/> is thrown.
        /// </remarks>
        /// <param name="value">The SEC code as a string. Expected to be a three-character code corresponding to valid ACH transaction types.</param>
        /// <returns>The <see cref="StandardEntryClassCode"/> enum value that matches the provided SEC code.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided SEC code does not match any known Standard Entry Class Codes.</exception>
        public static StandardEntryClassCode ParseSEC(string value)
        {
            switch (value)
            {
                case "CCD":
                    return StandardEntryClassCode.CCD;
                case "PPD":
                    return StandardEntryClassCode.PPD;
                case "WEB":
                    return StandardEntryClassCode.WEB;
                case "TEL":
                    return StandardEntryClassCode.TEL;
                case "COR":
                    return StandardEntryClassCode.COR;
                case "POP":
                    return StandardEntryClassCode.POP;
                case "POS":
                    return StandardEntryClassCode.POS;
                case "BOC":
                    return StandardEntryClassCode.BOC;
                case "ARC":
                    return StandardEntryClassCode.ARC;
                case "RCK":
                    return StandardEntryClassCode.RCK;
                case "MTE":
                    return StandardEntryClassCode.MTE;
                case "SHR":
                    return StandardEntryClassCode.SHR;
                case "CTX":
                    return StandardEntryClassCode.CTX;
                case "IAT":
                    return StandardEntryClassCode.IAT;
                case "ENR":
                    return StandardEntryClassCode.ENR;
                case "TRC":
                    return StandardEntryClassCode.TRC;
                case "ADV":
                    return StandardEntryClassCode.ADV;
                case "XCK":
                    return StandardEntryClassCode.XCK;
                default:
                    throw new ArgumentException($"'{value}' is not a valid StandardEntryClassCode.");
            }
        }

        private static bool IsServiceClassCode(ServiceClassCode sc)
        {
            switch (sc)
            {
                case ServiceClassCode.MixDebitAndCredit:
                case ServiceClassCode.CreditOnly:
                case ServiceClassCode.DebitOnly:
                    return true;
                case ServiceClassCode.AutomatedAccountingAdvices:
                    return false;
                default:
                    throw new ArgumentException($"'{sc}' is not a valid ServiceClassCode.");
            }
        }
        #endregion 
    }
}