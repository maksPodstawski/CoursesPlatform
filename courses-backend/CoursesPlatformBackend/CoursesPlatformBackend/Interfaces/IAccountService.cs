using CoursesPlatformBackend.DTO;

namespace CoursesPlatformBackend.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequestDTO registerRequest);
        Task LoginAsync(LoginRequestDTO loginRequest);
        Task RefreshTokenAsync(string? refreshToken);
    }
}
