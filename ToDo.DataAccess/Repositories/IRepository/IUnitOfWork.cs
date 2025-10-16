namespace ToDo.DataAccess.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        ITDTaskRepository TDTask { get; }
        Task Save();
    }
}
