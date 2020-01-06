using ProtoBuf;

namespace LibScrapeTP
{
    [ProtoContract]
    public enum AcademicTitle
    {
        Engineer,
        Master,
        PhD,
        Pdd, // post-doctoral degree
        Profesor,
    }
}