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
            var cBatch = new Batch { EntryRecords = new List<EntryDetailRecord>() };
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
            return new ParseDataResult { Root = root, LinesProcessed = lineNumber - 1 };
        }
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
        /// Processes a single line of the input file.
        /// </summary>
        /// <param name="recordType">The type of record to process.</param>
        /// <param name="line">The line to process.</param>
        /// <param name="root">The root object to populate.</param>
        /// <param name="cBatch">The current batch to populate.</param>
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
        private static void ProcessFileHeader(string line, Root root)
        {
            root.FileContents.AchFile.FHeader = FileHeaderRecord.ParseFileHeader(line);
            root.FileContents.FileId = Guid.NewGuid().ToString();
            Console.WriteLine($"New root.FileHd.FileId at parsing is: '{root.FileContents.FileId}'");
        }
        private static void ProcessBatchHeader(string line, ref Batch cBatch)
        {
            cBatch = new Batch { BatchHeader = BatchHeaderRecord.ParseBatchHeader(line) };
            cBatch.BchId = Guid.NewGuid().ToString();
            Console.WriteLine($"New cBatch.BchId at parsing is: '{cBatch.BchId}'");
        }
        /// <summary>
        /// Process an entry detail line
        /// </summary>
        /// <param name="line">The line from the file being processed</param>
        /// <param name="cBatch">The current batch</param>
        /// <param name="lineNumber">The line number of the line being processed</param>
        private static void ProcessEntryDetail(string line, Batch cBatch, int lineNumber)
        {
            var entryDetail = EntryDetails.ParseEntryDetail(line, cBatch.BatchHeader, lineNumber);

            entryDetail.EntryDetails.EntDetailsId = Guid.NewGuid().ToString();

            cBatch.EntryRecords.Add(entryDetail);

            Console.WriteLine($"ProcessEntryDetail entryDetail.EntryRecordId guid: '{entryDetail.EntRecId}' on line '{lineNumber}' added to batch '{cBatch.BchId}'");
            Console.WriteLine($"ProcessEntryDetail entryDetail.EntryDetails.EntryDetailsId guid: '{entryDetail.EntryDetails.EntDetailsId}' on line '{lineNumber}' added to batch '{cBatch.BchId}'");
        }
        /// <summary>
        /// This method processes addenda records.
        /// </summary>
        /// <param name="line">The line read from the file.</param>
        /// <param name="cBatch">The current batch.</param>
        /// <param name="lineNumber">The line number of the line read from the file.</param>
        private static void ProcessAddenda(string line, Batch cBatch, int lineNumber)
        {
            var addendaRecord = Addenda.ParseAddenda(line, lineNumber);

            var lastEntryDetail = cBatch.EntryRecords.Last();

            if (lastEntryDetail.EntryDetails.aDRecIndicator == AddendaRecordIndicator.Addenda)
            {
                addendaRecord.Addenda.Addenda05Id = Guid.NewGuid().ToString();

                lastEntryDetail.EntryDetails.AddendaRecords.Add(addendaRecord);
            }
            Console.WriteLine($"ProcessAddenda addendaRecord.AddendaRecordId  guid: '{addendaRecord.AddendaRecId}'");
        }
        /// <summary>
        /// Processes the batch control record.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="root">The root.</param>
        /// <param name="cBatch">The c batch.</param>
        private static void ProcessBatchControl(string line, Root root, ref Batch cBatch)
        {
            cBatch.BatchTrailer.BControl = BatchControlRecord.ParseBatchControl(line);
            root.FileContents.AchFile.Batches.Add(cBatch);
            cBatch = new Batch();
        }
        private static void ProcessFileControl(string line, Root root)
        {
            root.FileContents.AchFile.FTrailer.FControl = FileControlRecord.ParseFileControl(line);
            root.FileContents.AchFile.FTrailer.FileTrailerId = Guid.NewGuid().ToString();
            Console.WriteLine($"New root.FileHd.FTrailer.FileTrailerId at parsing is: '{root.FileContents.AchFile.FTrailer.FileTrailerId}'");
        }
        /// <summary>
        /// AssociateIds
        /// </summary>
        /// <param name="root">The root.</param>
        public static void AssociateIds(Root root)
        {
            foreach (Batch batch in root.FileContents.AchFile.Batches)
            {
                batch.BchId = Guid.NewGuid().ToString();
                Console.WriteLine($"AssociateIds batch.bchid guid: '{batch.BchId}'");

                foreach (EntryDetailRecord entryRecord in batch.EntryRecords)
                {
                    entryRecord.EntRecId = batch.BchId;
                    Console.WriteLine($"AssociateIds entryrecord.entrecid guid: '{entryRecord.EntRecId}'");

                    foreach (AddendaRecord addendaRecord in entryRecord.EntryDetails.AddendaRecords)
                    {
                        addendaRecord.AddendaRecId = entryRecord.EntRecId;
                        Console.WriteLine($"AssociateIds addendarecord.addendarecid guid: '{addendaRecord.AddendaRecId}'");
                    }
                }
            }
        }
    }
}