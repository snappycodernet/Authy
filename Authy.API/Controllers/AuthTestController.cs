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
using Microsoft.AspNetCore.Authorization;

namespace Authy.API.Controllers
{
    [ApiController]
    [Route("/auth-test")]
    public class AuthTestController : ControllerBase
    {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly IRepository<User, long> _userRepo;
        private readonly IRepository<ApiKey, long> _apiKeyRepo;
        private readonly ISecurityService _securityService;
        private readonly IUserRoleService _roleService;
        private readonly ILogger<AuthTestController> _logger;

        public AuthTestController(
            IDbConnectionFactory _dbFactory,
            IRepository<User, long> userRepo, 
            IRepository<ApiKey, long> apiKeyRepo,
            ISecurityService securityService,
            IUserRoleService roleService,
            ILogger<AuthTestController> logger)
        {
            this._dbFactory = _dbFactory;
            _userRepo = userRepo;
            _apiKeyRepo = apiKeyRepo;
            _securityService = securityService;
            _roleService = roleService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<object> TestAuthorizedGetRoute()
        {
            var response = new
            {
                success = true,
                message = "Woot! Authorized!!"
            };

            return Ok(response);
        }
    }
}
