using BlogAPI.DTOs;

namespace BlogAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(CreateUserDto createUserDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<bool> DeleteUserAsync(int userId);
    }
}
