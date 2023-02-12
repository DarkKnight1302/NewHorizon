using Microsoft.AspNetCore.Mvc;
using NewHorizon.Repositories.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SaltController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public SaltController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetSalt(string username)
        {
            var user = await _userRepository.GetUserByUserNameAsync(username).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Salt);
        }
    }
}
