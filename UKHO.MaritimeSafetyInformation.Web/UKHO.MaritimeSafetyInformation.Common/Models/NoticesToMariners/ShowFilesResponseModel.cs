using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    [ExcludeFromCodeCoverage]
    public class ShowFilesResponseModel
    {
        [DataMember(Name = "batchId", EmitDefaultValue = false)]
        public string BatchId { get; set; }

        [DataMember(Name = "filename", EmitDefaultValue = false)]
        public string Filename { get; set; }

        [DataMember(Name = "filedescription", EmitDefaultValue = false)]
        public string FileDescription { get; set; }

        [DataMember(Name = "fileextension", EmitDefaultValue = false)]
        public string FileExtension { get; set; }

        [DataMember(Name = "fileSize", EmitDefaultValue = false)]
        public long? FileSize { get; set; }

        [DataMember(Name = "filesizeinkb", EmitDefaultValue = false)]
        public string FileSizeinKB { get; set; }

        [DataMember(Name = "mimeType", EmitDefaultValue = false)]
        public string MimeType { get; set; }

        [DataMember(Name = "hash", EmitDefaultValue = false)]
        public string Hash { get; set; }

        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public List<BatchDetailsAttributes> Attributes { get; set; }

        [DataMember(Name = "isdistributoruser", EmitDefaultValue = false)]
        public bool IsDistributorUser { get; set; }

        [DataMember(Name = "links", EmitDefaultValue = false)]
        public BatchDetailsLinks Links { get; set; }
    }
}
