using Shop.Interfaces.Services;
using Shop.Interfaces.UnitOfWork;
using Shop.DAL.UnitOfWork;
using Shop.Domain.Entities;

namespace Shop.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public User Authenticate(string username, string password)
        {
            // Здесь бизнес-логика аутентификации
            return _unitOfWork.Users.GetByUsernameAndPassword(username, password);
        }
    }
}