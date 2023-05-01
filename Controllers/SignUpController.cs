using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using NewHorizon.Constants;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using System.Text.RegularExpressions;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ISignUpTokenService signUpTokenService;
        private readonly ISessionTokenManager sessionTokenManager;

        public SignUpController(IUserRepository userRepository, ISignUpTokenService signUpTokenService, ISessionTokenManager sessionTokenManager)
        {
            this.userRepository = userRepository;
            this.signUpTokenService = signUpTokenService;
            this.sessionTokenManager = sessionTokenManager;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Username != request.EmailId)
            {
                ModelState.AddModelError("Username", "Invalid Username");
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber) && !Regex.IsMatch(request.PhoneNumber, @"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$"))
            {
                ModelState.AddModelError("PhoneNumber", "Invalid Phone number");
                return BadRequest(ModelState);
            }

            if (!SupportedCompanies.IsCorporateEmail(request.CorporateEmailId))
            {
                return BadRequest("Invalid Email Address");
            }

            if (SupportedCompanies.IsCorporateEmail(request.EmailId))
            {
                return BadRequest("Invalid Personal Email Address");
            }

            bool validToken = await this.signUpTokenService.VerifySignUpTokenAsync(request.SignUpToken, request.CorporateEmailId);
            if (!validToken)
            {
                return BadRequest("Invalid SignUp Token");
            }
            if (!string.IsNullOrEmpty(request.PreSignUpToken))
            {
                bool validPreSignUpToken = await this.signUpTokenService.VerifySignUpTokenAsync(request.PreSignUpToken, request.Username.ToLower());
                if (!validPreSignUpToken)
                {
                    return BadRequest("Invalid PreSignUp token");
                }
                request.Username = request.Username.ToLower();
            }
            bool success = await this.userRepository.CreateUserIfNotExist(username: request.Username, password: request.Password, name: request.Name, phoneNumber: request.PhoneNumber, email: request.EmailId, corporateEmailId: request.CorporateEmailId, experience: request.ExperienceInYears);
            if (!success)
            {
                return BadRequest("User already exist");
            }
            string sessionToken = await this.sessionTokenManager.GenerateSessionToken(request.Username).ConfigureAwait(false);
            return Ok(new SignInResponse(sessionToken));
        }
    }
}
