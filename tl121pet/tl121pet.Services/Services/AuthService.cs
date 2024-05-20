using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class AuthService(IConfiguration configuration
        , DataContext dataContext
        , IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private DataContext _dataContext = dataContext;
        public string Role { get; set; } = string.Empty;

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task ChangeLocaleByUserIdAsync(long userId, Locale locale)
        {
            User user = await _dataContext.Users.FindAsync(userId);

            if (user == null)
                return;

            user.Locale = locale;
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, await GetRoleNameByIdAsync(user.RoleId)),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            string secret = _configuration.GetSection("AppSettings:TokenSecret").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler();

            string returnedToken = "";

            try {
                returnedToken = jwt.WriteToken(token);
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }

            return returnedToken;
        }

        public long? GetMyUserId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return  Convert.ToInt64(result);
            }
            else
                return null;
        }

        public async Task<LoginResponseDTO> LoginAsync(UserLoginRequestDTO request)
        {
            User user = await GetUserByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("User not found");

            if (VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                string token = await CreateTokenAsync(user);
                LoginResponseDTO loginResponse = new LoginResponseDTO() { 
                    User = user,
                    Role = user.Role,
                    Token = token,
                    Locale = user.Locale,
                };
                return loginResponse;
            }

            throw new Exception("Wrong password");
        }

        public async Task RegisterAsync(UserRegisterRequestDTO request)
        {
            User existsUserByEmail = await GetUserByEmailAsync(request.Email);
            if (existsUserByEmail != null)
                throw new Exception("A User with the same email already exists");
            User existsUserByName = await GetUserByNameAsync(request.UserName);
            if (existsUserByName != null)
                throw new Exception("A User with the same UserName already exists");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User newUser = new User { 
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await CreateUserAsync(newUser);
        }

        public async Task ChangePasswordAsync(ChangeUserPasswordRequestDTO changeUserPasswordRequest)
        {
            User user = await GetUserByIdAsync(changeUserPasswordRequest.UserId);
            if (user == null)
                throw new Exception("User not found");
            if (VerifyPasswordHash(changeUserPasswordRequest.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                await SaveNewPasswordAsync(changeUserPasswordRequest.NewPassword, user);
            }
            else
            {
                throw new Exception("Wrong password");
            }
        }

        private async Task SaveNewPasswordAsync(string newPassword, User user)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await UpdateUserAsync(user);
        }

        public async Task<string> RecoverPasswordAsync(string email)
        {
            try
            {
                User user = await GetUserByEmailAsync(email) ?? throw new DataFoundException("User not found");
                string newPassword = GenerateRandomString(10);
                await SaveNewPasswordAsync(newPassword, user);
                return newPassword;
            }
            catch(Exception ex) { throw new Exception(ex.Message); }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(long userId)
        {
            User user = _dataContext.Users.Find(userId);
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }



        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users
                .Where(u => u.Email == email)
                .Include(r => r.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            return await _dataContext.Users
                .Where(u => u.UserName == userName)
                .Include(r => r.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<List<User>> GetUserListAsync()
        {
            return await _dataContext.Users.Include(p => p.Role).ToListAsync() ?? new List<User>();
        }

        public async Task<List<ProjectTeam>> GetUserProjectsAsync(long userId)
        {
            List<ProjectTeam> usersProjects = new List<ProjectTeam>();

            var projects = (
                from proj in _dataContext.ProjectTeams
                join usrproj in _dataContext.UserProjects on proj.ProjectTeamId equals usrproj.ProjectTeamId
                where usrproj.UserId == userId
                select proj
            ).ToListAsync();

            foreach (var proj in await projects)
            {
                usersProjects.Add(proj);
            }

            return usersProjects;
        }

        public async Task UpdateUserAsync(User user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
        {
            User user = await GetUserByIdAsync(userDto.Id);

            if (user == null)
                throw new Exception("User not found");

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.RoleId = userDto.RoleId;

            await UpdateUserAsync(user);

            return user.ToDto();
        }

        #region Role
        public async Task<string> GetRoleNameByIdAsync(int? id)
        {
            if (id != null)
            {
                Role role = await _dataContext.Roles.FindAsync(id);
                return role.RoleName;
            }
            else
                return string.Empty;
        }
        #endregion Role

        public static string GenerateRandomString(int length)
        {
            const string pattern = "[A-Za-z]";
            Random random = new Random();
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                char randomChar = GetRandomCharFromPattern(pattern, random);
                result.Append(randomChar);
            }

            return result.ToString();
        }

        private static char GetRandomCharFromPattern(string pattern, Random random)
        {
            Regex regex = new Regex(pattern);
            char randomChar;

            do
            {
                randomChar = (char)random.Next(32, 127); // Генерируем случайный символ в диапазоне печатных символов ASCII
            } while (!regex.IsMatch(randomChar.ToString()));

            return randomChar;
        }
    }
}
