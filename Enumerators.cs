
namespace NACHAParser
{
    public enum RecordTypes
    {
        fh = 1,
        bh = 5,
        ed = 6,
        ad = 7,
        bc = 8,
        fc = 9
    }
    public enum StandardEntryClassCode : int
    {
        CCD,
        PPD,
        WEB,
        TEL,
        COR,
    }
    public enum ServiceClass
    {
        MixDebitAndCredit = 200,
        DebitOnly = 220,
        CreditOnly = 225
    }
    public enum AddendTypeCode
    {
        POSAddenda = 02,
        StandardAddenda = 05,
        IAT1Addenda = 10,
        IAT2Addenda = 11,
        IAT3Addenda = 12,
        IAT4Addenda = 13,
        IAT5Addenda = 14,
        IAT6Addenda = 15,
        IAT7Addenda = 16,
        IAT8Addenda = 17,
        IAT9Addenda = 18,
        ReturnAddenda = 99,
        NOCAddenda = 98
    }
    public enum AddendaRecordIndicator
    {
        NoAddenda = 0,
        Addenda = 1
    }
}