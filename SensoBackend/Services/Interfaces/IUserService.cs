using SensoBackend.Contracts.User;

namespace SensoBackend.Services;

public interface IUserService
{
    IEnumerable<UserDto> GetAll();
    bool Create(CreateUserDto newUser);
}
