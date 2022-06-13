using System.Runtime.Serialization;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class AttributesModel
    {
        [DataMember(Name = "batchid", EmitDefaultValue = false)]
        public string BatchId { get; set; }

        [DataMember(Name = "datadate", EmitDefaultValue = false)]
        public string DataDate { get; set; }

        [DataMember(Name = "weeknumber", EmitDefaultValue = false)]
        public string WeekNumber { get; set; }

        [DataMember(Name = "year", EmitDefaultValue = false)]
        public string Year { get; set; }

        [DataMember(Name = "yearweek", EmitDefaultValue = false)]
        public string YearWeek { get; set; }

        [DataMember(Name = "allfileszipsize", EmitDefaultValue = false)]
        public long AllFilesZipSize { get; set; }

        [DataMember(Name = "batchpublisheddate", EmitDefaultValue = false)]
        public DateTime? BatchPublishedDate { get; set; }
    }
}
