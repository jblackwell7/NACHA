namespace NACHAParser
{
    public class WEBBatch : BatchBase
    {
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
                            DFIAcctNum = line.Substring(12, 17),
                            Amt = line.Substring(29, 10),
                            IndivIdNum = line.Substring(39, 15).Trim(),
                            IndivName = line.Substring(54, 22).Trim(),
                            PaymtTypeCode = line.Substring(76, 2),
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
                            case AddendaTypeCode.StandardAddenda:
                                ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                ad.PaymtRelatedInfo = line.Substring(3, 80).Trim();
                                ad.AddendaSeqNum = line.Substring(83, 4);
                                ad.EntDetailSeqNum = line.Substring(87, 7);
                                lastEntry.AddendaRecord.Add(ad);
                                break;
                            case AddendaTypeCode.ReturnAddenda:
                                var rc = Addenda.ParseReturnCode(line.Substring(3, 3));
                                bool isDisHonor = ad.IsDisHonor(lastEntry, rc);
                                if (isDisHonor != false)
                                {
                                    throw new Exception("Invalid Return Addenda");
                                }
                                else
                                {
                                    ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                    ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                    ad.ReturnReasonCode = (ReturnCode)int.Parse(line.Substring(3, 3));
                                    ad.OrigTraceNum = line.Substring(6, 15);
                                    ad.DateOfDeath = line.Substring(21, 6).Trim();
                                    ad.OrigReceivingDFIId = line.Substring(27, 8);
                                    ad.AddendaInfo = line.Substring(35, 44).Trim();
                                    ad.AdTraceNum = line.Substring(79, 15);
                                }
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
    }
}
