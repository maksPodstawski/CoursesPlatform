using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost("invite-by-email")]
        public async Task<IActionResult> InviteByEmail([FromBody] InviteByEmailDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var invitation = await _invitationService.InviteCoAuthorByEmailAsync(
                    dto.Email,
                    dto.CourseId,
                    Guid.Parse(userId)
                );
                return Ok(invitation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accept")]
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

        [HttpPost("decline")]
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

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetInvitationsByCourse(Guid courseId)
        {
            var invitations = await _invitationService.GetInvitationsByCourseAsync(courseId);
            return Ok(invitations);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetInvitationsByEmail([FromQuery] string email)
        {
            var invitations = await _invitationService.GetInvitationsByEmailAsync(email);

            var result = invitations.Select(inv => new {
                inv.Id,
                inv.Email,
                inv.Status,
                inv.CreatedAt,
                CourseName = inv.Course?.Name,
                CreatorNames = inv.Course?.Creators.Select(c => c.User.UserName).ToList(),
                InvitedBy = inv.InvitedBy?.UserName
            });

            return Ok(result);
        }
    }
} 