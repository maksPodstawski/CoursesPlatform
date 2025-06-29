using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IBL;
using Model;
using Model.Constans;
using Model.DTO;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/stages")]
    public class StageController : ControllerBase
    {
        private readonly IStageService _stageService;
        private readonly ICreatorService _creatorService;

        public StageController(
            IStageService stageService,
            ICreatorService creatorService)
        {
            _stageService = stageService;
            _creatorService = creatorService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StageResponseDTO>>> GetAllStages()
        {
            var stages = await _stageService.GetAllStagesAsync();
            return Ok(stages.Select(StageResponseDTO.FromStage));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<StageResponseDTO>> GetStageById(Guid id)
        {
            var stage = await _stageService.GetStageByIdAsync(id);
            if (stage == null)
                return NotFound();

            return Ok(StageResponseDTO.FromStage(stage));
        }

        [Authorize]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<StageResponseDTO>>> GetStagesByCourseId(Guid courseId)
        {
            var stages = await _stageService.GetStagesByCourseIdAsync(courseId);
            return Ok(stages.Select(StageResponseDTO.FromStage));
        }

        [Authorize(Roles = IdentityRoleConstants.User)]
        [HttpPost]
        public async Task<ActionResult<StageResponseDTO>> CreateStage([FromBody] CreateStageDTO createStageDto)
        {
            if (createStageDto == null)
                return BadRequest("Stage data is required");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isCreator = await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), createStageDto.CourseId);
            if (!isCreator)
                return Forbid();

            var stageEntity = CreateStageDTO.ToEntity(createStageDto);

            var createdStage = await _stageService.AddStageAsync(stageEntity);
            if (createdStage == null)
                return BadRequest("Failed to create stage");

            var responseDto = StageResponseDTO.FromStage(createdStage);

            return CreatedAtAction(nameof(GetStageById), new { id = createdStage.Id }, responseDto);
        }

        [Authorize(Roles = IdentityRoleConstants.User)]
        [HttpPut("{id}")]
        public async Task<ActionResult<StageResponseDTO>> UpdateStage(Guid id, [FromBody] UpdateStageDTO updateStageDto)
        {
            if (updateStageDto == null)
                return BadRequest("Stage data is required");

            if (id != updateStageDto.Id)
                return BadRequest("Stage ID mismatch");

            var existingStage = await _stageService.GetStageByIdAsync(id);
            if (existingStage == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isCreator = await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), existingStage.CourseId);
            if (!isCreator)
                return Forbid();

            existingStage.Name = updateStageDto.Name;
            existingStage.Description = updateStageDto.Description;
            existingStage.Duration = updateStageDto.Duration;

            var updatedStage = await _stageService.UpdateStageAsync(existingStage);
            if (updatedStage == null)
                return BadRequest("Failed to update stage");

            return Ok(StageResponseDTO.FromStage(updatedStage));
        }

        [Authorize(Roles = IdentityRoleConstants.User)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<StageResponseDTO>> DeleteStage(Guid id)
        {
            var stage = await _stageService.GetStageByIdAsync(id);
            if (stage == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isCreator = await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), stage.CourseId);
            if (!isCreator)
                return Forbid();

            var deletedStage = await _stageService.DeleteStageAsync(id);
            if (deletedStage == null)
                return BadRequest("Failed to delete stage");

            return Ok(StageResponseDTO.FromStage(deletedStage));
        }
    }
} 