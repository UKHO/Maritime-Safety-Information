using System.Runtime.Serialization;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowDailyFilesResponseModel
    {

        [DataMember(Name = "YearWeek", EmitDefaultValue = false)]
        public string YearWeek { get; set; }
        
        [DataMember(Name = "weeknumber", EmitDefaultValue = false)]
        public string WeekNumber { get; set; }

        [DataMember(Name = "year", EmitDefaultValue = false)]
        public string Year { get; set; }

        [DataMember(Name = "DataDate", EmitDefaultValue = false)]
        public List<DailyFilesDataModel> DailyFilesData { get; set; }
    }
}
