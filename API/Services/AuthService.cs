using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;

namespace BlogAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly BlogDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(BlogDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(CreateUserDto createUserDto)
        {
            // Проверяем, существует ли пользователь
            if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username))
            {
                throw new ArgumentException("Пользователь с таким именем уже существует");
            }

            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            {
                throw new ArgumentException("Пользователь с таким email уже существует");
            }

            // Создаем нового пользователя
            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
            }

            return GenerateJwtToken(user);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Posts)
                .ThenInclude(p => p.Likes)
                .Include(u => u.Likes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            // Удаляем все лайки пользователя
            _context.Likes.RemoveRange(user.Likes);

            // Удаляем все лайки постов пользователя
            foreach (var post in user.Posts)
            {
                _context.Likes.RemoveRange(post.Likes);
            }

            // Удаляем все посты пользователя
            _context.Posts.RemoveRange(user.Posts);

            // Удаляем самого пользователя
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "your-secret-key-that-is-at-least-32-characters-long"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
