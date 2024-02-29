namespace NACHAParser
{

    public class ParseDataResult
    {
        public Root? Root { get; set; }
        public int LinesProcessed { get; set; }
    }
    public class FileReader
    {
        public static ParseDataResult ParseData(List<string> lines)
        {
            IBatch? iBatch = null;
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
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.All(c => c == '9'))
                {
                    continue;
                }
                if (Enum.TryParse<RecordType>(line.Substring(0, 1), out RecordType recordType))
                {
                    string nextLine = lines[i + 1];
                    lineNumber++;
                    ProcessLine(recordType, line, root, ref iBatch, lineNumber, nextLine);
                }
                else
                {
                    Console.WriteLine($"Unsupported record type '{(int)recordType}' found on line {lineNumber}: {line}");
                    continue;
                }
            }
            // foreach (string line in File.ReadLines(inputACHFile))
            // {
            //     if (line.All(c => c == '9'))
            //     {
            //         continue;
            //     }
            //     if (Enum.TryParse<RecordType>(line.Substring(0, 1), out RecordType recordType))
            //     {
            //         lineNumber++;
            //         ProcessLine(recordType, line, root, ref iBatch, lineNumber);
            //     }
            //     else
            //     {
            //         Console.WriteLine($"Unsupported record type '{(int)recordType}' found on line {lineNumber}: {line}");
            //         continue;
            //     }
            // }
            return new ParseDataResult
            {
                Root = root,
                LinesProcessed = lineNumber - 1
            };
        }
        private static void ProcessLine(RecordType recordType, string line, Root root, ref IBatch iBatch, int lineNumber, string nextLine)
        {
            try
            {
                switch (recordType)
                {
                    case RecordType.fh:
                        ProcessFileHeader(line, root);
                        break;
                    case RecordType.bh:
                        var sec = BatchHeaderRecord.ParseSEC(line.Substring(50, 3));
                        iBatch = BatchFactory.CreateBatch(sec);
                        iBatch.ProcessBatchHeader(line, lineNumber, sec);
                        break;
                    case RecordType.ed:
                        iBatch.ProcessEntryDetail(line);
                        break;
                    case RecordType.ad:
                        iBatch.ProcessAddenda(line);
                        break;
                    case RecordType.bc:
                        iBatch.ProcessBatchControl(line, root);
                        break;
                    case RecordType.fc:
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
            root.FileContents.AchFile.FileHeader = FileHeaderRecord.ParseFileHeader(line);
        }
        /// <summary>
        /// Parses the file control line
        /// </summary>
        /// <param name="line">The file control line from the ACH file.</param>
        /// <param name="root">The root object where the parsed file control information is stored.</param>
        private static void ProcessFileControl(string line, Root root)
        {
            root.FileContents.AchFile.FileControl = FileControlRecord.ParseFileControl(line);
        }
    }
}