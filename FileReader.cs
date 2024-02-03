namespace NACHAParser
{

    public class ParseDataResult
    {
        public Root? Root { get; set; }
        public int LinesProcessed { get; set; }
    }
    public class FileReader
    {
        public static ParseDataResult ParseData(string inputACHFile)
        {
            ValidateFile(inputACHFile);
            var cBatch = new Batch
            {
                EntryRecords = new List<EntryDetailRecord>()
            };
            int lineNumber = 1;
            var root = new Root
            {
                FileContents = new FileContents
                {
                    AchFile = new AchFile
                    {
                        Batches = new List<Batch>()
                    }
                }
            };
            foreach (string line in File.ReadLines(inputACHFile))
            {
                if (line.All(c => c == '9'))
                {
                    continue;
                }
                if (Enum.TryParse<RecordTypes>(line.Substring(0, 1), out RecordTypes recordType))
                {
                    lineNumber++;
                    ProcessLine(recordType, line, root, ref cBatch, lineNumber);
                }
                else
                {
                    Console.WriteLine($"Unsupported record type '{(int)recordType}' found on line {lineNumber}: {line}");
                    continue;
                }
            }
            return new ParseDataResult
            {
                Root = root,
                LinesProcessed = lineNumber - 1
            };
        }

        /// <summary>
        /// Validates the input file.
        /// </summary>
        /// <param name="inputACHFile">Path to the ACH file intended for parsing.</param>
        /// <exception cref="Exception"></exception>
        private static void ValidateFile(string inputACHFile)
        {
            if (!File.Exists(inputACHFile))
            {
                if (Path.HasExtension(inputACHFile))
                {
                    throw new Exception($"File '{inputACHFile}' does not have a valid file extension.");
                }
                else
                {
                    Console.WriteLine($"File has extension'{0}' returns '{1}'.", inputACHFile, Path.HasExtension(inputACHFile));
                }
                throw new Exception($"File '{inputACHFile}' does not exist.");
            }
            else if (new FileInfo(inputACHFile).Length == 0)
            {
                throw new Exception($"File '{inputACHFile}' is empty.");
            }
            else if (new FileInfo(inputACHFile).Length > 1000000000)
            {
                throw new Exception($"File '{inputACHFile}' is too large.");
            }
        }

        /// <summary>
        /// Processes a each line of the input ACH file.
        /// </summary>
        /// <param name="recordType">Enumerated type of the record to be process.</param>
        /// <param name="line">The current line from the ACH file being processed.</param>
        /// <param name="root">The root object being populated with parsed data.</param>
        /// <param name="cBatch">The current batch object being populated.</param>
        /// <param name="lineNumber">The line number currently being processed.</param>
        private static void ProcessLine(RecordTypes recordType, string line, Root root, ref Batch cBatch, int lineNumber)
        {
            try
            {
                switch (recordType)
                {
                    case RecordTypes.fh:
                        ProcessFileHeader(line, root);
                        break;
                    case RecordTypes.bh:
                        ProcessBatchHeader(line, ref cBatch);
                        break;
                    case RecordTypes.ed:
                        ProcessEntryDetail(line, cBatch, lineNumber);
                        break;
                    case RecordTypes.ad:
                        ProcessAddenda(line, cBatch, lineNumber);
                        break;
                    case RecordTypes.bc:
                        ProcessBatchControl(line, root, ref cBatch);
                        break;
                    case RecordTypes.fc:
                        ProcessFileControl(line, root);
                        break;
                    default:
                        throw new Exception($"Record type '{recordType}' is not supported. Line Number: '{lineNumber}' Found on line: '{line}'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing record type '{recordType}' on line '{lineNumber}'. Line length '{line.Length}'. Line: '{line}'. {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Parses the file header line of the ACH file
        /// </summary>
        /// <param name="line">The file header line from the ACH file.</param>
        /// <param name="root">The root object where the parsed file header information is stored.</param>
        private static void ProcessFileHeader(string line, Root root)
        {
            root.FileContents.AchFile.FHeader = FileHeaderRecord.ParseFileHeader(line);
        }
        /// <summary>
        /// Parses the batch header line
        /// </summary>
        /// <param name="line">The batch header line from the ACH file.</param>
        /// <param name="cBatch">Reference to the current batch object being populated.</param>
        private static void ProcessBatchHeader(string line, ref Batch cBatch)
        {
            cBatch = new Batch
            {
                BatchHeader = BatchHeaderRecord.ParseBatchHeader(line)
            };
        }
        /// <summary>
        /// Parses the entry detail line
        /// </summary>
        /// <param name="line">The entry detail line from the ACH file.</param>
        /// <param name="cBatch">The current batch object to which the entry detail record is added</param>
        /// <param name="lineNumber">The line number currently being processed.</param>
        private static void ProcessEntryDetail(string line, Batch cBatch, int lineNumber)
        {
            var entryDetail = EntryDetails.ParseEntryDetail(line, cBatch.BatchHeader, lineNumber);

            cBatch.EntryRecords.Add(entryDetail);
        }
        /// <summary>
        /// Parses the addenda line
        /// </summary>
        /// <param name="line">The addenda record line from the ACH file.</param>
        /// <param name="cBatch">The current batch containing the entry detail record to which the addenda is added.</param>
        /// <param name="lineNumber">The line number currently being processed.</param>
        private static void ProcessAddenda(string line, Batch cBatch, int lineNumber)
        {
            var addendaRecord = Addenda.ParseAddenda(line, lineNumber);

            var lastEntryDetail = cBatch.EntryRecords.Last();

            if (lastEntryDetail.EntryDetails.aDRecIndicator == AddendaRecordIndicator.Addenda)
            {
                lastEntryDetail.EntryDetails.AddendaRecords.Add(addendaRecord);
            }
        }
        /// <summary>
        /// Parses the batch control line.
        /// </summary>
        /// <param name="line">The batch control line from the ACH file.</param>
        /// <param name="root">The root object to which the completed batch is added.</param>
        /// <param name="cBatch">TReference to the current batch, which will be reset after addition to the root object.</param>
        private static void ProcessBatchControl(string line, Root root, ref Batch cBatch)
        {
            cBatch.BatchTrailer.BControl = BatchControlRecord.ParseBatchControl(line);
            root.FileContents.AchFile.Batches.Add(cBatch);
            cBatch = new Batch();
        }
        /// <summary>
        /// Parses the file control line
        /// </summary>
        /// <param name="line">The file control line from the ACH file.</param>
        /// <param name="root">The root object where the parsed file control information is stored.</param>
        private static void ProcessFileControl(string line, Root root)
        {
            root.FileContents.AchFile.FTrailer.FControl = FileControlRecord.ParseFileControl(line);
        }
    }
}