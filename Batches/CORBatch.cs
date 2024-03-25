using NACHAParser;
namespace NACHAParser
{
    public class CORBatch : BatchBase
    {
        public override BatchHeaderRecord ProcessBatchHeader(string line, int lineNumber, ACHFile achFile, StandardEntryClassCode sec)
        {
            var batch = new Batch
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
            achFile.AddBatch(batch);
            return batch.BatchHeader;
        }
        public override void ProcessEntryDetail(string line, string nextLine, ACHFile achFile, int lineNumber)
        {
            if (achFile.CurrentBatch != null)
            {
                if (achFile.CurrentBatch.EntryRecord != null)
                {
                    EntryDetailRecord entry = new EntryDetailRecord()
                    {
                        RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                        TransCode = (TransactionCode)int.Parse(line.Substring(1, 2)),
                        RDFIId = line.Substring(3, 8),
                        CheckDigit = line.Substring(11, 1),
                        DFIAcctNum = line.Substring(12, 17),
                        Amt = int.Parse(line.Substring(29, 10)),
                        IndivIdNum = line.Substring(39, 15).Trim(),
                        IndivName = line.Substring(54, 22).Trim(),
                        PaymtTypeCode = line.Substring(76, 2),
                        aDRecIndicator = (AddendaRecordIndicator)int.Parse(line.Substring(78, 1)),
                        TraceNum = line.Substring(79, 15)
                    };
                    entry.ValidateEntryDetail(nextLine);
                    achFile.CurrentBatch.EntryRecord.Add(entry);
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
        public override void ProcessAddenda(string line, ACHFile achFile, int lineNumber)
        {
            if (achFile.CurrentBatch != null)
            {
                if (achFile.CurrentBatch.EntryRecord != null)
                {
                    var lastEntry = achFile.CurrentBatch.EntryRecord.LastOrDefault();
                    if (lastEntry != null)
                    {
                        var ad = new Addenda();
                        var typeCode = Addenda.ParseAddendaType(line.Substring(1, 2));
                        var cc = ad.ParseChangeCode(line.Substring(3, 3));
                        bool isRefusedCOR = ad.IsRefusedCORCode(cc);

                        if (isRefusedCOR == false && typeCode == AddendaTypeCode.Addenda98)
                        {
                            ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                            ad.AdTypeCode = typeCode;
                            ad.ChangeCode = cc;
                            ad.OrigTraceNum = line.Substring(6, 15);
                            ad.Reserved1 = line.Substring(21, 6).Trim();
                            ad.OrigReceivingDFIId = line.Substring(27, 8);
                            ad.CorrectedData = line.Substring(35, 29).Trim();
                            ad.Reserved2 = line.Substring(64, 15).Trim();
                            ad.AdTraceNum = line.Substring(79, 15);
                            lastEntry.AddendaRecord.Add(ad);
                        }
                        else if (isRefusedCOR == true && typeCode == AddendaTypeCode.Addenda98)
                        {
                            ad.RecType = (RecordType)int.Parse(line.Substring(0, 1));
                            ad.AdTypeCode = typeCode;
                            ad.RefusedCORCode = cc;
                            ad.OrigTraceNum = line.Substring(6, 15);
                            ad.Reserved1 = line.Substring(21, 6).Trim();
                            ad.OrigReceivingDFIId = line.Substring(27, 8);
                            ad.CorrectedData = line.Substring(35, 29).Trim();
                            ad.ChangeCode = (ChangeCode)int.Parse(line.Substring(64, 3));
                            ad.CorTraceSeqNum = line.Substring(67, 7);
                            ad.Reserved2 = line.Substring(74, 5).Trim();
                            ad.AdTraceNum = line.Substring(79, 15);
                            lastEntry.AddendaRecord.Add(ad);
                        }
                        else
                        {
                            throw new Exception($"Addenda Type Code '{(int)typeCode}' is not supported on line '{line}'");
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
        public override BatchControlRecord ProcessBatchControl(string line, Root root, ACHFile achFile)
        {
            if (achFile.CurrentBatch != null)
            {
                if (achFile.CurrentBatch.BatchControl == null)
                {
                    BatchControlRecord bc = new BatchControlRecord()
                    {
                        RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                        ServiceClassCode = (ServiceClassCode)int.Parse(line.Substring(1, 3)),
                        EntAddendaCnt = line.Substring(4, 6),
                        EntHash = line.Substring(10, 10),
                        TotBchDrEntAmt = line.Substring(20, 12),
                        TotBchCrEntAmt = line.Substring(32, 12),
                        CoId = line.Substring(44, 10).Trim(),
                        MsgAuthCode = line.Substring(54, 19).Trim(),
                        Reserved = line.Substring(73, 6).Trim(),
                        OriginatingDFIId = line.Substring(79, 8),
                        BchNum = line.Substring(87, 7)
                    };
                    achFile.CurrentBatch.BatchControl = bc;
                    return achFile.CurrentBatch.BatchControl;
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