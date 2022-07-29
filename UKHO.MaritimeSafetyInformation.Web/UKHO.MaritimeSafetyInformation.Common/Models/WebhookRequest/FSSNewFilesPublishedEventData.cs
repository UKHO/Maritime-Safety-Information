namespace UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest
{
    public class BatchDetails
    {
        public string Href { get; set; }
    }

    public class BatchStatus
    {
        public string Href { get; set; }
    }

    public class Links
    {
        public BatchDetails BatchDetails { get; set; }
        public BatchStatus BatchStatus { get; set; }
        public Get Get { get; set; }
    }

    public class Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Get
    {
        public string Href { get; set; }
    }

    public class File
    {
        public Links Links { get; set; }
        public string Hash { get; set; }
        public int FileSize { get; set; }
        public string MimeType { get; set; }
        public string Filename { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

    public class FSSNewFilesPublishedEventData
    {
        public Links Links { get; set; }
        public string BusinessUnit { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<File> Files { get; set; }
        public string BatchId { get; set; }
        public DateTime? BatchPublishedDate { get; set; }
    }
}
