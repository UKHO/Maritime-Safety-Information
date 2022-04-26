namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    public class FileShareServiceConfiguration
    {
        public string BaseUrl { get; set; }
        public string BusinessUnit { get; set; }
        public string ProductType { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }
    }
}
