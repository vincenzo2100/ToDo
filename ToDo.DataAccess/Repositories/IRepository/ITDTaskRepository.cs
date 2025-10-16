


using ToDo.Models.Models;

namespace ToDo.DataAccess.Repositories.IRepository
{
    public interface ITDTaskRepository : IRepository<TDTask>
    {
        void Update(TDTask obj);
    }
}
