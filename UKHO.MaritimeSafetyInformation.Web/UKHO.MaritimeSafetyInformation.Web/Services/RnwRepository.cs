using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : Repository<RadioNavigationalWarnings>, IRnwRepository
    {
        public RnwRepository(RadioNavigationalWarningsContext rnwContext) :
         base(rnwContext)
        {

        }

        public void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            Add(radioNavigationalWarnings);
            SaveEntities();
        }
    }
}
