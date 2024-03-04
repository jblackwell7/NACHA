namespace NACHAParser
{
    public class CTXBatch : BatchBase
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
                    EntryDetailRecord entry = new EntryDetailRecord()
                    {
                        RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                        TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                        RDFIId = line.Substring(3, 8),
                        CheckDigit = line[11],
                        DFIAcctNum = line.Substring(12, 17),
                        Amt = line.Substring(29, 10),
                        IndivIdNum = line.Substring(39, 15).Trim(),
                        NumOfAddendaRecords = int.Parse(line.Substring(54, 4)),
                        ReceiverCoName = line.Substring(58, 16).Trim(),
                        Reserved = line.Substring(74, 2).Trim(),
                        DiscretionaryData = line.Substring(76, 2).Trim(),
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
        public override void ProcessAddenda(string line, int lineNumber)
        {
            if (currentBatch != null)
            {
                if (currentBatch.EntryRecord != null)
                {
                    var lastEntry = currentBatch.EntryRecord.LastOrDefault();
                    if (lastEntry.aDRecIndicator == AddendaRecordIndicator.NoAddenda)
                    {
                        throw new Exception($"Missing Addenda Record Indicator Record line '{lineNumber}'");
                    }
                    else
                    {
                        var ad = new Addenda();
                        var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
                        if (lastEntry.AddendaRecord.Count > 9999)
                        {
                            throw new Exception($"SEC '{currentBatch.BatchHeader.SECCode}' Addenda Count exceeds 9999 on line '{lineNumber}'");
                        }
                        else
                        {
                            switch (typeCode)
                            {
                                case AddendaTypeCode.StandardAddenda:
                                    ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                    ad.AdTypeCode = typeCode;
                                    ad.PaymtRelatedInfo = line.Substring(3, 80).Trim();
                                    ad.AddendaSeqNum = line.Substring(83, 4);
                                    ad.EntDetailSeqNum = line.Substring(87, 7);
                                    lastEntry.AddendaRecord.Add(ad);
                                    break;
                                case AddendaTypeCode.ReturnAddenda:
                                    var rc = Addenda.ParseReturnCode(line.Substring(3, 3));
                                    bool isDisHonor = ad.IsDisHonor(lastEntry, rc);
                                    bool isContestedDisHonor = ad.IsContestedDishonor(lastEntry, rc);
                                    if (isDisHonor == true)
                                    {
                                        //TODO: Dishonor Support
                                        throw new Exception("Dishonor Support is not implemented");
                                    }
                                    else if (isContestedDisHonor == true)
                                    {
                                        //TODO: Contested Dishonor Support
                                        throw new Exception("Contested Dishonor Support is not implemented");
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