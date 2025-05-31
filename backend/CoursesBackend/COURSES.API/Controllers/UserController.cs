using System.Security.Claims;
using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    // GET /api/user/me
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDTO>> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        return new UserProfileDTO
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureBase64 = user.ProfilePicture != null
            ? Convert.ToBase64String(user.ProfilePicture)
            : null
        };
    }

    // PUT /api/user/me
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileDTO dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.ProfilePictureBase64))
        {
            try
            {
                var base64Data = dto.ProfilePictureBase64;
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex >= 0)
                {
                    base64Data = base64Data.Substring(commaIndex + 1);
                }

                user.ProfilePicture = Convert.FromBase64String(base64Data);
            }
            catch
            {
                return BadRequest("Invalid image format.");
            }
        }

        var updated = await _userService.UpdateUserAsync(user);
        if (updated == null)
            return StatusCode(500, "Update failed");

        return NoContent();
    }
}
