using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IBL;
using Model;
using System.Net.Mime;
using Model.Constans;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/stages")]
    public class StageVideoController : ControllerBase
    {
        private readonly IStageService _stageService;
        private readonly IPurchasedCoursesService _purchasedCoursesService;
        private readonly IWebHostEnvironment _environment;
        private readonly ICreatorService _creatorService;
        private const string UPLOAD_DIRECTORY = "UploadedVideos";

        public StageVideoController(
            IStageService stageService,
            IPurchasedCoursesService purchasedCoursesService,
            IWebHostEnvironment environment,
            ICreatorService creatorService)
        {
            _stageService = stageService;
            _purchasedCoursesService = purchasedCoursesService;
            _environment = environment;
            _creatorService=creatorService;
        }

        [Authorize(Roles = IdentityRoleConstants.User)]
        [HttpPost("{stageId}/video")]
        public async Task<IActionResult> UploadVideo(Guid stageId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.ContentType.StartsWith("video/"))
                return BadRequest("Only video files are allowed");

            var stage = await _stageService.GetStageByIdAsync(stageId);
            if (stage == null)
                return NotFound("Stage not found");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isCreator = await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), stage.CourseId);
            if (!isCreator)
                return Forbid();

            var uploadPath = Path.Combine(_environment.ContentRootPath, UPLOAD_DIRECTORY, stageId.ToString());
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            stage.VideoPath = Path.Combine(UPLOAD_DIRECTORY, stageId.ToString(), fileName);
            var updatedStage = await _stageService.UpdateStageAsync(stage);
            if (updatedStage == null)
                return BadRequest("Failed to update stage with video path");

            return Ok(new { message = "Video uploaded successfully", path = updatedStage.VideoPath });
        }

        [Authorize]
        [HttpGet("{stageId}/video/stream")]
        public async Task<IActionResult> StreamVideo(Guid stageId)
        {
            var stage = await _stageService.GetStageByIdAsync(stageId);
            if (stage == null)
                return NotFound("Stage not found");

            if (string.IsNullOrEmpty(stage.VideoPath))
                return NotFound("No video found for this stage");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var hasAccess = await _purchasedCoursesService.HasUserPurchasedCourseAsync(
                Guid.Parse(userId), 
                stage.CourseId);

            if (!hasAccess)
                return Forbid();

            var filePath = Path.Combine(_environment.ContentRootPath, stage.VideoPath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Video file not found");

            return PhysicalFile(filePath, "video/mp4", enableRangeProcessing: true);
        }
    }
} 