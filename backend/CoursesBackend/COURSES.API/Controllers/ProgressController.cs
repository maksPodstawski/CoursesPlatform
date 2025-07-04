using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IBL;
using Model;
using Model.DTOs;
using System.Security.Claims;
using Model.DTO;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<Progress>>> GetUserProgress()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var progress = await _progressService.GetProgressByUserIdAsync(userId);
            return Ok(progress);
        }

        [HttpGet("stage/{stageId}")]
        public async Task<ActionResult<Progress>> GetStageProgress(Guid stageId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var progress = await _progressService.GetProgressByStageIdAsync(stageId);
            var userProgress = progress.FirstOrDefault(p => p.UserId == userId);
            return Ok(userProgress);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<StageWithProgressDto>>> GetCourseStagesWithProgress(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var stagesWithProgress = await _progressService.GetStagesWithProgressForCourseAsync(userId, courseId);
            return Ok(stagesWithProgress);
        }

        [HttpPost("stage/{stageId}/complete")]
        public async Task<ActionResult> MarkStageAsCompleted(Guid stageId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _progressService.MarkStageAsCompletedAsync(userId, stageId);
            return Ok();
        }

        [HttpPost("stage/{stageId}/start")]
        public async Task<ActionResult<ProgressDTO>> StartStage(Guid stageId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var existingProgress = await _progressService.GetProgressByStageIdAsync(stageId);
            var userProgress = existingProgress.FirstOrDefault(p => p.UserId == userId);

            if (userProgress != null)
            {
                userProgress.LastAccessedAt = DateTime.UtcNow;
                var updatedProgress = await _progressService.UpdateProgressAsync(userProgress);
                return Ok(ProgressDTO.FromEntity(updatedProgress)); 
            }

            var progressDto = new ProgressDTO
            {
                UserId = userId,
                StageId = stageId,
                StartedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            var result = await _progressService.AddProgressAsync(progressDto.ToEntity());
            return Ok(ProgressDTO.FromEntity(result)); 
        }
    }
} 