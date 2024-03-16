namespace NACHAParser
{
    public class ATXBatch : BatchBase
    {
        public override BatchHeaderRecord ProcessBatchHeader(string line, int lineNumber, StandardEntryClassCode sec)
        {
            currentBatch = new Batch
            {
                BatchHeader = new BatchHeaderRecord()
                {
                    RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                    ServiceClassCode = (ServiceClassCode)int.Parse(line.Substring(1, 3)),
                    CoName = line.Substring(4, 16).Trim(),
                    CoDiscretionaryData = line.Substring(20, 20).Trim(),
                    CoId = line.Substring(40, 10).Trim(),
                    SECCode = sec,
                    CoEntDescription = line.Substring(63, 10).Trim(),
                    CoDescriptiveDate = line.Substring(63, 6).Trim(),
                    EffectiveEntDate = line.Substring(69, 6),
                    SettlementDate = line.Substring(75, 3).Trim(),
                    OriginatorStatusCode = (OriginatorStatusCode)int.Parse(line.Substring(78, 1)),
                    OriginatingDFIId = line.Substring(78, 8),
                    BchNum = line.Substring(87, 7)
                }
            };
            return currentBatch.BatchHeader;
        }
        public override void ProcessEntryDetail(string line, string nextLine, int lineNumber)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    var tc = (TransactionCode)int.Parse(line.Substring(1, 2));
                    var amt = line.Substring(29, 10);

                    if ((tc == TransactionCode.CheckingZeroDollarRemitCredit || tc == TransactionCode.CheckingZeroDollarRemitCredit) && amt == null)
                    {
                        var adIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1));
                        if ((adIndicator == AddendaRecordIndicator.Addenda && nextLine.Substring(0, 1) == "7") || (adIndicator == AddendaRecordIndicator.NoAddenda && nextLine.Substring(0, 1) != "7"))
                        {
                            EntryDetailRecord entry = new EntryDetailRecord()
                            {
                                RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                                TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                                RDFIId = line.Substring(3, 8),
                                CheckDigit = line[11],
                                DFIAcctNum = line.Substring(12, 17),
                                Amt = line.Substring(29, 10),
                                OriginalTraceNum = line.Substring(39, 15),
                                ReceiverCoName = line.Substring(54, 22),
                                DiscretionaryData = line.Substring(76, 2),
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
                        throw new Exception($"Not a valid format for {currentBatch.BatchHeader.SECCode} on LineNumber '{lineNumber}'");
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
                    if (lastEntry != null)
                    {
                        var adCount = lastEntry.AddendaCount();
                        if (adCount > 1)
                        {
                            throw new Exception($"'{adCount}' Addenda Count exceeds the number of addenda record for '{currentBatch.BatchHeader.SECCode}'.");
                        }
                        else
                        {
                            var ad = new Addenda();
                            var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
                            switch (typeCode)
                            {
                                case AddendaTypeCode.NOCAddenda:
                                    ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                    ad.AdTypeCode = typeCode;
                                    ad.PaymtRelatedInfo = line.Substring(3, 80).Trim();
                                    ad.AddendaSeqNum = line.Substring(83, 4);
                                    ad.EntDetailSeqNum = line.Substring(87, 7);
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
                    BatchControlRecord bc = new BatchControlRecord();
                    currentBatch.BatchControl = bc.ParseBatchControl(line);
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