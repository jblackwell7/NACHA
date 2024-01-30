using Newtonsoft.Json;

namespace NACHAParser
{
    public class EntryDetailRecord
    {
        #region Properties

        [JsonProperty("entRecId")]
        public string EntRecId { get; set; } = string.Empty;

        [JsonProperty("entryDetails")]
        public EntryDetails EntryDetails { get; set; } = new EntryDetails();

        #endregion
    }
    public class EntryDetails
    {
        #region Properties

        [JsonProperty("entDetailsId")]
        public string EntDetailsId { get; set; } = string.Empty;

        [JsonProperty("lineNum")]
        public int LineNum { get; set; }

        [JsonProperty("recType")]
        public RecordTypes RecType { get; set; }

        [JsonProperty("transCode")]
        public string TransCode { get; set; } = string.Empty;

        [JsonProperty("rDFIId")]
        public string RDFIId { get; set; } = string.Empty;

        [JsonProperty("checkDigit")]
        public char CheckDigit { get; set; }

        [JsonProperty("dFIAcctNum")]
        public string DFIAcctNum { get; set; } = string.Empty;

        [JsonProperty("amt")]
        public string Amt { get; set; } = string.Empty;

        [JsonProperty("indivIdNum")]
        public string IndivIdNum { get; set; } = string.Empty;

        [JsonProperty("indivName")]
        public string IndivName { get; set; } = string.Empty;

        [JsonProperty("discretionaryData")]
        public string? DiscretionaryData { get; set; }

        [JsonProperty("paymtTypeCode")]
        public string? PaymtTypeCode { get; set; }

        [JsonProperty("addendaRecordIndicator")]
        public AddendaRecordIndicator aDRecIndicator { get; set; }

        [JsonProperty("traceNum")]
        public string TraceNum { get; set; } = string.Empty;

        [JsonProperty("addendaRecords")]
        public List<AddendaRecord> AddendaRecords { get; set; } = new List<AddendaRecord>();

        #endregion

        #region Methods
        public static EntryDetailRecord ParseEntryDetail(string line, BatchHeaderRecord batchHeader, int lineNumber)
        {

            EntryDetailRecord entry = new EntryDetailRecord
            {
                EntryDetails = new EntryDetails
                {
                    RecType = (RecordTypes)int.Parse(line.Substring(0, 1)),
                    TransCode = line.Substring(1, 2),
                    RDFIId = line.Substring(3, 8),
                    CheckDigit = line[11],
                    DFIAcctNum = line.Substring(12, 17),
                    Amt = line.Substring(29, 10),
                    IndivIdNum = line.Substring(39, 15),
                    IndivName = line.Substring(54, 22),
                    aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
                    TraceNum = line.Substring(79, 15)
                }
            };

            if (batchHeader.StandardEntryClass == StandardEntryClassCode.WEB || batchHeader.StandardEntryClass == StandardEntryClassCode.TEL)
            {
                entry.EntryDetails.PaymtTypeCode = line.Substring(76, 2);
                entry.EntryDetails.DiscretionaryData = null;
            }
            else if (batchHeader.StandardEntryClass == StandardEntryClassCode.CCD || batchHeader.StandardEntryClass == StandardEntryClassCode.PPD || batchHeader.StandardEntryClass == StandardEntryClassCode.COR)
            {
                entry.EntryDetails.DiscretionaryData = line.Substring(76, 2);
                entry.EntryDetails.PaymtTypeCode = null;
            }
            else
            {
                throw new InvalidOperationException($"Standard Entry Class {batchHeader.StandardEntryClass} is not supported.");
            }
            return entry;
        }

        public static int CountEntryDetailRecords(Batch batch)
        {
            int eDcount = batch.EntryRecords.Count;
            int aDcount = 0;

            foreach (var et in batch.EntryRecords)
            {
                aDcount += et.EntryDetails.AddendaRecords.Count;
            }
            int eDaDTotal = eDcount + aDcount;
            return eDaDTotal;
        }

        #endregion
    }
}
