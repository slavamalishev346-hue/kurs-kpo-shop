using System.Collections.Generic;
using System.Linq;
using Shop.DAL;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MagazContext _context;

        public UserRepository(MagazContext context)
        {
            _context = context;
        }

     
        public User GetByUsernameAndPassword(string login, string password)
        {
       
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

           
            if (user != null)
            {
                user.Role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            }

            return user;
        }

       
        public List<User> GetList()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Add(User entity)
        {
            _context.Users.Add(entity);
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
    }
}