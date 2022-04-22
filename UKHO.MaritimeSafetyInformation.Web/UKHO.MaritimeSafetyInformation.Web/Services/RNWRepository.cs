using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Web.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RNWRepository : Repository<RadioNavigationalWarnings>, IRNWRepository
    {
        public RNWRepository(RadioNavigationalWarningsContext rnwContext) :
         base(rnwContext)
        {

        }

        public RadioNavigationalWarningsContext? MyDBContext
        {
            get { return _context as RadioNavigationalWarningsContext; }
        }

        public void AddRadioNavigation(RadioNavigationalWarnings rnwModel)
        {
            Add(rnwModel);
            SaveEntities();
        }

        public RadioNavigationalWarnings EditRadioNavigation(int id)
        {
            
            return Get(id);
        }

        public void UpdateRadioNavigation(RadioNavigationalWarnings rnwModel)
        {
            Update(rnwModel);
            SaveEntities();
        }

        public void DeleteRadioNavigation(RadioNavigationalWarnings rnwModel)
        {
            Remove(rnwModel);
            SaveEntities();
        }

    }
}
