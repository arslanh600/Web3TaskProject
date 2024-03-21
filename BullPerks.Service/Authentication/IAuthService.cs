using BullPerks.Data.Dto;

namespace BullPerks.Service
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Authenticate(LoginDto request);
    }
}
