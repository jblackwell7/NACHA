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
                    if (adIndicator == AddendaRecordIndicator.NoAddenda)
                    {
                        throw new Exception($"POS requires Addenda Record. Line '{lineNumber}'");
                    }
                    if (adIndicator == AddendaRecordIndicator.Addenda && nextLine.Substring(0, 1) != "7")
                    {
                        throw new Exception($"Entry Detail Record is missing an Addenda Record on LineNumber '{lineNumber}'");
                    }
                    else
                    {
                        EntryDetailRecord entry = new EntryDetailRecord()
                        {
                            RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                            TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                            RDFIId = line.Substring(3, 8),
                            CheckDigit = line[11],
                            DFIAcctNum = line.Substring(12, 17),
                            Amt = line.Substring(29, 10).Trim(),
                            IndivIdNum = line.Substring(39, 15).Trim(),
                            IndivName = line.Substring(54, 22).Trim(),
                            CardTransTypeCode = line.Substring(76, 2),
                            aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
                            TraceNum = line.Substring(79, 15)
                        };
                        currentBatch.EntryRecord.Add(entry);
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
                        var ad = new Addenda();
                        var adCount = lastEntry.AddendaCount();
                        if (adCount > 1)
                        {
                            throw new Exception($"'{adCount}' Addenda Count exceeds the number of addenda record for '{currentBatch.BatchHeader.SECCode}'.");
                        }
                        else
                        {
                            var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
                            switch (typeCode)
                            {
                                case AddendaTypeCode.POSAddenda:
                                    ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                    ad.AdTypeCode = typeCode;
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
                                case AddendaTypeCode.ReturnAddenda:
                                    var rc = Addenda.ParseReturnCode(line.Substring(3, 3));
                                    bool isDisHonor = ad.IsDisHonor(lastEntry, rc);
                                    bool isContestedDisHonor = ad.IsContestedDishonor(lastEntry, rc);
                                    if (isDisHonor == true)
                                    {
                                        ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                                        ad.AdTypeCode = typeCode;
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
                                        ad.AdTypeCode = typeCode;
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
                                        ad.AdTypeCode = typeCode;
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