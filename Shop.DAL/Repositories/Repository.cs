using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Shop.DAL.Repositories
{
    public class Repository<T> : Interfaces.Repositories.IRepository<T> where T : class
    {
        protected MagazContext _context;

        public Repository(MagazContext context)
        {
            _context = context;
        }

       
        public List<T> GetList()
        {
            return _context.Set<T>().ToList();
        }

     
        public T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        
        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }
    }
}