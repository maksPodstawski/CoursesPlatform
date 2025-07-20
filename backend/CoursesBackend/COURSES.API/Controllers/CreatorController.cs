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
        private readonly IInvitationService _invitationService;

        public CreatorController(ICreatorService creatorService, IInvitationService invitationService)
        {
            _creatorService = creatorService;
            _invitationService = invitationService;
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

        [HttpPost("add")]
        public async Task<ActionResult<CreatorResponseDTO>> BecomeCreator([FromBody] AppendCreatorDTO appendCreatorDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!(await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), appendCreatorDto.CourseId)))
            {
                return BadRequest("You are already a creator of this course");
            }

            try
            {
                var creator = await _creatorService.AddCreatorFromUserAsync(appendCreatorDto.UserId, appendCreatorDto.CourseId);
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

        [HttpPost("invite-coauthor")] 
        public async Task<IActionResult> InviteCoAuthor([FromBody] InviteByEmailDTO dto)
        {
            try
            {
                var invitation = await _invitationService.InviteCoAuthorByEmailAsync(dto.Email, dto.CourseId);
                return Ok(invitation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accept-invitation")] 
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {
                await _invitationService.AcceptInvitationAsync(dto.InvitationId, Guid.Parse(userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("decline-invitation")] 
        public async Task<IActionResult> DeclineInvitation([FromBody] AcceptInvitationDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            try
            {
                await _invitationService.DeclineInvitationAsync(dto.InvitationId, Guid.Parse(userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("invitations-by-course/{courseId}")]
        public async Task<IActionResult> GetInvitationsByCourse(Guid courseId)
        {
            var invitations = await _invitationService.GetInvitationsByCourseAsync(courseId);
            return Ok(invitations);
        }

        [HttpGet("invitations-by-email")]
        public async Task<IActionResult> GetInvitationsByEmail([FromQuery] string email)
        {
            var invitations = await _invitationService.GetInvitationsByEmailAsync(email);
            return Ok(invitations);
        }
    }
} 