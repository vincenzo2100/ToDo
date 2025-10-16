


using ToDo.DataAccess.Data;
using ToDo.DataAccess.Repositories.IRepository;

namespace ToDo.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDataContext _db;
        public ITDTaskRepository TDTask { get; private set; }

        public UnitOfWork(AppDataContext db)
        {
            _db = db;
            TDTask = new TDTaskRepository(_db);
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

    }
}
