using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowWeeklyFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseModel { get; set; }
        public List<SelectListItem> Years { get; set; }
        public List<SelectListItem> Weeks { get; set; }
    }
}
