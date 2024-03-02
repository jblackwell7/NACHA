namespace NACHAParser
{
    public class POSBatch : BatchBase
    {
        #region Methods
        public override void ProcessBatchHeader(string line, int lineNumber, StandardEntryClassCode sec)
        {
            currentBatch = new Batch()
            {
                BatchHeader = BatchHeaderRecord.ParseBatchHeader(line, lineNumber, sec)
            };
        }
        public override void ProcessEntryDetail(string line, string nextLine, int lineNumber)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    var adIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1));

                    if (adIndicator == AddendaRecordIndicator.Addenda && nextLine.Substring(0, 1) == "7")
                    {
                        EntryDetailRecord entry = new EntryDetailRecord()
                        {
                            RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                            TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                            RDFIId = line.Substring(3, 8),
                            CheckDigit = line[11],
                            Amt = line.Substring(29, 10).Trim(),
                            IndivIdNum = line.Substring(39, 15).Trim(),
                            IndivName = line.Substring(54, 22).Trim(),
                            CardTransTypeCode = line.Substring(76, 2),
                            aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
                            TraceNum = line.Substring(79, 15)
                        };
                        currentBatch.EntryRecord.Add(entry);
                    }
                    else
                    {
                        throw new Exception($"Entry Detail Record is missing an Addenda Record on LineNumber '{lineNumber}'");
                    }
                }
                else
                {
                    throw new Exception("EntryDetailRecord is null");
                }
            }
            else
            {
                throw new Exception("Batch is null");
            }
        }
        public override void ProcessAddenda(string line, int lineNumber)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    var lastEntry = currentBatch.EntryRecord.LastOrDefault();
                    if (lastEntry.aDRecIndicator == AddendaRecordIndicator.NoAddenda)
                    {
                        throw new Exception("No Addenda Record Indicator");
                    }
                    else
                    {
                        var ad = new Addenda();
                        var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
                        switch (typeCode)
                        {
                            case AddendaTypeCode.POSAddenda:
                                ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                ad.RefInfo1 = line.Substring(03, 7).Trim();
                                ad.RefInfo2 = line.Substring(10, 3).Trim();
                                ad.TerminalIDCode = line.Substring(13, 6).Trim();
                                ad.TransSerialNum = line.Substring(19, 6).Trim();
                                ad.TransDate = line.Substring(25, 4).Trim();
                                ad.AuthCodeOrExpDate = line.Substring(29, 6).Trim();
                                ad.TerminalLoc = line.Substring(35, 27).Trim();
                                ad.TerminalCity = line.Substring(62, 15).Trim();
                                ad.TerminalState = line.Substring(77, 2).Trim();
                                ad.AdTraceNum = line.Substring(79, 15).Trim();
                                lastEntry.AddendaRecord.Add(ad);
                                break;
                            default:
                                throw new Exception($"Addenda Type Code '{typeCode}' is not supported on line '{line}'");
                        }
                    }
                }
                else
                {
                    throw new Exception("EntryDetailRecord is null");
                }
            }
            else
            {
                throw new Exception("batch is null");
            }
        }
        public override void ProcessBatchControl(string line, Root root)
        {
            if (currentBatch != null)
            {
                if (currentBatch.BatchControl == null)
                {
                    currentBatch.BatchControl = BatchControlRecord.ParseBatchControl(line);
                    root.FileContents.AchFile.Batches.Add(currentBatch);
                }
                else
                {
                    throw new Exception("BatchControlRecord is null");
                }
            }
            else
            {
                throw new Exception("Batch is null");
            }
        }

        #endregion
    }
}