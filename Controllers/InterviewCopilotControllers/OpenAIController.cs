﻿using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.InterviewCopilotModels;
using NewHorizon.Services.InterviewerCopilotServices.Interfaces;

namespace NewHorizon.Controllers.InterviewCopilotControllers
{
    [Route("api/[controller]")]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAIService openAIService;
        public OpenAIController(IOpenAIService openAIService)
        {
            this.openAIService = openAIService;
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v2")]
        [HttpPost("generate-question")]
        public async Task<IActionResult> GenerateQuestion([FromBody] GenerateQuestion generateQuestion)
        {
            if (generateQuestion == null)
            {
                return BadRequest("Bad request");
            }

            string generatedQuestion = await this.openAIService.GenerateQuestionAsync(generateQuestion.Prompt, generateQuestion.SystemContext, generateQuestion.PreviouslyAskedQuestion, generateQuestion.Temperature).ConfigureAwait(false);

            return Ok(generatedQuestion);
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v2")]
        [HttpPost("analyze-solution")]
        public async Task<IActionResult> AnalyzeSolution([FromBody] AnalyzeSolution analyzeSolution)
        {
            if (analyzeSolution == null)
            {
                return BadRequest("Bad request");
            }

            string analyzedSolution = await this.openAIService.AnalyzeSolutionAsync(analyzeSolution.Question, analyzeSolution.Solution, analyzeSolution.SystemContext).ConfigureAwait(false);

            return Ok(analyzedSolution);
        }
    }
}
