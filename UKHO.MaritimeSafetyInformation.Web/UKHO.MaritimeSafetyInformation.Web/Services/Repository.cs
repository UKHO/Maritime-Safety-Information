using Microsoft.EntityFrameworkCore;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void SaveEntities()
        {
            _context.SaveChanges();
        }
    }
}
