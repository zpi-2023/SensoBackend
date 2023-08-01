using Mapster;
using SensoBackend.Contracts.User;
using SensoBackend.Data;
using SensoBackend.Models;

namespace SensoBackend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<UserDto> GetAll()
    {
        return _context.Users.Adapt<IEnumerable<UserDto>>();
    }

    public bool Create(CreateUserDto newUser)
    {
        _context.Users.Add(newUser.Adapt<User>());
        _context.SaveChanges();
        return true;
    }
}