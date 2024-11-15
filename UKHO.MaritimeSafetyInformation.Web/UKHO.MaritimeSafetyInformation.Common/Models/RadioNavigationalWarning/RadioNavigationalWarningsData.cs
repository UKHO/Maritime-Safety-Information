﻿using System.ComponentModel;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    public class RadioNavigationalWarningsData
    {
        public int Id { get; set; }

        public string Reference { get; set; }
       
        public DateTime DateTimeGroup { get; set; }

        public string Description { get; set; }

        [DisplayName("Date Time Group")]
        public string DateTimeGroupRnwFormat { get; set; }

        public string Content { get; set; }

        public string WarningType { get; set; }
    }
}
