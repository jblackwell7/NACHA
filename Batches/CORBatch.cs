using NACHAParser;

public class CORBatch : BatchBase
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
                        Amt = line.Substring(29, 10),
                        IndivIdNum = line.Substring(39, 15).Trim(),
                        IndivName = line.Substring(54, 22).Trim(),
                        PaymtTypeCode = line.Substring(76, 2),
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
                        var cc = Addenda.ParseChangeCode(line.Substring(3, 3));
                        bool isRefusedCOR = ad.IsRefusedCORCode(cc);

                        if (isRefusedCOR == false && typeCode == AddendaTypeCode.NOCAddenda)
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
                        else if (isRefusedCOR == true && typeCode == AddendaTypeCode.NOCAddenda)
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
