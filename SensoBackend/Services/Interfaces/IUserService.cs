using SensoBackend.Contracts.User;

namespace SensoBackend.Services;

public interface IUserService
{
    IList<UserDto> GetAll();
    bool Create(CreateUserDto newUser);
}
