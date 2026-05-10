using api.src.Models.DTO;

namespace api.src.Services
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterRequestDTO registerRequestDTO);

        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginrequestDTO);

        Task<bool> IsEmailExistsAsync(string email);
    }
}
