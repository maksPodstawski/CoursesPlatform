using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IBL;
using Model;
using Model.DTO;
using System.Security.Claims;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreatorController : ControllerBase
    {
        private readonly ICreatorService _creatorService;

        public CreatorController(ICreatorService creatorService)
        {
            _creatorService = creatorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreatorResponseDTO>>> GetAllCreators()
        {
            var creators = await _creatorService.GetAllCreatorsAsync();
            return Ok(creators.Select(CreatorResponseDTO.FromCreator));
        }

        [HttpGet("me")]
        public async Task<ActionResult<CreatorResponseDTO>> GetMyCreatorProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var courses = await _creatorService.GetCoursesByCreatorAsync(Guid.Parse(userId));
            if (!courses.Any())
            {
                return NotFound("You are not a creator yet");
            }

            var creator = await _creatorService.GetCreatorByIdAsync(courses.First().Creators.First().Id);
            if (creator == null)
            {
                return NotFound("Creator profile not found");
            }

            return Ok(CreatorResponseDTO.FromCreator(creator));
        }

        [HttpPost("become")]
        public async Task<ActionResult<CreatorResponseDTO>> BecomeCreator([FromBody] BecomeCreatorDTO becomeCreatorDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var creator = await _creatorService.AddCreatorFromUserAsync(Guid.Parse(userId), becomeCreatorDto.CourseId);
                return Ok(CreatorResponseDTO.FromCreator(creator));
            }
            catch (ArgumentNullException)
            {
                return NotFound("Course not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<CourseResponseDTO>>> GetMyCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var courses = await _creatorService.GetCoursesByCreatorAsync(Guid.Parse(userId));
            if (!courses.Any())
            {
                return NotFound("You don't have any courses yet");
            }

            return Ok(courses.Select(CourseResponseDTO.FromCourse));
        }
    }
} 