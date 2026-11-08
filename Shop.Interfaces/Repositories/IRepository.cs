using System.Collections.Generic;

namespace Shop.Interfaces.Repositories
{
   
    public interface IRepository<T> where T : class
    {
        List<T> GetList();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}