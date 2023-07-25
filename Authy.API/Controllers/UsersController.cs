using Authy.Data.Interfaces;
using Authy.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authy.API.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IAsyncRepository<User, long> _userRepo;
        private readonly IAsyncRepository<ApiKey, string> _apiKeyRepo;

        public UsersController(IAsyncRepository<User, long> userRepo, IAsyncRepository<ApiKey, string> apiKeyRepo)
        {
            _userRepo = userRepo;
            _apiKeyRepo = apiKeyRepo;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(User user)
        {
            var newUser = await _userRepo.CreateAsync(user);

            return Ok(newUser);
        }
    }
}
