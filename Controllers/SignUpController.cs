using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public SignUpController(IUserRepository userRepository, ISignUpTokenService signUpTokenService)
        {
            this.userRepository = userRepository;
            this.signUpTokenService = signUpTokenService;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Regex.IsMatch(request.Username, @"^[a-zA-Z0-9]+$"))
            {
                ModelState.AddModelError("Username", "Username can only contain letters and numbers.");
                return BadRequest(ModelState);
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber) && !Regex.IsMatch(request.PhoneNumber, @"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$"))
            {
                ModelState.AddModelError("PhoneNumber", "Invalid Phone number");
                return BadRequest(ModelState);
            }
            if (!SupportedCompanies.IsValidCompany(request.CorporateEmailId))
            {
                return BadRequest("Invalid Email Address");
            }
            bool validToken = await this.signUpTokenService.VerifySignUpTokenAsync(request.SignUpToken, request.CorporateEmailId);
            if (!validToken)
            {
                return BadRequest("Invalid SignUp Token");
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
