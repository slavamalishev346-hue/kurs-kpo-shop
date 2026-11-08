using Shop.Domain.Entities;

namespace Shop.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUsernameAndPassword(string username, string password);
    }
}