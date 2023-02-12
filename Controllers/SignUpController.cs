using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Constants;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ISignUpTokenService signUpTokenService;

        public SignUpController(IUserRepository userRepository, ISignUpTokenService signUpTokenService)
        {
            this.userRepository = userRepository;
            this.signUpTokenService = signUpTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool validToken = await this.signUpTokenService.VerifySignUpTokenAsync(request.SignUpToken, request.CorporateEmailId);
            if (!validToken)
            {
                return BadRequest("Invalid SignUp Token");
            }
            if (!SupportedCompanies.IsValidCompany(request.CorporateEmailId))
            {
                return BadRequest("Invalid Email Address");
            }
            bool success = await this.userRepository.CreateUserIfNotExist(username: request.Username, password: request.Password, name: request.Name, phoneNumber: request.PhoneNumber, email: request.EmailId, corporateEmailId: request.CorporateEmailId);
            if (!success)
            {
                return BadRequest("User already exist");
            }
            return Ok();
        }
    }
}
