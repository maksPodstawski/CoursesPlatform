using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DTO;

namespace IBL
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequestDTO registerRequestDTO);
        Task LoginAsync(LoginRequestDTO loginRequestDTO);
        Task LogoutAsync();
        Task RefreshTokenAsync(string? refreshToken);
        Task<UserDTO> GetMeAsync();
    }
}
