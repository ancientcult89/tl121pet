using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataContext _dataContext;
        public string Role { get; set; } = string.Empty;

        public AuthService(IConfiguration configuration
            , DataContext dataContext
            , IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, await GetRoleNameByIdAsync(user.RoleId)),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        //TODO: вспомнить какую логику я хотел авязать на получение роли
        public string GetMyRole()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                return result;
            }
            else
                return "no role";
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

        public async Task<User?> LoginAsync(UserLoginRequestDTO request)
        {
            User user = await GetUserByEmailAsync(request.Email);
            if (user == null)
                return null;

            if (VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                string token = await CreateTokenAsync(user);
                
                //temporary
                Role = "Admin";
                return user;
            }

            return null;
        }

        public async Task Register(UserRegisterRequestDTO request)
        {
            User existsUser = await GetUserByEmailAsync(request.Email);
            if (existsUser != null)
                return;

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User newUser = new User { 
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await CreateUserAsync(newUser);
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task CreateRoleAsync(Role role)
        {
            _dataContext.Roles.Add(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            if (user != null)
            {
                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            Role role = _dataContext.Roles.Find(roleId);
            _dataContext.Roles.Remove(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(long userId)
        {
            User user = _dataContext.Users.Find(userId);
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Role>> GetRoleListAsync()
        {
            return await _dataContext.Roles.ToListAsync();
        }

        public async Task<string> GetRoleNameByIdAsync(int id)
        {
            Role role = await _dataContext.Roles.FindAsync(id);
            return role.RoleName;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<List<User>> GetUserListAsync()
        {
            return await _dataContext.Users.Include(p => p.Role).ToListAsync();
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

        public async Task UpdateRoleAsync(Role role)
        {
            _dataContext.Roles.Update(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }
    }
}
