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
                    if (lastEntry != null)
                    {
                        if (lastEntry.aDRecIndicator == AddendaRecordIndicator.Addenda)
                        {
                            var adCount = lastEntry.AddendaCount();
                            if (adCount >= 9999)
                            {
                                throw new Exception($"'{adCount}' Addenda Count exceeds the number of addenda record for '{currentBatch.BatchHeader.SECCode}'.");
                            }
                            else
                            {
                                var ad = new Addenda();
                                var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
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
                                            ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                            ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                            ad.DisHonorReturnReasonCode = rc;
                                            ad.OrigTraceNum = line.Substring(6, 15);
                                            ad.Reserved1 = line.Substring(21, 6).Trim();
                                            ad.OrigReceivingDFIId = line.Substring(27, 8);
                                            ad.Reserved2 = line.Substring(35, 3).Trim();
                                            ad.ReturnTraceNum = line.Substring(38, 15);
                                            ad.ReturnSettlementDate = line.Substring(53, 3);
                                            ad.DReturnReasonCode = line.Substring(56, 2);
                                            ad.AddendaInfo = line.Substring(58, 21).Trim();
                                            ad.AdTraceNum = line.Substring(79, 15);
                                        }
                                        else if (isContestedDisHonor == true)
                                        {
                                            ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                            ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                            ad.ContestedDisHonorReturnReasonCode = rc;
                                            ad.OrigTraceNum = line.Substring(6, 15);
                                            ad.DateOriginalEntryReturned = line.Substring(21, 6).Trim();
                                            ad.OrigReceivingDFIId = line.Substring(27, 8);
                                            ad.OriginalSettlementDate = line.Substring(35, 3).Trim();
                                            ad.ReturnTraceNum = line.Substring(38, 15);
                                            ad.ReturnSettlementDate = line.Substring(53, 3);
                                            ad.DReturnReasonCode = line.Substring(56, 2);
                                            ad.DisHonrorReturnTraceNum = line.Substring(58, 15);
                                            ad.ReturnSettlementDate = line.Substring(73, 3);
                                            ad.CReturnReasonCode = line.Substring(76, 2);
                                            ad.Reserved1 = line.Substring(78, 1).Trim();
                                            ad.AdTraceNum = line.Substring(79, 15);
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
                                    case AddendaTypeCode.NOCAddenda:
                                        ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                        ad.AdTypeCode = (AddendaTypeCode)int.Parse(line.Substring(1, 2));
                                        ad.ChangeCode = (ChangeCode)int.Parse(line.Substring(3, 3));
                                        ad.OrigTraceNum = line.Substring(6, 15);
                                        ad.Reserved1 = line.Substring(21, 6).Trim();
                                        ad.OrigReceivingDFIId = line.Substring(27, 8);
                                        ad.CorrectedData = line.Substring(35, 29).Trim();
                                        ad.Reserved2 = line.Substring(64, 15).Trim();
                                        ad.AdTraceNum = line.Substring(79, 15);
                                        lastEntry.AddendaRecord.Add(ad);
                                        break;
                                    default:
                                        throw new Exception($"Addenda Type Code '{typeCode}' is not supported on line '{line}'");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception($"Missing Addenda Record Indicator Record line");
                        }
                    }
                    else
                    {
                        throw new Exception("EntryDetailRecord is null");
                    }
                }
                else
                {
                    throw new Exception("EntryDetail is null");
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