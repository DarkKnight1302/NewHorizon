namespace NewHorizon.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using NewHorizon.Constants;
    using NewHorizon.Services.ColleagueCastleServices.Interfaces;

    namespace OTPExample.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class OTPController : ControllerBase
        {
            private readonly IOtpService otpService;

            public OTPController(IOtpService otpService)
            {
                this.otpService = otpService;
            }

            [HttpPost("generate-and-send")]
            public async Task<IActionResult> GenerateAndSendOTP(string emailAddress)
            {
                if (!SupportedCompanies.IsValidCompany(emailAddress))
                {
                    return BadRequest("Invalid email address");
                }
                emailAddress = emailAddress.Trim().ToLower();
                try
                {
                    await this.otpService.GenerateAndSendOtpAsync(emailAddress).ConfigureAwait(false);
                    return Ok("OTP sent successfully");
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed to send OTP: " + ex.Message);
                }
            }

            [HttpPost("validate")]
            public async Task<IActionResult> ValidateOTP(string emailAddress, int otp)
            {
                if (string.IsNullOrEmpty(emailAddress) || !SupportedCompanies.IsValidCompany(emailAddress))
                {
                    return BadRequest("Invalid email Address");
                }
                emailAddress = emailAddress.Trim().ToLower();
                bool isOtpValid = await this.otpService.IsOtpValidAsync(emailAddress, otp).ConfigureAwait(false);
                if (isOtpValid)
                {
                    return Ok("OTP validated successfully");
                }
                return Unauthorized("Incorrect Otp");
            }
        }
    }

}
