﻿using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class PropertyPostController : Controller
    {
        private readonly IPropertyPostService propertyPostService;

        public PropertyPostController(IPropertyPostService propertyPostService)
        {
            this.propertyPostService = propertyPostService;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> CreatePropertyPost([FromBody] CreatePropertyPostRequest createPropertyPostRequest)
        {
            string postId = await this.propertyPostService.CreatePropertyPostAsync(createPropertyPostRequest.SessionId, createPropertyPostRequest.PlaceId, createPropertyPostRequest.Title, createPropertyPostRequest.Description, createPropertyPostRequest.Images);
            if (string.IsNullOrEmpty(postId))
            {
                return Problem("Not able to create post");
            }
            return Ok(postId);
        }
    }
}
