using System.Runtime.Serialization;

namespace UKHO.MaritimeSafetyInformation.Common.Models
{
    public class AttributesModel
    {
        [DataMember(Name = "datadate", EmitDefaultValue = false)]
        public string DataDate
        {
            get;
            set;
        }
        [DataMember(Name = "weeknumber", EmitDefaultValue = false)]
        public string WeekNumber
        {
            get;
            set;
        }
        [DataMember(Name = "year", EmitDefaultValue = false)]
        public string Year
        {
            get;
            set;
        }

        [DataMember(Name = "yearweek", EmitDefaultValue = false)]
        public string YearWeek
        {
            get;
            set;
        }
    }
}
