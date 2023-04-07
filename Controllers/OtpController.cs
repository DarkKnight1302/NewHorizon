﻿namespace NewHorizon.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using NewHorizon.Constants;
    using NewHorizon.Models.ColleagueCastleModels;
    using NewHorizon.Services.ColleagueCastleServices.Interfaces;

    namespace OTPExample.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class OTPController : ControllerBase
        {
            private readonly IOtpService otpService;
            private readonly ISignUpTokenService signUpTokenService;

            public OTPController(IOtpService otpService, ISignUpTokenService signUpTokenService)
            {
                this.otpService = otpService;
                this.signUpTokenService = signUpTokenService;
            }

            [ApiKeyRequired]
            [ApiExplorerSettings(GroupName = "v1")]
            [HttpPost("generate-and-send")]
            public async Task<IActionResult> GenerateAndSendOTP([FromBody] GenerateOtpRequest generateOtpRequest)
            {
                string emailAddress = generateOtpRequest.EmailId;
                var apiKey = HttpContext.Request.Headers["X-Api-Key"];
                if (apiKey != emailAddress)
                {
                    return BadRequest("Invalid Header value");
                }
                if (!SupportedCompanies.IsCorporateEmail(generateOtpRequest.EmailId))
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

            [ApiExplorerSettings(GroupName = "v1")]
            [HttpPost("validate")]
            public async Task<IActionResult> ValidateOTP([FromBody] ValidateOtpRequest validateOtpRequest)
            {
                string emailAddress = validateOtpRequest.EmailId;
                int otp = validateOtpRequest.Otp;
                if (string.IsNullOrEmpty(emailAddress) || !SupportedCompanies.IsCorporateEmail(emailAddress))
                {
                    return BadRequest("Invalid email Address");
                }
                emailAddress = emailAddress.Trim().ToLower();
                bool isOtpValid = await this.otpService.IsOtpValidAsync(emailAddress, otp).ConfigureAwait(false);
                if (isOtpValid)
                {
                    string signUpToken = await this.signUpTokenService.GenerateSignUpTokenAsync(emailAddress).ConfigureAwait(false);
                    return Ok(new ValidateOtpResponse { Message = "Opt Validated Successfully", SignUpToken = signUpToken});
                }
                return Unauthorized("Incorrect Otp");
            }
        }
    }

}
