using Authy.Data.Interfaces;
using Authy.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using Authy.Domain.Interfaces;
using Authy.Common.Entities.DTO;
using System.Text;
using System.Security.Claims;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.IdentityModel.Tokens.Jwt;
using Authy.Common.Enums;

namespace Authy.API.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly IRepository<User, long> _userRepo;
        private readonly IRepository<ApiKey, long> _apiKeyRepo;
        private readonly ISecurityService _securityService;
        private readonly IUserRoleService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IDbConnectionFactory _dbFactory,
            IRepository<User, long> userRepo, 
            IRepository<ApiKey, long> apiKeyRepo,
            ISecurityService securityService,
            IUserRoleService roleService,
            ILogger<UsersController> logger)
        {
            this._dbFactory = _dbFactory;
            _userRepo = userRepo;
            _apiKeyRepo = apiKeyRepo;
            _securityService = securityService;
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserCreationDTO userDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userDto.Email)) return BadRequest();

                var exUser = _userRepo.FindByCondition(x => x.Email == userDto.Email).FirstOrDefault();

                if (exUser != null)
                {
                    return BadRequest();
                }

                if (string.IsNullOrEmpty(userDto.FirstName)) return BadRequest();
                if (string.IsNullOrEmpty(userDto.LastName)) return BadRequest();
                if (string.IsNullOrEmpty(userDto.PIN)) return BadRequest();
                if (string.IsNullOrEmpty(userDto.Password)) return BadRequest();
                if (string.IsNullOrEmpty(userDto.ConfirmPassword)) return BadRequest();
                if (userDto.Password != userDto.ConfirmPassword) return BadRequest();

                Tenant existingTenant = null;

                using (var db = _dbFactory.OpenDbConnection())
                {
                    existingTenant = await db.SingleByIdAsync<Tenant>(userDto.TenantId);
                }

                if (existingTenant == null) return BadRequest();

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

                _userRepo.Create(user);
                await _roleService.AddUserToRole(user, "User");

                var dto = new UserDTO
                {
                    IsActive = user.IsActive,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    MiddleName = user.MiddleName,
                    PIN = user.PIN,
                    Roles = user.Roles.Select(x => new UserRoleDTO { Code = x.Role.Code, Description = x.Role.Description, Id = x.Role.Id, IsCoreRole = x.Role.IsCoreRole, Name = x.Role.Name }),
                    TenantId = user.TenantId
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            try
            {
                List<UserDTO> dtos = new List<UserDTO>();

                var users = _userRepo.FindAll();

                foreach(var user in users)
                {
                    var dto = new UserDTO
                    {
                        IsActive = user.IsActive,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        MiddleName = user.MiddleName,
                        PIN = user.PIN,
                        Roles = user.Roles.Select(x => new UserRoleDTO { Code = x.Role.Code, Description = x.Role.Description, Id = x.Role.Id, IsCoreRole = x.Role.IsCoreRole, Name = x.Role.Name }),
                        TenantId = user.TenantId
                    };

                    dtos.Add(dto);
                }

                return Ok(dtos);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthDTO>> Login(UserLoginDTO loginDto)
        {
            try
            {
                var user = _userRepo.FindByCondition(x => x.Email == loginDto.Email).FirstOrDefault();

                if (user == null) return NotFound();

                var isValid = _securityService.VerifyPassword(loginDto.Password, user.PasswordHash, Convert.FromBase64String(user.Salt));

                if (!isValid) return Unauthorized();

                var tokenExpiration = DateTime.UtcNow.AddDays(1);
                var claims = new List<Claim>();
                var secretPhrase = Environment.GetEnvironmentVariable(EnvironmentVariableEnum.SECRET_PHRASE);

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
                        PIN = user.PIN,
                        Roles = user.Roles.Select(x => new UserRoleDTO { Code = x.Role.Code, Description = x.Role.Description, Id = x.Role.Id, IsCoreRole = x.Role.IsCoreRole, Name = x.Role.Name }),
                        TenantId = user.TenantId
                    },
                    Token = _securityService.EncodeToken(authToken),
                    TokenExpiration = tokenExpiration,
                };

                var apiKey = new ApiKey()
                {
                    UserAuthId = user.Id,
                    CreatedDate = authToken.ValidFrom,
                    ExpiryDate = authToken.ValidTo,
                    KeyType = typeof(JwtSecurityToken).Name,
                    TenantId = user.TenantId,
                    RefIdStr = authDto.Token,
                };

                _apiKeyRepo.Create(apiKey);

                this.HttpContext.Response.Headers.Add("Set-Cookie", $"{Environment.GetEnvironmentVariable(EnvironmentVariableEnum.JWT_TOKEN_NAME)}={authDto.Token}; secure; httpOnly; sameSite=Lax;");

                return Ok(authDto);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
