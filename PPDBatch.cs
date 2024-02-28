namespace NACHAParser
{
    public class PPDBatch : BatchBase
    {
        public override void ProcessBatchHeader(string line, int lineNumber, StandardEntryClassCode sec)
        {
            currentBatch = new Batch()
            {
                BatchHeader = BatchHeaderRecord.ParseBatchHeader(line, lineNumber, sec)
            };
        }
        public override void ProcessEntryDetail(string line)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    EntryDetailRecord entry = new EntryDetailRecord()
                    {
                        RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                        TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                        RDFIId = line.Substring(3, 8),
                        CheckDigit = line[11],
                        IndivIdNum = line.Substring(39, 15),
                        IndivName = line.Substring(54, 22),
                        DiscretionaryData = line.Substring(76, 2),
                        aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
                        TraceNum = line.Substring(79, 15)
                    };
                    currentBatch.EntryRecord.Add(entry);
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
        public override void ProcessAddenda(string line)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    var lastEntry = currentBatch.EntryRecord.LastOrDefault();
                    var ad = new AddendaRecord();
                    var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));

                    switch (typeCode)
                    {
                        case AddendaTypeCode.StandardAddenda:
                            ad.Addenda = new Addenda
                            {
                                RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                                AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2)),
                                PaymtRelatedInfo = line.Substring(3, 80),
                                AddendaSeqNum = line.Substring(83, 4),
                                EntDetailSeqNum = line.Substring(87, 7)
                            };
                            lastEntry.AddendaRecords.Add(ad);
                            break;
                        case AddendaTypeCode.ReturnAddenda:
                            var rc = Addenda.ParseReturnCode(line.Substring(3, 3));
                            bool isDisHonor = ad.Addenda.IsDisHonor(lastEntry, rc);
                            if (isDisHonor != false)
                            {
                                throw new Exception("Invalid Return Addenda");
                            }
                            else
                            {
                                ad.Addenda = new Addenda
                                {
                                    RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                                    AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2)),
                                    ReturnReasonCode = (ReturnCode)int.Parse(line.Substring(3, 3)),
                                    OrigTraceNum = line.Substring(6, 15),
                                    DateOfDeath = line.Substring(21, 6),
                                    OrigReceivingDFIId = line.Substring(27, 8),
                                    AddendaInfo = line.Substring(35, 44),
                                    AdTraceNum = line.Substring(79, 15)
                                };

                            }
                            lastEntry.AddendaRecords.Add(ad);
                            break;
                        default:
                            throw new Exception("Addenda Type Code '{typeCode}' is not supported");
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
    }
}