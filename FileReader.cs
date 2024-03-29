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
                    ACHFile = new ACHFile()
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
                    ProcessLine(recordType, line, root, ref iBatch!, root.FileContents.ACHFile, lineNumber, nextLine);
                }
                else
                {
                    Console.WriteLine($"Unsupported record type '{(int)recordType}' found on line {lineNumber}: {line}.");
                    continue;
                }
            }
            return new ParseDataResult
            {
                Root = root,
                LinesProcessed = lineNumber - 1
            };
        }
        private static void ProcessLine(RecordType recordType, string line, Root root, ref IBatch iBatch, ACHFile achFile, int lineNumber, string nextLine)
        {
            try
            {
                switch (recordType)
                {
                    case RecordType.fh:
                        ProcessFileHeader(line, root);
                        break;
                    case RecordType.bh:
                        BatchFactory bf = new BatchFactory();
                        var sec = bf.ParseSEC(line.Substring(50, 3));
                        iBatch = bf.CreateBatch(sec);
                        iBatch.ProcessBatchHeader(line, lineNumber, achFile, sec);
                        break;
                    case RecordType.ed:
                        iBatch.ProcessEntryDetail(line, nextLine, achFile, lineNumber);
                        break;
                    case RecordType.ad:
                        iBatch.ProcessAddenda(line, achFile, lineNumber);
                        break;
                    case RecordType.bc:
                        iBatch.ProcessBatchControl(line, root, achFile);
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
            FileHeaderRecord fh = new FileHeaderRecord();
            root.FileContents.ACHFile.FileHeader = fh.ParseFileHeader(line);
        }
        /// <summary>
        /// Parses the file control line
        /// </summary>
        /// <param name="line">The file control line from the ACH file.</param>
        /// <param name="root">The root object where the parsed file control information is stored.</param>
        private static void ProcessFileControl(string line, Root root)
        {
            FileControlRecord fc = new FileControlRecord();
            root.FileContents.ACHFile.FileControl = fc.ParseFileControl(line);
        }
    }
}