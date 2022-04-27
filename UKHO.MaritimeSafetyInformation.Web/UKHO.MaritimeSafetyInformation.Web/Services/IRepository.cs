
namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void SaveEntities();
    }
}
