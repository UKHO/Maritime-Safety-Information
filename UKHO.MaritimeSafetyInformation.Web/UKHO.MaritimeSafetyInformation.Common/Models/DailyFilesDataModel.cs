using System.Runtime.Serialization;

namespace UKHO.MaritimeSafetyInformation.Common.Models
{
    public class DailyFilesDataModel
    {
        [DataMember(Name = "datadate", EmitDefaultValue = false)]
        public string DataDate
        {
            get;
            set;
        }
        [DataMember(Name = "batchid", EmitDefaultValue = false)]
        public string BatchId
        {
            get;
            set;
        }
        [DataMember(Name = "filename", EmitDefaultValue = false)]
        public string Filename
        {
            get;
            set;
        }

        [DataMember(Name = "filedescription", EmitDefaultValue = false)]
        public string FileDescription
        {
            get;
            set;
        }

        [DataMember(Name = "fileextension", EmitDefaultValue = false)]
        public string FileExtension
        {
            get;
            set;
        }

        [DataMember(Name = "fileSize", EmitDefaultValue = false)]
        public long? FileSize
        {
            get;
            set;
        }

        [DataMember(Name = "filesizeinkb", EmitDefaultValue = false)]
        public string FileSizeinKB
        {
            get;
            set;
        }

        [DataMember(Name = "mimeType", EmitDefaultValue = false)]
        public string MimeType
        {
            get;
            set;
        }

        [DataMember(Name = "links", EmitDefaultValue = false)]
        public string Links
        {
            get;
            set;
        }
        [DataMember(Name = "allfileszipsize", EmitDefaultValue = false)]
        public long AllFilesZipSize
        {
            get;
            set;
        }

    }
}
