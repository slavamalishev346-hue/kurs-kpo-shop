using Shop.Domain.Entities;

namespace Shop.Interfaces.Services
{
    public interface IUserService
    {
        
        User Authenticate(string username, string password);
    }
}