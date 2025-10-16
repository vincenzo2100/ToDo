

using ToDo.DataAccess.Data;
using ToDo.DataAccess.Repositories.IRepository;
using ToDo.Models.Models;

namespace ToDo.DataAccess.Repositories
{
    public class TDTaskRepository : Repository<TDTask>,ITDTaskRepository
    {
        private readonly AppDataContext _db;

        public TDTaskRepository(AppDataContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(TDTask obj)
        {
            _db.TDTasks.Update(obj);
        }
    }
}
