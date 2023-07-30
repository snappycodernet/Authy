using Authy.Data.Interfaces;
using Authy.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using Authy.Domain.Interfaces;
using Authy.Common.Entities.DTO;
using System.Text;
using System.Security.Claims;

namespace Authy.API.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IAsyncRepository<User, long> _userRepo;
        private readonly IAsyncRepository<ApiKey, string> _apiKeyRepo;
        private readonly ISecurityService _securityService;
        private readonly IUserRoleService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IAsyncRepository<User, long> userRepo, 
            IAsyncRepository<ApiKey, string> apiKeyRepo,
            ISecurityService securityService,
            IUserRoleService roleService,
            ILogger<UsersController> logger)
        {
            _userRepo = userRepo;
            _apiKeyRepo = apiKeyRepo;
            _securityService = securityService;
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserCreationDTO userDto)
        {
            var user = new User()
            {
                FirstName = userDto.FirstName,
                Email = userDto.Email,
                IsActive = userDto.IsActive,
                MiddleName = userDto.MiddleName,
                PIN = userDto.PIN,
                LastName = userDto.LastName,
                TenantId = userDto.TenantId,
            };

            var pwHash = _securityService.HashPasword(userDto.Password, out byte[] saltBytes);

            user.PasswordHash = pwHash;
            user.Salt = Convert.ToBase64String(saltBytes);

            await _userRepo.CreateAsync(user);
            await _roleService.AddUserToRole(user, "User");

            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _userRepo.FindAllAsync();

            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthDTO>> Login(UserLoginDTO loginDto)
        {
            var user = (await _userRepo.FindByConditionAsync(x => x.Email == loginDto.Email)).FirstOrDefault();

            if (user == null) return NotFound();

            var isValid = _securityService.VerifyPassword(loginDto.Password, user.PasswordHash, Convert.FromBase64String(user.Salt));

            if (!isValid) return Unauthorized();

            var tokenExpiration = DateTime.UtcNow.AddDays(14);
            var claims = new List<Claim>();
            var secretPhrase = Environment.GetEnvironmentVariable("secretPhrase");

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.Role.Name));
            }

            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var authToken = _securityService.GenerateToken("https://localhost:5001", "https://localhost:3000", secretPhrase, claims, tokenExpiration);

            var authDto = new UserAuthDTO()
            {
                User = new UserDTO
                {
                    IsActive = user.IsActive,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    MiddleName = user.MiddleName,
                    PIN = user.MiddleName,
                    Roles = user.Roles.Select(x => new UserRoleDTO { Code = x.Role.Code, Description = x.Role.Description, Id = x.Role.Id, IsCoreRole = x.Role.IsCoreRole, Name = x.Role.Name }),
                    TenantId = user.TenantId
                },
                Token = _securityService.EncodeToken(authToken),
                TokenExpiration = tokenExpiration,
            };

            return Ok(authDto);
        }
    }
}
